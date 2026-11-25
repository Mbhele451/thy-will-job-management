using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Major_Project.UI.Forms.Common;
using Major_Project.UI.Forms.Manager;
using Major_Project.UI.Forms.Admin;
using MajorProject.Business.Models.DTOs;
using MajorProject.Business.Services;
using MajorProject.Data.Data;

internal static class DesktopWorkflowChecks
{
    private static int checks;
    private static void Check(bool ok, string message)
    {
        checks++;
        if (!ok)
            throw new Exception(message);
    }

    private static Control Find(Control owner, string name) => owner.Controls.Find(name, true).Single();
    private static void OpenOffscreen(Form form)
    {
        form.ShowInTaskbar = false;
        form.WindowState = FormWindowState.Normal;
        form.StartPosition = FormStartPosition.Manual;
        form.Size = new Size(1240, 920);
        form.Location = new Point(-20000, -20000);
        form.Show();
        Application.DoEvents();
    }

    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            if (args.Contains("--database-only"))
            {
                CheckDatabase();
                return 0;
            }

            Application.EnableVisualStyles();
            string output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WorkflowPreviews");
            Directory.CreateDirectory(output);
            using (var manager = new ManagerForm())
            {
                OpenOffscreen(manager);
                var staff = new DataTable();
                staff.Columns.Add("StaffID", typeof(int));
                staff.Columns.Add("FullName");
                staff.Columns.Add("PhoneNumber");
                staff.Columns.Add("RoleName");
                staff.Rows.Add(1, "Sample Manager", "0310000001", "Manager");
                staff.Rows.Add(2, "Sample Technician A", "0310000002", "Technician");
                staff.Rows.Add(3, "Sample Technician B", "0310000003", "Technician");
                var grid = (DataGridView)Find(manager, "dataGridView4");
                ((TabControl)Find(manager, "tabControl1")).SelectedTab = (TabPage)Find(manager, "tabPage5");
                grid.DataSource = staff;
                ((TextBox)Find(manager, "txtPhone2")).Text = "0310000002";
                Check(grid.Rows.Count == 3, "Editing a staff phone keeps every directory row");
                ((TextBox)Find(manager, "txtPhone2")).Clear();
                Check(grid.Rows.Count == 3, "Clearing the contact editor preserves the staff list");
                CheckInventory(manager);
                CheckJobs(manager, output);
            }

            using (var admin = new AdminForm())
            {
                OpenOffscreen(admin);
                CheckJobs(admin, output);
            }

            CheckPdf(args.Contains("--render-pdf"));
            Console.WriteLine("Desktop workflow checks: " + checks + " passed.");
            if (args.Contains("--database"))
                CheckDatabase();
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void CheckInventory(ManagerForm manager)
    {
        var tabs = (TabControl)Find(manager, "tabControl1");
        tabs.SelectedTab = (TabPage)Find(manager, "tabPage4");
        var stock = new DataTable();
        stock.Columns.Add("ItemID", typeof(int));
        stock.Columns.Add("ItemName");
        stock.Columns.Add("QuantityInStock", typeof(int));
        stock.Columns.Add("UnitPrice", typeof(decimal));
        stock.Columns.Add("ReorderLevel", typeof(int));
        stock.Columns.Add("SupplierName");
        stock.Columns.Add("UpdatedAt", typeof(DateTime));
        stock.Rows.Add(1, "First item", 15, 5m, 2, "Supplier", DateTime.Today);
        stock.Rows.Add(2, "Selected item", 20, 8m, 3, "Supplier", DateTime.Today);
        var grid = (DataGridView)Find(manager, "dataGridView1");
        grid.DataSource = stock;
        grid.CurrentCell = grid.Rows[1].Cells.Cast<DataGridViewCell>().First(c => c.Visible);
        typeof(DataGridView).GetMethod("OnCellDoubleClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(grid, new object[] { new DataGridViewCellEventArgs(grid.CurrentCell.ColumnIndex, 1) });
        Check(((TabControl)Find(manager, "inventoryTabs")).SelectedTab.Name == "tabUpdateStock", "Double-click opens Update stock");
        Check(Find(manager, "textBox17").Text == "Selected item", "Update stock shows the clicked item");
        var quantity = (NumericUpDown)Find(manager, "numUpdateQuantity");
        quantity.Value = 20000;
        Check(quantity.Value == quantity.Maximum && quantity.Minimum == 1, "Restock accepts the full quantity range");
        using (var help = new Major_Project.Utilities.HelpManager())
        {
            var trigger = (Button)Find(manager, "btnUpdateStockHelp");
            help.RegisterControl(quantity, "Adjustment", "Enter the quantity to add or subtract.");
            help.RegisterContainer(manager);
            help.AttachHelpButton(trigger, grid);
            trigger.Focus();
            quantity.Focus();
            Check(help.ActiveControl == quantity, "Selecting the stock field records the Help target");
            trigger.Focus();
            trigger.PerformClick();
            Check(Find(Find(manager, "tabUpdateStock"), "InlineFeedback").Text.Contains("quantity to add or subtract"), "Help remembers the chosen stock field and its description");
        }
    }

    private static void CheckJobs(Form form, string output)
    {
        var tabs = (TabControl)Find(form, "tabControl1");
        tabs.SelectedTab = (TabPage)Find(form, "tabPage3");
        Application.DoEvents();
        var grid = (DataGridView)Find(form, "dgvJobs");
        Check(Find(form, "btnUpdate").Left > Find(form, "btnDelJob").Right, "Update Job appears to the right of Archive");
        Check(grid.Width > form.ClientSize.Width * 0.85, form.Name + " job grid uses the page width");
        Check(grid.Height > 300, form.Name + " job grid has room for multiple rows");
        Check(grid.Columns.Cast<DataGridViewColumn>().Any(c => c.DataPropertyName == "PaymentStatus"), "Payment column is in Designer");
        Check(Find(form, "btnViewInvoicePdf").Parent.Name == "jobActionsToolbar", "View invoice button is Designer-owned");
        Check(Find(form, "btnViewReceiptPdf").Parent.Name == "jobActionsToolbar", "View receipt button is Designer-owned");
        var formatting = form.GetType().Assembly.GetType("Major_Project.UI.Forms.Common.GridPresentation");
        formatting.GetMethod("Attach", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { form });
        var jobs = new DataTable();
        jobs.Columns.Add("JobID", typeof(int));
        jobs.Columns.Add("CustomerID", typeof(int));
        jobs.Columns.Add("CustomerName");
        jobs.Columns.Add("TechnicianName");
        jobs.Columns.Add("JobStatus");
        jobs.Columns.Add("PaymentStatus");
        jobs.Columns.Add("PromiseDate", typeof(DateTime));
        jobs.Columns.Add("GrandTotal", typeof(decimal));
        jobs.Columns.Add("HasInvoicePdf", typeof(bool));
        jobs.Rows.Add(123, 1, "Example Customer A", "Example Technician", "Pending", "Unpaid", DateTime.Today, 115m, true);
        jobs.Rows.Add(124, 2, "Example Customer B", "Example Technician", "Completed", "Paid", DateTime.Today, 230m, true);
        grid.AutoGenerateColumns = true;
        grid.Columns.Clear();
        grid.DataSource = jobs;
        Check(grid.Columns["PaymentStatus"].Visible, "Payment status is visible after live-style binding");
        Check(!grid.Columns["HasInvoicePdf"].Visible && !grid.Columns["CustomerID"].Visible, "Document flags and foreign keys stay hidden");
        Check(grid.Rows.Count == 2, "Each job keeps its own payment status");
        // Explicit owner routing must put the final outcome on the currently visible destination tab.
        var workflow = form.GetType().Assembly.GetType("Major_Project.UI.Forms.Common.WorkflowUi");
        workflow.GetMethod("Notify", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { form, "Job #123 saved successfully." });
        Check(Find(tabs.SelectedTab, "WorkflowFeedback").Visible, "Save outcome appears on the visible destination page");
        using (var image = new Bitmap(form.Width, form.Height))
        {
            form.DrawToBitmap(image, new Rectangle(Point.Empty, form.Size));
            image.Save(Path.Combine(output, form.Name + "-jobs.png"));
        }
    }

    private static void CheckPdf(bool render)
    {
        var isPdf = typeof(PdfViewerForm).GetMethod("IsPdf", BindingFlags.Static | BindingFlags.NonPublic);
        Check(!(bool)isPdf.Invoke(null, new object[] { null }), "Missing PDF is rejected");
        Check(!(bool)isPdf.Invoke(null, new object[] { new byte[] { 1, 2, 3, 4, 5 } }), "Non-PDF bytes are rejected");
        var invoice = new InvoiceDto
        {
            JobId = 123,
            CustomerName = "Preview Customer",
            CustomerEmail = "preview@example.com",
            InvoiceDate = DateTime.Today,
            LabourCost = 100,
            GrandTotal = 115,
            TaxAmount = 15
        };
        byte[] pdf = new InvoicePdfService().GenerateInvoicePdf(invoice);
        Check((bool)isPdf.Invoke(null, new object[] { pdf }), "Generated invoice is accepted by the viewer");
        if (!render)
            return;
        using (var viewer = (Form)Activator.CreateInstance(typeof(PdfViewerForm), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { pdf, "Invoice-123.pdf" }, null))
        {
            OpenOffscreen(viewer);
            var status = (Label)Find(viewer, "lblStatus");
            var deadline = DateTime.UtcNow.AddSeconds(25);
            while (DateTime.UtcNow < deadline && status.Text == "Loading PDF…")
            {
                Application.DoEvents();
                Thread.Sleep(20);
            }

            Check(status.Text.StartsWith("Invoice-123.pdf —"), "Embedded WebView2 loads a real generated invoice PDF: " + status.Text);
            string folder = (string)viewer.GetType().GetField("_previewFolder", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(viewer);
            viewer.Close();
            Check(!Directory.Exists(folder), "Closing the viewer removes the temporary PDF");
        }
    }

    private static void CheckDatabase()
    {
        // Read-only verification: never create jobs, payments or test staff in the live database.
        var config = new XmlDocument();
        config.Load("App.config");
        string cs = config.SelectSingleNode("//connectionStrings/add[contains(@name, 'GroupWst34ConnectionString')]").Attributes["connectionString"].Value;
        var builder = new SqlConnectionStringBuilder(cs)
        {
            ConnectTimeout = 5
        };
        var jobs = new JobDataAccess(builder.ConnectionString);
        var list = jobs.GetJobViews();
        Check(list.Columns.Contains("PaymentStatus") && list.Columns.Contains("HasInvoicePdf"), "Live job query returns payment/document availability");
        Check(list.Rows.Cast<DataRow>().Select(row => Convert.ToInt32(row["JobID"])).Distinct().Count() == list.Rows.Count, "Payment history does not duplicate jobs");
        Check(list.Rows.Cast<DataRow>().All(row => !string.IsNullOrWhiteSpace(Convert.ToString(row["PaymentStatus"]))), "Every live job has a payment status");
        var all = jobs.GetAllJobs();
        Check(all.Columns.Contains("PaymentStatus"), "Dashboard query returns payment status");
        var staff = new StaffDataAccess(builder.ConnectionString).GetAllStaff();
        var technician = staff.Rows.Cast<DataRow>().FirstOrDefault(row => Convert.ToString(row["RoleName"]) == "Technician");
        if (technician != null)
        {
            var assignedJobs = jobs.GetTechnicianJobs(Convert.ToInt32(technician["StaffID"]));
            Check(assignedJobs.Columns.Contains("HasPendingChange") && !assignedJobs.Columns.Contains("PaymentStatus"), "Technician job query exposes operational state only");
        }
        if (list.Rows.Count > 0)
            Check(jobs.GetJobsByCustomerID(Convert.ToInt32(list.Rows[0]["CustomerID"])).Columns.Contains("PaymentStatus"), "Customer job query includes payment status");
        Console.WriteLine("Read-only database checks passed: " + list.Rows.Count + " listed jobs, " + staff.Rows.Count + " staff records. No records changed.");
    }
}
