using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.Web.UI.WebControls;

namespace MajorProject.Web.Manager
{
    public partial class StaffManagement : ManagerBasePage
    {
        // ============================================================
        // CONNECTION STRING
        // ============================================================
        private string ConnectionString
        {
            get
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"];
                if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    throw new ConfigurationErrorsException("Connection string 'GroupWst34ConnectionString' was not found.");
                }

                return settings.ConnectionString;
            }
        }

        // ============================================================
        // SERVICES
        // ============================================================
        private StaffService StaffService
        {
            get
            {
                return new StaffService(ConnectionString);
            }
        }

        private RoleService RoleService
        {
            get
            {
                return new RoleService(ConnectionString);
            }
        }

        // ============================================================
        // PAGE LOAD
        // ============================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
                LoadRoles();
                LoadStaff();
            }
        }

        private void InitializePage()
        {
            ClearSelectedStaff();
            ClearNewStaffForm();
        }

        // ============================================================
        // ROLE LOADING
        // ============================================================
        private void LoadRoles()
        {
            try
            {
                DataTable roles = RoleService.GetAllRoles();
                ddlRoleFilter.Items.Clear();
                ddlRoleFilter.Items.Add(new ListItem("All roles", "All"));
                ddlNewRole.Items.Clear();
                ddlNewRole.Items.Add(new ListItem("-- Select role --", ""));
                ddlEditRole.Items.Clear();
                ddlEditRole.Items.Add(new ListItem("-- Select role --", ""));
                if (roles == null || roles.Rows.Count == 0)
                {
                    return;
                }

                foreach (DataRow row in roles.Rows)
                {
                    if (!row.Table.Columns.Contains("RoleID") || !row.Table.Columns.Contains("RoleName"))
                    {
                        continue;
                    }

                    string roleID = Convert.ToString(row["RoleID"]);
                    string roleName = Convert.ToString(row["RoleName"]);
                    if (string.IsNullOrWhiteSpace(roleID) || string.IsNullOrWhiteSpace(roleName))
                    {
                        continue;
                    }

                    ddlRoleFilter.Items.Add(new ListItem(roleName, roleName));
                    ddlNewRole.Items.Add(new ListItem(roleName, roleID));
                    ddlEditRole.Items.Add(new ListItem(roleName, roleID));
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("Failed to load staff roles.", ex);
            }
        }

        // ============================================================
        // LOAD STAFF
        // ============================================================
        private void LoadStaff()
        {
            try
            {
                DataTable staff = StaffService.GetAllStaff();
                UpdateStatistics(staff);
                DataTable filteredStaff = FilterStaff(staff);
                dgvStaff.SelectedIndex = -1;
                ClearSelectedStaff();
                if (filteredStaff == null || filteredStaff.Rows.Count == 0)
                {
                    dgvStaff.DataSource = null;
                    dgvStaff.DataBind();
                    lblMessage.Text = "No staff members found.";
                    return;
                }

                dgvStaff.DataSource = filteredStaff;
                dgvStaff.DataBind();
                lblMessage.Text = filteredStaff.Rows.Count + " staff member(s) found. Select a staff member using the View button.";
            }
            catch (Exception ex)
            {
                dgvStaff.DataSource = null;
                dgvStaff.DataBind();
                dgvStaff.SelectedIndex = -1;
                ClearSelectedStaff();
                lblMessage.Text = HandleUnexpectedError("Failed to load staff members.", ex);
            }
        }

        // ============================================================
        // FILTER STAFF
        // ============================================================
        private DataTable FilterStaff(DataTable staff)
        {
            if (staff == null)
            {
                return null;
            }

            DataTable result = staff.Clone();
            string search = (txtSearch.Text ?? string.Empty).Trim();
            string role = ddlRoleFilter.SelectedValue;
            string status = ddlStatusFilter.SelectedValue;
            foreach (DataRow row in staff.Rows)
            {
                if (!MatchesSearch(row, search))
                {
                    continue;
                }

                if (!MatchesRole(row, role))
                {
                    continue;
                }

                if (!MatchesStatus(row, status))
                {
                    continue;
                }

                result.ImportRow(row);
            }

            return result;
        }

        private bool MatchesSearch(DataRow row, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return true;
            }

            string fullName = GetDatabaseString(row, "FullName");
            string username = GetDatabaseString(row, "Username");
            string email = GetDatabaseString(row, "EmailAddress");
            string phone = GetDatabaseString(row, "PhoneNumber");
            return fullName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || username.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || email.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || phone.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool MatchesRole(DataRow row, string role)
        {
            if (string.IsNullOrWhiteSpace(role) || role.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string rowRole = GetDatabaseString(row, "RoleName");
            return rowRole.Equals(role, StringComparison.OrdinalIgnoreCase);
        }

        private bool MatchesStatus(DataRow row, string status)
        {
            if (string.IsNullOrWhiteSpace(status) || status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string rowStatus = GetDatabaseString(row, "EmploymentStatus");
            return rowStatus.Equals(status, StringComparison.OrdinalIgnoreCase);
        }

        // ============================================================
        // STATISTICS
        // ============================================================
        private void UpdateStatistics(DataTable staff)
        {
            int total = 0;
            int managers = 0;
            int administrators = 0;
            int technicians = 0;
            if (staff != null)
            {
                foreach (DataRow row in staff.Rows)
                {
                    total++;
                    string role = GetDatabaseString(row, "RoleName");
                    if (role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
                    {
                        managers++;
                    }
                    else if (role.Equals("Administrator", StringComparison.OrdinalIgnoreCase))
                    {
                        administrators++;
                    }
                    else if (role.Equals("Technician", StringComparison.OrdinalIgnoreCase))
                    {
                        technicians++;
                    }
                }
            }

            lblTotalStaff.Text = total.ToString();
            lblManagers.Text = managers.ToString();
            lblAdministrators.Text = administrators.ToString();
            lblTechnicians.Text = technicians.ToString();
        }

        // ============================================================
        // SEARCH
        // ============================================================
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dgvStaff.SelectedIndex = -1;
            LoadStaff();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            if (ddlRoleFilter.Items.FindByValue("All") != null)
            {
                ddlRoleFilter.SelectedValue = "All";
            }

            if (ddlStatusFilter.Items.FindByValue("All") != null)
            {
                ddlStatusFilter.SelectedValue = "All";
            }

            dgvStaff.SelectedIndex = -1;
            LoadStaff();
        }

        // ============================================================
        // STAFF SELECTION
        // ============================================================
        protected void dgvStaff_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedStaff();
        }

        private int GetSelectedStaffID()
        {
            if (dgvStaff.SelectedIndex < 0)
            {
                return 0;
            }

            if (dgvStaff.DataKeys == null || dgvStaff.DataKeys.Count <= dgvStaff.SelectedIndex)
            {
                return 0;
            }

            DataKey key = dgvStaff.DataKeys[dgvStaff.SelectedIndex];
            if (key == null || key.Value == null)
            {
                return 0;
            }

            int staffID;
            if (!int.TryParse(Convert.ToString(key.Value), out staffID))
            {
                return 0;
            }

            return staffID;
        }

        // ============================================================
        // DISPLAY SELECTED STAFF
        // ============================================================
        private void ShowSelectedStaff()
        {
            try
            {
                int staffID = GetSelectedStaffID();
                if (staffID <= 0)
                {
                    ClearSelectedStaff();
                    lblMessage.Text = "Select a staff member from the list to view their details.";
                    return;
                }

                DataTable staff = StaffService.GetStaffByID(staffID);
                if (staff == null || staff.Rows.Count == 0)
                {
                    ClearSelectedStaff();
                    lblMessage.Text = "The selected staff member could not be found.";
                    return;
                }

                DataRow row = staff.Rows[0];
                string role = GetDatabaseString(row, "RoleName");
                string status = GetDatabaseString(row, "EmploymentStatus");
                bool available = GetDatabaseBool(row, "IsAvailable");
                lblStaffID.Text = staffID.ToString();
                lblSelectedStaffName.Text = GetDatabaseString(row, "FullName");
                lblSelectedStaffRole.Text = role;
                lblUsername.Text = GetDatabaseString(row, "Username");
                lblEmail.Text = GetDatabaseString(row, "EmailAddress");
                lblPhone.Text = GetDatabaseString(row, "PhoneNumber");
                lblHireDate.Text = GetDatabaseDate(row, "HireDate");
                lblSalary.Text = GetDatabaseMoney(row, "Salary");
                lblEmploymentStatus.Text = status;
                lblAvailability.Text = available ? "Available" : "Unavailable";
                txtEditFullName.Text = GetDatabaseString(row, "FullName");
                txtEditPhone.Text = GetDatabaseString(row, "PhoneNumber");
                txtEditEmail.Text = GetDatabaseString(row, "EmailAddress");
                txtEditSalary.Text = GetDatabaseDecimal(row, "Salary").ToString("0.00", CultureInfo.InvariantCulture);
                SelectRole(ddlEditRole, GetDatabaseInt(row, "RoleID"));
                bool isCurrentManager = staffID == WebSession.StaffID;
                ConfigureSelfProtection(isCurrentManager);
                ConfigureAccountButtons(status, available, isCurrentManager);
                pnlSelectedStaff.Visible = true;
                lblMessage.Text = "Staff member #" + staffID + " selected.";
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("Failed to display the selected staff member.", ex);
            }
        }

        // ============================================================
        // SELF PROTECTION
        // ============================================================
        private void ConfigureSelfProtection(bool isCurrentManager)
        {
            pnlSelfWarning.Visible = isCurrentManager;
            ddlEditRole.Enabled = !isCurrentManager;
            btnDeactivate.Enabled = !isCurrentManager;
            btnArchive.Enabled = !isCurrentManager;
        }

        // ============================================================
        // ACCOUNT BUTTONS
        // ============================================================
        private void ConfigureAccountButtons(string status, bool available, bool isCurrentManager)
        {
            bool isActive = status.Equals("Active", StringComparison.OrdinalIgnoreCase);
            btnActivate.Enabled = !isActive;
            btnDeactivate.Enabled = isActive && !isCurrentManager;
            btnArchive.Enabled = !isCurrentManager;
            // Availability applies to all staff roles:
            // Manager
            // Administrator
            // Technician
            btnAvailable.Visible = true;
            btnUnavailable.Visible = true;
            // An active staff member who is currently unavailable
            // can be marked available.
            btnAvailable.Enabled = isActive && !available;
            // An active staff member who is currently available
            // can be marked unavailable.
            btnUnavailable.Enabled = isActive && available;
        }

        // ============================================================
        // SAVE STAFF DETAILS
        // ============================================================
        protected void btnSaveDetails_Click(object sender, EventArgs e)
        {
            try
            {
                int staffID = GetSelectedStaffID();
                if (staffID <= 0)
                {
                    lblMessage.Text = "Select a staff member first.";
                    return;
                }

                DataTable currentStaff = StaffService.GetStaffByID(staffID);
                if (currentStaff == null || currentStaff.Rows.Count == 0)
                {
                    lblMessage.Text = "The selected staff member could not be found.";
                    return;
                }

                DataRow currentRow = currentStaff.Rows[0];
                int currentRoleID = GetDatabaseInt(currentRow, "RoleID");
                int roleID;
                if (!int.TryParse(ddlEditRole.SelectedValue, out roleID) || roleID <= 0)
                {
                    lblMessage.Text = "Select a valid role.";
                    return;
                }

                if (staffID == WebSession.StaffID && roleID != currentRoleID)
                {
                    lblMessage.Text = "You cannot change your own Manager role.";
                    return;
                }

                string fullName = txtEditFullName.Text.Trim();
                string phone = txtEditPhone.Text.Trim();
                string email = txtEditEmail.Text.Trim();
                decimal salary;
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    lblMessage.Text = "Full Name is required.";
                    return;
                }

                if (!IsValidPhone(phone))
                {
                    lblMessage.Text = "Enter a valid 10-digit phone number.";
                    return;
                }

                if (!IsValidEmail(email))
                {
                    lblMessage.Text = "Enter a valid email address.";
                    return;
                }

                if (!decimal.TryParse(txtEditSalary.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out salary) || salary <= 0)
                {
                    lblMessage.Text = "Enter a valid positive salary.";
                    return;
                }

                StaffService.UpdateStaffDetails(staffID, roleID, fullName, phone, email, salary);
                lblMessage.Text = "Staff member #" + staffID + " details were successfully updated.";
                LoadStaff();
            }
            catch (InvalidOperationException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (ArgumentException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("The staff details could not be updated.", ex);
            }
        }

        // ============================================================
        // RESET PASSWORD
        // ============================================================
        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                int staffID = GetSelectedStaffID();
                if (staffID <= 0)
                {
                    lblMessage.Text = "Select a staff member first.";
                    return;
                }

                string password = txtResetPassword.Text;
                string confirmPassword = txtConfirmResetPassword.Text;
                if (string.IsNullOrEmpty(password))
                {
                    lblMessage.Text = "Enter a new password.";
                    return;
                }

                if (string.IsNullOrEmpty(confirmPassword))
                {
                    lblMessage.Text = "Confirm the new password.";
                    return;
                }

                if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
                {
                    lblMessage.Text = "The new password and confirmation password must match.";
                    return;
                }

                if (!IsValidPassword(password))
                {
                    lblMessage.Text = "Password must contain at least 8 characters, including uppercase, lowercase, a number and a special character.";
                    return;
                }

                StaffService.ChangePassword(staffID, password);
                txtResetPassword.Text = string.Empty;
                txtConfirmResetPassword.Text = string.Empty;
                lblMessage.Text = "The password for staff member #" + staffID + " was successfully reset.";
            }
            catch (InvalidOperationException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (ArgumentException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("The staff password could not be reset.", ex);
            }
        }

        // ============================================================
        // ACTIVATE
        // ============================================================
        protected void btnActivate_Click(object sender, EventArgs e)
        {
            ChangeEmploymentStatus("Active");
        }

        // ============================================================
        // DEACTIVATE
        // ============================================================
        protected void btnDeactivate_Click(object sender, EventArgs e)
        {
            int staffID = GetSelectedStaffID();
            if (staffID == WebSession.StaffID)
            {
                lblMessage.Text = "You cannot deactivate your own Manager account.";
                return;
            }

            ChangeEmploymentStatus("Inactive");
        }

        private void ChangeEmploymentStatus(string status)
        {
            try
            {
                int staffID = GetSelectedStaffID();
                if (staffID <= 0)
                {
                    lblMessage.Text = "Select a staff member first.";
                    return;
                }

                if (staffID == WebSession.StaffID && status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                {
                    lblMessage.Text = "You cannot deactivate your own Manager account.";
                    return;
                }

                StaffService.ChangeEmploymentStatus(staffID, status);
                if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                {
                    StaffService.ChangeAvailability(staffID, false);
                }

                lblMessage.Text = "Staff member #" + staffID + " is now " + status + ".";
                LoadStaff();
            }
            catch (InvalidOperationException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (ArgumentException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("The staff employment status could not be changed.", ex);
            }
        }

        // ============================================================
        // STAFF AVAILABILITY
        // ============================================================
        protected void btnAvailable_Click(object sender, EventArgs e)
        {
            ChangeAvailability(true);
        }

        protected void btnUnavailable_Click(object sender, EventArgs e)
        {
            ChangeAvailability(false);
        }

        private void ChangeAvailability(bool available)
        {
            try
            {
                int staffID = GetSelectedStaffID();
                if (staffID <= 0)
                {
                    lblMessage.Text = "Select a staff member first.";
                    return;
                }

                DataTable staff = StaffService.GetStaffByID(staffID);
                if (staff == null || staff.Rows.Count == 0)
                {
                    lblMessage.Text = "The selected staff member could not be found.";
                    return;
                }

                DataRow row = staff.Rows[0];
                string status = GetDatabaseString(row, "EmploymentStatus");
                if (!status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    lblMessage.Text = "Only active staff members can have their availability changed.";
                    return;
                }

                StaffService.ChangeAvailability(staffID, available);
                lblMessage.Text = "Staff member #" + staffID + " is now " + (available ? "available." : "unavailable.");
                LoadStaff();
            }
            catch (InvalidOperationException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (ArgumentException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("Staff availability could not be changed.", ex);
            }
        }

        // ============================================================
        // ARCHIVE
        // ============================================================
        protected void btnArchive_Click(object sender, EventArgs e)
        {
            try
            {
                int staffID = GetSelectedStaffID();
                if (staffID <= 0)
                {
                    lblMessage.Text = "Select a staff member first.";
                    return;
                }

                if (staffID == WebSession.StaffID)
                {
                    lblMessage.Text = "You cannot archive your own Manager account.";
                    return;
                }

                StaffService.ArchiveStaff(staffID);
                lblMessage.Text = "Staff member #" + staffID + " was archived and marked inactive.";
                LoadStaff();
            }
            catch (InvalidOperationException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (ArgumentException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("The staff member could not be archived.", ex);
            }
        }

        // ============================================================
        // ADD STAFF
        // ============================================================
        protected void btnAddStaff_Click(object sender, EventArgs e)
        {
            try
            {
                int roleID;
                if (!int.TryParse(ddlNewRole.SelectedValue, out roleID) || roleID <= 0)
                {
                    lblMessage.Text = "Select a valid staff role.";
                    return;
                }

                string fullName = txtNewFullName.Text.Trim();
                string phone = txtNewPhone.Text.Trim();
                string email = txtNewEmail.Text.Trim();
                string username = txtNewUsername.Text.Trim();
                string password = txtNewPassword.Text;
                string confirmPassword = txtConfirmNewPassword.Text;
                decimal salary;
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    lblMessage.Text = "Full Name is required.";
                    return;
                }

                if (!IsValidPhone(phone))
                {
                    lblMessage.Text = "Enter a valid 10-digit phone number.";
                    return;
                }

                if (!IsValidEmail(email))
                {
                    lblMessage.Text = "Enter a valid email address.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    lblMessage.Text = "Username is required.";
                    return;
                }

                if (!decimal.TryParse(txtNewSalary.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out salary) || salary <= 0)
                {
                    lblMessage.Text = "Enter a valid positive salary.";
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    lblMessage.Text = "Enter an initial password.";
                    return;
                }

                if (string.IsNullOrEmpty(confirmPassword))
                {
                    lblMessage.Text = "Confirm the initial password.";
                    return;
                }

                if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
                {
                    lblMessage.Text = "The initial password and confirmation password must match.";
                    return;
                }

                if (!IsValidPassword(password))
                {
                    lblMessage.Text = "Password must contain at least 8 characters, including uppercase, lowercase, a number and a special character.";
                    return;
                }

                StaffService.InsertStaff(roleID, fullName, phone, email, salary, username, password);
                ClearNewStaffForm();
                lblMessage.Text = "Staff member was successfully created.";
                LoadStaff();
            }
            catch (InvalidOperationException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (ArgumentException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("The staff account could not be created.", ex);
            }
        }

        protected void btnClearNewStaff_Click(object sender, EventArgs e)
        {
            ClearNewStaffForm();
            lblMessage.Text = "New staff form cleared.";
        }

        private void ClearNewStaffForm()
        {
            txtNewFullName.Text = string.Empty;
            txtNewPhone.Text = string.Empty;
            txtNewEmail.Text = string.Empty;
            txtNewSalary.Text = string.Empty;
            txtNewUsername.Text = string.Empty;
            txtNewPassword.Text = string.Empty;
            txtConfirmNewPassword.Text = string.Empty;
            if (ddlNewRole.Items.Count > 0)
            {
                ddlNewRole.SelectedIndex = 0;
            }
        }

        // ============================================================
        // CLEAR SELECTED STAFF
        // ============================================================
        private void ClearSelectedStaff()
        {
            pnlSelectedStaff.Visible = false;
            pnlSelfWarning.Visible = false;
            lblStaffID.Text = "—";
            lblSelectedStaffName.Text = "—";
            lblSelectedStaffRole.Text = "—";
            lblUsername.Text = "—";
            lblEmail.Text = "—";
            lblPhone.Text = "—";
            lblHireDate.Text = "—";
            lblSalary.Text = "0.00";
            lblEmploymentStatus.Text = "—";
            lblAvailability.Text = "—";
            txtEditFullName.Text = string.Empty;
            txtEditPhone.Text = string.Empty;
            txtEditEmail.Text = string.Empty;
            txtEditSalary.Text = string.Empty;
            txtResetPassword.Text = string.Empty;
            txtConfirmResetPassword.Text = string.Empty;
            if (ddlEditRole.Items.Count > 0)
            {
                ddlEditRole.SelectedIndex = 0;
            }

            btnActivate.Enabled = false;
            btnDeactivate.Enabled = false;
            btnArchive.Enabled = false;
            btnAvailable.Enabled = false;
            btnUnavailable.Enabled = false;
        }

        // ============================================================
        // ROLE HELPER
        // ============================================================
        private void SelectRole(DropDownList dropdown, int roleID)
        {
            if (dropdown == null || roleID <= 0)
            {
                return;
            }

            ListItem item = dropdown.Items.FindByValue(roleID.ToString());
            if (item == null)
            {
                return;
            }

            dropdown.ClearSelection();
            item.Selected = true;
        }

        // ============================================================
        // DATABASE HELPERS
        // ============================================================
        private string GetDatabaseString(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
            {
                return string.Empty;
            }

            if (row[columnName] == DBNull.Value)
            {
                return string.Empty;
            }

            string value = Convert.ToString(row[columnName]);
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value;
        }

        private int GetDatabaseInt(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
            {
                return 0;
            }

            if (row[columnName] == DBNull.Value)
            {
                return 0;
            }

            int value;
            if (!int.TryParse(Convert.ToString(row[columnName]), out value))
            {
                return 0;
            }

            return value;
        }

        private bool GetDatabaseBool(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
            {
                return false;
            }

            if (row[columnName] == DBNull.Value)
            {
                return false;
            }

            bool value;
            if (!bool.TryParse(Convert.ToString(row[columnName]), out value))
            {
                return false;
            }

            return value;
        }

        private decimal GetDatabaseDecimal(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
            {
                return 0m;
            }

            if (row[columnName] == DBNull.Value)
            {
                return 0m;
            }

            decimal value;
            if (!decimal.TryParse(Convert.ToString(row[columnName]), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return 0m;
            }

            return value;
        }

        private string GetDatabaseMoney(DataRow row, string columnName)
        {
            return GetDatabaseDecimal(row, columnName).ToString("N2");
        }

        private string GetDatabaseDate(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
            {
                return "—";
            }

            if (row[columnName] == DBNull.Value)
            {
                return "—";
            }

            DateTime date;
            if (!DateTime.TryParse(Convert.ToString(row[columnName]), out date))
            {
                return "—";
            }

            return date.ToString("dd MMM yyyy");
        }

        // ============================================================
        // VALIDATION
        // ============================================================
        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            if (phone.Length != 10)
            {
                return false;
            }

            foreach (char character in phone)
            {
                if (!char.IsDigit(character))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                MailAddress address = new MailAddress(email);
                return string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                return false;
            }

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSpecial = false;
            foreach (char character in password)
            {
                if (char.IsUpper(character))
                {
                    hasUpper = true;
                }
                else if (char.IsLower(character))
                {
                    hasLower = true;
                }
                else if (char.IsDigit(character))
                {
                    hasDigit = true;
                }
                else
                {
                    hasSpecial = true;
                }
            }

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }
    }
}
