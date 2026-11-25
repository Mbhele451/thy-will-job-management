using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

internal static class DesktopStateAudit
{
    private const BindingFlags Members = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    private static Assembly desktop;
    private static int passed, failed;
    private static readonly List<string> results = new List<string>();
    private static Type Common(string name) => desktop.GetType("Major_Project.UI.Forms.Common." + name, true);
    private static object Call(Type type, object owner, string name, params object[] args) => type.GetMethod(name, Members).Invoke(owner, args);
    private static IEnumerable<Control> Controls(Control root)
    {
        foreach (Control child in root.Controls)
        {
            yield return child;
            foreach (var nested in Controls(child))
                yield return nested;
        }
    }

    private static Control Find(Control root, string name) => Controls(root).Single(c => c.Name == name);
    private static void Check(bool ok, string name)
    {
        if (!ok)
            throw new Exception(name);
        passed++;
    }

    private static void Test(string name, Action action)
    {
        try
        {
            action();
            results.Add("PASS " + name);
            Console.WriteLine("PASS " + name);
        }
        catch (Exception error)
        {
            while (error is TargetInvocationException && error.InnerException != null)
                error = error.InnerException;
            if (error is AggregateException aggregate)
                error = aggregate.Flatten().InnerExceptions[0];
            failed++;
            results.Add("FAIL " + name + ": " + error.Message);
            Console.WriteLine(results.Last());
            results.Add(error.ToString());
        }
    }

    private static Form NewForm(Type type, params object[] args) => (Form)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, args, null);
    private static void RemoveLoad(Form form, string method)
    {
        var handler = (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), form, form.GetType().GetMethod(method, Members));
        form.Load -= handler;
    }

    private static void Show(Form form)
    {
        form.ShowInTaskbar = false;
        form.WindowState = FormWindowState.Normal;
        form.StartPosition = FormStartPosition.Manual;
        form.Location = new Point(-20000, -20000);
        form.Size = new Size(1240, 920);
        form.Show();
        Application.DoEvents();
    }

    private static void Pump(Func<bool> done)
    {
        DateTime end = DateTime.UtcNow.AddSeconds(6);
        while (!done() && DateTime.UtcNow < end)
        {
            Application.DoEvents();
            Thread.Sleep(5);
        }

        Check(done(), "UI transition completes without a stuck operation");
    }

    private static DataTable Jobs(string payment = "Pending")
    {
        var table = new DataTable();
        foreach (var pair in new[]
        {
            "JobID:int",
            "CustomerID:int",
            "StaffID:int",
            "CustomerName:string",
            "TechnicianName:string",
            "JobStatus:string",
            "PaymentStatus:string",
            "PromiseDate:date",
            "GrandTotal:decimal",
            "HasInvoicePdf:bool",
            "InvoiceFilePath:string",
            "LabourCost:decimal",
            "TotalMaterialCost:decimal",
            "TransportCost:decimal",
            "TaxAmount:decimal",
            "Notes:string",
            "Area:string"
        }

        )
        {
            var parts = pair.Split(':');
            Type type = parts[1] == "int" ? typeof(int) : parts[1] == "date" ? typeof(DateTime) : parts[1] == "decimal" ? typeof(decimal) : parts[1] == "bool" ? typeof(bool) : typeof(string);
            table.Columns.Add(parts[0], type);
        }

        table.Rows.Add(1, 1, 1, "Fixture Customer", "Fixture Technician", "Pending", "Unpaid", DateTime.Today.AddDays(1), 100m, true, "fixture.pdf", 70m, 10m, 10m, 10m, "Fixture", "Durban");
        table.Rows.Add(2, 1, 1, "Fixture Customer", "Fixture Technician", "Completed", payment, DateTime.Today, 100m, true, "fixture.pdf", 70m, 10m, 10m, 10m, "Fixture", "Durban");
        return table;
    }

    private static void Bind(DataGridView grid, BindingSource source, DataTable rows) => Call(Common("GridPresentation"), null, "BindJobs", grid, source, rows);
    [STAThread]
    private static int Main(string[] args)
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        Application.EnableVisualStyles();
        int exit = 1;
        using (var runner = new Form
        {
            ShowInTaskbar = false,
            StartPosition = FormStartPosition.Manual,
            Location = new Point(-20000, -20000)
        }

        )
        {
            runner.Shown += (sender, e) => runner.BeginInvoke(new Action(() =>
            {
                try
                {
                    exit = RunTests(args);
                }
                catch (Exception error)
                {
                    Console.Error.WriteLine(error);
                }
                finally
                {
                    runner.Close();
                }
            }));
            // A real message loop is necessary for repeated async WinForms workflows.
            // Calling DoEvents without Application.Run uninstalls the UI context.
            Application.Run(runner);
        }

        return exit;
    }

    private static int RunTests(string[] args)
    {
        desktop = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Major_Project.exe"));
        if (args.Contains("--read-only"))
        {
            Test("Actual Manager/Admin repeated job refreshes against configured database (read-only)", LiveRefresh);
        }
        else
        {
            Test("Repeated binding, cash Paid refresh, filters, empty results and selection preservation", BindingStates);
            Test("Staff action eligibility across job/payment/document states", ActionStates);
            Test("Customer payment/document and staff-selection eligibility", GenericActionStates);
            foreach (string role in new[]
            {
                "Admin",
                "Manager",
                "Technician",
                "SystemAdmin"
            }

            )
            {
                string current = role;
                Test(current + " workspace: every tab, empty grids, inline validation/save/back/Escape", () => Workspace(current));
            }

            Test("Inline nested dialogs, tab switching and owner disposal", InlineLifetime);
            Test("Cash: exact, change, partial, invalid, back, expiry, rejection and failure", CashStates);
            Test("Technician start/complete/cancel eligibility and date boundaries", TechnicianStates);
            Test("Sign-in, password masking, recovery validation and return feedback", AuthenticationStates);
            Test("Registration, cash, booking detail, PDF and reports controls initialize safely", AuxiliaryForms);
        }

        string report = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, args.Contains("--read-only") ? "desktop-live-state-results.txt" : "desktop-state-results.txt");
        results.Add("Assertions passed: " + passed + "; failed groups: " + failed);
        File.WriteAllLines(report, results);
        Console.WriteLine(results.Last());
        return failed == 0 ? 0 : 1;
    }

    private static void BindingStates()
    {
        using (var form = new Form())
        using (var grid = new DataGridView
        {
            Name = "dgvJobs",
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false
        }

        )
        using (var source = new BindingSource())
        {
            form.Controls.Add(grid);
            Show(form);
            Call(Common("GridPresentation"), null, "Attach", form);
            Bind(grid, source, Jobs());
            grid.CurrentCell = grid.Rows[1].Cells["JobID"];
            for (int i = 0; i < 15; i++)
            {
                Bind(grid, source, Jobs(i == 0 ? "Pending" : "Paid"));
                Check(grid.Rows.Count == 2 && grid.Columns["PaymentStatus"].Visible, "Refresh retains rows and columns");
                Check(Convert.ToInt32(grid.CurrentRow.Cells["JobID"].Value) == 2, "Refresh keeps the collected job selected");
                Check(Convert.ToString(grid.Rows[1].Cells["PaymentStatus"].Value) == (i == 0 ? "Pending" : "Paid"), "Refreshed payment status is current");
            }

            source.Filter = "JobStatus = 'Completed'";
            Bind(grid, source, Jobs("Paid"));
            Check(grid.Rows.Count == 1 && source.Filter.Contains("Completed"), "A status filter survives payment refresh");
            source.Filter = "JobStatus = 'Cancelled'";
            Bind(grid, source, Jobs("Paid"));
            Check(grid.Rows.Count == 0 && grid.Columns.Contains("JobID") && grid.CurrentRow == null, "An empty filter keeps usable columns without stale selection");
            source.Filter = null;
            Bind(grid, source, Jobs("Paid"));
            Check(grid.Rows.Count == 2, "Clearing a filter restores every job");
            grid.CurrentCell = grid.Rows[1].Cells["JobID"];
            var removed = Jobs("Paid");
            removed.Rows.RemoveAt(1);
            Bind(grid, source, removed);
            Check(grid.Rows.Count == 1 && Convert.ToInt32(grid.CurrentRow.Cells["JobID"].Value) == 1, "Removing the selected job chooses another valid row");
            Bind(grid, source, Jobs().Clone());
            Check(grid.Rows.Count == 0, "Empty server result is handled");
            Bind(grid, source, Jobs());
            Check(grid.Rows.Count == 2, "Populating after an empty server result restores rows");
        }
    }

    private static void ActionStates()
    {
        using (var owner = new Form())
        {
            var buttons = Enumerable.Range(0, 6).Select(i => new Button()).ToArray();
            foreach (var button in buttons)
                owner.Controls.Add(button);
            foreach (string status in new[]
            {
                "",
                "Pending",
                "In Progress",
                "Completed",
                "Cancelled",
                "Archived",
                "Unknown"
            }

            )
                foreach (string payment in new[]
                {
                    "",
                    "Unpaid",
                    "Pending",
                    "Paid",
                    "Failed"
                }

                )
                    foreach (bool pdf in new[]
                    {
                        false,
                        true
                    }

                    )
                    {
                        Call(Common("StaffJobActions"), null, "ApplyActionState", status, payment, pdf, buttons[0], buttons[1], buttons[2], buttons[3], buttons[4], buttons[5]);
                        Check(buttons[0].Enabled == (status == "Pending" || status == "In Progress"), "Cancel status rule");
                        Check(buttons[1].Enabled == (status == "Completed" && (payment == "Pending" || payment == "Unpaid")), "Cash status rule");
                        Check(buttons[2].Enabled == pdf && buttons[4].Enabled == pdf, "Invoice availability rule");
                        bool hasReceipt = (status == "Completed" || status == "Archived") && payment == "Paid";
                        Check(buttons[3].Enabled == hasReceipt && buttons[5].Enabled == hasReceipt, "Receipt payment rule");
                    }
        }
    }

    private static bool Eligible(string caption, DataRow row) => (bool)Call(Common("WorkflowUi"), null, "SelectionEligible", caption, row);
    private static void GenericActionStates()
    {
        foreach (string caption in new[]
        {
            "Pay Invoice",
            "Job Details",
            "Save Invoice PDF",
            "Save Receipt PDF",
            "Edit Customer",
            "Edit Setting",
            "Revoke Session",
            "Remove",
            "Set Quantity"
        }

        )
            Check(!Eligible(caption, null), caption + " requires a selection");
        var row = Jobs().Rows[1];
        foreach (string status in new[]
        {
            "Pending",
            "In Progress",
            "Completed",
            "Cancelled",
            "Archived"
        }

        )
            foreach (string payment in new[]
            {
                "Unpaid",
                "Pending",
                "Paid",
                "Failed"
            }

            )
            {
                row["JobStatus"] = status;
                row["PaymentStatus"] = payment;
                Check(Eligible("Pay Invoice", row) == (status == "Completed" && (payment == "Unpaid" || payment == "Pending")), "Customer checkout state rule");
                Check(Eligible("Save Receipt PDF", row) == ((status == "Completed" || status == "Archived") && payment == "Paid"), "Customer receipt state rule");
            }

        row["JobStatus"] = "Completed";
        row["PaymentStatus"] = "Pending";
        row["InvoiceFilePath"] = "";
        row["HasInvoicePdf"] = false;
        Check(!Eligible("Pay Invoice", row), "Missing document cannot enter checkout");
        var staff = new DataTable();
        staff.Columns.Add("RoleName");
        staff.Columns.Add("EmploymentStatus");
        foreach (string role in new[]
        {
            "Administrator",
            "Manager",
            "Technician",
            "SysAdmin"
        }

        )
            foreach (string status in new[]
            {
                "Active",
                "Inactive"
            }

            )
            {
                var person = staff.NewRow();
                person[0] = role;
                person[1] = status;
                staff.Rows.Add(person);
                Check(Eligible("Set Availability", person) == (role == "Technician" && status == "Active"), "Only active technicians expose availability");
                Check(Eligible("Reset Staff Password", person) == (role != "SysAdmin"), "SysAdmin is protected from Manager password actions");
            }
    }

    private static void Workspace(string role)
    {
        bool special = role == "SystemAdmin";
        var type = special ? Common(role + "Form") : desktop.GetType("Major_Project.UI.Forms." + role + "." + role + "Form", true);
        object[] args = role == "SystemAdmin" ? new object[0] : new object[]
        {
            1,
            "Fixture staff"
        };
        using (var workspace = NewForm(type, args))
        using (var host = NewForm(Common("MDIForm")))
        {
            RemoveLoad(host, "MDIForm_Load");
            if (!special)
                RemoveLoad(workspace, role + "_Load");
            if (role == "Admin" || role == "Manager")
                ((TabControl)type.GetProperty("NavigationTabs", Members).GetValue(workspace)).SelectedIndexChanged -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), workspace, type.GetMethod("tabControl1_SelectedIndexChanged", Members));
            Show(host);
            var tabs = (TabControl)type.GetProperty("NavigationTabs", Members).GetValue(workspace);
            Call(host.GetType(), host, "OpenWorkspace", workspace, tabs, role == "Manager");
            var navigation = Controls(host).OfType<FlowLayoutPanel>().Single(c => c.Name == "_navigation");
            Check(navigation.Controls.OfType<Button>().Count(b => b.Visible) == tabs.TabPages.Count + (role == "Manager" ? 1 : 0), role + " navigation covers its pages");
            int beforeNavigation = tabs.SelectedIndex;
            navigation.Controls.OfType<Button>().Where(button => button.Visible).Skip(1).First().PerformClick();
            Check(tabs.SelectedIndex == beforeNavigation, "Workspace navigation rejects a missing session");
            foreach (TabPage page in tabs.TabPages)
            {
                tabs.SelectedTab = page;
                Application.DoEvents();
                Check(page.Visible, role + " / " + page.Text + " opens");
                if (page.Name == "JobChanges")
                {
                    bool technician = role == "Technician";
                    Check(Find(page, "RequestHistoryButton").Visible == !technician, role + " request-history access");
                    Check(Find(page, "ReviewSelectedButton").Visible == !technician, role + " review access");
                    Check(Find(page, "MaterialReportsButton").Visible == technician, role + " technician reports access");
                    Check(Find(page, "NewAllocationsButton").Visible == technician, role + " allocation notifications access");
                }
                foreach (var grid in Controls(page).OfType<DataGridView>())
                    Check(grid.ReadOnly && !grid.AllowUserToAddRows, role + " / " + page.Text + " table protects bound records");
                InlineStates(page);
            }

            if (role == "Manager" || role == "Admin")
            {
                tabs.SelectedIndex = 1;
                ((Button)Find(workspace, "button6")).PerformClick();
                Check(Find(workspace, "button6").Enabled, "Invalid job submission releases the save button");
                Check(Controls(tabs.SelectedTab).Any(c => c.Name == "WorkflowFeedback" && c.Visible && c.Text != "Status message"), "Invalid job submission displays validation feedback");
                IdentityStates(workspace);
                var filter = (ComboBox)Find(workspace, "comboBox5");
                if (filter.Items.Count == 0)
                    filter.Items.Add("All"); // Load is intentionally detached in this fixture.
                filter.SelectedIndex = 0;
                filter.SelectedIndex = -1;
                var binding = (BindingSource)type.GetField("jobsViewBindingSource", Members).GetValue(workspace);
                Check(binding.Filter == "JobStatus <> 'Archived'", "Clearing the status dropdown safely restores the All filter");
            }


        }
    }

    private static Panel Panel(string title) => (Panel)Activator.CreateInstance(Common("WorkflowPanel"), Members, null, new object[] { title }, null);
    private static Task<bool> Open(Panel panel, Control owner) => (Task<bool>)Call(panel.GetType(), panel, "OpenAsync", owner);
    private static Button Back(Panel panel) => Controls(panel).OfType<Button>().Single(button => button.Text == "Back");
    private static void InlineStates(Control owner)
    {
        var original = owner.Controls.Cast<Control>().Where(c => c.Visible).ToArray();
        using (var panel = (Panel)Activator.CreateInstance(Common("WorkflowPanel"), Members, null, new object[] { "State audit", Activator.CreateInstance(Common("SettingEditor")) }, null))
        {
            var input = (TextBox)Call(panel.GetType(), panel, "Input", "Setting value", 100, "", false);
            int saves = 0;
            var submit = (Button)Call(panel.GetType(), panel, "Submit", "Save fixture", (Action)(() =>
            {
                if (input.Text.Length == 0)
                    throw new ArgumentException("Enter a value.");
                saves++;
            }), false);
            var done = Open(panel, owner);
            Check(original.All(c => !c.Visible), "Opening inline view hides the underlying page");
            submit.PerformClick();
            Check(!done.IsCompleted && submit.Enabled, "Validation failure permits correction and retry");
            input.Text = "Fixture";
            submit.PerformClick();
            submit.PerformClick();
            Pump(() => done.IsCompleted);
            Check(done.Result && saves == 1 && original.All(c => c.Visible), "Submit once restores the original page");
        }

        using (var panel = Panel("Back audit"))
        {
            var done = Open(panel, owner);
            Back(panel).PerformClick();
            Pump(() => done.IsCompleted);
            Check(!done.Result && original.All(c => c.Visible), "Back restores the original page");
        }

        using (var panel = Panel("Escape audit"))
        {
            var done = Open(panel, owner);
            Call(panel.GetType(), panel, "ProcessDialogKey", Keys.Escape);
            Pump(() => done.IsCompleted);
            Check(!done.Result && original.All(c => c.Visible), "Escape restores the original page");
        }
    }

    private static void IdentityStates(Form workspace)
    {
        var type = (ComboBox)Find(workspace, "cmbCustomerType");
        var document = (ComboBox)Find(workspace, "cmbIdentityDocument");
        type.SelectedItem = "Residential";
        document.SelectedIndex = 1;
        ((TextBox)Find(workspace, "txtPassportNumber")).Text = "PASS123";
        ((TextBox)Find(workspace, "txtPassportCountry")).Text = "France";
        var identity = workspace.GetType().GetField("_customerIdentity", Members).GetValue(workspace);
        Call(identity.GetType(), identity, "Validate");
        Check(Find(workspace, "txtPassportNumber").Visible && !Find(workspace, "txtSouthAfricanIDNumber").Visible, "Passport branch hides SA ID");
        type.Enabled = false;
        Check(!document.Enabled && !Find(workspace, "txtPassportNumber").Enabled, "Editing existing contact locks identity");
        type.Enabled = true;
        type.SelectedItem = "Business";
        Check(!Find(workspace, "txtPassportNumber").Visible && Find(workspace, "txtCompanyRegistrationNumber").Visible, "Business branch hides individual document fields");
    }

    private static void InlineLifetime()
    {
        using (var owner = new Form())
        {
            var tabs = (TabControl)Activator.CreateInstance(Common("WorkspaceTabs"));
            tabs.Dock = DockStyle.Fill;
            tabs.TabPages.Add("One");
            tabs.TabPages.Add("Two");
            owner.Controls.Add(tabs);
            Show(owner);
            using (var outer = Panel("Outer"))
            using (var inner = Panel("Inner"))
            {
                var first = Open(outer, tabs.TabPages[0]);
                var second = Open(inner, tabs.TabPages[0]);
                Back(inner).PerformClick();
                Pump(() => second.IsCompleted);
                Check(outer.Visible, "Nested back restores the outer dialog");
                tabs.SelectedIndex = 1;
                Check(!outer.Visible, "Switching tab hides its pending dialog");
                tabs.SelectedIndex = 0;
                Check(outer.Visible, "Returning to the tab restores its pending dialog");
                Back(outer).PerformClick();
                Pump(() => first.IsCompleted);
            }

            var pending = Panel("Closing owner");
            var task = Open(pending, tabs.SelectedTab);
            owner.Close();
            pending.Dispose();
            Pump(() => task.IsCompleted);
            Check(!task.Result, "Closing owner cancels an awaiting dialog");
        }
    }

    private static void CashStates()
    {
        foreach (string scenario in new[]
        {
            "exact",
            "change",
            "partial",
            "invalid",
            "back",
            "expired",
            "rejected",
            "exception"
        }

        )
            using (var owner = new Form())
            using (var source = new BindingSource())
            {
                Console.WriteLine("Cash scenario: " + scenario);
                var tabs = (TabControl)Activator.CreateInstance(Common("WorkspaceTabs"));
                tabs.Dock = DockStyle.Fill;
                tabs.TabPages.Add("Jobs");
                owner.Controls.Add(tabs);
                var page = tabs.TabPages[0];
                var grid = new DataGridView
                {
                    Name = "dgvJobs",
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false
                };
                page.Controls.Add(grid);
                Show(owner);
                Call(Common("GridPresentation"), null, "Attach", page);
                Bind(grid, source, Jobs());
                grid.CurrentCell = grid.Rows[1].Cells["JobID"];
                int calls = 0;
                decimal received = 0;
                bool session = true;
                Func<decimal, DataTable> record = amount =>
                {
                    calls++;
                    received = amount;
                    if (scenario == "exception")
                        throw new InvalidOperationException("Fixture service failure");
                    var result = new DataTable();
                    result.Columns.Add("IsSuccessful", typeof(bool));
                    result.Columns.Add("Message");
                    result.Columns.Add("ChangeDue", typeof(decimal));
                    result.Rows.Add(scenario != "rejected", "Fixture rejection", Math.Max(0, amount - 100));
                    return result;
                };
                var cash = (Task<bool>)Call(Common("StaffJobActions"), null, "CollectCashAsync", page, 2, 100m, (Func<bool>)(() => session), record);
                Func<Control> view = () => Controls(page).FirstOrDefault(c => c.GetType() == Common("CashCollectionView"));
                Pump(() => view() != null);
                if (scenario == "back")
                    Back((Panel)Find(page, "InlineWorkflow")).PerformClick();
                else
                {
                    if (scenario == "expired")
                        session = false;
                    var initialView = view();
                    var input = (TextBox)Find(initialView, "Amount");
                    var submit = (Button)initialView.GetType().GetField("SubmitButton", Members).GetValue(initialView);
                    if (scenario == "invalid")
                    {
                        input.Text = "-1";
                        submit.PerformClick();
                        Check(!cash.IsCompleted && submit.Enabled && calls == 0, "Invalid cash keeps the form open without a payment");
                    }

                    input.Text = scenario == "change" ? "120" : scenario == "partial" ? "40" : "100";
                    submit.PerformClick();
                    submit.PerformClick();
                    if (scenario == "expired")
                    {
                        Check(!cash.IsCompleted && calls == 0, "An expired session cannot record cash");
                        Back((Panel)Find(page, "InlineWorkflow")).PerformClick();
                    }

                    if (scenario == "partial")
                    {
                        Pump(() => cash.IsCompleted || (view() != null && view() != initialView));
                        if (cash.IsFaulted)
                            throw cash.Exception.Flatten().InnerExceptions[0];
                        Check(!cash.IsCompleted, "Partial cash opens the additional-amount state");
                        Check(calls == 0, "Partial cash is accumulated without an early ledger write");
                        var next = view();
                        ((TextBox)Find(next, "Amount")).Text = "60";
                        var nextSubmit = (Button)next.GetType().GetField("SubmitButton", Members).GetValue(next);
                        Console.WriteLine("Partial cash next state: visible=" + next.Visible + ", enabled=" + nextSubmit.Enabled + ", selectable=" + nextSubmit.CanSelect);
                        nextSubmit.PerformClick();
                    }
                }

                Pump(() => cash.IsCompleted);
                bool success = scenario != "back" && scenario != "expired" && scenario != "rejected" && scenario != "exception";
                Check(grid.Visible && Controls(page).All(c => c.Name != "InlineWorkflow"), "Cash " + scenario + " restores the jobs page");
                if (scenario == "exception")
                    Check(cash.IsFaulted, "A payment-service exception is surfaced");
                else
                    Check(cash.Result == success, "Cash " + scenario + " returns the correct outcome");
                Check(calls == (scenario == "back" || scenario == "expired" ? 0 : 1), "Cash action is recorded at most once");
                if (success)
                {
                    Bind(grid, source, Jobs("Paid"));
                    Check(grid.Rows.Count == 2 && grid.Columns.Contains("JobID") && Convert.ToString(grid.CurrentRow.Cells["PaymentStatus"].Value) == "Paid", "After successful cash, refreshed jobs remain listed and selected");
                    Check(received == (scenario == "change" ? 120m : 100m), "Cash totals match entered amounts");
                }
            }
    }

    private static void TechnicianStates()
    {
        var type = desktop.GetType("Major_Project.UI.Forms.Technician.TechnicianForm", true);
        using (var tech = NewForm(type))
        {
            Show(tech);
            var tabs = (TabControl)Find(tech, "tabControl1");
            tabs.SelectedTab = (TabPage)Find(tech, "tabPage3");
            var grid = (DataGridView)Find(tech, "dgvJobs");
            grid.AutoGenerateColumns = true;
            grid.Columns.Clear();
            foreach (string state in new[]
            {
                "Pending",
                "In Progress",
                "Completed",
                "Cancelled",
                "Archived"
            }

            )
                foreach (int days in new[]
                {
                    -1,
                    0,
                    1
                }

                )
                {
                    var jobs = Jobs();
                    jobs.Columns.Add(new DataColumn("HasPendingChange", typeof(bool)) { DefaultValue = false });
                    jobs.Columns.Add(new DataColumn("MaterialReportRequired", typeof(bool)) { DefaultValue = false });
                    jobs.Rows[0]["JobStatus"] = state;
                    jobs.Rows[0]["PromiseDate"] = DateTime.Today.AddDays(days);
                    grid.DataSource = jobs;
                    grid.CurrentCell = grid.Rows[0].Cells["JobID"];
                    Call(type, tech, "dgvJobs_SelectionChanged_1", grid, EventArgs.Empty);
                    Check(Find(tech, "button4").Enabled == (state == "Pending" && days <= 0), "Technician start-date/state rule");
                    Check(Find(tech, "btnInvoice").Enabled == (state == "In Progress"), "Technician completion rule");
                    Check(tech.Controls.Find("_cancelJob", true).Length == 0, "Technician cannot initiate a change");
                    jobs.Rows[0]["HasPendingChange"] = true;
                    jobs.Rows[0]["MaterialReportRequired"] = state == "In Progress";
                    Call(type, tech, "dgvJobs_SelectionChanged_1", grid, EventArgs.Empty);
                    Check(!Find(tech, "button4").Enabled && !Find(tech, "btnInvoice").Enabled, "Technician cannot start or finish work while it is on hold");
                }

            var assigned = Jobs().DefaultView.ToTable(false, "JobID", "CustomerName", "Area", "PromiseDate", "JobStatus");
            assigned.Columns.Add(new DataColumn("HasPendingChange", typeof(bool)) { DefaultValue = false });
            assigned.Columns.Add(new DataColumn("MaterialReportRequired", typeof(bool)) { DefaultValue = false });
            Call(type, tech, "BindMyJobs", assigned);
            Check(assigned.Rows.Count == grid.Rows.Count && (!grid.Columns.Contains("HasPendingChange") || !grid.Columns["HasPendingChange"].Visible), "Technician grid binds jobs without exposing change tracking fields");
            grid.DataSource = Jobs().Clone();
            Call(type, tech, "dgvJobs_SelectionChanged_1", grid, EventArgs.Empty);
            Check(!Find(tech, "button4").Enabled && !Find(tech, "btnInvoice").Enabled, "Empty technician results disable all mutations");
        }
    }

    private static void AuthenticationStates()
    {
        using (var login = NewForm(Common("LoginForm")))
        {
            Show(login);
            var password = (TextBox)Find(login, "textBox2");
            Check(password.UseSystemPasswordChar, "Sign-in password starts masked");
            ((CheckBox)Find(login, "checkBox1")).Checked = true;
            Check(!password.UseSystemPasswordChar, "Show password works");
            ((CheckBox)Find(login, "checkBox1")).Checked = false;
            Check(password.UseSystemPasswordChar, "Hide password works");
            ((Button)Find(login, "button1")).PerformClick();
            Check(Find(login, "statusLabel").Visible && Find(login, "statusLabel").Text.Contains("Enter your"), "Blank sign-in displays inline validation");
            Check(!Controls(login).Any(c => c.Name == "lnkRegisterCustomer" && c.Visible), "Desktop does not offer customer self-registration");
            Call(login.GetType(), login, "linkLabel1_LinkClicked", null, null);
            var recovery = Controls(login).OfType<Form>().Single(form => form.GetType() == Common("ResetPasswordForm"));
            Check(!Find(recovery, "button2").Enabled && !Find(recovery, "textBox2").Enabled, "Recovery requires a requested code");
            ((TextBox)Find(recovery, "textBox1")).Text = "not-an-email";
            ((Button)Find(recovery, "button1")).PerformClick();
            Check(Find(recovery, "button1").Enabled, "Invalid recovery email permits retry without sending mail");
            recovery.GetType().GetProperty("ResetSucceeded", Members).SetValue(recovery, true);
            recovery.Close();
            Pump(() => Controls(login).All(c => c.Name != "InlineWorkflow"));
            Pump(() => Find(login, "panel1").Visible);
            Pump(() => Controls(login).OfType<Label>().Any(label => label.Name == "WorkflowFeedback" && label.Text.Contains("Password reset successfully")));
            Check(Find(login, "panel1").Visible, "Recovery close restores sign-in");
            Check(((Label)Find(login, "WorkflowFeedback")).Text.Contains("Password reset successfully"), "Successful recovery feedback survives return to sign-in");
        }
    }

    private static void AuxiliaryForms()
    {
        foreach (string name in new[]
        {
            "RegistrationView",
            "CashCollectionView",
            "BookingDetailsView"
        }

        )
            using (var view = (Control)Activator.CreateInstance(Common(name)))
            {
                Check(view.Controls.Count > 0, name + " has Designer controls");
                foreach (var grid in Controls(view).OfType<DataGridView>())
                    Check(!grid.AllowUserToAddRows, name + " has protected rows");
            }

        using (var pdf = NewForm(Common("PdfViewerForm")))
        {
            Show(pdf);
            Check(Find(pdf, "btnCloseViewer").Enabled, "PDF viewer has a working close control");
            pdf.Close();
        }

        var reportType = desktop.GetType("Major_Project.UI.Forms.Reports.ReportsForm", true);
        using (var report = NewForm(reportType))
        {
            RemoveLoad(report, "Reports_Load");
            Show(report);
            Check(Find(report, "webViewReport").Dock == DockStyle.Fill, "Reports browser fits the form");
        }
    }

    private static void LiveRefresh()
    {
        foreach (string role in new[]
        {
            "Manager",
            "Admin"
        }

        )
        {
            var type = desktop.GetType("Major_Project.UI.Forms." + role + "." + role + "Form", true);
            using (var form = NewForm(type, 0, "Read-only verification"))
            {
                RemoveLoad(form, role + "_Load");
                var tabs = (TabControl)type.GetProperty("NavigationTabs", Members).GetValue(form);
                tabs.SelectedIndexChanged -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), form, type.GetMethod("tabControl1_SelectedIndexChanged", Members));
                Show(form);
                tabs.SelectedTab = (TabPage)Find(form, "tabPage3");
                var grid = (DataGridView)Find(form, "dgvJobs");
                int count = -1;
                for (int pass = 0; pass < 5; pass++)
                {
                    Call(type, form, "LoadJobsGrid");
                    Application.DoEvents();
                    Check(grid.Columns.Contains("PaymentStatus") && grid.Columns.Contains("JobID"), role + " refresh retains columns");
                    if (count < 0)
                        count = grid.Rows.Count;
                    Check(grid.Rows.Count == count, role + " repeated refresh retains every job");
                }

                Console.WriteLine(role + " read-only repeated refresh: " + count + " jobs remain listed.");
            }
        }
    }
}
