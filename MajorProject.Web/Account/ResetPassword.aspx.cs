using MajorProject.Business.Helpers;
using MajorProject.Business.Services;
using System;
using System.Configuration;
using System.Web.UI;

namespace MajorProject.Web.Account
{
    public partial class ResetPassword : Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString;
        private AuthenticationService _authService;
        protected void Page_Load(object sender, EventArgs e)
        {
            _authService = new AuthenticationService(_connectionString);
            if (!IsPostBack)
            {
                // =====================================================
                // VERIFY RESET SESSION
                // =====================================================
                string email = Session["ResetEmail"] as string;
                string accountType = Session["ResetAccountType"] as string;
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(accountType) || Session["ResetAccountID"] == null)
                {
                    RedirectToForgotPassword();
                    return;
                }

                // Make sure the account type is valid.
                if (!accountType.Equals("Staff", StringComparison.OrdinalIgnoreCase) && !accountType.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                {
                    ClearResetSession();
                    RedirectToForgotPassword();
                    return;
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            lblMessage.Text = string.Empty;
            string otp = txtOtp.Text.Trim();
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            string email = Session["ResetEmail"] as string;
            string accountType = Session["ResetAccountType"] as string;
            // =========================================================
            // SESSION VALIDATION
            // =========================================================
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(accountType) || Session["ResetAccountID"] == null)
            {
                ShowError("Your password reset session has expired. " + "Please request a new verification code.");
                return;
            }

            if (!accountType.Equals("Staff", StringComparison.OrdinalIgnoreCase) && !accountType.Equals("Customer", StringComparison.OrdinalIgnoreCase))
            {
                ClearResetSession();
                ShowError("Unable to process the password reset. " + "Please request a new verification code.");
                return;
            }

            // =========================================================
            // ACCOUNT ID
            // =========================================================
            if (!int.TryParse(Session["ResetAccountID"].ToString(), out int accountID) || accountID <= 0)
            {
                ClearResetSession();
                ShowError("Your password reset session is invalid. " + "Please request a new verification code.");
                return;
            }

            // =========================================================
            // OTP VALIDATION
            // =========================================================
            if (string.IsNullOrWhiteSpace(otp))
            {
                ShowError("Please enter the 6-digit verification code.");
                txtOtp.Focus();
                return;
            }

            if (otp.Length != 6 || !IsNumeric(otp))
            {
                ShowError("The verification code must contain 6 digits.");
                txtOtp.Focus();
                return;
            }

            // =========================================================
            // PASSWORD VALIDATION
            // =========================================================
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ShowError("Please enter a new password.");
                txtNewPassword.Focus();
                return;
            }

            if (!ValidationHelper.IsValidPassword(newPassword))
            {
                ShowError("Password does not meet all security requirements.");
                txtNewPassword.Focus();
                return;
            }

            if (newPassword != confirmPassword)
            {
                ShowError("Passwords do not match.");
                txtConfirmPassword.Focus();
                return;
            }

            try
            {
                // =====================================================
                // STAFF RESET
                // =====================================================
                if (accountType.Equals("Staff", StringComparison.OrdinalIgnoreCase))
                {
                    bool otpValid = _authService.ValidateOtp(accountID, otp);
                    if (!otpValid)
                    {
                        ShowError("Invalid or expired verification code. " + "Please request a new one.");
                        txtOtp.Focus();
                        return;
                    }

                    _authService.ResetPassword(accountID, newPassword);
                }
                // =====================================================
                // CUSTOMER RESET
                // =====================================================
                else if (accountType.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                {
                    bool otpValid = _authService.ValidateCustomerOtp(accountID, otp);
                    if (!otpValid)
                    {
                        ShowError("Invalid or expired verification code. " + "Please request a new one.");
                        txtOtp.Focus();
                        return;
                    }

                    _authService.ResetCustomerPassword(accountID, newPassword);
                }

                // =====================================================
                // CLEAR RESET SESSION
                // =====================================================
                ClearResetSession();
                // =====================================================
                // SUCCESS
                // =====================================================
                Response.Redirect("~/Account/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error resetting password for account type " + $"{accountType}.", ex);
                ShowError("An unexpected error occurred while resetting " + "your password. Please try again later.");
            }
        }

        // =============================================================
        // NUMERIC VALIDATION
        // =============================================================
        private bool IsNumeric(string value)
        {
            foreach (char character in value)
            {
                if (!char.IsDigit(character))
                    return false;
            }

            return true;
        }

        // =============================================================
        // CLEAR RESET SESSION
        // =============================================================
        private void ClearResetSession()
        {
            Session.Remove("ResetEmail");
            Session.Remove("ResetAccountType");
            Session.Remove("ResetAccountID");
        }

        // =============================================================
        // REDIRECT TO FORGOT PASSWORD
        // =============================================================
        private void RedirectToForgotPassword()
        {
            Response.Redirect("~/Account/ForgotPassword.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
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
    }
}
