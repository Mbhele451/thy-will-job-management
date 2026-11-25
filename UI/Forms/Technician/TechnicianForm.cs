using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Major_Project.Core;
using MajorProject.Business.Services;
using Major_Project.UI.Forms.Common;
using Major_Project.Utilities;
using MajorProject.Business.Helpers;

namespace Major_Project.UI.Forms.Technician
{
    public partial class TechnicianForm : Form
    {
        private readonly int _staffID;
        private readonly string _fullName;
        private readonly JobService _jobService;
        private readonly InventoryService _inventoryService;
        private HelpManager _helpManager;
        private DataTable _myJobs;
        internal WorkspaceTabs NavigationTabs => tabControl1;

        public TechnicianForm()
        {
            InitializeComponent();
        }

        public TechnicianForm(int staffID, string fullName)
        {
            InitializeComponent();
            GridPresentation.Attach(this);
            StaffWorkspacePages.AddProfile(tabControl1, technician: true);
            JobChangeWorkspace.Add(tabControl1, false);
            AddJobControls();
            var assignmentTimer = new Timer(components)
            {
                Interval = 30000
            };
            assignmentTimer.Tick += CheckAssignments;
            assignmentTimer.Start();
            _staffID = staffID;
            _fullName = fullName;
            string conn = Properties.Settings.Default.GroupWst34ConnectionString;
            _jobService = new JobService(conn);
            _inventoryService = new InventoryService(conn);
            tabControl1.SelectedIndexChanged += (sender, args) =>
            {
                if (tabControl1.SelectedTab == tabPage3)
                    LoadMyJobs();
                else if (tabControl1.SelectedTab == tabPage4)
                    LoadInventory();
            };
        }

        private void Technician_Load(object sender, EventArgs e)
        {
            if (_jobService == null)
                return; // Designer-only constructor has no database services.
            lblAdminGreeting.Text = "Welcome, " + _fullName;
            dgvJobs.SelectionChanged += dgvJobs_SelectionChanged_1;
            LoadMyJobs();
            LoadInventory();
            UpdateStatButtons();
            // Debug: log the staff ID
            Logger.Info($"TechnicianForm loaded with StaffID: {_staffID}, FullName: {_fullName}");
            _helpManager = new HelpManager();
            //dgvJobs.AutoGenerateColumns = false;
            RegisterTechnicianHelp();
            _helpManager.AttachHelpButton(btnHelpTech, defaultControl: dgvJobs);
            _helpManager.AttachHelpButton(btnTechDash, defaultControl: dataGridView4);
        }

        // ── DATA LOADING ──
        private System.Collections.Generic.HashSet<int> assignedJobs;
        private bool checkingAssignments;
        private long lastNoticeShown;
        private static System.Collections.Generic.HashSet<int> JobIds(DataTable jobs)
        {
            return new System.Collections.Generic.HashSet<int>(jobs.Rows.Cast<DataRow>().Select(row => Convert.ToInt32(row["JobID"])));
        }

        private async void CheckAssignments(object sender, EventArgs args)
        {
            if (checkingAssignments || !UserSession.HasRole("Technician") || UserSession.StaffID != _staffID)
                return;
            checkingAssignments = true;
            try
            {
                var jobs = await System.Threading.Tasks.Task.Run(() => _jobService.GetTechnicianJobs(_staffID));
                var notices = await System.Threading.Tasks.Task.Run(() => new JobChangeService(Properties.Settings.Default.GroupWst34ConnectionString).Notifications(_staffID));
                if (IsDisposed || UserSession.StaffID != _staffID)
                    return;
                var requestPage = tabControl1.TabPages["JobChanges"];
                if (requestPage != null)
                    requestPage.Text = "Allocations & materials" + (notices.Rows.Count > 0 ? " (" + notices.Rows.Count + " unread)" : "");
                if (!UserSession.Check(false))
                    return;
                if (_myJobs == null || !_myJobs.Rows.Cast<DataRow>().SequenceEqual(jobs.Rows.Cast<DataRow>(), DataRowComparer.Default))
                    BindMyJobs(jobs);
                if (notices.Rows.Count > 0 && !tabControl1.SelectedTab.Controls.OfType<WorkflowPanel>().Any(panel => panel.Visible))
                {
                    var newest = notices.Rows[0];
                    long id = Convert.ToInt64(newest["NotificationID"]);
                    if (id > lastNoticeShown)
                    {
                        WorkflowUi.Notify(this, "Job #" + newest["JobID"] + ": " + newest["Message"] + " Open Allocations & materials for all notices.");
                        lastNoticeShown = id;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Assignment check failed.", ex);
            }
            finally
            {
                checkingAssignments = false;
            }
        }

        private void LoadMyJobs()
        {
            try
            {
                BindMyJobs(_jobService.GetTechnicianJobs(_staffID));
            }
            catch (Exception ex)
            {
                Logger.Error($"LoadMyJobs error: {ex.Message}", ex);
                UIHelper.ShowError("Your jobs could not be loaded. Check your connection and select Refresh.");
            }
        }

        private void BindMyJobs(DataTable jobs)
        {
            _myJobs = jobs;
            _myJobs.Columns["HasPendingChange"].ColumnMapping = MappingType.Hidden;
            _myJobs.Columns["MaterialReportRequired"].ColumnMapping = MappingType.Hidden;
            var current = JobIds(_myJobs);
            if (assignedJobs != null)
            {
                var added = current.Except(assignedJobs).OrderBy(id => id).ToArray();
                if (added.Length > 0)
                    WorkflowUi.Notify(this, "New job assigned: #" + string.Join(", #", added) + ". Open Jobs for details.");
            }

            assignedJobs = current;
            dgvJobs.AutoGenerateColumns = true;
            dgvJobs.Columns.Clear();
            FilterJobs();
            dataGridView4.AutoGenerateColumns = true;
            dataGridView4.Columns.Clear();
            var view = new DataView(_myJobs)
            {
                Sort = "JobID DESC",
                RowFilter = "JobStatus <> 'Archived'"
            };
            var recent = _myJobs.Clone();
            for (int i = 0; i < Math.Min(10, view.Count); i++)
                recent.ImportRow(view[i].Row);
            dataGridView4.DataSource = recent;
            // Optionally, set column headers to friendly names
            if (dgvJobs.Columns.Contains("JobID"))
                dgvJobs.Columns["JobID"].HeaderText = "Job ID";
            if (dgvJobs.Columns.Contains("CustomerName"))
                dgvJobs.Columns["CustomerName"].HeaderText = "Customer";
            if (dgvJobs.Columns.Contains("JobStatus"))
                dgvJobs.Columns["JobStatus"].HeaderText = "Status";
            if (dgvJobs.Columns.Contains("PromiseDate"))
                dgvJobs.Columns["PromiseDate"].HeaderText = "Promise Date";
            if (dgvJobs.Columns.Contains("Notes"))
                dgvJobs.Columns["Notes"].HeaderText = "Notes";
            dgvJobs.Refresh();
            Logger.Info($"LoadMyJobs: Grid updated with {_myJobs.Rows.Count} jobs");
        }

        private void AddJobControls()
        {
            _jobStatus.Items.AddRange(new object[] { "All", "Pending", "In Progress", "Completed", "Cancelled" });
            _jobStatus.SelectedIndex = 0;
            _jobStatus.SelectedIndexChanged += (sender, args) => FilterJobs();
            _jobSearch.TextChanged += (sender, args) => FilterJobs();
            btnJobsRefresh.Click += (sender, args) => WorkflowUi.Run(this, LoadMyJobs);
            btnJobDetails.Click += async (sender, args) => await WorkflowUi.RunAsync(this, async () =>
            {
                int jobId = GetSelectedJobID();
                if (jobId <= 0)
                    return;
                var details = _jobService.GetTechnicianJobByID(jobId, _staffID);
                if (details.Rows.Count != 1)
                    throw new InvalidOperationException("The job is no longer assigned to you.");
                using (var dialog = new WorkflowPanel($"Job #{jobId}"))
                {
                    if (Convert.ToBoolean(details.Rows[0]["HasPendingChange"]))
                        dialog.Note("Work on this job is on hold.");
                    details.Columns.Remove("HasPendingChange");
                    details.Columns.Remove("MaterialReportRequired");
                    var record = new ProfileDetailsView();
                    record.Dock = DockStyle.Fill;
                    record.ShowRecord(details.Rows[0]);
                    dialog.UseContent().Controls.Add(record);
                    await dialog.OpenAsync(this);
                }
            });
        }

        private void FilterJobs()
        {
            if (_myJobs == null)
                return;
            string search = _jobSearch.Text.Trim();
            var filtered = _myJobs.Clone();
            foreach (DataRow row in _myJobs.Rows)
            {
                if (_jobStatus.SelectedIndex > 0 && Convert.ToString(row["JobStatus"]) != _jobStatus.Text)
                    continue;
                if (new[]
                {
                    "CustomerName",
                    "Area",
                    "JobID"
                }.Any(key => Convert.ToString(row[key]).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0))
                    filtered.ImportRow(row);
            }

            dgvJobs.DataSource = filtered;
            if (filtered.Rows.Count == 0)
                textBox1.Text = _myJobs.Rows.Count == 0 ? "You have no assigned jobs." : "No jobs match your search and status filter.";
        }

        private void LoadInventory()
        {
            try
            {
                var data = _inventoryService.GetAllInventoryItems();
                dataGridView1.DataSource = data;
                var low = GridPresentation.LowStock(data);
                dataGridView3.AutoGenerateColumns = true;
                dataGridView3.Columns.Clear();
                dataGridView3.DataSource = low.DefaultView;
                lblLowStockEmpty.Visible = low.Rows.Count == 0;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load inventory. Error: " + ex.Message);
            }
        }

        private void RegisterTechnicianHelp()
        {
            _helpManager.RegisterControlSection("Technician Dashboard", (dgvJobs, "My Jobs", "List of jobs assigned to you."), (button4, "Start Job", "Starts a pending job (changes status to In Progress)."), (btnInvoice, "Complete Job", "Completes an in-progress job (changes status to Completed)."), (textBox1, "Job Details", "Displays details of the selected job."), (button1, "Pending Count", "Shows number of pending jobs."), (button9, "In Progress Count", "Shows number of in-progress jobs."), (button2, "Completed Count", "Shows number of completed jobs."), (dataGridView1, "Inventory", "View inventory items."), (textBox7, "Search Inventory", "Search for an item by name."));
            _helpManager.RegisterContainer(this);
        }

        private void UpdateStatButtons()
        {
            try
            {
                int pending = _jobService.GetJobCountByStatus(_staffID, "Pending");
                int inProgress = _jobService.GetJobCountByStatus(_staffID, "In Progress");
                int completed = _jobService.GetJobCountByStatus(_staffID, "Completed");
                button1.Text = "Pending\n" + pending;
                button9.Text = "In progress\n" + inProgress;
                button2.Text = "Completed\n" + completed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("UpdateStatButtons error: " + ex.Message);
            }
        }

        // ── INVENTORY SEARCH ──
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = string.IsNullOrWhiteSpace(textBox7.Text) ? _inventoryService.GetAllInventoryItems() : _inventoryService.SearchInventoryItems(textBox7.Text.Trim());
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Search failed. Error: " + ex.Message);
            }
        }

        // ── JOB SELECTION ──
        private void dgvJobs_SelectionChanged_1(object sender, EventArgs e)
        {
            if (dgvJobs.CurrentRow == null || !(dgvJobs.CurrentRow.DataBoundItem is DataRowView drv))
            {
                btnInvoice.Enabled = false;
                button4.Enabled = false;
                textBox1.Clear();
                return;
            }

            DataRow row = drv.Row;
            // Display job details
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Job Details:");
            sb.AppendLine("---------------------------");
            sb.AppendLine($"Job ID:        {row["JobID"]}");
            sb.AppendLine($"Customer:      {row["CustomerName"]}");
            sb.AppendLine($"Service area:  {row["Area"]}");
            sb.AppendLine($"Status:        {row["JobStatus"]}");
            sb.AppendLine($"Promise Date:  {row["PromiseDate"]}");
            bool onHold = Convert.ToBoolean(row["HasPendingChange"]);
            if (onHold)
                sb.AppendLine(Convert.ToBoolean(row["MaterialReportRequired"])
                    ? "Work is on hold. Open Allocations & materials to report the quantities used."
                    : "Work is on hold while the office reviews the booking.");
            sb.AppendLine("---------------------------");
            textBox1.Text = sb.ToString();
            // Enable buttons based on role and job state
            string status = Convert.ToString(row["JobStatus"]);
            bool pending = status == "Pending";
            bool inProgress = status == "In Progress";
            button4.Enabled = !onHold && pending && row["PromiseDate"] != DBNull.Value && Convert.ToDateTime(row["PromiseDate"]).Date <= DateTime.Today;
            btnInvoice.Enabled = !onHold && inProgress;
        }

        // ── GET SELECTED JOB ID ──
        private int GetSelectedJobID()
        {
            if (dgvJobs.CurrentRow == null)
            {
                UIHelper.ShowWarning("Please select a job from the list first.");
                return -1;
            }

            try
            {
                DataRow row = ((DataRowView)dgvJobs.CurrentRow.DataBoundItem).Row;
                return Convert.ToInt32(row["JobID"]);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error retrieving Job ID: " + ex.Message);
                return -1;
            }
        }

        // ── START JOB ──
        private void button4_Click(object sender, EventArgs e)
        {
            int jobID = GetSelectedJobID();
            if (jobID == -1)
                return;
            try
            {
                if (!UserSession.Check())
                    return;
                _jobService.StartTechnicianJob(jobID, _staffID);
                LoadMyJobs();
                UpdateStatButtons();
                UIHelper.ShowSuccess("Job is now In Progress.");
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to start job: " + ex.Message);
            }
        }

        // ── COMPLETE JOB ──
        private void btnInvoice_Click_1(object sender, EventArgs e)
        {
            int jobID = GetSelectedJobID();
            if (jobID == -1)
                return;
            try
            {
                if (!UserSession.Check())
                    return;
                _jobService.CompleteTechnicianJob(jobID, _staffID);
                LoadMyJobs();
                UpdateStatButtons();
                UIHelper.ShowSuccess("Job completed. Your assigned jobs and dashboard have been updated.");
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to complete job: " + ex.Message);
            }
        }

        // ── LOGOUT ──
        private void Logout()
        {
            try
            {
                UserSession.ClearSession();
                // Close the MDI container – this triggers LoginContext to show the login form
                if (this.ParentForm != null)
                {
                    this.ParentForm.Close();
                }
                else
                {
                    // Fallback: close this form and show login directly
                    this.Close();
                    new LoginForm().Show();
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error during logout: " + ex.Message);
            }
        }

        // Attach all logout buttons to the Logout method
        private void button3_Click(object sender, EventArgs e) => Logout();
        private void button11_Click_1(object sender, EventArgs e) => Logout();
        private void button12_Click(object sender, EventArgs e) => Logout();
        // ── STAT BUTTONS ──
        private void button1_Click(object sender, EventArgs e) => OpenJobs("Pending");
        private void button9_Click(object sender, EventArgs e) => OpenJobs("In Progress");
        private void button2_Click(object sender, EventArgs e) => OpenJobs("Completed");
        private void OpenJobs(string status)
        {
            if (!UserSession.Check())
                return;
            _jobStatus.SelectedItem = status;
            tabControl1.SelectedTab = tabPage3;
        }

        // ── HELP ──
        private void OpenHelpGuide(string fileName)
        {
            try
            {
                string pdfPath = Path.Combine(Application.StartupPath, "resources", fileName);
                if (File.Exists(pdfPath))
                    System.Diagnostics.Process.Start(pdfPath);
                else
                    UIHelper.ShowWarning("Help guide not found.");
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Could not open help guide: " + ex.Message);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e) => OpenHelpGuide("Technician_Help_Guide.pdf");
        // ── UNUSED EVENTS (keep for designer) ──
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
        }

        private void button11_Click(object sender, EventArgs e)
        {
        }

        private void bindingSource2_CurrentChanged(object sender, EventArgs e)
        {
        }

        private void label26_Click(object sender, EventArgs e)
        {
        }

        private void groupBox8_Enter(object sender, EventArgs e)
        {
        }

        private void dgvJobs_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

        private void dgvJobs_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {
        }

        private void dgvJobs_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {
        }

        private void DashboardRefresh_Click(object sender, EventArgs e)
        {
            if (!UserSession.Check())
                return;
            LoadMyJobs();
            LoadInventory();
            UpdateStatButtons();
        }
    }
}
