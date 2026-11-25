using System;

namespace MajorProject.Web.Helpers
{
    public abstract class ManagerBasePage : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            AccessControl.RequireRole("Manager");
            base.OnInit(e);
        }
    }
}
