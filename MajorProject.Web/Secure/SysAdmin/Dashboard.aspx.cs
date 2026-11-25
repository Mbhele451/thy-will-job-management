using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MajorProject.Business.Services;
using MajorProject.Business.Helpers;
using MajorProject.Web.Helpers;

namespace MajorProject.Web.Secure.SysAdmin
{
    public partial class ConsolePage : SysAdminBasePage
    {
        private SystemAdminService Service
        {
            get
            {
                return new SystemAdminService(ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString);
            }
        }

        private int Actor
        {
            get
            {
                return WebSession.StaffID;
            }
        }

        private int SelectedID
        {
            get
            {
                return ViewState["SelectedID"] == null ? 0 : (int)ViewState["SelectedID"];
            }

            set
            {
                ViewState["SelectedID"] = value;
            }
        }

        private bool SelectedStaff
        {
            get
            {
                return ViewState["SelectedStaff"] != null && (bool)ViewState["SelectedStaff"];
            }

            set
            {
                ViewState["SelectedStaff"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFrom.Text = DateTime.UtcNow.Date.AddDays(-14).ToString("yyyy-MM-dd");
                txtTo.Text = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
                Run(() =>
                {
                    BindAccounts();
                    BindSessions();
                    BindSettings();
                    LoadSetting();
                }, "Users loaded.");
            }
        }

        private void Run(Action action, string success)
        {
            lblMessage.Visible = false;
            try
            {
                action();
                Message(success, false);
            }
            catch (ArgumentException ex)
            {
                Message(ex.Message, true);
            }
            catch (UnauthorizedAccessException)
            {
                Response.Redirect(ResolveUrl("~/Account/AccessDenied.aspx"), true);
            }
            catch (SqlException ex)
            {
                Logger.Error("SysAdmin database operation failed. SQL error " + ex.Number);
                Message(ex.Number == 2601 || ex.Number == 2627 ? "A unique account value is already registered." : ex.Number >= 51000 && ex.Number < 52000 ? ex.Message : "The operation failed. Check the database migration and try again.", true);
            }
            catch (Exception ex)
            {
                Logger.Error("SysAdmin operation failed.", ex);
                Message("The operation could not be completed. Please try again.", true);
            }
        }

        private void Message(string text, bool error)
        {
            lblMessage.Text = Server.HtmlEncode(text);
            lblMessage.CssClass = "tw-feedback " + (error ? "alert-danger" : "alert-success");
            lblMessage.Attributes["role"] = error ? "alert" : "status";
            lblMessage.Visible = true;
        }

        private void Open(string tab, bool detail = false)
        {
            string extra = tab == "users" ? "window.ThyWillUI.activateTab('sysadmin-users','" + (detail ? "details" : "list") + "');" : "";
            ScriptManager.RegisterStartupScript(this, GetType(), "sysadmin-tab", "if(window.ThyWillUI){window.ThyWillUI.initialize();window.ThyWillUI.activateTab('sysadmin','" + tab + "');" + extra + "}", true);
        }

        private void BindAccounts()
        {
            gvAccounts.DataSource = Service.Accounts(Actor, txtSearch.Text);
            gvAccounts.DataBind();
        }

        private void Range(out DateTime from, out DateTime to)
        {
            if (!DateTime.TryParseExact(txtFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out from) || !DateTime.TryParseExact(txtTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out to))
                throw new ArgumentException("Choose valid report dates.");
            to = to.AddDays(1);
            SystemAdminService.DateRange(from, to);
        }

        private DataTable CurrentReport()
        {
            DateTime f, t;
            Range(out f, out t);
            return Service.Report(Actor, ddlReport.SelectedValue, f, t);
        }

        private void BindSessions()
        {
            DateTime f, t;
            Range(out f, out t);
            var data = Service.Report(Actor, "Sessions", f, t);
            var view = new DataView(data);
            if (cbActive.Checked)
                view.RowFilter = "SessionStatus='Active'";
            gvSessions.DataSource = view;
            gvSessions.DataBind();
        }

        private void BindSettings()
        {
            gvSettings.DataSource = Service.Settings(Actor);
            gvSettings.DataBind();
        }

        private void LoadSetting()
        {
            var table = Service.Settings(Actor);
            foreach (DataRow row in table.Rows)
                if (Convert.ToString(row["SettingKey"]) == ddlSetting.SelectedValue)
                    txtSetting.Text = Convert.ToString(row["SettingValue"]);
        }

        protected void Search_Click(object s, EventArgs e)
        {
            Open("users");
            Run(() =>
            {
                gvAccounts.PageIndex = 0;
                BindAccounts();
            }, "Matching accounts loaded.");
        }

        protected void Accounts_Page(object s, GridViewPageEventArgs e)
        {
            Open("users");
            Run(() =>
            {
                gvAccounts.PageIndex = e.NewPageIndex;
                BindAccounts();
            }, "Account page loaded.");
        }

        protected void Account_Command(object s, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "ManageAccount")
                return;
            Open("users", true);
            Run(() =>
            {
                string[] value = Convert.ToString(e.CommandArgument).Split(':');
                int id;
                if (value.Length != 2 || !int.TryParse(value[1], out id) || (value[0] != "Staff" && value[0] != "Customer"))
                    throw new ArgumentException("Choose an account.");
                var data = Service.Accounts(Actor, txtSearch.Text);
                var rows = data.Select("AccountType='" + value[0] + "' AND AccountID=" + id.ToString(CultureInfo.InvariantCulture));
                if (rows.Length != 1)
                    throw new ArgumentException("Refresh the account list and select again.");
                var row = rows[0];
                SelectedID = id;
                SelectedStaff = value[0] == "Staff";
                txtUsername.Text = Convert.ToString(row["Username"]);
                txtUsername.Enabled = SelectedStaff;
                txtEmail.Text = Convert.ToString(row["EmailAddress"]);
                ddlRole.Enabled = SelectedStaff;
                if (SelectedStaff && ddlRole.Items.FindByValue(Convert.ToString(row["RoleName"])) != null)
                    ddlRole.SelectedValue = Convert.ToString(row["RoleName"]);
                lblSelected.Text = Server.HtmlEncode(value[0] + " #" + id + ": " + Convert.ToString(row["FullName"]));
                pnlAccount.Visible = true;
                txtReason.Text = Convert.ToString(row["BlockReason"]);
                btnBlock.Enabled = !Convert.ToBoolean(row["IsBlocked"]);
                btnUnblock.Enabled = Convert.ToBoolean(row["IsBlocked"]);
            }, "Selected account loaded.");
        }

        protected void Save_Click(object s, EventArgs e)
        {
            Open("users", true);
            Run(() =>
            {
                if (txtPassword.Text != txtConfirm.Text)
                    throw new ArgumentException("The password confirmation does not match.");
                Service.Credentials(Actor, SelectedStaff, SelectedID, txtUsername.Text, txtEmail.Text, txtPassword.Text, ddlRole.SelectedValue, SecurityGateway.IP);
                txtPassword.Text = txtConfirm.Text = "";
                BindAccounts();
            }, "Credentials saved. Tracked sessions were revoked; the user must sign in again.");
        }

        protected void Block_Click(object s, EventArgs e)
        {
            Block(true);
        }

        protected void Unblock_Click(object s, EventArgs e)
        {
            Block(false);
        }

        private void Block(bool blocked)
        {
            Open("users", true);
            Run(() =>
            {
                Service.Block(Actor, SelectedStaff, SelectedID, blocked, txtReason.Text, SecurityGateway.IP);
                BindAccounts();
                btnBlock.Enabled = !blocked;
                btnUnblock.Enabled = blocked;
            }, blocked ? "Account blocked and tracked sessions revoked." : "Security block removed. Account activation and employment status still apply.");
        }

        protected void Create_Click(object s, EventArgs e)
        {
            Open("create");
            Run(() =>
            {
                decimal salary;
                if (!decimal.TryParse(txtSalary.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out salary))
                    throw new ArgumentException("Enter a valid salary amount.");
                if (txtNewPassword.Text != txtNewConfirm.Text)
                    throw new ArgumentException("The initial password confirmation does not match.");
                int id = Service.CreateStaff(Actor, txtNewName.Text, txtNewPhone.Text, txtNewEmail.Text, txtNewUsername.Text, txtNewPassword.Text, ddlNewRole.SelectedValue, salary, SecurityGateway.IP);
                txtNewPassword.Text = txtNewConfirm.Text = "";
                txtNewUsername.Text = txtNewEmail.Text = txtNewName.Text = txtNewPhone.Text = "";
                BindAccounts();
                Message("Staff account #" + id + " created.", false);
            }, "Staff account created. Deliver the initial credential through your approved private channel.");
        }

        protected void Sessions_Click(object s, EventArgs e)
        {
            Open("sessions");
            Run(() =>
            {
                gvSessions.PageIndex = 0;
                BindSessions();
            }, "Sessions refreshed using the UTC date range on Reports.");
        }

        protected void Sessions_Page(object s, GridViewPageEventArgs e)
        {
            Open("sessions");
            Run(() =>
            {
                gvSessions.PageIndex = e.NewPageIndex;
                BindSessions();
            }, "Session page loaded.");
        }

        protected void Session_Command(object s, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "RevokeSession")
                return;
            Open("sessions");
            Run(() =>
            {
                Guid id;
                if (!Guid.TryParse(Convert.ToString(e.CommandArgument), out id))
                    throw new ArgumentException("Choose a valid session.");
                Service.Revoke(Actor, id, SecurityGateway.IP);
                BindSessions();
            }, "Session revoked. Its next authenticated request is rejected.");
        }

        protected void Report_Click(object s, EventArgs e)
        {
            Open("reports");
            Run(() =>
            {
                gvReport.PageIndex = 0;
                gvReport.DataSource = CurrentReport();
                gvReport.DataBind();
            }, "Report generated. Latest 2,000 matching rows maximum.");
        }

        protected void Report_Page(object s, GridViewPageEventArgs e)
        {
            Open("reports");
            Run(() =>
            {
                gvReport.PageIndex = e.NewPageIndex;
                gvReport.DataSource = CurrentReport();
                gvReport.DataBind();
            }, "Report page loaded.");
        }

        private void Export(DataTable table, string name)
        {
            string csv = CsvReportWriter.Write(table);
            Response.Clear();
            Response.ContentType = "text/csv";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + name + ".csv\"");
            Response.Write('\uFEFF');
            Response.Write(csv);
            Response.Flush();
            Response.SuppressContent = true;
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void Csv_Click(object s, EventArgs e)
        {
            Run(() => Export(CurrentReport(), "sysadmin-" + ddlReport.SelectedValue.ToLowerInvariant()), "Report exported.");
        }

        protected void AccountsCsv_Click(object s, EventArgs e)
        {
            Run(() => Export(Service.Accounts(Actor, txtSearch.Text), "sysadmin-accounts"), "Accounts exported.");
        }

        protected void Setting_Changed(object s, EventArgs e)
        {
            Open("settings");
            Run(LoadSetting, "Setting loaded.");
        }

        protected void Setting_Click(object s, EventArgs e)
        {
            Open("settings");
            Run(() =>
            {
                Service.SaveSetting(Actor, ddlSetting.SelectedValue, txtSetting.Text, SecurityGateway.IP);
                HttpRuntime.Cache.Remove("AppSetting:" + ddlSetting.SelectedValue);
                BindSettings();
                LoadSetting();
            }, "Setting saved.");
        }
    }
}
