using System;
using System.Configuration;
using System.Data;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;

namespace MajorProject.Web.Manager
{
    public partial class Manager : ManagerBasePage
    {
        private JobService _jobService;
        private InventoryService _inventoryService;
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"]?.ConnectionString;
            }
        }

        // =========================================================
        // PAGE LOAD
        // =========================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!WebSession.IsLoggedIn)
                return;
            if (!WebSession.HasRole("Manager"))
                return;
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                ShowMessage("The database connection is not configured. Please contact the system administrator.");
                return;
            }

            InitialiseServices();
            if (!IsPostBack)
            {
                LoadDashboardStatistics();
                LoadAllJobs();
                LoadInventory();
            }
        }

        // =========================================================
        // SERVICE INITIALISATION
        // =========================================================
        private void InitialiseServices()
        {
            if (_jobService == null)
            {
                _jobService = new JobService(ConnectionString);
            }

            if (_inventoryService == null)
            {
                _inventoryService = new InventoryService(ConnectionString);
            }
        }

        // =========================================================
        // DASHBOARD STATISTICS
        // =========================================================
        private void LoadDashboardStatistics()
        {
            try
            {
                int total = _jobService.GetAllJobs()?.Rows.Count ?? 0;
                int pendingJobs = _jobService.GetTotalJobCountByStatus("Pending");
                int inProgressJobs = _jobService.GetTotalJobCountByStatus("In Progress");
                int completedJobs = _jobService.GetTotalJobCountByStatus("Completed");
                lblPendingJobs.Text = pendingJobs.ToString();
                lblInProgressJobs.Text = inProgressJobs.ToString();
                lblCompletedJobs.Text = completedJobs.ToString();
                lblTotalJobs.Text = total.ToString();
            }
            catch (Exception ex)
            {
                string message = HandleUnexpectedError("Failed to load Manager dashboard statistics.", ex);
                ShowMessage(message);
            }
        }

        // =========================================================
        // LOAD ALL JOBS
        // =========================================================
        private void LoadAllJobs()
        {
            try
            {
                DataTable jobs = _jobService.GetAllJobs();
                dgvJobs.DataSource = jobs;
                dgvJobs.DataBind();
                lblJobFilter.Text = "All Jobs";
            }
            catch (Exception ex)
            {
                string message = HandleUnexpectedError("Failed to load Manager dashboard jobs.", ex);
                ShowMessage(message);
            }
        }

        // =========================================================
        // JOB STATUS CARD CLICK
        // =========================================================
        protected void btnJobStatus_Click(object sender, EventArgs e)
        {
            var button = sender as System.Web.UI.WebControls.LinkButton;
            if (button == null)
                return;
            string status = button.CommandArgument;
            if (string.IsNullOrWhiteSpace(status))
            {
                LoadAllJobs();
                return;
            }

            FilterJobsByStatus(status);
        }

        // =========================================================
        // FILTER ALL JOBS BY STATUS
        // =========================================================
        private void FilterJobsByStatus(string status)
        {
            try
            {
                DataTable jobs = _jobService.GetJobsByStatus(status);
                dgvJobs.DataSource = jobs;
                dgvJobs.DataBind();
                lblJobFilter.Text = Server.HtmlEncode(status);
            }
            catch (Exception ex)
            {
                string message = HandleUnexpectedError("Failed to filter Manager dashboard jobs.", ex);
                ShowMessage(message);
            }
        }

        protected void btnAllJobs_Click(object sender, EventArgs e)
        {
            LoadAllJobs();
        }

        // =========================================================
        // INVENTORY
        // =========================================================
        private void LoadInventory()
        {
            try
            {
                dgvInventory.DataSource = _inventoryService.GetAllInventoryItems();
                dgvInventory.DataBind();
            }
            catch (Exception ex)
            {
                string message = HandleUnexpectedError("Failed to load Manager dashboard inventory.", ex);
                ShowMessage(message);
            }
        }

        // =========================================================
        // MESSAGE
        // =========================================================
        private void ShowMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            string encodedMessage = Server.HtmlEncode(message);
            ClientScript.RegisterStartupScript(GetType(), "managerMessage", "alert('" + encodedMessage.Replace("'", "\\'") + "');", true);
        }
    }
}
