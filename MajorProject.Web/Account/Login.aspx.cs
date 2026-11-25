using MajorProject.Business.Helpers;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Web.Security;

namespace MajorProject.Web.Account
{
    public partial class Login : BasePage
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

        private AuthenticationService AuthenticationService
        {
            get
            {
                return new AuthenticationService(ConnectionString);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblError.Visible = false;
                lblResetSuccess.Visible = false;
                if (WebSession.IsAuthenticated)
                {
                    RedirectToDashboard();
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;
            string usernameOrEmail = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(usernameOrEmail))
            {
                ShowError("Please enter your username or email address.");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Please enter your password.");
                return;
            }

            try
            {
                AuthenticationService authentication = AuthenticationService;
                // =====================================================
                // TRY STAFF LOGIN
                // =====================================================
                int staffID;
                string staffFullName;
                string roleName;
                bool staffAuthenticated = authentication.AuthenticateUser(usernameOrEmail, password, out staffID, out staffFullName, out roleName);
                if (staffAuthenticated)
                {
                    EstablishAuthenticatedSession(staffID, staffFullName, roleName);
                    FormsAuthentication.SetAuthCookie(staffID.ToString(), false);
                    RedirectToDashboard();
                    return;
                }

                // =====================================================
                // TRY CUSTOMER LOGIN
                // =====================================================
                int customerID;
                string customerFullName;
                string customerEmail;
                bool customerAuthenticated = authentication.AuthenticateCustomer(usernameOrEmail, password, out customerID, out customerFullName, out customerEmail);
                if (customerAuthenticated)
                {
                    EstablishCustomerSession(customerID, customerFullName, customerEmail);
                    FormsAuthentication.SetAuthCookie("Customer-" + customerID, false);
                    RedirectToDashboard();
                    return;
                }

                // =====================================================
                // INVALID LOGIN
                // =====================================================
                ShowError("Invalid username/email or password.");
            }
            catch (ConfigurationErrorsException)
            {
                Logger.Error("Login failed because authentication configuration is invalid.");
                ShowError("The application authentication configuration is unavailable.");
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error during web login.", ex);
                ShowError("Unable to sign you in right now. Please try again.");
            }
        }

        // =============================================================
        // STAFF SESSION
        // =============================================================
        private void EstablishAuthenticatedSession(int staffID, string fullName, string roleName)
        {
            SecurityGateway.EndCurrent();
            WebSession.StartSession(staffID, fullName, roleName);
        }

        // =============================================================
        // CUSTOMER SESSION
        // =============================================================
        private void EstablishCustomerSession(int customerID, string fullName, string email)
        {
            SecurityGateway.EndCurrent();
            WebSession.StartCustomerSession(customerID, fullName, email);
        }

        // =============================================================
        // DASHBOARD REDIRECTION
        // =============================================================
        private void RedirectToDashboard()
        {
            string destination;
            switch (WebSession.Role.Trim().ToLowerInvariant())
            {
                case "sysadmin":
                    destination = "~/Secure/SysAdmin/Dashboard.aspx";
                    break;
                case "administrator":
                    destination = "~/Secure/Admin/Dashboard.aspx";
                    break;
                case "manager":
                    destination = "~/Secure/Manager/Dashboard.aspx";
                    break;
                case "technician":
                    destination = "~/Secure/Technician/Dashboard.aspx";
                    break;
                case "customer":
                    destination = "~/Secure/Customer/Dashboard.aspx";
                    break;
                default:
                    Logger.Warning("Authenticated user has an unrecognised role. " + "StaffID: " + WebSession.StaffID + ", CustomerID: " + WebSession.CustomerID + ", Role: [" + WebSession.Role + "]");
                    FormsAuthentication.SignOut();
                    WebSession.Clear();
                    Response.Redirect(ResolveUrl("~/Account/AccessDenied.aspx"), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
            }

            Response.Redirect(ResolveUrl(destination), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        // =============================================================
        // ERROR MESSAGE
        // =============================================================
        private void ShowError(string message)
        {
            lblError.Text = Server.HtmlEncode(message ?? "Unable to sign in.");
            lblError.Visible = true;
        }

        // =============================================================
        // LEGACY EVENT
        // =============================================================
        protected void AccountType_Changed(object sender, EventArgs e)
        {
        // Retained for compatibility with older designer/event references.
        }
    }
}
