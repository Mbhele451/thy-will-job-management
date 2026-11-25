using System;

namespace MajorProject.Web.Helpers
{
    public abstract class AdminBasePage : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            AccessControl.RequireRole("Administrator");
            base.OnInit(e);
        }
    }
}
