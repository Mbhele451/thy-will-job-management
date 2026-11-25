using System;
using System.Web.UI;
using MajorProject.Business.Helpers;

namespace MajorProject.Web.Helpers
{
    /// <summary>
    /// Common base page for authenticated Web Forms pages.
    ///
    /// Provides:
    /// - ViewStateUserKey CSRF mitigation
    /// - Response cache prevention
    /// - Centralised unexpected-error handling
    /// </summary>
    public abstract class BasePage : Page
    {
        protected override void OnInit(EventArgs e)
        {
            // Bind ViewState to the current user's session.
            // This must happen before ViewState is loaded.
            if (Session != null && !string.IsNullOrEmpty(Session.SessionID))
            {
                ViewStateUserKey = Session.SessionID + "|" + WebSession.StaffID.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":" + WebSession.CustomerID.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            // Prevent authenticated/private pages from being cached.
            SecurityHelper.PreventCaching();
            base.OnInit(e);
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            PrepareDataTables(this);
            base.OnPreRenderComplete(e);
        }

        private static void PrepareDataTables(System.Web.UI.Control root)
        {
            foreach (System.Web.UI.Control control in root.Controls)
            {
                var grid = control as System.Web.UI.WebControls.GridView;
                if (grid != null)
                {
                    grid.UseAccessibleHeader = true;
                    if (grid.HeaderRow != null)
                    {
                        grid.HeaderRow.TableSection = System.Web.UI.WebControls.TableRowSection.TableHeader;
                        foreach (System.Web.UI.WebControls.TableCell cell in grid.HeaderRow.Cells)
                            cell.Attributes["scope"] = "col";
                    }
                }

                var button = control as System.Web.UI.WebControls.IButtonControl;
                var webControl = control as System.Web.UI.WebControls.WebControl;
                if (button != null && webControl != null && (button.CommandName.Equals("Select", StringComparison.OrdinalIgnoreCase) || button.CommandName.StartsWith("View", StringComparison.OrdinalIgnoreCase)))
                    webControl.Attributes["data-record-select"] = "details";
                if (control.HasControls())
                    PrepareDataTables(control);
            }
        }

        /// <summary>
        /// Logs the real exception server-side and returns a safe
        /// generic message to the browser.
        /// </summary>
        protected string HandleUnexpectedError(string context, Exception ex)
        {
            Logger.Error(context, ex);
            return "Something went wrong on our end. Please try again in a moment.";
        }
    }
}
