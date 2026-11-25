using MajorProject.Business.Helpers;
using MajorProject.Business.Services;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI;

namespace MajorProject.Web.Account
{
    public partial class ForgotPassword : Page
    {
        private string ConnectionString
        {
            get
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"];
                if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    throw new InvalidOperationException("The database connection string 'GroupWst34ConnectionString' is not configured.");
                }

                return settings.ConnectionString;
            }
        }

        private AuthenticationService _authService;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _authService = new AuthenticationService(ConnectionString);
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to initialize AuthenticationService.", ex);
                ShowError("Unable to process your request right now. " + "Please try again later.");
                return;
            }

            if (!IsPostBack)
            {
                lblMessage.Visible = false;
                lblMessage.Text = string.Empty;
                ClearResetSession();
            }
        }

        // =============================================================
        // SEND OTP
        // =============================================================
        protected async void btnSendOtp_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            lblMessage.Text = string.Empty;
            string email = txtEmail.Text.Trim();
            // =========================================================
            // BASIC VALIDATION
            // =========================================================
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Please enter your email address.");
                txtEmail.Focus();
                return;
            }

            if (!ValidationHelper.IsValidEmail(email))
            {
                ShowError("Please enter a valid email address.");
                txtEmail.Focus();
                return;
            }

            try
            {
                // =====================================================
                // STEP 1: CHECK STAFF ACCOUNT
                // =====================================================
                DataTable staffDt = _authService.GetStaffByEmail(email);
                if (staffDt != null && staffDt.Rows.Count > 0)
                {
                    DataRow staffRow = staffDt.Rows[0];
                    int staffID = Convert.ToInt32(staffRow["StaffID"]);
                    string employmentStatus = staffRow["EmploymentStatus"]?.ToString() ?? string.Empty;
                    // Only active staff members can reset passwords.
                    if (string.Equals(employmentStatus, "Active", StringComparison.OrdinalIgnoreCase))
                    {
                        bool otpSent = await _authService.GenerateAndSendOtpAsync(email);
                        if (otpSent)
                        {
                            SetResetSession(email, "Staff", staffID);
                            RedirectToResetPassword();
                            return;
                        }
                    }

                    // Always return the same generic response.
                    ShowGenericSuccess();
                    return;
                }

                // =====================================================
                // STEP 2: CHECK CUSTOMER ACCOUNT
                // =====================================================
                DataTable customerDt = _authService.GetCustomerByEmail(email);
                if (customerDt != null && customerDt.Rows.Count > 0)
                {
                    DataRow customerRow = customerDt.Rows[0];
                    string accountStatus = customerRow["AccountStatus"]?.ToString() ?? string.Empty;
                    // Deactivated customers must not be allowed
                    // to recover their account password.
                    if (string.Equals(accountStatus, "Active", StringComparison.OrdinalIgnoreCase))
                    {
                        int customerID = Convert.ToInt32(customerRow["CustomerID"]);
                        bool otpSent = await _authService.GenerateAndSendCustomerOtpAsync(email);
                        if (otpSent)
                        {
                            SetResetSession(email, "Customer", customerID);
                            RedirectToResetPassword();
                            return;
                        }
                    }

                    // Do not reveal that the account exists.
                    ShowGenericSuccess();
                    return;
                }

                // =====================================================
                // STEP 3: NO ACCOUNT FOUND
                // =====================================================
                /*
                 * Do not reveal whether an email address exists.
                 * This helps prevent account enumeration.
                 */
                ShowGenericSuccess();
            }
            catch (Exception ex)
            {
                Logger.Error("Error processing forgot password request.", ex);
                ShowError("Unable to process your request right now. " + "Please try again later.");
            }
        }

        // =============================================================
        // RESET SESSION
        // =============================================================
        private void SetResetSession(string email, string accountType, int accountID)
        {
            Session["ResetEmail"] = email;
            Session["ResetAccountType"] = accountType;
            Session["ResetAccountID"] = accountID;
        }

        private void ClearResetSession()
        {
            Session["ResetEmail"] = null;
            Session["ResetAccountType"] = null;
            Session["ResetAccountID"] = null;
        }

        // =============================================================
        // REDIRECT
        // =============================================================
        private void RedirectToResetPassword()
        {
            Response.Redirect("~/Account/ResetPassword.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        // =============================================================
        // GENERIC SUCCESS MESSAGE
        // =============================================================
        private void ShowGenericSuccess()
        {
            ShowSuccess("If an account matches that email address, " + "a verification code has been sent.");
        }

        // =============================================================
        // ERROR MESSAGE
        // =============================================================
        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "tw-booking-message text-danger d-block mb-3";
            lblMessage.Visible = true;
        }

        // =============================================================
        // SUCCESS MESSAGE
        // =============================================================
        private void ShowSuccess(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "tw-booking-message text-success d-block mb-3";
            lblMessage.Visible = true;
        }
    }
}
