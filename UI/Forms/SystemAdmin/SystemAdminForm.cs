using System.Threading.Tasks;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Major_Project.Core;
using MajorProject.Business.Services;

namespace Major_Project.UI.Forms.Common
{
    internal sealed partial class SystemAdminForm : Form
    {
        private static readonly string[] Roles =
        {
            "Administrator",
            "Manager",
            "Technician",
            "SysAdmin"
        };
        private readonly SystemAdminService _service;
        internal WorkspaceTabs NavigationTabs => workspaceTabs;
        private int Actor => UserSession.StaffID;

        public SystemAdminForm()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;
            _service = new SystemAdminService(WorkflowUi.ConnectionString);
            foreach (TabPage page in NavigationTabs.TabPages)
            {
                page.BackColor = DesktopTheme.Background;
                foreach (var grid in page.Controls.OfType<DataGridView>())
                    DesktopTheme.Apply(grid);
                foreach (var toolbar in page.Controls.OfType<FlowLayoutPanel>())
                    toolbar.BackColor = DesktopTheme.Background;
            }
            AddAccounts();
            AddReport("Sessions", true);
            AddReport("Audit", false);
            AddSettings();
        }

        private void AddAccounts()
        {
            var grid = accountsGrid;
            var search = accountSearch;
            void LoadAccounts()
            {
                var accounts = _service.Accounts(Actor, search.Text);
                grid.DataSource = accounts;
                if (accounts.Rows.Count == 0)
                    WorkflowUi.Notify(this, "No matching accounts. Clear the search and try again.");
            }

            WorkflowUi.BindButton(accountsSearch, () => WorkflowUi.Run(this, LoadAccounts), true);
            WorkflowUi.BindButton(accountsEditCredentials, async () => await WorkflowUi.RunAsync(this, async () =>
            {
                var row = WorkflowUi.Selected(grid);
                int id = Convert.ToInt32(row["AccountID"]);
                bool isStaff = Convert.ToString(row["AccountType"]) == "Staff";
                using (var dialog = new WorkflowPanel($"Credentials • account #{id}", new CredentialEditor()))
                {
                    TextBox username = null;
                    ComboBox role = null;
                    if (isStaff)
                    {
                        username = dialog.Input("Username", 50, Convert.ToString(row["Username"]));
                        role = dialog.Choice("Role", Roles);
                        role.SelectedItem = Convert.ToString(row["RoleName"]);
                    }

                    var email = dialog.Input("Email", 100, Convert.ToString(row["EmailAddress"]));
                    var password = dialog.Input("New password (optional)", 128, password: true);
                    var confirmation = dialog.Input("Confirm new password", 128, password: true);
                    dialog.Submit("Save", () =>
                    {
                        RequireMatch(password, confirmation);
                        _service.Credentials(Actor, isStaff, id, username?.Text, email.Text, password.Text, role?.Text, UserSession.ClientIp);
                    });
                    if (await dialog.OpenAsync(this) && UserSession.Check())
                        LoadAccounts();
                }
            }));
            WorkflowUi.BindButton(accountsBlockUnblock, async () => await WorkflowUi.RunAsync(this, async () =>
            {
                var row = WorkflowUi.Selected(grid);
                bool blocked = !Convert.ToBoolean(row["IsBlocked"]);
                int id = Convert.ToInt32(row["AccountID"]);
                bool isStaff = Convert.ToString(row["AccountType"]) == "Staff";
                using (var dialog = new WorkflowPanel(blocked ? "Block account" : "Unblock account", new BlockAccountEditor()))
                {
                    dialog.Note($"{row["AccountType"]} account #{id}");
                    if (isStaff && blocked)
                        dialog.Note("Blocking ends access and marks the staff member unavailable. Open jobs are preserved and appear in the manager's reassignment queue. A manager must record a handover to an eligible technician.");
                    var reason = dialog.Input("Reason", 300);
                    dialog.Submit(blocked ? "Block Account" : "Unblock Account", () => _service.Block(Actor, isStaff, id, blocked, reason.Text, UserSession.ClientIp));
                    if (await dialog.OpenAsync(this))
                        LoadAccounts();
                }
            }));
            WorkflowUi.BindButton(accountsNewStaff, async () => await WorkflowUi.RunAsync(this, async () =>
            {
                using (var dialog = new WorkflowPanel("Register staff", new StaffRegistrationEditor()))
                {
                    var name = dialog.Input("Full name", 100);
                    var phone = dialog.Input("Phone", 10);
                    var email = dialog.Input("Email", 100);
                    var username = dialog.Input("Username", 50);
                    var role = dialog.Choice("Role", Roles);
                    var salary = dialog.Field<NumericUpDown>("Salary", control =>
                    {
                        control.Maximum = 99999999.99m;
                        control.DecimalPlaces = 2;
                    });
                    var password = dialog.Input("Initial password", 128, password: true);
                    var confirmation = dialog.Input("Confirm password", 128, password: true);
                    dialog.Submit("Register", () =>
                    {
                        RequireMatch(password, confirmation);
                        _service.CreateStaff(Actor, name.Text, phone.Text, email.Text, username.Text, password.Text, role.Text, salary.Value, UserSession.ClientIp);
                    });
                    if (await dialog.OpenAsync(this))
                        LoadAccounts();
                }
            }));
            WorkflowUi.BindButton(accountsNewCustomer, async () =>
            {
                if (UserSession.Check() && await CustomerRegistration.ShowAsync(this))
                    WorkflowUi.Run(this, LoadAccounts);
            });
            WorkflowUi.BindButton(accountsExportCSV, async () => await WorkflowUi.RunAsync(this, () => Export(_service.Accounts(Actor, search.Text), "accounts.csv")));
            Load += (sender, args) => WorkflowUi.Run(this, LoadAccounts);
        }

        private void AddReport(string kind, bool sessions)
        {
            var page = sessions ? sessionsPage : auditPage;
            var grid = sessions ? sessionsGrid : auditGrid;
            var from = sessions ? sessionsFrom : auditFrom;
            var to = sessions ? sessionsTo : auditTo;
            from.Value = DateTime.UtcNow.Date.AddDays(-14);
            to.Value = DateTime.UtcNow.Date;
            var active = activeSessions;
            DataTable Query()
            {
                if (from.Value.Date > to.Value.Date)
                    throw new ArgumentException("Choose an end date on or after the start date.");
                var data = _service.Report(Actor, kind, from.Value.Date, to.Value.Date.AddDays(1));
                return sessions && active.Checked ? new DataView(data)
                {
                    RowFilter = "SessionStatus = 'Active'"
                }.ToTable() : data;
            }

            void Refresh()
            {
                var rows = Query();
                grid.DataSource = rows;
                if (rows.Rows.Count == 0)
                    WorkflowUi.Notify(this, "No records match these dates and filters.");
            }

            if (sessions)
                active.CheckedChanged += (sender, args) => WorkflowUi.Run(this, Refresh);
            WorkflowUi.BindButton(sessions ? sessionsRefresh : auditRefresh, () => WorkflowUi.Run(this, Refresh), true);
            if (sessions)
                WorkflowUi.BindButton(sessionsRevokeSession, async () => await WorkflowUi.RunAsync(this, async () =>
                {
                    Guid id = (Guid)WorkflowUi.Selected(grid)["SessionRecordID"];
                    if (!await WorkflowUi.ConfirmAsync(this, "Revoke the selected session?") || !UserSession.Check())
                        return;
                    _service.Revoke(Actor, id, UserSession.ClientIp);
                    if (UserSession.Check())
                        Refresh();
                }));
            WorkflowUi.BindButton(sessions ? sessionsExportCSV : auditExportCSV, async () => await WorkflowUi.RunAsync(this, () => Export(Query(), kind.ToLowerInvariant() + ".csv")));
            NavigationTabs.SelectedIndexChanged += (sender, args) =>
            {
                if (NavigationTabs.SelectedTab == page && grid.DataSource == null)
                    WorkflowUi.Run(this, Refresh);
            };
        }

        private void AddSettings()
        {
            var page = settingsPage;
            var grid = settingsGrid;
            void Refresh() => grid.DataSource = _service.Settings(Actor);
            WorkflowUi.BindButton(settingsRefresh, () => WorkflowUi.Run(this, Refresh));
            WorkflowUi.BindButton(settingsEditSetting, async () => await WorkflowUi.RunAsync(this, async () =>
            {
                var row = WorkflowUi.Selected(grid);
                string key = Convert.ToString(row["SettingKey"]);
                using (var dialog = new WorkflowPanel("Edit setting", new SettingEditor()))
                {
                    dialog.Note(key);
                    var value = dialog.Input("Setting value", 500, Convert.ToString(row["SettingValue"]));
                    dialog.Submit("Save", () => _service.SaveSetting(Actor, key, value.Text, UserSession.ClientIp));
                    if (await dialog.OpenAsync(this))
                        Refresh();
                }
            }), true);
            NavigationTabs.SelectedIndexChanged += (sender, args) =>
            {
                if (NavigationTabs.SelectedTab == page && grid.DataSource == null)
                    WorkflowUi.Run(this, Refresh);
            };
        }

        private static void RequireMatch(TextBox password, TextBox confirmation)
        {
            if (password.Text != confirmation.Text)
                throw new ArgumentException("The password confirmation does not match.");
        }

        private Task Export(DataTable data, string name)
        {
            return WorkflowUi.SaveAsync(this, name, stream =>
            {
                using (var writer = new StreamWriter(stream, new UTF8Encoding(true)))
                {
                    writer.WriteLine(string.Join(",", data.Columns.Cast<DataColumn>().Select(c => CsvField(c.ColumnName))));
                    foreach (DataRow row in data.Rows)
                        writer.WriteLine(string.Join(",", row.ItemArray.Select(CsvField)));
                }
            });
        }

        internal static string CsvField(object value)
        {
            string text = value is DateTime date ? date.ToString("o", CultureInfo.InvariantCulture) : Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
            string trimmed = text.TrimStart();
            if (trimmed.Length > 0 && "=+-@".IndexOf(trimmed[0]) >= 0)
                text = "'" + text;
            return "\"" + text.Replace("\"", "\"\"") + "\"";
        }
    }
}
