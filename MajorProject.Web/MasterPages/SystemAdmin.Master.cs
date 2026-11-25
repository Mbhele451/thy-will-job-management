using System;
using System.Web.UI;
using MajorProject.Web.Helpers;

namespace MajorProject.Web.MasterPages
{
    public class SystemAdmin : MasterPage
    {
        protected string CompanyName
        {
            get
            {
                return SecurityGateway.Setting("CompanyDisplayName", "THY WILL");
            }
        }

        protected string SupportEmail
        {
            get
            {
                return SecurityGateway.Setting("SupportEmail", "");
            }
        }

        protected override void OnInit(EventArgs e)
        {
            AccessControl.RequireRole("SysAdmin");
            base.OnInit(e);
        }
    }
}
