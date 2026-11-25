using MajorProject.Business.Helpers;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;

namespace MajorProject.Web.Admin
{
    public partial class Admin : AdminBasePage
    {
        private JobService _jobService;
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString;
            }
        }

        private JobService JobService
        {
            get
            {
                if (_jobService == null)
                {
                    _jobService = new JobService(ConnectionString);
                }

                return _jobService;
            }
        }

        private InventoryService InventoryService
        {
            get
            {
                return new InventoryService(ConnectionString);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }

        private void LoadDashboard()
        {
            try
            {
                string fullName = WebSession.FullName;
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    fullName = "Admin";
                }

                lblAdminGreeting.Text = Server.HtmlEncode(fullName);
                LoadJobStatistics();
                LoadInventory();
                LoadAllJobs();
            }
            catch (Exception ex)
            {
                Logger.Error("Admin dashboard failed to load.", ex);
                pendingJobsValue.InnerText = "—";
                inProgressJobsValue.InnerText = "—";
                completedJobsValue.InnerText = "—";
                lblTotalCount.Text = "—";
            }
        }

        private void LoadJobStatistics()
        {
            try
            {
                int total = JobService.GetAllJobs()?.Rows.Count ?? 0;
                int pending = JobService.GetTotalJobCountByStatus("Pending");
                int inProgress = JobService.GetTotalJobCountByStatus("In Progress");
                int completed = JobService.GetTotalJobCountByStatus("Completed");
                lblTotalCount.Text = total.ToString();
                pendingJobsValue.InnerText = pending.ToString();
                inProgressJobsValue.InnerText = inProgress.ToString();
                completedJobsValue.InnerText = completed.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load Admin job statistics.", ex);
                lblTotalCount.Text = "—";
                pendingJobsValue.InnerText = "—";
                inProgressJobsValue.InnerText = "—";
                completedJobsValue.InnerText = "—";
            }
        }

        private void LoadInventory()
        {
            try
            {
                dgvInventory.DataSource = InventoryService.GetAllInventoryItems();
                dgvInventory.DataBind();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load Admin dashboard inventory.", ex);
            }
        }

        // =========================================================
        // LOAD ALL JOBS
        // =========================================================
        private void LoadAllJobs()
        {
            try
            {
                DataTable jobs = JobService.GetAllJobs();
                dgvJobs.DataSource = jobs;
                dgvJobs.DataBind();
                lblJobFilter.Text = "All Jobs";
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load Admin dashboard jobs.", ex);
                ShowMessage("Failed to load Admin dashboard jobs.");
            }
        }

        // =========================================================
        // TOTAL JOBS CARD
        // =========================================================
        protected void btnTotalStat_Click(object sender, EventArgs e)
        {
            LoadAllJobs();
        }

        // =========================================================
        // JOB STATUS STAT CARDS
        // =========================================================
        protected void btnJobStatus_Click(object sender, EventArgs e)
        {
            var button = sender as System.Web.UI.WebControls.LinkButton;
            if (button == null)
            {
                return;
            }

            string status = button.CommandArgument;
            if (string.IsNullOrWhiteSpace(status))
            {
                LoadAllJobs();
                return;
            }

            FilterJobsByStatus(status);
        }

        private void FilterJobsByStatus(string status)
        {
            try
            {
                DataTable jobs = JobService.GetJobsByStatus(status);
                dgvJobs.DataSource = jobs;
                dgvJobs.DataBind();
                lblJobFilter.Text = Server.HtmlEncode(status);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to filter Admin dashboard jobs.", ex);
                ShowMessage("Failed to filter jobs.");
            }
        }

        // =========================================================
        // ALL JOBS BUTTON
        // =========================================================
        protected void btnAllJobs_Click(object sender, EventArgs e)
        {
            LoadAllJobs();
        }

        // =========================================================
        // MESSAGE
        // =========================================================
        private void ShowMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            string encodedMessage = Server.HtmlEncode(message);
            ClientScript.RegisterStartupScript(GetType(), "adminMessage", "alert('" + encodedMessage.Replace("'", "\\'") + "');", true);
        }
    }
}
