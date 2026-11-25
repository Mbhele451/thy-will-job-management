using System;
using System.Web.UI;
using MajorProject.Web.Helpers;

namespace MajorProject.Web.MasterPages
{
    public partial class Dashboard : MasterPage
    {
        /// <summary>
        /// Returns the dashboard URL appropriate for the
        /// authenticated staff member.
        ///
        /// Customer navigation does not use this property.
        /// Customers have their own sidebar.
        /// </summary>
        protected string StaffDashboardUrl
        {
            get
            {
                if (WebSession.HasRole("Administrator"))
                {
                    return ResolveUrl("~/Secure/Admin/Dashboard.aspx");
                }

                if (WebSession.HasRole("Manager"))
                {
                    return ResolveUrl("~/Secure/Manager/Dashboard.aspx");
                }

                if (WebSession.HasRole("Technician"))
                {
                    return ResolveUrl("~/Secure/Technician/Dashboard.aspx");
                }

                return ResolveUrl("~/Account/AccessDenied.aspx");
            }
        }

        /// <summary>
        /// Dashboard.Master is only intended for authenticated
        /// sections of the application.
        ///
        /// Individual pages remain responsible for enforcing
        /// their specific role requirements through:
        ///
        /// AdminBasePage
        /// ManagerBasePage
        /// TechnicianBasePage
        /// CustomerBasePage
        /// </summary>
        protected void Page_Init(object sender, EventArgs e)
        {
            AccessControl.RequireLogin();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!WebSession.IsLoggedIn)
            {
                return;
            }

            ConfigureShell();
            ConfigureUserInterface();
            ConfigureRoleNavigation();
        }

        /// <summary>
        /// Determines which application shell is displayed.
        ///
        /// Customer:
        ///     Sidebar navigation
        ///     Chatbot
        ///
        /// Admin/Manager/Technician:
        ///     Top navigation
        ///     Chatbot
        ///
        /// The same Chatbot.ascx control is used for both
        /// customers and staff. Chatbot.ascx.cs determines
        /// which chatbot service method should be called
        /// based on the authenticated role.
        /// </summary>
        private void ConfigureShell()
        {
            bool isCustomer = WebSession.HasRole("Customer");
            bool isStaff = WebSession.HasRole("Administrator") || WebSession.HasRole("Manager") || WebSession.HasRole("Technician");
            /*
             * Customer gets the sidebar.
             */
            customerSidebar.Visible = isCustomer;
            customerSidebarOverlay.Visible = isCustomer;
            /*
             * Staff get the top navigation.
             */
            staffTopbar.Visible = isStaff;
            /*
             * The chatbot is available to authenticated
             * customers and authenticated staff.
             *
             * Chatbot.ascx.cs handles the role-specific
             * routing:
             *
             * Customer   -> ProcessCustomerMessage()
             * Technician -> ProcessTechnicianMessage()
             *
             * Manager/Admin routing can be added later.
             */
            bool canUseChatbot = (isCustomer && WebSession.CustomerID > 0) || (isStaff && (WebSession.StaffID > 0));
            CustomerChatbotContainer.Visible = canUseChatbot;
            /*
             * The content area is visible for authenticated
             * customers and staff.
             */
            dashboardMain.Visible = isCustomer || isStaff;
        }

        /// <summary>
        /// Populates user information displayed in the
        /// dashboard shell.
        ///
        /// Values are HTML encoded before being rendered.
        /// </summary>
        private void ConfigureUserInterface()
        {
            string fullName = WebSession.FullName ?? string.Empty;
            string role = WebSession.Role ?? string.Empty;
            string initials = GetInitials(fullName);
            /*
             * Staff user information.
             */
            StaffUserFullName.Text = Server.HtmlEncode(fullName);
            StaffUserRole.Text = Server.HtmlEncode(role);
            StaffUserInitials.Text = Server.HtmlEncode(initials);
        }

        /// <summary>
        /// Controls role-specific navigation.
        ///
        /// IMPORTANT:
        ///
        /// These visibility settings are only a UI convenience.
        /// They are NOT authorization.
        ///
        /// Authorization remains enforced by the individual
        /// page BasePage classes and server-side permission checks.
        /// </summary>
        private void ConfigureRoleNavigation()
        {
            bool isAdmin = WebSession.HasRole("Administrator");
            bool isManager = WebSession.HasRole("Manager");
            bool isTechnician = WebSession.HasRole("Technician");
            AdminTopNavigation.Visible = isAdmin;
            ManagerTopNavigation.Visible = isManager;
            TechnicianTopNavigation.Visible = isTechnician;
        }

        /// <summary>
        /// Creates two-letter initials from a user's full name.
        ///
        /// Examples:
        ///
        /// John Smith -> JS
        /// Lerato -> LE
        /// </summary>
        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "TW";
            }

            string[] parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            /*
             * Single-name user.
             */
            if (parts.Length == 1)
            {
                string name = parts[0];
                return name.Substring(0, Math.Min(2, name.Length)).ToUpperInvariant();
            }

            /*
             * First name + last name.
             */
            string first = parts[0];
            string last = parts[parts.Length - 1];
            return (first.Substring(0, 1) + last.Substring(0, 1)).ToUpperInvariant();
        }
    }
}
