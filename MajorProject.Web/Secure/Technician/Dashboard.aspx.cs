using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.IO;

namespace MajorProject.Web.Technician
{
    public partial class Dashboard : TechnicianBasePage
    {
        private JobService _jobService;
        private InventoryService _inventoryService;
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString;
            }
        }

        // =========================================================
        // PAGE LOAD
        // =========================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            InitialiseServices();
            if (!IsPostBack)
            {
                LoadDashboard();
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
        // DASHBOARD
        // =========================================================
        private void LoadDashboard()
        {
            LoadTechnicianGreeting();
            LoadJobStatistics();
            LoadAssignedJobs();
            LoadInventory();
        }

        // =========================================================
        // GREETING
        // =========================================================
        private void LoadTechnicianGreeting()
        {
            string fullName = WebSession.FullName;
            if (string.IsNullOrWhiteSpace(fullName))
            {
                lblAdminGreeting.Text = "Welcome back";
                return;
            }

            lblAdminGreeting.Text = $"Welcome back, {Server.HtmlEncode(fullName)}";
        }

        // =========================================================
        // JOB STATISTICS
        // =========================================================
        private void LoadJobStatistics()
        {
            int staffID = GetLoggedInStaffID();
            DataTable jobs = _jobService.GetJobsByStaffID(staffID);
            int totalJobs = jobs?.Rows.Count ?? 0;
            lblTotalCount.Text = totalJobs.ToString();
            lblPendingCount.Text = _jobService.GetJobCountByStatus(staffID, "Pending").ToString();
            lblInProgressCount.Text = _jobService.GetJobCountByStatus(staffID, "In Progress").ToString();
            lblCompletedCount.Text = _jobService.GetJobCountByStatus(staffID, "Completed").ToString();
        }

        // =========================================================
        // ASSIGNED JOBS
        // =========================================================
        private void LoadAssignedJobs()
        {
            int staffID = GetLoggedInStaffID();
            DataTable jobs = _jobService.GetJobsByStaffID(staffID);
            BindJobs(jobs);
            lblJobFilter.Text = "Total Jobs";
        }

        // =========================================================
        // BIND JOBS
        // =========================================================
        private void BindJobs(DataTable jobs)
        {
            dgvJobs.DataSource = jobs;
            dgvJobs.DataBind();
        }

        // =========================================================
        // GET LOGGED-IN TECHNICIAN
        // =========================================================
        private int GetLoggedInStaffID()
        {
            if (!WebSession.IsLoggedIn)
            {
                throw new InvalidOperationException("No authenticated user session was found.");
            }

            int staffID = WebSession.StaffID;
            if (staffID <= 0)
            {
                throw new InvalidOperationException("The logged-in technician does not have a valid Staff ID.");
            }

            return staffID;
        }

        // =========================================================
        // INVENTORY
        // =========================================================
        private void LoadInventory()
        {
            DataTable inventory = _inventoryService.GetAllInventoryItems();
            dgvInventory.DataSource = inventory;
            dgvInventory.DataBind();
        }

        // =========================================================
        // STATISTIC CARD CLICK
        // =========================================================
        protected void btnStat_Click(object sender, EventArgs e)
        {
            var button = sender as System.Web.UI.WebControls.LinkButton;
            if (button == null)
            {
                return;
            }

            string status = button.CommandArgument;
            if (string.IsNullOrWhiteSpace(status))
            {
                LoadAssignedJobs();
                return;
            }

            FilterJobsByStatus(status);
        }

        // =========================================================
        // FILTER JOBS BY STATUS
        // =========================================================
        private void FilterJobsByStatus(string status)
        {
            int staffID = GetLoggedInStaffID();
            DataTable jobs = _jobService.GetJobsByStaffID(staffID);
            if (jobs == null)
            {
                dgvJobs.DataSource = null;
                dgvJobs.DataBind();
                lblJobFilter.Text = status;
                return;
            }

            DataView view = jobs.DefaultView;
            string safeStatus = status.Replace("'", "''");
            view.RowFilter = $"JobStatus = '{safeStatus}'";
            DataTable filteredJobs = view.ToTable();
            BindJobs(filteredJobs);
            lblJobFilter.Text = Server.HtmlEncode(status);
        }

        // =========================================================
        // TOTAL JOBS CARD
        // =========================================================
        protected void btnTotalStat_Click(object sender, EventArgs e)
        {
            LoadAssignedJobs();
        }
    }
}
