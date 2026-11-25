using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Major_Project.Core;

namespace Major_Project.UI.Forms.Common
{
    public partial class MDIForm : Form
    {
        private int _nextNavigationIndex;
        private Form _workspace;
        private Reports.ReportsForm _reports;
        private readonly Timer _heartbeat;
        private int _heartbeatSeconds;
        private bool _checkingStock;
        public MDIForm()
        {
            InitializeComponent();
            btnSignOut.Click += (sender, args) => Close();
            _heartbeat = new Timer
            {
                Interval = 1000
            };
            _heartbeat.Tick += Heartbeat_Tick;
            lblStockAlert.LinkClicked += (sender, args) =>
            {
                if (!UserSession.Check() || !(_workspace is Manager.ManagerForm manager))
                    return;
                _reports?.Hide();
                _workspace.Show();
                manager.ShowLowStock();
            };
        }

        private void MDIForm_Load(object sender, EventArgs e)
        {
            UserSession.Expired += SessionExpired;
            if (!UserSession.Check())
                return;
            _heartbeat.Start();
            string role = (UserSession.RoleName ?? string.Empty).Trim();
            int staffId = UserSession.StaffID;
            string fullName = UserSession.FullName;
            _account.Text = fullName + "  •  " + role;
            if (role.Equals("SysAdmin", StringComparison.OrdinalIgnoreCase))
            {
                var sysAdmin = new SystemAdminForm();
                OpenWorkspace(sysAdmin, sysAdmin.NavigationTabs, false);
            }
            else if (role.Equals("Administrator", StringComparison.OrdinalIgnoreCase) || role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                var admin = new Admin.AdminForm(staffId, fullName);
                OpenWorkspace(admin, admin.NavigationTabs, false);
            }
            else if (role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            {
                var manager = new Manager.ManagerForm(staffId, fullName);
                manager.LowStockChanged += count =>
                {
                    lblStockAlert.Text = count + " item(s) at reorder level — view stock";
                    lblStockAlert.Visible = count > 0;
                };
                OpenWorkspace(manager, manager.NavigationTabs, true);
            }
            else if (role.Equals("Technician", StringComparison.OrdinalIgnoreCase))
            {
                var technician = new Technician.TechnicianForm(staffId, fullName);
                OpenWorkspace(technician, technician.NavigationTabs, false);
            }
            else
            {
                WorkflowUi.ShowError(this, "This account role has no desktop workspace.");
            }
        }

        private async void Heartbeat_Tick(object sender, EventArgs args)
        {
            var remaining = UserSession.Remaining;
            lblSessionCountdown.Text = "Session remaining " + (remaining > TimeSpan.Zero ? Math.Floor(remaining.TotalMinutes).ToString("00") + ":" + remaining.Seconds.ToString("00") : "00:00");
            lblSessionCountdown.ForeColor = remaining.TotalMinutes <= 5 ? System.Drawing.Color.FromArgb(185, 50, 66) : DesktopTheme.Ink;
            if (remaining <= TimeSpan.Zero || ++_heartbeatSeconds % 45 == 0)
                if (!UserSession.Check(userActivity: false))
                    return;
            if (!UserSession.HasRole("Manager") || _checkingStock || (_heartbeatSeconds != 1 && _heartbeatSeconds % 60 != 0))
                return;
            _checkingStock = true;
            try
            {
                var low = await System.Threading.Tasks.Task.Run(() => GridPresentation.LowStock(new MajorProject.Business.Services.InventoryService(WorkflowUi.ConnectionString).GetAllInventoryItems()));
                if (IsDisposed || !UserSession.IsLoggedIn)
                    return;
                lblStockAlert.Text = low.Rows.Count + " item(s) at reorder level — view stock";
                lblStockAlert.Visible = low.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                MajorProject.Business.Helpers.Logger.Error("Could not refresh reorder alert.", ex);
            }
            finally
            {
                _checkingStock = false;
            }
        }

        private void SessionExpired(object sender, EventArgs args)
        {
            _heartbeat.Stop();
            Enabled = false;
            BeginInvoke(new Action(() =>
            {
                if (IsDisposed)
                    return;
                SessionEnded = true;
                Close();
            }));
        }

        internal bool SessionEnded { get; private set; }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _heartbeat.Dispose();
            UserSession.Expired -= SessionExpired;
            base.OnFormClosed(e);
        }

        private void OpenWorkspace(Form workspace, WorkspaceTabs tabs, bool showReports)
        {
            _workspace = workspace;
            if (UserSession.IsLoggedIn && !UserSession.HasRole("SysAdmin"))
                AssistantPage.Add(tabs);
            tabs.HideHeaders();
            var pages = tabs.TabPages.Cast<TabPage>().ToArray();
            var buttons = pages.Select(page => AddNavigationButton(page.Text == "Job Management" ? "Jobs" : page.Text)).ToArray();
            Button reportsButton = showReports ? AddNavigationButton("Reports") : null;
            void RefreshNavigation()
            {
                bool reportsVisible = _reports != null && _reports.Visible;
                for (int i = 0; i < pages.Length; i++)
                    SetActive(buttons[i], !reportsVisible && pages[i] == tabs.SelectedTab);
                if (reportsButton != null)
                    SetActive(reportsButton, reportsVisible);
            }

            for (int i = 0; i < pages.Length; i++)
            {
                TabPage page = pages[i];
                buttons[i].Click += (sender, args) =>
                {
                    if (!UserSession.Check())
                        return;
                    _reports?.Hide();
                    tabs.SelectedTab = page;
                    _workspace.Show();
                    RefreshNavigation();
                };
            }

            if (reportsButton != null)
            {
                reportsButton.Click += (sender, args) =>
                {
                    if (!UserSession.Check())
                        return;
                    _workspace.Hide();
                    if (_reports == null)
                    {
                        _reports = new Reports.ReportsForm();
                        Host(_reports);
                    }
                    else
                    {
                        _reports.Show();
                    }

                    RefreshNavigation();
                };
            }

            tabs.SelectedIndexChanged += (sender, args) => RefreshNavigation();
            Host(workspace);
            RefreshNavigation();
        }

        private Button AddNavigationButton(string caption)
        {
            var button = _navigation.Controls.OfType<Button>().ElementAt(_nextNavigationIndex++);
            button.Text = caption;
            button.Visible = true;
            button.PreviewKeyDown += (sender, args) =>
            {
                if (args.KeyCode == Keys.Left || args.KeyCode == Keys.Right || args.KeyCode == Keys.Home || args.KeyCode == Keys.End)
                    args.IsInputKey = true;
            };
            button.KeyDown += (sender, args) =>
            {
                var items = _navigation.Controls.OfType<Button>().Where(item => item.Visible).ToArray();
                int index = Array.IndexOf(items, button);
                switch (args.KeyCode)
                {
                    case Keys.Left:
                        index = (index + items.Length - 1) % items.Length;
                        break;
                    case Keys.Right:
                        index = (index + 1) % items.Length;
                        break;
                    case Keys.Home:
                        index = 0;
                        break;
                    case Keys.End:
                        index = items.Length - 1;
                        break;
                    default:
                        return;
                }

                args.SuppressKeyPress = true;
                _navigation.ScrollControlIntoView(items[index]);
                items[index].Focus();
                items[index].PerformClick();
            };
            return button;
        }

        private static void SetActive(Button button, bool active)
        {
            button.BackColor = active ? DesktopTheme.Selected : DesktopTheme.Surface;
            button.ForeColor = active ? DesktopTheme.Primary : DesktopTheme.Muted;
        }

        private void Host(Form form)
        {
            form.TopLevel = false;
            form.WindowState = FormWindowState.Normal;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            _workspaceHost.Controls.Add(form);
            form.Show();
            form.BringToFront();
        }
    }
}
