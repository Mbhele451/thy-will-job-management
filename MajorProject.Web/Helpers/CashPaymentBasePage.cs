using System;

namespace MajorProject.Web.Helpers
{
    public abstract class CashPaymentBasePage : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            AccessControl.RequireAnyRole("Manager", "Administrator");
            base.OnInit(e);
        }
    }
}
