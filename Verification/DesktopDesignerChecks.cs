using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Major_Project.UI.Forms.Common;

internal static class DesktopDesignerChecks
{
    private static int assertions;
    private static void Check(bool condition, string message)
    {
        assertions++;
        if (!condition)
            throw new Exception(message);
    }

    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            Application.EnableVisualStyles();
            CheckDataPresentation();
            CheckDocumentColumns();
            string output = args.Length > 0 ? args[0] : ".";
            Directory.CreateDirectory(output);
            var assembly = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Major_Project.exe"));
            foreach (var viewType in assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(UserControl)) && type.Namespace.StartsWith("Major_Project.UI.Forms.")))
            {
                using (var view = (UserControl)Activator.CreateInstance(viewType))
                {
                    Check(view.Controls.Count > 0, viewType.Name + " opens with its designer layout");
                    if (!viewType.Name.EndsWith("Editor"))
                        continue;
                    var fields = view.Controls.OfType<FlowLayoutPanel>().Single();
                    var inputs = fields.Controls.Cast<Control>().Where(control => control.Name.EndsWith("Field")).ToArray();
                    Check(inputs.Length > 0, viewType.Name + " contains its input controls before opening a job or signing in");
                    foreach (var input in inputs)
                    {
                        string caption = input.Name.Substring(0, input.Name.Length - "Field".Length) + "Label";
                        Check(fields.Controls[caption] is Label && input.Visible, viewType.Name + " displays " + caption);
                    }
                }
            }
            using (var admin = (Form)Activator.CreateInstance(assembly.GetType("Major_Project.UI.Forms.Common.SystemAdminForm", true)))
            {
                var tabs = admin.Controls.OfType<TabControl>().Single();
                Check(tabs.TabPages.Count == 4, "System administration has four designer pages");
                foreach (TabPage page in tabs.TabPages)
                {
                    Check(page.Controls.OfType<DataGridView>().Any(), page.Text + " has a designer grid");
                    Check(page.Controls.OfType<FlowLayoutPanel>().Single().Controls.OfType<Button>().Any(), page.Text + " has designer actions");
                }
            }
            foreach (string role in new[]
            {
                "Admin",
                "Manager",
                "Technician"
            }

            )
            {
                using (var form = (Form)Activator.CreateInstance(assembly.GetType("Major_Project.UI.Forms." + role + "." + role + "Form", true)))
                {
                    // The parameterless constructor initializes only Designer controls: no sign-in or SQL.
                    form.WindowState = FormWindowState.Normal;
                    form.Size = new Size(1240, 920);
                    form.ShowInTaskbar = false;
                    form.StartPosition = FormStartPosition.Manual;
                    form.Location = new Point(-20000, -20000);
                    form.Show();
                    form.PerformLayout();
                    Check(form.Controls.Find("dashboardLayout", true).Length == 1, role + " dashboard is designer-owned");
                    var cards = (TableLayoutPanel)form.Controls.Find("dashboardCards", true).Single();
                    Check(cards.Controls.OfType<Button>().Count() == 3, role + " has three KPI cards");
                    var tabs = (TabControl)form.Controls.Find("tabControl1", true).Single();
                    Check(tabs.Width <= form.ClientSize.Width, role + " navigation fits the form width");
                    var dashboard = form.Controls.Find("panel2", true).Single();
                    Check(dashboard.Controls.Cast<Control>().Where(control => control.Visible).All(control => control.Name == "dashboardLayout"), role + " has no old controls covering the dashboard");
                    var archive = form.Controls.Find("btnDelJob", true).OfType<Button>().FirstOrDefault();
                    if (archive != null)
                        Check(!archive.Enabled && archive.Text == "Archive", role + " archive starts disabled");
                    using (var image = new Bitmap(form.Width, form.Height))
                    {
                        form.DrawToBitmap(image, new Rectangle(Point.Empty, form.Size));
                        image.Save(Path.Combine(output, role + "-designer.png"));
                    }
                }
            }

            foreach (string name in new[]
            {
                "LoginForm",
                "ResetPasswordForm",
                "MDIForm"
            }

            )
            {
                using (var form = (Form)Activator.CreateInstance(assembly.GetType("Major_Project.UI.Forms.Common." + name, true)))
                {
                    form.WindowState = FormWindowState.Normal;
                    form.Size = new Size(1240, 920);
                    form.ShowInTaskbar = false;
                    form.StartPosition = FormStartPosition.Manual;
                    form.Location = new Point(-20000, -20000);
                    if (name == "MDIForm")
                    {
                        var add = form.GetType().GetMethod("AddNavigationButton", BindingFlags.Instance | BindingFlags.NonPublic);
                        var first = (Button)add.Invoke(form, new object[] { "Dashboard" });
                        var second = (Button)add.Invoke(form, new object[] { "Jobs" });
                        Check(first != second, "Navigation binds distinct designer buttons before the form becomes visible");
                        Check(first.Name == "nav1" && second.Name == "nav2", "Navigation uses existing controls");
                    }

                    form.Show();
                    form.PerformLayout();
                    if (name != "MDIForm")
                    {
                        var title = form.Controls.Find("lblAuthTitle", true).Single();
                        Check(title.Width > 100, name + " heading is in Designer");
                        var card = form.Controls.Find("panel1", true).Single();
                        Check(card.Width >= 440 && card.Height >= 460, name + " card fits its controls");
                    }

                    using (var image = new Bitmap(form.Width, form.Height))
                    {
                        form.DrawToBitmap(image, new Rectangle(Point.Empty, form.Size));
                        image.Save(Path.Combine(output, name + "-designer.png"));
                    }
                }
            }

            Console.WriteLine("Desktop designer checks: " + assertions + " passed; no database used.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void CheckDocumentColumns()
    {
        using (var host = new Form())
        using (var grid = new DataGridView
        {
            Dock = DockStyle.Fill
        }

        )
        {
            host.Controls.Add(grid);
            host.ShowInTaskbar = false;
            host.StartPosition = FormStartPosition.Manual;
            host.Location = new Point(-20000, -20000);
            GridPresentation.Attach(host);
            host.Show();
            int errors = 0;
            grid.DataError += (sender, args) =>
            {
                errors++;
                args.ThrowException = false;
                args.Cancel = true;
            };
            var jobs = new DataTable();
            jobs.Columns.Add("JobID", typeof(int));
            jobs.Columns.Add("InvoicePDFData", typeof(byte[]));
            jobs.Columns.Add("Attachment", typeof(byte[]));
            jobs.Columns.Add("InvoiceFilePath");
            byte[] pdf = System.Text.Encoding.ASCII.GetBytes("%PDF-1.7\nNot an image");
            jobs.Rows.Add(1, pdf, pdf, "~/App_Data/Invoices/Invoice-1.pdf");
            jobs.Rows.Add(2, DBNull.Value, DBNull.Value, DBNull.Value);
            for (int pass = 0; pass < 2; pass++)
            {
                // Clear and regenerate columns as the dashboard does on refresh.
                grid.DataSource = null;
                grid.Columns.Clear();
                grid.DataSource = pass == 0 ? jobs : jobs.Copy().DefaultView;
                Application.DoEvents();
                Check(!grid.Columns["InvoicePDFData"].Visible, "PDF payload is hidden on binding " + pass);
                Check(!grid.Columns["Attachment"].Visible, "Other binary data is hidden on binding " + pass);
                Check(!grid.Columns["InvoiceFilePath"].Visible, "Internal storage path is hidden on binding " + pass);
                Check(grid.Columns["JobID"].Visible, "Normal job reference stays visible");
                using (var image = new Bitmap(host.Width, host.Height))
                    host.DrawToBitmap(image, new Rectangle(Point.Empty, host.Size));
                Check(errors == 0, "PDF and null payload rows render without image-conversion errors");
            }

            Check(((DataRowView)grid.Rows[0].DataBoundItem)["InvoicePDFData"] is byte[], "PDF bytes remain available for downloads");
        }
    }

    private static void CheckDataPresentation()
    {
        var inventory = new DataTable();
        inventory.Columns.Add("QuantityInStock", typeof(int));
        inventory.Columns.Add("ReorderLevel", typeof(int));
        inventory.Rows.Add(0, 5);
        inventory.Rows.Add(5, 5);
        inventory.Rows.Add(6, 5);
        inventory.Rows.Add(DBNull.Value, 5);
        var low = GridPresentation.LowStock(inventory);
        Check(low.Rows.Count == 2, "Low stock includes zero and exact reorder boundary, excludes above/null quantities");
        Check(inventory.Rows.Count == 4, "Filtering preserves original inventory");
        using (var host = new Form())
        using (var grid = new DataGridView())
        using (var edit = new Button())
        using (var archive = new Button())
        {
            host.Controls.Add(grid);
            host.CreateControl();
            grid.CreateControl();
            var jobs = new DataTable();
            jobs.Columns.Add("JobID", typeof(int));
            jobs.Columns.Add("CustomerID", typeof(int));
            jobs.Columns.Add("JobStatus");
            jobs.Columns.Add("BaseLabourCost", typeof(decimal));
            jobs.Rows.Add(1, 2, "Pending", 1234.5m);
            jobs.Rows.Add(2, 3, "Completed", 20m);
            jobs.Rows.Add(3, 4, "Cancelled", 20m);
            jobs.Rows.Add(4, 5, "Archived", 20m);
            GridPresentation.StyleGrid(grid);
            grid.DataSource = jobs;
            GridPresentation.FormatColumns(grid);
            Check(grid.Columns["JobID"].Visible && !grid.Columns["CustomerID"].Visible, "Job reference remains visible; foreign key is hidden");
            var money = grid.Columns["BaseLabourCost"];
            Check(money.HeaderText == "Labour" && money.DefaultCellStyle.Format == "C2", "Friendly currency column");
            Check(string.Format(money.DefaultCellStyle.FormatProvider, "{0:C2}", 1234.5m).Contains("R"), "Currency uses rand");
            foreach (DataGridViewRow row in grid.Rows)
            {
                grid.CurrentCell = row.Cells["JobID"];
                GridPresentation.JobActions(grid, edit, archive);
                string status = Convert.ToString(row.Cells["JobStatus"].Value);
                Check(edit.Enabled == (status == "Pending"), "Edit eligibility for " + status);
                Check(archive.Enabled == (status == "Completed" || status == "Cancelled"), "Archive eligibility for " + status);
            }

            grid.DataSource = jobs.Clone();
            GridPresentation.JobActions(grid, edit, archive);
            Check(!edit.Enabled && !archive.Enabled, "Empty result disables selection actions");
            Check(!grid.Columns["CustomerID"].Visible && grid.Columns["BaseLabourCost"].DefaultCellStyle.Format == "C2", "Formatting survives rebinding");
        }
    }
}
