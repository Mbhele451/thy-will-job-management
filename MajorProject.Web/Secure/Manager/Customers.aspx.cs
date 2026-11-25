using MajorProject.Business.Helpers;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace MajorProject.Web.Manager
{
    public partial class Customers : ManagerBasePage
    {
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

        private CustomerService CustomerService
        {
            get
            {
                return new CustomerService(ConnectionString);
            }
        }

        // ============================================================
        // PAGE LOAD
        // ============================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlSort.SelectedValue = "NameAsc";
                btnSaveProfile.Enabled = true;
                btnUpdateProfile.Enabled = false;
                btnDeactivateCustomer.Enabled = false;
                btnActivateCustomer.Enabled = false;
                ddlAccountStatus.SelectedValue = "Active";
                SetIdentityFieldsForRegistration();
                LoadCustomers();
            }
        }

        // ============================================================
        // LOAD CUSTOMERS
        // ============================================================
        private void LoadCustomers()
        {
            try
            {
                DataTable customers;
                string search = txtSearch.Text.Trim();
                if (string.IsNullOrWhiteSpace(search))
                {
                    customers = CustomerService.GetAllCustomers();
                }
                else
                {
                    customers = CustomerService.SearchCustomersByName(search);
                }

                SortCustomers(customers);
                gvCustomers.DataSource = customers;
                gvCustomers.DataBind();
            }
            catch (SqlException ex)
            {
                HandleUnexpectedError("Unable to load the customer directory.", ex);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Unable to load the customer directory.", ex);
            }
        }

        // ============================================================
        // SORT
        // ============================================================
        private void SortCustomers(DataTable customers)
        {
            if (customers == null || customers.Rows.Count == 0)
            {
                return;
            }

            string sortExpression;
            switch (ddlSort.SelectedValue)
            {
                case "NameDesc":
                    sortExpression = "FullName DESC";
                    break;
                case "DateDesc":
                    sortExpression = "DateRegistered DESC";
                    break;
                case "DateAsc":
                    sortExpression = "DateRegistered ASC";
                    break;
                case "CustomerType":
                    sortExpression = "CustomerType ASC, FullName ASC";
                    break;
                case "Area":
                    sortExpression = "Area ASC, FullName ASC";
                    break;
                case "AccountStatus":
                    sortExpression = "AccountStatus ASC, FullName ASC";
                    break;
                case "NameAsc":
                default:
                    sortExpression = "FullName ASC";
                    break;
            }

            customers.DefaultView.Sort = sortExpression;
        }

        // ============================================================
        // SEARCH
        // ============================================================
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            ddlSort.SelectedValue = "NameAsc";
            LoadCustomers();
        }

        // ============================================================
        // SORT DROPDOWN
        // ============================================================
        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        // ============================================================
        // SELECT CUSTOMER
        // ============================================================
        protected void gvCustomers_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "SelectCustomer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                int customerID;
                if (!int.TryParse(Convert.ToString(e.CommandArgument), out customerID))
                {
                    ShowError("The selected customer could not be identified.");
                    return;
                }

                LoadCustomerProfile(customerID);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Unable to load the selected customer.", ex);
            }
        }

        private void LoadCustomerProfile(int customerID)
        {
            DataTable customers = CustomerService.GetAllCustomers();
            if (customers == null || customers.Rows.Count == 0)
            {
                ShowError("The customer could not be found.");
                return;
            }

            DataRow[] rows = customers.Select("CustomerID = " + customerID);
            if (rows.Length == 0)
            {
                ShowError("The selected customer could not be found.");
                return;
            }

            DataRow customer = rows[0];
            // ========================================================
            // LOAD CUSTOMER DETAILS
            // ========================================================
            txtCustName.Text = GetValue(customer, "FullName");
            txtEmail.Text = GetValue(customer, "EmailAddress");
            txtPhone.Text = GetValue(customer, "PhoneNumber");
            txtAddress.Text = GetValue(customer, "FullAddress");
            txtSouthAfricanID.Text = GetValue(customer, "SouthAfricanIDNumber");
            txtCompanyRegistration.Text = GetValue(customer, "CompanyRegistrationNumber");
            // ========================================================
            // LOAD DROPDOWNS
            // ========================================================
            SetDropDownValue(ddlCustomerType, GetValue(customer, "CustomerType"));
            SetDropDownValue(ddlServiceArea, GetValue(customer, "Area"));
            SetDropDownValue(ddlAccountStatus, GetValue(customer, "AccountStatus"));
            // ========================================================
            // STORE SELECTED CUSTOMER
            // ========================================================
            txtPassportNumber.Text = GetValue(customer, "PassportNumber");
            txtPassportCountry.Text = GetValue(customer, "PassportCountry");
            ddlNationality.SelectedValue = string.IsNullOrWhiteSpace(txtPassportNumber.Text) ? "SA" : "Passport";
            ddlNationality.Enabled = false;
            txtPassportNumber.ReadOnly = true;
            txtPassportCountry.ReadOnly = true;
            hfSelectedCustomerID.Value = customerID.ToString();
            ViewState["SelectedCustomerID"] = customerID;
            Session["ManagerSelectedCustomerID"] = customerID;
            // ========================================================
            // PROFILE STATE
            // ========================================================
            /*
             * Full name, customer type and identity information
             * are not editable during an update.
             *
             * CustomerService.UpdateCustomer() currently changes:
             *
             * - Phone number
             * - Email address
             * - Full address
             * - Service area
             */
            txtCustName.ReadOnly = true;
            txtSouthAfricanID.ReadOnly = true;
            txtCompanyRegistration.ReadOnly = true;
            ddlCustomerType.Enabled = false;
            btnSaveProfile.Enabled = false;
            btnUpdateProfile.Enabled = true;
            // ========================================================
            // ACCOUNT STATUS / ACTION BUTTONS
            // ========================================================
            string accountStatus = GetValue(customer, "AccountStatus");
            if (string.Equals(accountStatus, "Deactivated", StringComparison.OrdinalIgnoreCase))
            {
                btnDeactivateCustomer.Enabled = false;
                btnActivateCustomer.Enabled = true;
                ShowInfo("This customer account is currently deactivated.");
            }
            else
            {
                btnDeactivateCustomer.Enabled = true;
                btnActivateCustomer.Enabled = false;
                ShowInfo("Customer profile loaded.");
            }

            SetIdentityFieldsForExistingCustomer();
            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "open-customer-profile", "if(window.ThyWillUI){window.ThyWillUI.initialize();window.ThyWillUI.activateTab('secure-manager-customers','details');}", true);
        }

        // ============================================================
        // CREATE CUSTOMER
        // ============================================================
        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateCustomerForm();
                string temporaryPassword;
                string southAfricanIDNumber = txtSouthAfricanID.Text.Trim();
                string companyRegistrationNumber = txtCompanyRegistration.Text.Trim();
                if (ddlCustomerType.SelectedValue == "Residential")
                {
                    companyRegistrationNumber = string.Empty;
                }
                else if (ddlCustomerType.SelectedValue == "Business")
                {
                    southAfricanIDNumber = string.Empty;
                }

                int customerID = CustomerService.CreateCustomer(txtCustName.Text.Trim(), txtPhone.Text.Trim(), txtEmail.Text.Trim(), txtAddress.Text.Trim(), ddlServiceArea.SelectedValue, "Durban", "KwaZulu-Natal", ddlCustomerType.SelectedValue, ddlNationality.SelectedValue == "Passport" ? string.Empty : southAfricanIDNumber, companyRegistrationNumber, ddlCustomerType.SelectedValue == "Residential" && ddlNationality.SelectedValue == "Passport" ? txtPassportNumber.Text.Trim() : string.Empty, ddlCustomerType.SelectedValue == "Residential" && ddlNationality.SelectedValue == "Passport" ? txtPassportCountry.Text.Trim() : string.Empty, out temporaryPassword);
                Session["ManagerSelectedCustomerID"] = customerID;
                LoadCustomers();
                ClearForm();
                ShowSuccess("Customer registered successfully. The customer can use Forgot Password with their registered email address.");
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
            catch (SqlException ex)
            {
                HandleUnexpectedError("The customer could not be registered because of a database error.", ex);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Unable to register the customer.", ex);
            }
        }

        // ============================================================
        // UPDATE CUSTOMER
        // ============================================================
        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            try
            {
                int customerID = GetSelectedCustomerID();
                if (customerID <= 0)
                {
                    ShowError("Please select a customer first.");
                    return;
                }

                ValidateCustomerUpdateForm();
                /*
                 * CustomerService.UpdateCustomer() accepts only:
                 *
                 * customerID
                 * phoneNumber
                 * email
                 * address
                 * area
                 *
                 * Customer type and identity fields are intentionally
                 * not changed here.
                 */
                CustomerService.UpdateCustomer(customerID, txtPhone.Text.Trim(), txtEmail.Text.Trim(), txtAddress.Text.Trim(), ddlServiceArea.SelectedValue);
                LoadCustomers();
                LoadCustomerProfile(customerID);
                ShowSuccess("Customer profile updated successfully.");
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
            catch (SqlException ex)
            {
                HandleUnexpectedError("The customer could not be updated because of a database error.", ex);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Unable to update the customer profile.", ex);
            }
        }

        // ============================================================
        // DEACTIVATE CUSTOMER
        // ============================================================
        protected void btnDeactivateCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                int customerID = GetSelectedCustomerID();
                if (customerID <= 0)
                {
                    ShowError("Please select a customer first.");
                    return;
                }

                CustomerService.DeactivateCustomer(customerID);
                LoadCustomers();
                LoadCustomerProfile(customerID);
                ShowSuccess("Customer account has been deactivated. " + "The customer record has been retained.");
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
            catch (SqlException ex)
            {
                HandleUnexpectedError("The customer account could not be deactivated because of a database error.", ex);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Unable to deactivate the customer account.", ex);
            }
        }

        // ============================================================
        // ACTIVATE CUSTOMER
        // ============================================================
        protected void btnActivateCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                int customerID = GetSelectedCustomerID();
                if (customerID <= 0)
                {
                    ShowError("Please select a customer first.");
                    return;
                }

                CustomerService.ActivateCustomer(customerID);
                LoadCustomers();
                LoadCustomerProfile(customerID);
                ShowSuccess("Customer account has been activated successfully.");
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
            catch (SqlException ex)
            {
                HandleUnexpectedError("The customer account could not be activated because of a database error.", ex);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Unable to activate the customer account.", ex);
            }
        }

        // ============================================================
        // VALIDATION - NEW CUSTOMER
        // ============================================================
        private void ValidateCustomerForm()
        {
            string fullName = txtCustName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string customerType = ddlCustomerType.SelectedValue;
            string serviceArea = ddlServiceArea.SelectedValue;
            string southAfricanIDNumber = txtSouthAfricanID.Text.Trim();
            string companyRegistrationNumber = txtCompanyRegistration.Text.Trim();
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Customer full name is required.");
            }

            if (!ValidationHelper.IsValidName(fullName))
            {
                throw new ArgumentException("Please enter a valid customer name.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Customer email address is required.");
            }

            if (!ValidationHelper.IsValidEmail(email))
            {
                throw new ArgumentException("Please enter a valid email address.");
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Phone number is required.");
            }

            if (!ValidationHelper.IsValidPhoneNumber(phone))
            {
                throw new ArgumentException("Phone number must be a valid 10-digit South African number starting with 0.");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Full address is required.");
            }

            if (address.Length > 255)
            {
                throw new ArgumentException("Full address cannot exceed 255 characters.");
            }

            if (string.IsNullOrWhiteSpace(serviceArea))
            {
                throw new ArgumentException("Please select a service area.");
            }

            if (string.IsNullOrWhiteSpace(customerType))
            {
                throw new ArgumentException("Please select a customer type.");
            }

            bool business = customerType == "Business";
            bool foreign = ddlNationality.SelectedValue == "Passport";
            MajorProject.Business.Models.CustomerIdentity.Validate(customerType, !business && !foreign ? southAfricanIDNumber : string.Empty, business ? companyRegistrationNumber : string.Empty, !business && foreign ? txtPassportNumber.Text.Trim() : string.Empty, !business && foreign ? txtPassportCountry.Text.Trim() : string.Empty);
        }

        private void ValidateSouthAfricanID(string southAfricanIDNumber)
        {
            if (string.IsNullOrWhiteSpace(southAfricanIDNumber))
            {
                throw new ArgumentException("South African ID number is required for residential customers.");
            }

            if (!Regex.IsMatch(southAfricanIDNumber, @"^\d{13}$"))
            {
                throw new ArgumentException("South African ID number must contain exactly 13 digits.");
            }
        }

        private void ValidateCompanyRegistration(string companyRegistrationNumber)
        {
            if (string.IsNullOrWhiteSpace(companyRegistrationNumber))
            {
                throw new ArgumentException("Company registration number is required for business customers.");
            }

            if (companyRegistrationNumber.Length > 50)
            {
                throw new ArgumentException("Company registration number cannot exceed 50 characters.");
            }

            if (!Regex.IsMatch(companyRegistrationNumber, @"^\d{4}/\d{6}/\d{2}$"))
            {
                throw new ArgumentException("Company registration number must use the format YYYY/NNNNNN/NN.");
            }
        }

        // ============================================================
        // VALIDATION - UPDATE
        // ============================================================
        private void ValidateCustomerUpdateForm()
        {
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string serviceArea = ddlServiceArea.SelectedValue;
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Customer email address is required.");
            }

            if (!ValidationHelper.IsValidEmail(email))
            {
                throw new ArgumentException("Please enter a valid email address.");
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Phone number is required.");
            }

            if (!ValidationHelper.IsValidPhoneNumber(phone))
            {
                throw new ArgumentException("Phone number must be a valid 10-digit South African number starting with 0.");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Full address is required.");
            }

            if (address.Length > 255)
            {
                throw new ArgumentException("Full address cannot exceed 255 characters.");
            }

            if (string.IsNullOrWhiteSpace(serviceArea))
            {
                throw new ArgumentException("Please select a service area.");
            }
        }

        // ============================================================
        // CLEAR FORM
        // ============================================================
        protected void btnClearForm_Click(object sender, EventArgs e)
        {
            ClearForm();
            ShowInfo("Customer form cleared. You can now register a new customer.");
        }

        private void ClearForm()
        {
            txtCustName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtSouthAfricanID.Text = string.Empty;
            txtCompanyRegistration.Text = string.Empty;
            ddlCustomerType.SelectedIndex = 0;
            ddlServiceArea.SelectedIndex = 0;
            ddlAccountStatus.SelectedValue = "Active";
            hfSelectedCustomerID.Value = string.Empty;
            ViewState.Remove("SelectedCustomerID");
            txtCustName.ReadOnly = false;
            txtSouthAfricanID.ReadOnly = false;
            txtCompanyRegistration.ReadOnly = false;
            ddlCustomerType.Enabled = true;
            btnSaveProfile.Enabled = true;
            btnUpdateProfile.Enabled = false;
            btnDeactivateCustomer.Enabled = false;
            btnActivateCustomer.Enabled = false;
            txtPassportNumber.Text = txtPassportCountry.Text = string.Empty;
            ddlNationality.SelectedValue = "SA";
            ddlNationality.Enabled = true;
            txtPassportNumber.ReadOnly = false;
            txtPassportCountry.ReadOnly = false;
            SetIdentityFieldsForRegistration();
        }

        // ============================================================
        // IDENTITY FIELD STATE
        // ============================================================
        private void SetIdentityFieldsForRegistration()
        {
            txtSouthAfricanID.ReadOnly = false;
            txtCompanyRegistration.ReadOnly = false;
            ddlCustomerType.Enabled = true;
            string customerType = ddlCustomerType.SelectedValue;
            if (customerType == "Business")
            {
                residentialIdentityGroup.Style["display"] = "none";
                businessIdentityGroup.Style["display"] = "";
            }
            else
            {
                residentialIdentityGroup.Style["display"] = "";
                businessIdentityGroup.Style["display"] = "none";
            }
        }

        private void SetIdentityFieldsForExistingCustomer()
        {
            string customerType = ddlCustomerType.SelectedValue;
            if (customerType == "Business")
            {
                residentialIdentityGroup.Style["display"] = "none";
                businessIdentityGroup.Style["display"] = "";
            }
            else
            {
                residentialIdentityGroup.Style["display"] = "";
                businessIdentityGroup.Style["display"] = "none";
            }
        }

        // ============================================================
        // SELECTED CUSTOMER
        // ============================================================
        private int GetSelectedCustomerID()
        {
            int customerID;
            if (int.TryParse(hfSelectedCustomerID.Value, out customerID))
            {
                return customerID;
            }

            if (ViewState["SelectedCustomerID"] != null && int.TryParse(ViewState["SelectedCustomerID"].ToString(), out customerID))
            {
                return customerID;
            }

            return 0;
        }

        // ============================================================
        // DROPDOWN HELPER
        // ============================================================
        private void SetDropDownValue(System.Web.UI.WebControls.DropDownList control, string value)
        {
            if (control == null)
                return;
            control.ClearSelection();
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            System.Web.UI.WebControls.ListItem item = control.Items.FindByValue(value);
            if (item != null)
            {
                item.Selected = true;
            }
            else
            {
                control.SelectedIndex = 0;
            }
        }

        // ============================================================
        // DATA HELPER
        // ============================================================
        private string GetValue(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return string.Empty;
            }

            return row[column]?.ToString() ?? string.Empty;
        }

        // ============================================================
        // MESSAGES
        // ============================================================
        private void ShowSuccess(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "admin-customers-message admin-customers-message-success";
            lblMessage.Visible = true;
        }

        private void ShowError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "Please check the information entered and try again.";
            }

            Logger.Warning("Manager customer operation validation error: " + message);
            lblMessage.Text = message;
            lblMessage.CssClass = "admin-customers-message admin-customers-message-error";
            lblMessage.Visible = true;
        }

        private void ShowInfo(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "admin-customers-message admin-customers-message-info";
            lblMessage.Visible = true;
        }
    }
}
