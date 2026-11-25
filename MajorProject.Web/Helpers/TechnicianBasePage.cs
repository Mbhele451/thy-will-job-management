using System;

namespace MajorProject.Web.Helpers
{
    public abstract class TechnicianBasePage : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            AccessControl.RequireRole("Technician");
            base.OnInit(e);
        }
    }
}
