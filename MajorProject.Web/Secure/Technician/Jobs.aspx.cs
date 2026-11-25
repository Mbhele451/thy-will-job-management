using MajorProject.Business.Helpers;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

namespace MajorProject.Web.Secure.Technician
{
    public partial class Jobs : TechnicianBasePage
    {
        private JobService _jobService;
        private string ConnectionString
        {
            get
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"];
                if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    throw new InvalidOperationException("The GroupWst34ConnectionString connection string is not configured.");
                }

                return settings.ConnectionString;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAreaFilter();
                LoadJobs();
                ClearJobDetails();
            }
        }

        private void LoadJobs()
        {
            try
            {
                int staffID = GetLoggedInStaffID();
                DataTable jobs = JobService.GetTechnicianJobs(staffID);
                DataTable filteredJobs = FilterJobs(jobs);
                BindJobs(filteredJobs);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load technician jobs.", ex);
                ShowJobMessage("Failed to load your jobs. Please try again.", false);
            }
        }

        private void BindJobs(DataTable jobs)
        {
            dgvJobs.DataSource = jobs;
            dgvJobs.DataBind();
        }

        private void LoadAreaFilter()
        {
            try
            {
                int staffID = GetLoggedInStaffID();
                DataTable jobs = JobService.GetTechnicianJobs(staffID);
                ddlAreaFilter.Items.Clear();
                ddlAreaFilter.Items.Add(new ListItem("All Areas", "All"));
                if (jobs == null || jobs.Rows.Count == 0)
                {
                    return;
                }

                foreach (DataRow row in jobs.Rows)
                {
                    string area = GetValue(row, "Area");
                    if (string.IsNullOrWhiteSpace(area))
                    {
                        continue;
                    }

                    if (ddlAreaFilter.Items.FindByValue(area) == null)
                    {
                        ddlAreaFilter.Items.Add(new ListItem(area, area));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load technician job areas.", ex);
                ddlAreaFilter.Items.Clear();
                ddlAreaFilter.Items.Add(new ListItem("All Areas", "All"));
            }
        }

        protected void txtJobSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        protected void ddlAreaFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            try
            {
                int staffID = GetLoggedInStaffID();
                DataTable jobs = JobService.GetTechnicianJobs(staffID);
                DataTable filteredJobs = FilterJobs(jobs);
                BindJobs(filteredJobs);
                ClearJobDetails();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to filter technician jobs.", ex);
                ShowJobMessage("Failed to filter your jobs. Please try again.", false);
            }
        }

        private DataTable FilterJobs(DataTable jobs)
        {
            if (jobs == null)
            {
                return null;
            }

            DataTable filteredJobs = jobs.Clone();
            string search = (txtJobSearch.Text ?? string.Empty).Trim();
            string selectedArea = ddlAreaFilter.SelectedValue;
            string selectedStatus = ddlStatusFilter.SelectedValue;
            foreach (DataRow row in jobs.Rows)
            {
                bool matchesSearch = true;
                bool matchesArea = true;
                bool matchesStatus = true;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    string jobID = GetValue(row, "JobID");
                    string customerName = GetValue(row, "CustomerName");
                    bool jobIDMatches = jobID.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                    bool customerMatches = customerName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                    matchesSearch = jobIDMatches || customerMatches;
                }

                if (!string.IsNullOrWhiteSpace(selectedArea) && selectedArea != "All")
                {
                    string area = GetValue(row, "Area");
                    matchesArea = string.Equals(area, selectedArea, StringComparison.OrdinalIgnoreCase);
                }

                if (!string.IsNullOrWhiteSpace(selectedStatus) && selectedStatus != "All")
                {
                    string status = GetValue(row, "JobStatus");
                    matchesStatus = string.Equals(status, selectedStatus, StringComparison.OrdinalIgnoreCase);
                }

                if (matchesSearch && matchesArea && matchesStatus)
                {
                    filteredJobs.ImportRow(row);
                }
            }

            return filteredJobs;
        }

        protected void dgvJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvJobs.SelectedIndex < 0)
            {
                return;
            }

            try
            {
                DataKey key = dgvJobs.DataKeys[dgvJobs.SelectedIndex];
                if (key == null)
                {
                    return;
                }

                int jobID;
                if (!int.TryParse(Convert.ToString(key.Value), out jobID) || jobID <= 0)
                {
                    ShowJobMessage("The selected job has an invalid Job ID.", false);
                    return;
                }

                LoadSelectedJob(jobID);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load selected technician job.", ex);
                ShowJobMessage("Failed to load the selected job.", false);
            }
        }

        private void LoadSelectedJob(int jobID)
        {
            int staffID = GetLoggedInStaffID();
            DataTable job = JobService.GetTechnicianJobByID(jobID, staffID);
            if (job == null || job.Rows.Count == 0)
            {
                ClearJobDetails();
                ShowJobMessage("The selected job could not be found or is not assigned to you.", false);
                return;
            }

            DataRow row = job.Rows[0];
            string customerName = GetValue(row, "CustomerFullName");
            string phoneNumber = GetValue(row, "PhoneNumber");
            string fullAddress = GetValue(row, "FullAddress");
            string area = GetValue(row, "Area");
            lblCustomerFullName.Text = Server.HtmlEncode(customerName);
            lblCustomerArea.Text = Server.HtmlEncode(area);
            ConfigureCustomerPhone(phoneNumber);
            ConfigureCustomerAddress(fullAddress);
            ConfigureCustomerActions(phoneNumber, fullAddress);
            ConfigureCustomerMap(fullAddress);
            lblJobID.Text = Server.HtmlEncode(GetValue(row, "JobID"));
            string status = GetValue(row, "JobStatus");
            lblJobStatus.Text = Server.HtmlEncode(status);
            bool onHold = Convert.ToBoolean(row["HasPendingChange"]);
            lblSelectedJobStatus.Text = Server.HtmlEncode(onHold ? status + " · Work on hold" : status);
            lblJobCreatedAt.Text = Server.HtmlEncode(FormatDateTime(row, "CreatedAt"));
            lblJobPromiseDate.Text = Server.HtmlEncode(FormatDateTime(row, "PromiseDate"));
            string notes = GetValue(row, "Notes");
            if (string.IsNullOrWhiteSpace(notes))
            {
                notes = "No notes provided.";
            }

            lblJobNotes.Text = Server.HtmlEncode(notes);
            ConfigureActionButtons(status, row);
            pnlJobDetails.Visible = true;
            pnlNoJobSelected.Visible = false;
            HideJobMessage();
        }

        private void ConfigureActionButtons(string status, DataRow row)
        {
            btnStartJob.Enabled = false;
            btnFinishJob.Enabled = false;
            btnCancelJob.Visible = Convert.ToBoolean(row["MaterialReportRequired"]);
            btnCancelJob.Enabled = btnCancelJob.Visible;
            if (Convert.ToBoolean(row["HasPendingChange"]))
                return;
            if (string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase))
            {
                DateTime promiseDate;
                bool validPromiseDate = TryGetDate(row, "PromiseDate", out promiseDate);
                bool dateIsValid = validPromiseDate && promiseDate.Date <= DateTime.Today;
                btnStartJob.Enabled = dateIsValid;
            }
            else if (string.Equals(status, "In Progress", StringComparison.OrdinalIgnoreCase))
            {
                btnFinishJob.Enabled = true;
            }
        }

        protected void btnStartJob_Click(object sender, EventArgs e)
        {
            try
            {
                int jobID = GetSelectedJobID();
                int staffID = GetLoggedInStaffID();
                JobService.StartTechnicianJob(jobID, staffID);
                LoadJobs();
                LoadSelectedJob(jobID);
                ShowJobMessage("Job started successfully.", true);
            }
            catch (ArgumentException ex)
            {
                Logger.Error("Technician start job validation failed.", ex);
                ShowJobMessage(ex.Message, false);
            }
            catch (InvalidOperationException ex)
            {
                Logger.Error("Technician start job was rejected.", ex);
                ShowJobMessage(ex.Message, false);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error while starting technician job.", ex);
                ShowJobMessage("The job could not be started. Please try again.", false);
            }
        }

        protected void btnFinishJob_Click(object sender, EventArgs e)
        {
            try
            {
                int jobID = GetSelectedJobID();
                int staffID = GetLoggedInStaffID();
                JobService.CompleteTechnicianJob(jobID, staffID);
                LoadJobs();
                LoadSelectedJob(jobID);
                ShowJobMessage("Job marked as completed successfully.", true);
            }
            catch (ArgumentException ex)
            {
                Logger.Error("Technician finish job validation failed.", ex);
                ShowJobMessage(ex.Message, false);
            }
            catch (InvalidOperationException ex)
            {
                Logger.Error("Technician finish job was rejected.", ex);
                ShowJobMessage(ex.Message, false);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error while completing technician job.", ex);
                ShowJobMessage("The job could not be completed. Please try again.", false);
            }
        }

        protected void btnCancelJob_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Secure/JobChanges.aspx?view=materials"), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private int GetSelectedJobID()
        {
            if (dgvJobs.SelectedIndex < 0)
            {
                throw new InvalidOperationException("Please select a job first.");
            }

            DataKey key = dgvJobs.DataKeys[dgvJobs.SelectedIndex];
            if (key == null)
            {
                throw new InvalidOperationException("The selected job could not be identified.");
            }

            int jobID;
            if (!int.TryParse(Convert.ToString(key.Value), out jobID) || jobID <= 0)
            {
                throw new InvalidOperationException("The selected job has an invalid Job ID.");
            }

            return jobID;
        }

        private int GetLoggedInStaffID()
        {
            if (!WebSession.IsAuthenticated)
            {
                throw new InvalidOperationException("No authenticated technician session was found.");
            }

            if (!WebSession.HasRole("Technician"))
            {
                throw new InvalidOperationException("You are not authorized to access technician jobs.");
            }

            int staffID = WebSession.StaffID;
            if (staffID <= 0)
            {
                throw new InvalidOperationException("The logged-in technician does not have a valid Staff ID.");
            }

            return staffID;
        }

        private void ConfigureCustomerMap(string fullAddress)
        {
            if (string.IsNullOrWhiteSpace(fullAddress))
            {
                btnCustomerMap.Enabled = false;
                btnCustomerMap.NavigateUrl = string.Empty;
                return;
            }

            string encodedAddress = HttpUtility.UrlEncode(fullAddress);
            btnCustomerMap.NavigateUrl = "https://www.google.com/maps/search/?api=1&query=" + encodedAddress;
            btnCustomerMap.Enabled = true;
        }

        private void ClearJobDetails()
        {
            pnlJobDetails.Visible = false;
            pnlNoJobSelected.Visible = true;
            lblCustomerFullName.Text = string.Empty;
            btnCustomerPhone.Text = string.Empty;
            btnCustomerPhone.NavigateUrl = string.Empty;
            btnCustomerPhone.Enabled = false;
            btnCustomerAddress.Text = string.Empty;
            btnCustomerAddress.NavigateUrl = string.Empty;
            btnCustomerAddress.Enabled = false;
            lblCustomerArea.Text = string.Empty;
            btnCallCustomer.Text = "<i class=\"bi bi-telephone-fill\"></i>" + "<span>Call Customer</span>";
            btnCallCustomer.NavigateUrl = string.Empty;
            btnCallCustomer.Enabled = false;
            btnNavigateCustomer.Text = "<i class=\"bi bi-geo-alt-fill\"></i>" + "<span>Navigate</span>";
            btnNavigateCustomer.NavigateUrl = string.Empty;
            btnNavigateCustomer.Enabled = false;
            lblJobID.Text = string.Empty;
            lblJobStatus.Text = string.Empty;
            lblSelectedJobStatus.Text = string.Empty;
            lblJobCreatedAt.Text = string.Empty;
            lblJobPromiseDate.Text = string.Empty;
            lblJobNotes.Text = string.Empty;
            btnCustomerMap.Enabled = false;
            btnCustomerMap.NavigateUrl = string.Empty;
            btnStartJob.Enabled = false;
            btnFinishJob.Enabled = false;
            btnCancelJob.Enabled = false;
        }

        private void ShowJobMessage(string message, bool success)
        {
            pnlJobMessage.Visible = true;
            lblJobMessage.Text = Server.HtmlEncode(message);
            if (success)
            {
                pnlJobMessage.CssClass = "dashboard-job-message dashboard-job-message-success";
            }
            else
            {
                pnlJobMessage.CssClass = "dashboard-job-message dashboard-job-message-error";
            }
        }

        private void HideJobMessage()
        {
            pnlJobMessage.Visible = false;
            lblJobMessage.Text = string.Empty;
        }

        private string GetValue(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToString(row[column]) ?? string.Empty;
        }

        private string FormatDateTime(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return string.Empty;
            }

            DateTime value;
            if (DateTime.TryParse(row[column].ToString(), out value))
            {
                return value.ToString("dd MMM yyyy HH:mm");
            }

            return row[column].ToString();
        }

        private bool TryGetDate(DataRow row, string column, out DateTime value)
        {
            value = DateTime.MinValue;
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return false;
            }

            return DateTime.TryParse(row[column].ToString(), out value);
        }

        private void ConfigureCustomerPhone(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                btnCustomerPhone.Text = "No phone number available";
                btnCustomerPhone.NavigateUrl = string.Empty;
                btnCustomerPhone.Enabled = false;
                return;
            }

            string cleanPhone = phoneNumber.Trim();
            btnCustomerPhone.Text = Server.HtmlEncode(cleanPhone);
            btnCustomerPhone.NavigateUrl = "tel:" + HttpUtility.UrlEncode(cleanPhone);
            btnCustomerPhone.Enabled = true;
        }

        private void ConfigureCustomerAddress(string fullAddress)
        {
            if (string.IsNullOrWhiteSpace(fullAddress))
            {
                btnCustomerAddress.Text = "No address available";
                btnCustomerAddress.NavigateUrl = string.Empty;
                btnCustomerAddress.Enabled = false;
                return;
            }

            btnCustomerAddress.Text = Server.HtmlEncode(fullAddress);
            string encodedAddress = HttpUtility.UrlEncode(fullAddress);
            btnCustomerAddress.NavigateUrl = "https://www.google.com/maps/search/?api=1&query=" + encodedAddress;
            btnCustomerAddress.Enabled = true;
        }

        private void ConfigureCustomerActions(string phoneNumber, string fullAddress)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                btnCallCustomer.Text = "<i class=\"bi bi-telephone-fill\"></i>" + "<span>Call Customer</span>";
                btnCallCustomer.NavigateUrl = string.Empty;
                btnCallCustomer.Enabled = false;
            }
            else
            {
                string cleanPhone = phoneNumber.Trim();
                btnCallCustomer.Text = "<i class=\"bi bi-telephone-fill\"></i>" + "<span>Call Customer</span>";
                btnCallCustomer.NavigateUrl = "tel:" + HttpUtility.UrlEncode(cleanPhone);
                btnCallCustomer.Enabled = true;
            }

            if (string.IsNullOrWhiteSpace(fullAddress))
            {
                btnNavigateCustomer.Text = "<i class=\"bi bi-geo-alt-fill\"></i>" + "<span>Navigate</span>";
                btnNavigateCustomer.NavigateUrl = string.Empty;
                btnNavigateCustomer.Enabled = false;
            }
            else
            {
                string encodedAddress = HttpUtility.UrlEncode(fullAddress);
                btnNavigateCustomer.Text = "<i class=\"bi bi-geo-alt-fill\"></i>" + "<span>Navigate</span>";
                btnNavigateCustomer.NavigateUrl = "https://www.google.com/maps/dir/?api=1&destination=" + encodedAddress;
                btnNavigateCustomer.Enabled = true;
            }
        }
    }
}
