using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using MajorProject.Business.Models;
using MajorProject.Business.Models.DTOs;
using MajorProject.Business.Models.Payments;
using MajorProject.Business.Services;
using MajorProject.Business.Services.Payments;

internal static class ParitySmoke
{
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    private const BindingFlags Static = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
    private static Assembly Desktop;
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Contains("schema"))
            return VerifySchema();
        Application.EnableVisualStyles();
        int result = 1;
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
                result = RunChecks(args);
                runner.Close();
            }));
            Application.Run(runner);
        }

        return result;
    }

    private static int RunChecks(string[] args)
    {
        try
        {
            if (args.Contains("schema"))
                return VerifySchema();
            Desktop = Assembly.LoadFrom("Major_Project.exe");
            CheckAuth();
            CheckWorkspaces();
            CheckCheckoutAndExports();
            CheckBookingAndDocuments();
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static Type Type(string name) => Desktop.GetType("Major_Project." + name, true);
    private static Type Common(string name) => Type("UI.Forms.Common." + name);
    private static Form NewForm(Type type, params object[] values)
    {
        var form = (Form)Activator.CreateInstance(type, Instance, null, values, null);
        form.ShowInTaskbar = false;
        form.StartPosition = FormStartPosition.Manual;
        form.Location = new Point(-20000, -20000);
        return form;
    }

    private static Control Find(Control owner, string name) => owner.Controls.Find(name, true).Single();
    private static void CheckAuth()
    {
        var session = Type("Core.UserSession");
        var role = session.GetMethod("SupportsRole", Static);
        Assert((bool)role.Invoke(null, new object[] { true, "SysAdmin" }), "SysAdmin is routed as staff");
        Assert(!(bool)role.Invoke(null, new object[] { false, "Customer" }), "Desktop rejects customer sessions");
        Assert(!(bool)role.Invoke(null, new object[] { false, "Manager" }), "Customer cannot take a staff role");
        Assert(!(bool)session.GetMethod("Check", Static).Invoke(null, new object[] { true }), "No ticket fails closed");
        using (var login = NewForm(Common("LoginForm")))
        {
            login.Show();
            var password = (TextBox)Find(login, "textBox2");
            Assert(password.UseSystemPasswordChar, "Password is masked");
            ((CheckBox)Find(login, "checkBox1")).Checked = true;
            Assert(!password.UseSystemPasswordChar, "Password visibility toggles");
            var card = Find(login, "panel1");
            Assert(!card.Controls.OfType<LinkLabel>().Any(link => link.Text == "Create customer account"), "Desktop has no public registration entry point");
            login.GetType().GetMethod("linkLabel1_LinkClicked", Instance).Invoke(login, new object[] { null, null });
            var recovery = (Form)Find(login, "ResetPasswordForm");
            Assert(!recovery.TopLevel && Find(recovery, "button1").Enabled && !Find(recovery, "button2").Enabled, "Recovery opens inline and requires a code");
            ((Button)Find(recovery, "button3")).PerformClick();
            PumpUntil(() => login.Controls.Find("InlineWorkflow", false).Length == 0);
            Assert(card.Visible, "Back restores the sign-in card");
        }
    }

    private static void CheckWorkspaces()
    {
        var workspaces = new[]
        {
            "Admin",
            "Manager",
            "Technician",
            "SystemAdmin"
        };
        foreach (var role in workspaces)
        {
            bool administrative = role == "SystemAdmin";
            var formType = administrative ? Common(role + "Form") : Type("UI.Forms." + role + "." + role + "Form");
            object[] values = administrative ? new object[0] : new object[]
            {
                1,
                "Test fixture"
            };
            using (var host = NewForm(Common("MDIForm")))
            using (var workspace = NewForm(formType, values))
            {
                host.Load -= Event(host, "MDIForm_Load");
                if (!administrative)
                    workspace.Load -= Event(workspace, role + "_Load");
                host.WindowState = FormWindowState.Normal;
                host.Size = new Size(1100, 720);
                host.Show();
                var tabs = (TabControl)formType.GetProperty("NavigationTabs", Instance).GetValue(workspace);
                if (role == "Admin" || role == "Manager")
                    tabs.SelectedIndexChanged -= Event(workspace, "tabControl1_SelectedIndexChanged");
                host.GetType().GetMethod("OpenWorkspace", Instance).Invoke(host, new object[] { workspace, tabs, role == "Manager" });
                var tabStrip = (FlowLayoutPanel)host.GetType().GetField("_navigation", Instance).GetValue(host);
                var buttons = tabStrip.Controls.OfType<Button>().Where(button => button.Visible).ToArray();
                Assert(tabStrip.Dock == DockStyle.Top && buttons.Count() == tabs.TabPages.Count + (role == "Manager" ? 1 : 0), role + " uses a horizontal tab strip");
                buttons[1].PerformClick();
                Assert(tabs.SelectedIndex == 0, role + " navigation rejects missing session");
                tabs.SelectedIndex = 1;
                Assert(buttons[1].BackColor == Color.FromArgb(224, 234, 250), role + " highlights the selected tab");
                Assert(workspace.ParentForm == host, role + " is hosted natively");
                CheckInlinePanel(tabs, workspace);
                if (role == "Admin" || role == "Manager")
                {
                    var type = (ComboBox)Find(workspace, "cmbCustomerType");
                    type.SelectedItem = "Residential";
                    var document = type.Parent.Controls.OfType<ComboBox>().Single(c => c.AccessibleName == "Identity document");
                    document.SelectedItem = "Passport";
                    type.Parent.Controls.OfType<TextBox>().Single(t => t.AccessibleName == "Passport number").Text = "PASS123";
                    type.Parent.Controls.OfType<TextBox>().Single(t => t.AccessibleName == "Issuing country").Text = "France";
                    var identity = formType.GetField("_customerIdentity", Instance).GetValue(workspace);
                    identity.GetType().GetMethod("Validate", Instance).Invoke(identity, null);
                    Assert((string)identity.GetType().GetProperty("SouthAfricanID", Instance).GetValue(identity) == "", role + " passport excludes SA ID");
                    type.SelectedItem = "Business";
                    Assert((string)identity.GetType().GetProperty("Passport", Instance).GetValue(identity) == "", role + " business excludes passport");
                    tabs.SelectedIndex = 2;
                    Assert(Find(workspace, "dgvJobs").Width >= tabs.ClientSize.Width * 0.8, role + " jobs page uses the available width");
                }
            }
        }
    }

    private static void CheckInlinePanel(TabControl tabs, Control workspace)
    {
        var page = tabs.SelectedTab;
        var original = page.Controls.Cast<Control>().Where(control => control.Visible).ToArray();
        int topLevelWindowCount = Application.OpenForms.Cast<Form>().Count(form => form.TopLevel);
        var type = Common("WorkflowPanel");
        using (var panel = (Panel)Activator.CreateInstance(type, Instance, null, new object[] { "Edit record", Activator.CreateInstance(Common("SettingEditor")) }, null))
        {
            var field = (TextBox)type.GetMethod("Input", Instance).Invoke(panel, new object[] { "Setting value", 100, "", false });
            int saved = 0;
            var submit = (Button)type.GetMethod("Submit", Instance).Invoke(panel, new object[] { "Save", (Action)(() =>
            {
                if (string.IsNullOrWhiteSpace(field.Text))
                    throw new ArgumentException("Enter a description.");
                saved++;
            }), false });
            var done = (Task<bool>)type.GetMethod("OpenAsync", Instance).Invoke(panel, new object[] { workspace });
            Assert(panel.Parent == page && !original.Any(control => control.Visible) && Application.OpenForms.Cast<Form>().Count(form => form.TopLevel) == topLevelWindowCount, "Edit opens inside the selected tab");
            var header = panel.Controls.Find("Header", true).Single();
            Assert(field.PointToScreen(Point.Empty).Y >= header.PointToScreen(new Point(0, header.Height)).Y, "Inline inputs fit below the header");
            submit.PerformClick();
            Assert(!done.IsCompleted && panel.Controls.Find("WorkflowFeedback", true).Any(label => label.Visible && label.Text == "Enter a description."), "Invalid input stays on the inline form");
            tabs.SelectedIndex = 0;
            Assert(!panel.Visible, "Switching tabs hides the edit without opening a window");
            tabs.SelectedTab = page;
            field.Text = "Smoke test";
            submit.PerformClick();
            submit.PerformClick();
            PumpUntil(() => done.IsCompleted);
            Assert(done.Result && saved == 1 && original.All(control => control.Visible), "Save returns to the tab and submits once");
        }
    }

    private static void PumpUntil(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(3);
        while (!condition() && DateTime.UtcNow < deadline)
        {
            Application.DoEvents();
            System.Threading.Thread.Sleep(5);
        }

        Assert(condition(), "Inline navigation completes");
    }

    private static EventHandler Event(Control target, string name) => (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), target, target.GetType().GetMethod(name, Instance));
    private static void CheckCheckoutAndExports()
    {
        var cash = Common("StaffJobActions").GetMethod("TryCash", Static);
        foreach (var text in new[]
        {
            "0",
            "-1",
            "1.123",
            "100000000.00",
            "1e3",
            "NaN"
        }

        )
            Assert(!(bool)cash.Invoke(null, new object[] { text, 0m }), "Cash rejects " + text);
        var amount = new object[]
        {
            "212.50",
            0m
        };
        Assert((bool)cash.Invoke(null, amount) && (decimal)amount[1] == 212.50m, "Cash accepts cents");
        var csv = Common("SystemAdminForm").GetMethod("CsvField", Static);
        Assert((string)csv.Invoke(null, new object[] { " =SUM(A1)" }) == "\"' =SUM(A1)\"", "CSV neutralizes spreadsheet formula");
        Assert((string)csv.Invoke(null, new object[] { "a,\"b\"" }) == "\"a,\"\"b\"\"\"", "CSV quotes embedded delimiter and quote");
    }

    private static void CheckBookingAndDocuments()
    {
        var cart = new BookingCart(123);
        cart.AddService(10, "Test service", 2);
        cart.AddService(11, "Second service", 1);
        cart.SetQuantity(10, 3);
        DataTable rows = cart.ToDataTable();
        Assert(rows.Rows.Count == 2 && (int)rows.Rows[0]["Quantity"] == 3, "Multi-service booking cart preserves edited quantities");
        Assert(PaymentInputValidation.IsValidCardNumber("4111111111111111") && !PaymentInputValidation.IsValidCardNumber("1111111111111111"), "Shared simulated card validation accepts test card and rejects repeated digits");
        var invoice = new InvoiceDto
        {
            JobId = 123,
            CustomerName = "Desktop test fixture",
            InvoiceDate = DateTime.Today,
            LabourCost = 100m,
            TaxAmount = 15m,
            GrandTotal = 115m
        };
        invoice.Items.Add(new InvoiceItemDto { ServiceName = "Test service", Quantity = 1 });
        var invoicePdf = new InvoicePdfService().GenerateInvoicePdf(invoice);
        Assert(System.Text.Encoding.ASCII.GetString(invoicePdf, 0, 5) == "%PDF-", "Shared invoice generator returns a PDF");
        var payment = new PaymentResult
        {
            AmountDue = 115m,
            PaymentStatus = "Paid",
            AttemptStatus = "Successful",
            PaidAt = DateTime.Now,
            PaymentMethod = "EFT",
            ProviderReference = "DESKTOP-TEST"
        };
        var receiptPdf = new PaymentReceiptPdfService().GeneratePaymentReceiptPdf(invoice, payment);
        Assert(System.Text.Encoding.ASCII.GetString(receiptPdf, 0, 5) == "%PDF-", "Shared receipt generator returns a PDF for a paid invoice");
    }

    private static int VerifySchema()
    {
        try
        {
            var config = ConfigurationManager.OpenExeConfiguration(System.IO.Path.GetFullPath("Major_Project.exe"));
            var item = config.ConnectionStrings.ConnectionStrings.Cast<ConnectionStringSettings>().Single(entry => entry.Name.EndsWith("GroupWst34ConnectionString"));
            var builder = new SqlConnectionStringBuilder(item.ConnectionString)
            {
                ConnectTimeout = 5
            };
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 5;
                    command.CommandText = @"SELECT
CAST(CASE WHEN OBJECT_ID(N'dbo.AppUserSessions', 'U') IS NOT NULL AND OBJECT_ID(N'dbo.AppAccountSecurity','U') IS NOT NULL AND OBJECT_ID(N'dbo.AppSystemSettings','U') IS NOT NULL AND OBJECT_ID(N'dbo.AppSecurityAudit','U') IS NOT NULL THEN 1 ELSE 0 END AS bit),
CAST(CASE WHEN COL_LENGTH(N'dbo.Customers',N'PassportNumber') IS NOT NULL AND COL_LENGTH(N'dbo.Customers',N'PassportCountry') IS NOT NULL THEN 1 ELSE 0 END AS bit),
CAST(CASE WHEN OBJECT_ID(N'dbo.usp_GetCustomerPaymentCards','P') IS NOT NULL THEN 1 ELSE 0 END AS bit)";
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        Console.WriteLine("Session/security schema visible: " + reader.GetBoolean(0));
                        Console.WriteLine("Passport columns visible: " + reader.GetBoolean(1));
                        Console.WriteLine("Payment card procedure visible: " + reader.GetBoolean(2));
                        if (!reader.GetBoolean(0) || !reader.GetBoolean(1) || !reader.GetBoolean(2))
                            return 2;
                    }
                }
            }

            var catalog = new JobTypeService(builder.ConnectionString).GetAllJobTypes();
            Assert(catalog.Columns.Contains("JobTypeID") && catalog.Columns.Contains("JobTypeName"), "Desktop shared service can read the booking catalog");
            return 0;
        }
        catch (SqlException error)
        {
            Console.Error.WriteLine("SQL schema probe failed with SQL error " + error.Number);
            return 3;
        }
    }

    private static void Assert(bool result, string description)
    {
        if (!result)
            throw new InvalidOperationException(description);
        Console.WriteLine("PASS " + description);
    }
}
