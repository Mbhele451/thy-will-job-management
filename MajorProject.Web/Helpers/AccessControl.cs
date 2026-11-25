using System;
using System.Web;

namespace MajorProject.Web.Helpers
{
    public static class AccessControl
    {
        public static void RedirectToWorkspace(string page)
        {
            RequireLogin();
            string workspace;
            switch (WebSession.Role.Trim().ToLowerInvariant())
            {
                case "administrator":
                    workspace = "Admin";
                    break;
                case "manager":
                    workspace = "Manager";
                    break;
                case "technician":
                    workspace = "Technician";
                    break;
                case "customer":
                    workspace = "Customer";
                    break;
                case "sysadmin":
                    Redirect("~/Secure/SysAdmin/Dashboard.aspx");
                    return;
                default:
                    Redirect("~/Account/AccessDenied.aspx");
                    return;
            }

            if (page != "Dashboard.aspx" && page != "Profile.aspx")
                throw new ArgumentException("Unknown workspace page.", nameof(page));
            Redirect("~/Secure/" + workspace + "/" + page);
        }

        public static void RequireLogin()
        {
            if (!WebSession.IsLoggedIn)
            {
                Redirect("~/Account/Login.aspx");
            }
        }

        public static void RequireRole(string role)
        {
            RequireLogin();
            if (!WebSession.HasRole(role))
            {
                Redirect("~/Account/AccessDenied.aspx");
            }
        }

        public static void RequireAnyRole(params string[] roles)
        {
            RequireLogin();
            if (roles == null || roles.Length == 0)
            {
                Redirect("~/Account/AccessDenied.aspx");
                return;
            }

            foreach (string role in roles)
            {
                if (WebSession.HasRole(role))
                {
                    return;
                }
            }

            Redirect("~/Account/AccessDenied.aspx");
        }

        public static bool HasRole(string role)
        {
            return WebSession.HasRole(role);
        }

        public static void RequireInventoryView()
        {
            RequireAnyRole("Manager", "Administrator", "Technician");
        }

        public static void RequireInventoryModify()
        {
            RequireRole("Manager");
        }

        private static void Redirect(string url)
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
                return;
            // CompleteRequest does not stop page handlers. End a denied request immediately.
            context.Response.Redirect(url, true);
        }
    }
}
