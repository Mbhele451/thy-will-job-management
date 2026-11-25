using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.UI;
using MajorProject.Business.Helpers;
using MajorProject.Business.Services;

namespace MajorProject.Web.Account
{
    public partial class Register : MajorProject.Web.Helpers.BasePage
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
        // PAGE LOAD
        // ============================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!MajorProject.Web.Helpers.WebSession.HasRole("SysAdmin") && MajorProject.Web.Helpers.SecurityGateway.Setting("PublicRegistrationEnabled", "true") != "true")
                {
                    btnRegister.Enabled = false;
                    ShowError("Public registration is currently closed. Please contact the team.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Loading public registration availability.", ex);
                btnRegister.Enabled = false;
                ShowError("Registration is temporarily unavailable.");
            }

            if (!IsPostBack)
            {
                pnlForm.Visible = true;
                pnlSuccess.Visible = false;
                lblMessage.Visible = false;
                lblMessage.Text = string.Empty;
            }
        }

        // ============================================================
        // REGISTER CUSTOMER
        // ============================================================
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (!MajorProject.Web.Helpers.WebSession.HasRole("SysAdmin") && !string.Equals(MajorProject.Web.Helpers.SecurityGateway.Setting("PublicRegistrationEnabled", "true"), "true", StringComparison.Ordinal))
                {
                    ShowError("Public registration is currently closed. Please contact the team.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Checking public registration setting.", ex);
                ShowError("Registration is temporarily unavailable.");
                return;
            }

            lblMessage.Visible = false;
            lblMessage.Text = string.Empty;
            // ========================================================
            // 1. GATHER INPUT
            // ========================================================
            string fullName = txtFullName.Text.Trim();
            string phone = Regex.Replace(txtPhone.Text.Trim(), @"\D", string.Empty);
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            string customerType = ddlCustomerType.SelectedValue;
            string southAfricanIDNumber = txtSouthAfricanID.Text.Trim();
            string companyRegistrationNumber = txtCompanyRegistration.Text.Trim();
            string address = txtAddress.Text.Trim();
            string serviceArea = ddlServiceArea.SelectedValue;
            string city = txtCity.Text.Trim();
            string province = txtProvince.Text.Trim();
            // ========================================================
            // 2. REQUIRED GENERAL FIELDS
            // ========================================================
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowError("Please enter your full name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                ShowError("Please enter your phone number.");
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Please enter your email address.");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please create a password.");
                return;
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowError("Please confirm your password.");
                return;
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                ShowError("Please enter your full address.");
                return;
            }

            // ========================================================
            // 3. FULL NAME VALIDATION
            // ========================================================
            if (!ValidationHelper.IsValidName(fullName))
            {
                ShowError("Please enter a valid full name using letters and normal name punctuation.");
                return;
            }

            if (fullName.Length < 2 || fullName.Length > 100)
            {
                ShowError("Full name must be between 2 and 100 characters.");
                return;
            }

            // ========================================================
            // 4. PHONE VALIDATION
            // ========================================================
            if (!ValidationHelper.IsValidPhoneNumber(phone))
            {
                ShowError("Please enter a valid South African 10-digit phone number.");
                return;
            }

            // ========================================================
            // 5. EMAIL VALIDATION
            // ========================================================
            if (!ValidationHelper.IsValidEmail(email))
            {
                ShowError("Please enter a valid email address.");
                return;
            }

            if (email.Length > 100)
            {
                ShowError("Email address is too long.");
                return;
            }

            // ========================================================
            // 6. PASSWORD CONFIRMATION
            // ========================================================
            if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ShowError("Passwords do not match.");
                return;
            }

            // ========================================================
            // 7. PASSWORD STRENGTH
            // ========================================================
            if (!ValidationHelper.IsValidPassword(password))
            {
                ShowError("Password must contain at least 8 characters, including " + "an uppercase letter, lowercase letter, number and special character.");
                return;
            }

            // ========================================================
            // 8. ADDRESS VALIDATION
            // ========================================================
            if (address.Length < 5)
            {
                ShowError("Please enter a more complete address.");
                return;
            }

            if (address.Length > 255)
            {
                ShowError("Address is too long.");
                return;
            }

            // ========================================================
            // 9. CUSTOMER TYPE VALIDATION
            // ========================================================
            if (string.IsNullOrWhiteSpace(customerType))
            {
                ShowError("Please select a customer type.");
                return;
            }

            if (!customerType.Equals("Residential", StringComparison.OrdinalIgnoreCase) && !customerType.Equals("Business", StringComparison.OrdinalIgnoreCase))
            {
                ShowError("Invalid customer type selected.");
                return;
            }

            // ========================================================
            // 10. CUSTOMER IDENTITY VALIDATION
            // ========================================================
            string passport = ddlNationality.SelectedValue == "Passport" ? txtPassportNumber.Text.Trim() : string.Empty;
            string country = ddlNationality.SelectedValue == "Passport" ? txtPassportCountry.Text.Trim() : string.Empty;
            if (customerType == "Business")
            {
                southAfricanIDNumber = string.Empty;
                passport = country = string.Empty;
            }
            else
            {
                companyRegistrationNumber = string.Empty;
                if (ddlNationality.SelectedValue == "Passport")
                    southAfricanIDNumber = string.Empty;
            }

            try
            {
                MajorProject.Business.Models.CustomerIdentity.Validate(customerType, southAfricanIDNumber, companyRegistrationNumber, passport, country);
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
                return;
            }

            // ========================================================
            // 11. SERVICE AREA
            // ========================================================
            string[] allowedAreas =
            {
                "Umhlanga",
                "Bearea",
                "Morningside",
                "KwaMashu",
                "Durban North",
                "PineTown"
            };
            bool validArea = false;
            foreach (string allowedArea in allowedAreas)
            {
                if (serviceArea.Equals(allowedArea, StringComparison.OrdinalIgnoreCase))
                {
                    validArea = true;
                    break;
                }
            }

            if (!validArea)
            {
                ShowError("Please select a valid service area.");
                return;
            }

            // ========================================================
            // 12. CITY
            // ========================================================
            if (!city.Equals("Durban", StringComparison.OrdinalIgnoreCase))
            {
                ShowError("Invalid city value.");
                return;
            }

            // ========================================================
            // 13. PROVINCE
            // ========================================================
            if (!province.Equals("KwaZulu-Natal", StringComparison.OrdinalIgnoreCase))
            {
                ShowError("Invalid province value.");
                return;
            }

            try
            {
                // ====================================================
                // 14. INITIALISE CUSTOMER SERVICE
                // ====================================================
                CustomerService customerService = new CustomerService(ConnectionString);
                // ====================================================
                // 15. CHECK EMAIL
                // ====================================================
                if (customerService.TryFindCustomerByEmail(email, out int existingID, out string existingName))
                {
                    ShowError("An account with this email address already exists. " + "Please sign in or reset your password.");
                    return;
                }

                // ====================================================
                // 16. CREATE CUSTOMER
                // ====================================================
                int newCustomerID;
                if (MajorProject.Web.Helpers.WebSession.HasRole("SysAdmin"))
                    newCustomerID = customerService.CreateCustomerBySysAdmin(MajorProject.Web.Helpers.WebSession.StaffID, MajorProject.Web.Helpers.SecurityGateway.IP, fullName, phone, email, password, address, serviceArea, city, province, customerType, southAfricanIDNumber, companyRegistrationNumber, passport, country);
                else
                    newCustomerID = customerService.CreateCustomer(fullName, phone, email, password, address, serviceArea, city, province, customerType, southAfricanIDNumber, companyRegistrationNumber, passport, country);
                // ====================================================
                // 17. VERIFY CREATION
                // ====================================================
                if (newCustomerID <= 0)
                {
                    ShowError("We were unable to create your customer profile.");
                    return;
                }

                // ====================================================
                // 18. SUCCESS
                // ====================================================
                pnlForm.Visible = false;
                pnlSuccess.Visible = true;
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
                System.Diagnostics.Trace.TraceError("Customer registration failed: " + ex);
                System.Diagnostics.Debug.WriteLine("CUSTOMER REGISTRATION ERROR:");
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                ShowError("Registration could not be completed. Please try again or contact support.");
            }
        }

        // ============================================================
        // COMPANY REGISTRATION VALIDATION
        // ============================================================
        private bool IsValidCompanyRegistrationNumber(string registrationNumber)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
            {
                return false;
            }

            return Regex.IsMatch(registrationNumber, @"^\d{4}/\d{6}/\d{2}$");
        }

        // ============================================================
        // SHOW ERROR
        // ============================================================
        private void ShowError(string message)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.Visible = true;
        }
    }
}
