using System;

namespace MajorProject.Web.Helpers
{
    public abstract class CustomerBasePage : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            AccessControl.RequireRole("Customer");
            base.OnInit(e);
        }
    }
}
