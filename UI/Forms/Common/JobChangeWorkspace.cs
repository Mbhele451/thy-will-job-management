using System.Linq;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using Major_Project.Core;
using MajorProject.Business.Services;

namespace Major_Project.UI.Forms.Common
{
    internal static class JobChangeWorkspace
    {
        private static JobChangeService Service => new JobChangeService(WorkflowUi.ConnectionString);

        internal static async Task RequestCancellation(Control owner, int job)
        {
            using (var panel = new WorkflowPanel("In-person cancellation • Job #" + job, new ChangeRequestEditor()))
            {
                panel.Note("Record the customer's reason. A manager reviews the request. If work has started, the assigned technician must report the quantities used before approval.");
                panel.Note("Cancellation on the promise date, the day before, or after work starts carries a fee of 5% of the job total. Materials already used are added for work in progress. Earlier cancellations before work starts are free.");
                var reason = panel.Input("Reason (10–1,000 characters)", 1000);
                panel.Submit("Submit cancellation request", () => Service.RequestStaffCancellation(job, UserSession.StaffID, reason.Text));
                if (await panel.OpenAsync(owner))
                    WorkflowUi.Notify(owner, "Cancellation request submitted for manager review.");
            }
        }

        private static async Task<bool> RecordMaterials(Control owner, int request, int job)
        {
            var materials = Service.Materials(job, UserSession.StaffID);
            using (var panel = new WorkflowPanel("Material report • Job #" + job, new ChangeRequestEditor()))
            {
                panel.Note("Enter the quantity actually used for each item. Enter zero for unused items. If no items were allocated, submit the empty report to confirm.");
                var grid = panel.Field<DataGridView>("Allocated materials", control =>
                {
                    control.AutoGenerateColumns = true;
                    control.AllowUserToAddRows = false;
                    control.AllowUserToDeleteRows = false;
                    control.RowHeadersVisible = false;
                    control.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    control.DataSource = materials;
                });
                grid.DataBindingComplete += (sender, args) => ConfigureMaterialColumns(grid);
                ConfigureMaterialColumns(grid);
                grid.DataError += (sender, args) =>
                {
                    args.ThrowException = false;
                    WorkflowUi.ShowError(panel, "Enter a whole number between zero and the allocated quantity.");
                };
                panel.Submit("Submit material report", () =>
                {
                    if (!grid.EndEdit())
                        throw new ArgumentException("Correct the quantity before submitting.");
                    ((CurrencyManager)grid.BindingContext[materials]).EndCurrentEdit();
                    Service.SubmitMaterials(request, UserSession.StaffID, materials);
                });
                return await panel.OpenAsync(owner);
            }
        }

        private static void ConfigureMaterialColumns(DataGridView grid)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.ReadOnly = column.DataPropertyName != "UsedQuantity";
                switch (column.DataPropertyName)
                {
                    case "ItemID": column.Visible = false; break;
                    case "ItemName": column.HeaderText = "Material"; break;
                    case "AllocatedQuantity": column.HeaderText = "Allocated"; break;
                    case "UsedQuantity": column.HeaderText = "Used"; break;
                }
            }
        }

        internal static void Add(TabControl tabs, bool manager, bool rescheduleReviewOnly = false)
        {
            var page = tabs.TabPages["JobChanges"];
            var view = page.Controls.OfType<JobChangesView>().Single();
            var grid = view.Records;
            DesktopTheme.Apply(grid);
            var status = view.Feedback;
            var allOpen = view.AllOpen;
            string mode = manager ? "requests" : "notifications";
            view.RequestHistoryButton.Visible = manager;
            view.MaterialReportsButton.Visible = !manager;
            view.SubmitReportButton.Visible = false;
            view.ReassignmentQueueButton.Visible = manager && !rescheduleReviewOnly;
            view.ReassignSelectedJobButton.Visible = false;
            allOpen.Visible = false;
            view.ReviewSelectedButton.Visible = manager;
            view.NewAllocationsButton.Visible = view.MarkSelectedAsReadButton.Visible = !manager;
            Action load = () =>
            {
                grid.DataSource = null;
                view.SubmitReportButton.Visible = mode == "materials";
                view.MarkSelectedAsReadButton.Visible = mode == "notifications";
                view.ReviewSelectedButton.Visible = mode == "requests";
                view.ReassignSelectedJobButton.Visible = mode == "handover";
                allOpen.Visible = mode == "handover";
                switch (mode)
                {
                    case "notifications":
                        grid.DataSource = Service.Notifications(UserSession.StaffID);
                        status.Text = "New assignments and cancellation notices appear here.";
                        break;
                    case "materials":
                        grid.DataSource = Service.TechnicianMaterialReports(UserSession.StaffID);
                        status.Text = "Select a job to report the materials used before cancellation.";
                        break;
                    case "handover":
                        grid.DataSource = Service.ReassignmentQueue(UserSession.StaffID, allOpen.Checked);
                        status.Text = "Select an open job to record a handover.";
                        break;
                    default:
                        grid.DataSource = Service.Requests(UserSession.StaffID, true);
                        status.Text = "Select a pending request to review the decision.";
                        break;
                }
                if (((DataTable)grid.DataSource).Rows.Count == 0)
                    status.Text = "No records in this list.";
            };
            Action safeLoad = () => WorkflowUi.Run(page, load);
            WorkflowUi.BindButton(view.RefreshButton, safeLoad);
            if (manager)
            {
                WorkflowUi.BindButton(view.RequestHistoryButton, () =>
                {
                    mode = "requests";
                    safeLoad();
                });
                if (!rescheduleReviewOnly)
                {
                    WorkflowUi.BindButton(view.ReassignmentQueueButton, () =>
                    {
                        mode = "handover";
                        safeLoad();
                    });
                    allOpen.CheckedChanged += (s, e) =>
                    {
                        mode = "handover";
                        safeLoad();
                    };
                }

                var review = WorkflowUi.BindButton(view.ReviewSelectedButton, () =>
                {
                });
                review.Click += async (s, e) => await WorkflowUi.RunAsync(page, async () =>
                {
                    var row = WorkflowUi.Selected(grid);
                    if (mode != "requests")
                        throw new InvalidOperationException("Open request history first.");
                    if (Convert.ToString(row["RequestStatus"]) != "Pending")
                        throw new InvalidOperationException("This request is already reviewed.");
                    int id = Convert.ToInt32(row["RequestID"]);
                    bool cancel = Convert.ToString(row["RequestType"]) == "Cancellation";
                    if (cancel && rescheduleReviewOnly)
                        throw new InvalidOperationException("A manager reviews cancellation requests.");
                    using (var p = new WorkflowPanel("Review request #" + id, new ChangeReviewEditor()))
                    {
                        p.Note("Job #" + row["JobID"] + " • " + row["RequestType"] + " • " + row["Reason"]);
                        p.Note("Original quote: R " + Convert.ToDecimal(row["OriginalTotal"]).ToString("N2") + " | Fee on approval: R " + Convert.ToDecimal(row[cancel ? "CancellationFee" : "ReschedulingFee"]).ToString("N2"));
                        bool awaitingMaterials = cancel && Convert.ToBoolean(row["AwaitingMaterials"]);
                        if (cancel)
                        {
                            p.Note(Convert.ToString(row["FeeReason"]));
                            decimal materialCharge = Convert.ToDecimal(row["MaterialCharge"]);
                            p.Note(awaitingMaterials ? "Awaiting technician material report. Approval becomes available after submission." : "Materials used: R " + materialCharge.ToString("N2") + " • Final cancellation invoice: R " + (materialCharge + Convert.ToDecimal(row["CancellationFee"])).ToString("N2"));
                            p.Field<DataGridView>("Materials used / returned", control =>
                            {
                                control.DataSource = Service.RequestMaterials(UserSession.StaffID, id);
                                control.ReadOnly = true;
                                control.Height = 180;
                                control.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                                control.AllowUserToAddRows = false;
                            });
                        }
                        var decision = p.Choice("Decision", awaitingMaterials ? new[] { "Reject" } : new[] { "Reject", "Approve" });
                        var note = p.Input("Decision note", 1000);
                        var date = cancel ? null : p.Field<DateTimePicker>("Approved date (reschedule only)", control =>
                        {
                            control.MinDate = DateTime.Today;
                            control.Value = row["RequestedDate"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(row["RequestedDate"]) < DateTime.Today ? DateTime.Today : Convert.ToDateTime(row["RequestedDate"]);
                        });
                        var staff = cancel ? null : Technicians(p, date, Convert.ToInt32(row["JobID"]));
                        p.Submit("Save decision", () => Service.Decide(id, UserSession.StaffID, decision.Text == "Approve", note.Text, staff?.SelectedValue == null ? 0 : Convert.ToInt32(staff.SelectedValue), date?.Value));
                        if (await p.OpenAsync(page))
                        {
                            load();
                            WorkflowUi.Notify(page, "Request decision saved.");
                        }
                    }
                });
                if (!rescheduleReviewOnly)
                {
                    var handover = WorkflowUi.BindButton(view.ReassignSelectedJobButton, () =>
                    {
                    });
                    handover.Click += async (s, e) => await WorkflowUi.RunAsync(page, async () =>
                    {
                        if (mode != "handover")
                            throw new InvalidOperationException("Open the reassignment queue first.");
                        var row = WorkflowUi.Selected(grid);
                        int job = Convert.ToInt32(row["JobID"]);
                        int previous = row.IsNull("PreviousStaffID") ? 0 : Convert.ToInt32(row["PreviousStaffID"]);
                        using (var panel = new WorkflowPanel("Reassign job #" + job, new HandoverEditor()))
                        {
                            panel.Note("Record work completed and site handover details. Reassignment supersedes any pending change request.");
                            DateTime original = row.IsNull("PromiseDate") ? DateTime.Today : Convert.ToDateTime(row["PromiseDate"]);
                            var date = panel.Field<DateTimePicker>("Promise date", control =>
                            {
                                control.MinDate = DateTime.Today;
                                control.Value = original.Date < DateTime.Today ? DateTime.Today : original.Date;
                            });
                            var staff = Technicians(panel, date, job, previous);
                            var note = panel.Input("Handover reason", 1000);
                            panel.AddComponentHelp((date, "Choose the promise date to see technicians available that day."), (staff, "Choose the technician who will take over this job."), (note, "Record progress and instructions that the replacement technician needs."));
                            panel.Submit("Reassign job", () => Service.Reassign(job, UserSession.StaffID, Convert.ToInt32(staff.SelectedValue), date.Value, note.Text, previous));
                            if (await panel.OpenAsync(page))
                            {
                                load();
                                WorkflowUi.Notify(page, "Job reassigned. The replacement technician has a new allocation.");
                            }
                        }
                    });
                }
            }
            else
            {
                WorkflowUi.BindButton(view.MaterialReportsButton, () =>
                {
                    mode = "materials";
                    safeLoad();
                });
                WorkflowUi.BindButton(view.SubmitReportButton, async () => await WorkflowUi.RunAsync(page, async () =>
                {
                    if (mode != "materials")
                        return;
                    var row = WorkflowUi.Selected(grid);
                    if (await RecordMaterials(page, Convert.ToInt32(row["RequestID"]), Convert.ToInt32(row["JobID"])))
                    {
                        load();
                        WorkflowUi.Notify(page, "Material report submitted for manager review.");
                    }
                }));
                WorkflowUi.BindButton(view.NewAllocationsButton, () =>
                {
                    mode = "notifications";
                    safeLoad();
                });
                WorkflowUi.BindButton(view.MarkSelectedAsReadButton, () => WorkflowUi.Run(page, () =>
                {
                    if (mode != "notifications")
                        throw new InvalidOperationException("Open new allocations first.");
                    Service.ReadNotification(UserSession.StaffID, Convert.ToInt64(WorkflowUi.Selected(grid)["NotificationID"]));
                    load();
                }));
            }

            tabs.SelectedIndexChanged += (s, e) =>
            {
                if (tabs.SelectedTab == page)
                    safeLoad();
            };
        }

        private static ComboBox Technicians(WorkflowPanel panel, DateTimePicker date, int job, int previous = 0)
        {
            var box = panel.Field<ComboBox>("Available technician", control =>
            {
                control.DropDownStyle = ComboBoxStyle.DropDownList;
                control.DisplayMember = "FullName";
                control.ValueMember = "StaffID";
            });
            var staff = new StaffService(WorkflowUi.ConnectionString);
            Action load = () =>
            {
                box.DataSource = null;
                var candidates = staff.GetAvailableTechniciansForJob(date.Value.Date, job).DefaultView;
                if (previous > 0)
                    candidates.RowFilter = "StaffID <> " + previous;
                box.DisplayMember = "FullName";
                box.ValueMember = "StaffID";
                box.DataSource = candidates;
            };
            load();
            date.ValueChanged += (s, e) => WorkflowUi.Run(panel, load);
            return box;
        }
    }
}
