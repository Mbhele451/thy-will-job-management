using System;

namespace MajorProject.Web.Helpers
{
    /// <summary>
    /// Base class for pages that require an authenticated user.
    /// </summary>
    public abstract class AuthenticatedBasePage : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            AccessControl.RequireLogin();
            base.OnInit(e);
        }
    }
}
