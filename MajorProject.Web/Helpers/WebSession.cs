using System;
using System.Web;
using System.Web.SessionState;

namespace MajorProject.Web.Helpers
{
    public static class WebSession
    {
        private static HttpSessionState Session
        {
            get
            {
                return HttpContext.Current?.Session;
            }
        }

        // =============================================================
        // STAFF
        // =============================================================
        public static int StaffID
        {
            get
            {
                if (Session == null || Session["StaffID"] == null)
                    return 0;
                return Convert.ToInt32(Session["StaffID"]);
            }

            set
            {
                if (Session != null)
                    Session["StaffID"] = value;
            }
        }

        // =============================================================
        // CUSTOMER
        // =============================================================
        public static int CustomerID
        {
            get
            {
                if (Session == null || Session["CustomerID"] == null)
                    return 0;
                return Convert.ToInt32(Session["CustomerID"]);
            }

            set
            {
                if (Session != null)
                    Session["CustomerID"] = value;
            }
        }

        public static string CustomerEmail
        {
            get
            {
                return Convert.ToString(Session?["CustomerEmail"]) ?? string.Empty;
            }

            set
            {
                if (Session != null)
                    Session["CustomerEmail"] = value ?? string.Empty;
            }
        }

        // =============================================================
        // COMMON USER INFORMATION
        // =============================================================
        public static string FullName
        {
            get
            {
                return Convert.ToString(Session?["FullName"]) ?? string.Empty;
            }

            set
            {
                if (Session != null)
                    Session["FullName"] = value ?? string.Empty;
            }
        }

        public static string Role
        {
            get
            {
                return Convert.ToString(Session?["Role"]) ?? string.Empty;
            }

            set
            {
                if (Session != null)
                    Session["Role"] = value ?? string.Empty;
            }
        }

        // =============================================================
        // AUTHENTICATION
        // =============================================================
        public static bool IsLoggedIn
        {
            get
            {
                if (Session == null || Session["IsLoggedIn"] == null)
                {
                    return false;
                }

                return Convert.ToBoolean(Session["IsLoggedIn"]);
            }

            set
            {
                if (Session != null)
                    Session["IsLoggedIn"] = value;
            }
        }

        public static bool IsAuthenticated
        {
            get
            {
                return IsLoggedIn && (StaffID > 0 || CustomerID > 0);
            }
        }

        // =============================================================
        // ROLE CHECKING
        // =============================================================
        public static bool HasRole(string roleName)
        {
            return IsAuthenticated && !string.IsNullOrWhiteSpace(roleName) && string.Equals(Role, roleName, StringComparison.OrdinalIgnoreCase);
        }

        // =============================================================
        // INVENTORY PERMISSIONS
        // =============================================================
        public static string DashboardPath
        {
            get
            {
                if (!IsAuthenticated)
                    return "~/Account/Login.aspx";
                switch (Role.Trim().ToLowerInvariant())
                {
                    case "customer": return "~/Secure/Customer/Dashboard.aspx";
                    case "administrator": return "~/Secure/Admin/Dashboard.aspx";
                    case "manager": return "~/Secure/Manager/Dashboard.aspx";
                    case "technician": return "~/Secure/Technician/Dashboard.aspx";
                    case "sysadmin": return "~/Secure/SysAdmin/Dashboard.aspx";
                    default: return "~/Account/AccessDenied.aspx";
                }
            }
        }

        public static bool CanViewInventory
        {
            get
            {
                return HasRole("Manager") || HasRole("Administrator") || HasRole("Technician");
            }
        }

        public static bool CanModifyInventory
        {
            get
            {
                return HasRole("Manager");
            }
        }

        // =============================================================
        // START STAFF SESSION
        // =============================================================
        public static void StartSession(int staffID, string fullName, string role)
        {
            if (Session == null)
                return;
            Session.Clear();
            StaffID = staffID;
            CustomerID = 0;
            CustomerEmail = string.Empty;
            FullName = fullName;
            Role = role;
            IsLoggedIn = true;
        }

        // =============================================================
        // START CUSTOMER SESSION
        // =============================================================
        public static void StartCustomerSession(int customerID, string fullName, string email)
        {
            if (Session == null)
                return;
            Session.Clear();
            StaffID = 0;
            CustomerID = customerID;
            CustomerEmail = email;
            FullName = fullName;
            // Customer is an application role.
            Role = "Customer";
            IsLoggedIn = true;
        }

        // =============================================================
        // CLEAR SESSION
        // =============================================================
        public static void Clear()
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
                return;
            if (context.Session != null)
            {
                try
                {
                    SecurityGateway.EndCurrent();
                }
                catch (Exception ex)
                {
                    MajorProject.Business.Helpers.Logger.Error("Ending authenticated session.", ex);
                }

                context.Session.Clear();
                context.Session.RemoveAll();
                context.Session.Abandon();
            }
        }
    }
}
