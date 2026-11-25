using MajorProject.Business.Helpers;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace MajorProject.Web.Secure.Customer
{
    public partial class Profile : CustomerBasePage
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
        // CUSTOMER SERVICE
        // ============================================================
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
            if (!WebSession.IsAuthenticated || !WebSession.HasRole("Customer") || WebSession.CustomerID <= 0)
            {
                return;
            }

            if (!IsPostBack)
            {
                LoadServiceAreas();
                LoadCustomerProfile();
                pnlSuccess.Visible = false;
                pnlError.Visible = false;
            }
        }

        // ============================================================
        // LOAD SERVICE AREAS
        // ============================================================
        private void LoadServiceAreas()
        {
            ddlArea.Items.Clear();
            ddlArea.Items.Add(new System.Web.UI.WebControls.ListItem("-- Select Service Area --", ""));
            ddlArea.Items.Add(new System.Web.UI.WebControls.ListItem("Umhlanga", "Umhlanga"));
            ddlArea.Items.Add(new System.Web.UI.WebControls.ListItem("Bearea", "Bearea"));
            ddlArea.Items.Add(new System.Web.UI.WebControls.ListItem("Morningside", "Morningside"));
            ddlArea.Items.Add(new System.Web.UI.WebControls.ListItem("KwaMashu", "KwaMashu"));
            ddlArea.Items.Add(new System.Web.UI.WebControls.ListItem("Durban North", "Durban North"));
            ddlArea.Items.Add(new System.Web.UI.WebControls.ListItem("PineTown", "PineTown"));
        }

        // ============================================================
        // LOAD CUSTOMER PROFILE
        // ============================================================
        private void LoadCustomerProfile()
        {
            DataTable customer = CustomerService.GetCustomerByID(WebSession.CustomerID);
            if (customer == null || customer.Rows.Count == 0)
            {
                ShowError("Your customer account could not be found.");
                return;
            }

            DataRow row = customer.Rows[0];
            // --------------------------------------------------------
            // PERSONAL INFORMATION
            // --------------------------------------------------------
            txtFullName.Text = GetString(row, "FullName");
            txtPhoneNumber.Text = GetString(row, "PhoneNumber");
            txtEmailAddress.Text = GetString(row, "EmailAddress");
            txtAddress.Text = GetString(row, "FullAddress");
            // --------------------------------------------------------
            // SERVICE AREA
            // --------------------------------------------------------
            string area = GetString(row, "Area");
            if (!string.IsNullOrWhiteSpace(area) && ddlArea.Items.FindByValue(area) != null)
            {
                ddlArea.SelectedValue = area;
            }
            else
            {
                ddlArea.SelectedIndex = 0;
            }

            // --------------------------------------------------------
            // ACCOUNT INFORMATION
            // --------------------------------------------------------
            lblCustomerType.Text = Server.HtmlEncode(GetString(row, "CustomerType", "Not specified"));
            lblAccountStatus.Text = Server.HtmlEncode(GetString(row, "AccountStatus", "Unknown"));
            lblCity.Text = Server.HtmlEncode(GetString(row, "City", "Durban"));
            lblProvince.Text = Server.HtmlEncode(GetString(row, "Province", "KwaZulu-Natal"));
            // --------------------------------------------------------
            // REGISTRATION DATE
            // --------------------------------------------------------
            DateTime dateRegistered;
            if (TryGetDate(row, "DateRegistered", out dateRegistered))
            {
                lblDateRegistered.Text = dateRegistered.ToString("dd MMMM yyyy");
            }
            else
            {
                lblDateRegistered.Text = "Not available";
            }
        }

        // ============================================================
        // SAVE PROFILE
        // ============================================================
        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            pnlSuccess.Visible = false;
            pnlError.Visible = false;
            // --------------------------------------------------------
            // GET VALUES
            // --------------------------------------------------------
            string phoneNumber = txtPhoneNumber.Text.Trim();
            string emailAddress = txtEmailAddress.Text.Trim();
            string address = txtAddress.Text.Trim();
            string area = ddlArea.SelectedValue;
            // --------------------------------------------------------
            // VALIDATE PHONE
            // --------------------------------------------------------
            if (!IsValidPhoneNumber(phoneNumber))
            {
                ShowError("Please enter a valid 10-digit South African phone number.");
                return;
            }

            // --------------------------------------------------------
            // VALIDATE EMAIL
            // --------------------------------------------------------
            if (!IsValidEmail(emailAddress))
            {
                ShowError("Please enter a valid email address.");
                return;
            }

            // --------------------------------------------------------
            // VALIDATE ADDRESS
            // --------------------------------------------------------
            if (string.IsNullOrWhiteSpace(address))
            {
                ShowError("Please enter your physical address.");
                return;
            }

            if (address.Length > 500)
            {
                ShowError("Your physical address cannot exceed 500 characters.");
                return;
            }

            // --------------------------------------------------------
            // VALIDATE AREA
            // --------------------------------------------------------
            if (string.IsNullOrWhiteSpace(area))
            {
                ShowError("Please select your service area.");
                return;
            }

            try
            {
                // ----------------------------------------------------
                // UPDATE ONLY EDITABLE PROFILE INFORMATION
                // ----------------------------------------------------
                CustomerService.UpdateCustomer(WebSession.CustomerID, phoneNumber, emailAddress, address, area);
                // ----------------------------------------------------
                // SUCCESS
                // ----------------------------------------------------
                ShowSuccess("Your profile has been updated successfully.");
                // ----------------------------------------------------
                // RELOAD PROFILE
                // ----------------------------------------------------
                LoadCustomerProfile();
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to update customer profile. CustomerID: " + WebSession.CustomerID, ex);
                ShowError("Unable to update your profile right now. Please try again.");
            }
        }

        // ============================================================
        // CHANGE PASSWORD
        // ============================================================
        // ============================================================
        // VALIDATION
        // ============================================================
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return false;
            }

            phoneNumber = Regex.Replace(phoneNumber, @"\D", string.Empty);
            return Regex.IsMatch(phoneNumber, @"^\d{10}$");
        }

        private bool IsValidEmail(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return false;
            }

            if (emailAddress.Length > 255)
            {
                return false;
            }

            try
            {
                System.Net.Mail.MailAddress address = new System.Net.Mail.MailAddress(emailAddress);
                return string.Equals(address.Address, emailAddress, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        // ============================================================
        // DATA HELPERS
        // ============================================================
        private string GetString(DataRow row, string columnName)
        {
            return GetString(row, columnName, string.Empty);
        }

        private string GetString(DataRow row, string columnName, string fallback)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName))
            {
                return fallback;
            }

            if (row[columnName] == DBNull.Value)
            {
                return fallback;
            }

            string value = Convert.ToString(row[columnName]);
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            return value;
        }

        private bool TryGetDate(DataRow row, string columnName, out DateTime value)
        {
            value = DateTime.MinValue;
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName))
            {
                return false;
            }

            if (row[columnName] == DBNull.Value)
            {
                return false;
            }

            return DateTime.TryParse(Convert.ToString(row[columnName]), out value);
        }

        // ============================================================
        // MESSAGES
        // ============================================================
        private void ShowSuccess(string message)
        {
            lblSuccess.Text = Server.HtmlEncode(message);
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
        }

        private void ShowError(string message)
        {
            lblError.Text = Server.HtmlEncode(message);
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
        }
    }
}
