using MajorProject.Web.Helpers;
using System;
using System.Configuration;

namespace MajorProject.Web.Manager
{
    public partial class Reports : ManagerBasePage
    {
        // ============================================================
        // PAGE LOAD
        // ============================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadReport();
            }
        }

        // ============================================================
        // LOAD POWER BI REPORT
        // ============================================================
        private void LoadReport()
        {
            string reportUrl = ConfigurationManager.AppSettings["PowerBIReportUrl"];
            if (string.IsNullOrWhiteSpace(reportUrl))
            {
                throw new ConfigurationErrorsException("PowerBIReportUrl was not found in Web.config.");
            }

            powerBiReport.Src = reportUrl;
        }
    }
}
