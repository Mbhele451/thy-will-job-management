using MajorProject.Business.Helpers;
using MajorProject.Web.Helpers;
using System;
using System.Web;

namespace MajorProject.Web.Account
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PerformLogout();
        }

        private void PerformLogout()
        {
            try
            {
                /*
                 * 1. Destroy the application session.
                 *
                 * This removes StaffID, FullName, Role and all other
                 * session data and abandons the current session.
                 */
                WebSession.Clear();
                /*
                 * 2. Destroy the Forms Authentication cookie.
                 *
                 * This prevents the browser from remaining
                 * authenticated after logout.
                 */
                System.Web.Security.FormsAuthentication.SignOut();
                /*
                 * 3. Prevent the browser and intermediate proxies
                 * from caching authenticated pages.
                 *
                 * This helps prevent a logged-out user from seeing
                 * an old dashboard through the browser Back button.
                 */
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
                Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
                /*
                 * 4. Redirect to the login page.
                 */
                Response.Redirect(ResolveUrl("~/Account/Login.aspx"), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                /*
                 * Never expose internal errors to the user.
                 * Logging is server-side only.
                 */
                Logger.Error("Unexpected error during logout.", ex);
                /*
                 * Even if something goes wrong, attempt to ensure
                 * the authentication cookie is removed.
                 */
                try
                {
                    System.Web.Security.FormsAuthentication.SignOut();
                    WebSession.Clear();
                }
                catch
                {
                // Do not allow cleanup failure to expose an error.
                }

                Response.Redirect(ResolveUrl("~/Account/Login.aspx"), false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }
}
