using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

internal static class DesktopCorrectionChecks
{
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    private const BindingFlags Static = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
    private static Assembly desktop;
    private static int checks;
    private static int gridNullReferences;
    private static bool live;
    private static void Capture(Form form, string name)
    {
        string output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CorrectionPreviews");
        Directory.CreateDirectory(output);
        using (var image = new Bitmap(form.Width, form.Height))
        {
            form.DrawToBitmap(image, new Rectangle(Point.Empty, form.Size));
            image.Save(Path.Combine(output, name + ".png"));
        }
    }

    private static Type Common(string name) => desktop.GetType("Major_Project.UI.Forms.Common." + name, true);
    private static object Call(Type type, object target, string name, params object[] args) => type.GetMethod(name, target == null ? Static : Instance).Invoke(target, args);
    private static Control Find(Control owner, string name) => owner.Controls.Find(name, true).Single();
    private static void Check(bool ok, string message)
    {
        checks++;
        if (!ok)
            throw new Exception(message);
    }

    private static void Show(Form form)
    {
        form.ShowInTaskbar = false;
        form.StartPosition = FormStartPosition.Manual;
        form.Location = new Point(-20000, -20000);
        form.WindowState = FormWindowState.Normal;
        form.Size = new Size(1240, 920);
        form.Show();
        Application.DoEvents();
    }

    [STAThread]
    private static int Main(string[] args)
    {
        live = args.Contains("--read-only");
        Application.EnableVisualStyles();
        desktop = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Major_Project.exe"));
        AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
        {
            string stack = e.Exception.StackTrace ?? "";
            if (e.Exception is NullReferenceException && (stack.Contains("DataGridView") || stack.Contains("GridPresentation")))
                gridNullReferences++;
        };
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
                    foreach (string role in new[]
                    {
                        "Manager",
                        "Admin"
                    }

                    )
                    {
                        CheckForm(role);
                        Check(gridNullReferences == 0, role + " has no grid-related first-chance null-reference exceptions");
                    }

                    Console.WriteLine("Desktop correction checks: " + checks + " passed" + (live ? " (read-only database)." : " (fixtures; no database)."));
                    exit = 0;
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
            Application.Run(runner);
        }

        return exit;
    }

    private static void CheckForm(string role)
    {
        var type = desktop.GetType("Major_Project.UI.Forms." + role + "." + role + "Form", true);
        using (var form = (Form)Activator.CreateInstance(type, Instance, null, new object[] { 0, "Fixture" }, null))
        {
            form.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), form, type.GetMethod(role + "_Load", Instance));
            var tabs = (TabControl)Find(form, "tabControl1");
            tabs.SelectedIndexChanged -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), form, type.GetMethod("tabControl1_SelectedIndexChanged", Instance));
            Show(form);
            if (live)
            {
                Call(type, form, role + "_Load", form, EventArgs.Empty);
                Check(!form.Controls.Find("WorkflowFeedback", true).OfType<Label>().Any(label => label.ForeColor == Color.Maroon), role + " full read-only startup has no error feedback");
                Call(type, form, "LoadJobsGrid");
                var grid = (DataGridView)Find(form, "dgvJobs");
                Check(grid.Columns.Contains("JobID"), role + " loads without the Admin column-sizing exception");
                if (grid.Rows.Count > 0)
                {
                    var row = (DataRowView)grid.Rows[0].DataBoundItem;
                    Check(row.Row.Table.Columns.Contains("Services"), "The live job row includes service descriptions");
                    Call(type, form, "OpenJobDetails_Click", null, EventArgs.Empty);
                    Check(tabs.SelectedTab.Name == "tabJobDetails", "Live job opens the full details tab");
                }

                foreach (int days in new[]
                {
                    0,
                    1,
                    7
                }

                )
                {
                    Call(type, form, "PopulateTechnicianCombo", DateTime.Today.AddDays(days));
                    var technician = (ComboBox)Find(form, "comboBox4");
                    Check(technician.ValueMember == "StaffID" && technician.DisplayMember == "FullName", "Technician binding uses StaffID and FullName");
                    Check(technician.Items.Count >= 1 && technician.SelectedIndex == 0, "Even an empty availability result displays an explicit prompt");
                    Console.WriteLine(role + " technician options for selected date +" + days + ": " + (technician.Items.Count - 1));
                }

                if (role == "Manager")
                {
                    Call(type, form, "LoadStaffGrid");
                    var staffGrid = (DataGridView)Find(form, "dataGridView4");
                    Check(staffGrid.Rows.Count > 0, "Live staff list has records");
                    Check(!((TextBox)Find(form, "textBox2")).Text.Contains("JOB INVOICE"), "Live staff selection never displays an invoice");
                    foreach (DataGridViewRow displayed in staffGrid.Rows)
                    {
                        var person = (DataRowView)displayed.DataBoundItem;
                        if (Convert.ToString(person["RoleName"]) == "SysAdmin")
                            continue;
                        staffGrid.CurrentCell = displayed.Cells.Cast<DataGridViewCell>().First(cell => cell.OwningColumn.Visible);
                        Call(type, form, "LoadStaffProfile", Convert.ToInt32(person["StaffID"]));
                        Check(((TextBox)Find(form, "txtFullName")).Text == Convert.ToString(person["FullName"]), "Staff editor uses the selected database profile");
                        Check(((NumericUpDown)Find(form, "numSalary")).Value == Convert.ToDecimal(person["Salary"]), "Staff editor displays the stored salary accurately");
                    }
                }

                return;
            }

            tabs.SelectedTab = (TabPage)Find(form, "tabPage2");
            var cart = new DataTable();
            cart.Columns.Add("JobTypeID", typeof(int));
            cart.Columns.Add("JobTypeName");
            cart.Columns.Add("Quantity", typeof(int));
            foreach (string field in new[]
            {
                "BaseLabourCost",
                "BaseTransportCost",
                "MaterialCost",
                "TaxPercentage"
            }

            )
                cart.Columns.Add(field, typeof(decimal));
            cart.Rows.Add(1, "Fixture installation", 1, 2500m, 350m, 7552m, 15m);
            type.GetField("cartTable", Instance).SetValue(form, cart);
            var cartGrid = (DataGridView)Find(form, "dataGridViewCart");
            cartGrid.DataSource = cart;
            Call(type, form, "RecalculateTotal");
            var total = (TextBox)Find(form, "textBox5");
            Check(total.Text == 11962.30m.ToString("C2", CultureInfo.GetCultureInfo("en-ZA")), "Grand total includes labour, transport, material and VAT");
            Check(!total.Multiline && !total.Text.Contains("Labour") && !total.Text.Contains("Transport"), "The amount box contains one monetary value");
            Check(cartGrid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).All(c => new[] { "JobTypeName", "Quantity", "GrandTotal" }.Contains(c.DataPropertyName.Length == 0 ? c.Name : c.DataPropertyName)), "Cart hides individual cost components");
            Check(cartGrid.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible && c.DataPropertyName == "GrandTotal") == 1, "Exactly one grand-total column is displayed");
            Check(Convert.ToDecimal(cart.Rows[0]["BaseTransportCost"]) == 350m, "Hidden internal pricing remains intact");
            Capture(form, role + "-booking");
            var options = new DataTable();
            options.Columns.Add("StaffID", typeof(int));
            options.Columns.Add("FullName");
            var combo = (ComboBox)Find(form, "comboBox4");
            var availability = (Label)Find(form, "lblTechnicianAvailability");
            Call(Common("DesktopRecordPresentation"), null, "BindTechnicians", combo, options, availability);
            Check(combo.Items.Count == 1 && combo.Text.Contains("No technicians") && !(bool)Call(Common("DesktopRecordPresentation"), null, "HasTechnician", combo), "No availability is explicit and cannot be submitted as a staff ID");
            options.Rows.Add(11, "First Technician Full Name");
            options.Rows.Add(22, "Second Technician Full Name");
            Call(Common("DesktopRecordPresentation"), null, "BindTechnicians", combo, options, availability);
            combo.SelectedValue = 22;
            Check(combo.Text == "Second Technician Full Name", "Full technician name maps to its actual staff key");
            Call(Common("DesktopRecordPresentation"), null, "BindTechnicians", combo, options, availability);
            Check(Convert.ToInt32(combo.SelectedValue) == 22, "Refreshing availability preserves an eligible selection");
            options.Rows.RemoveAt(1);
            Call(Common("DesktopRecordPresentation"), null, "BindTechnicians", combo, options, availability);
            Check(Convert.ToInt32(combo.SelectedValue) == 0, "An unavailable prior selection resets to the prompt");
            string staffText = "";
            if (role == "Manager")
            {
                tabs.SelectedTab = (TabPage)Find(form, "tabPage5");
                var staff = new DataTable();
                staff.Columns.Add("StaffID", typeof(int));
                foreach (string name in new[]
                {
                    "FullName",
                    "RoleName",
                    "PhoneNumber",
                    "EmailAddress",
                    "EmploymentStatus",
                    "Username"
                }

                )
                    staff.Columns.Add(name);
                staff.Columns.Add("HireDate", typeof(DateTime));
                staff.Columns.Add("Salary", typeof(decimal));
                staff.Columns.Add("IsAvailable", typeof(bool));
                staff.Rows.Add(7, "Fixture Manager", "Manager", "0310000001", "manager@example.test", "Active", "fixture-manager", DateTime.Today, 100m, true);
                ((DataGridView)Find(form, "dataGridView4")).DataSource = staff;
                Call(type, form, "UpdateStaffActionState");
                staffText = ((TextBox)Find(form, "textBox2")).Text;
                Check(staffText.Contains("Fixture Manager") && staffText.Contains("manager@example.test") && !staffText.Contains("JOB"), "Staff profile shows the selected staff record");
                Check(staffText.Contains("Not applicable"), "Manager availability is not presented as technician availability");
                var staffGrid = (DataGridView)Find(form, "dataGridView4");
                var column = staffGrid.Columns.Cast<DataGridViewColumn>().Single(c => c.DataPropertyName == "IsAvailable");
                Check(Convert.ToString(staffGrid.Rows[0].Cells[column.Index].FormattedValue) == "Not applicable", "Staff availability cell uses the correct role meaning");
                Capture(form, role + "-staff");
            }

            tabs.SelectedTab = (TabPage)Find(form, "tabPage3");
            var jobs = new DataTable();
            jobs.Columns.Add("JobID", typeof(int));
            foreach (string field in new[]
            {
                "JobStatus",
                "PaymentStatus",
                "CustomerName",
                "TechnicianName",
                "Services",
                "Notes"
            }

            )
                jobs.Columns.Add(field);
            jobs.Rows.Add(123, "Pending", "Unpaid", "Fixture Customer", "Actual Technician Name", "Fixture service x 2", new string ('x', 2000));
            var jobsGrid = (DataGridView)Find(form, "dgvJobs");
            jobsGrid.DataSource = null;
            jobsGrid.Columns.Clear();
            jobsGrid.AutoGenerateColumns = false;
            foreach (DataColumn field in jobs.Columns)
                jobsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "designer_" + field.ColumnName, DataPropertyName = field.ColumnName });
            jobsGrid.DataSource = jobs;
            for (int i = 0; i < 20; i++)
                Call(Common("GridPresentation"), null, "FormatColumns", jobsGrid);
            Check(jobsGrid.Columns["designer_TechnicianName"].Visible, "Formatting recognizes bound field names, not just Designer names");
            Console.WriteLine(role + " tabs: " + string.Join(", ", tabs.TabPages.Cast<TabPage>().Select(page => page.Name)));
            Console.WriteLine("Detail tab attached: " + (((TabPage)type.GetField("tabJobDetails", Instance).GetValue(form)).Parent == tabs));
            Call(type, form, "OpenJobDetails_Click", null, EventArgs.Empty);
            Check(tabs.SelectedTab.Name == "tabJobDetails", "Job Details is a real tab");
            var details = (TextBox)Find(form, role == "Manager" ? "txtMgmtDetails" : "textBox2");
            Check(details.Text.Contains("Actual Technician Name") && details.Text.Contains("Fixture service x 2") && details.Text.Contains(new string ('x', 2000)), "Full technician, services and long notes appear in details");
            Check(details.Height > form.ClientSize.Height * 0.7 && details.ScrollBars == ScrollBars.Vertical, "Details use the full page with scrolling");
            if (role == "Manager")
                Check(((TextBox)Find(form, "textBox2")).Text == staffText, "Selecting a job cannot overwrite the staff profile");
            Call(type, form, "BackToJobs_Click", null, EventArgs.Empty);
            Check(tabs.SelectedTab.Name == "tabPage3", "Back to Jobs preserves the list");
            string output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CorrectionPreviews");
            Directory.CreateDirectory(output);
            tabs.SelectedTab = (TabPage)Find(form, "tabJobDetails");
            using (var image = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(image, new Rectangle(Point.Empty, form.Size));
                image.Save(Path.Combine(output, role + "-job-details.png"));
            }
        }
    }
}
