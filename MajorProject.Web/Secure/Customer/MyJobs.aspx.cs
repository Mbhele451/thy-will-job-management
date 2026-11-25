using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MajorProject.Web.Secure.Customer
{
    public partial class MyJobs : CustomerBasePage
    {
        private string ConnectionString
        {
            get
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"];
                if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    throw new ConfigurationErrorsException("Connection string 'GroupWst34ConnectionString' was not found.");
                }

                return settings.ConnectionString;
            }
        }

        private JobService JobService
        {
            get
            {
                return new JobService(ConnectionString);
            }
        }

        private string SelectedStatus
        {
            get
            {
                object value = ViewState["SelectedStatus"];
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return "All";
                }

                return value.ToString();
            }

            set
            {
                ViewState["SelectedStatus"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!WebSession.IsAuthenticated || !WebSession.HasRole("Customer") || WebSession.CustomerID <= 0)
            {
                return;
            }

            if (!IsPostBack)
            {
                SelectedStatus = "All";
                LoadJobs();
            }
        }

        // ============================================================
        // LOAD JOBS
        // ============================================================
        private void LoadJobs()
        {
            try
            {
                DataTable jobs = JobService.GetJobsByCustomerID(WebSession.CustomerID);
                if (jobs == null)
                {
                    ShowEmptyState();
                    return;
                }

                DataTable filteredJobs = FilterJobs(jobs, SelectedStatus);
                if (filteredJobs == null || filteredJobs.Rows.Count == 0)
                {
                    ShowEmptyState();
                    return;
                }

                gvJobs.DataSource = filteredJobs;
                gvJobs.DataBind();
                pnlJobs.Visible = true;
                pnlEmpty.Visible = false;
                lblJobCount.Text = filteredJobs.Rows.Count == 1 ? "1 Job" : filteredJobs.Rows.Count + " Jobs";
                UpdateFilterButtons();
            }
            catch (Exception ex)
            {
                ShowError("We could not load your jobs. Please try again.");
                pnlJobs.Visible = false;
                pnlEmpty.Visible = true;
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        // ============================================================
        // FILTER
        // ============================================================
        protected void StatusFilter_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
            {
                return;
            }

            string status = button.CommandArgument;
            if (string.IsNullOrWhiteSpace(status))
            {
                status = "All";
            }

            SelectedStatus = status;
            pnlSelectedJob.Visible = false;
            LoadJobs();
        }

        private DataTable FilterJobs(DataTable source, string status)
        {
            if (source == null)
            {
                return null;
            }

            DataTable result = source.Clone();
            foreach (DataRow row in source.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                string jobStatus = GetSafeString(row, "JobStatus");
                if (status.Equals("All", StringComparison.OrdinalIgnoreCase) || jobStatus.Equals(status, StringComparison.OrdinalIgnoreCase))
                {
                    result.ImportRow(row);
                }
            }

            return result;
        }

        // ============================================================
        // PAGING
        // ============================================================
        protected void gvJobs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvJobs.PageIndex = e.NewPageIndex;
            LoadJobs();
        }

        // ============================================================
        // VIEW JOB
        // ============================================================
        protected void gvJobs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("ViewJob", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            int jobID;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out jobID))
            {
                ShowError("The selected job is invalid.");
                return;
            }

            if (jobID <= 0)
            {
                ShowError("The selected job is invalid.");
                return;
            }

            LoadSelectedJob(jobID);
        }

        private void LoadSelectedJob(int jobID)
        {
            try
            {
                /*
                 * IMPORTANT:
                 *
                 * GetJobsByCustomerID returns only jobs belonging
                 * to the logged-in customer.
                 *
                 * We therefore load the customer's job collection
                 * first and locate the requested job inside it.
                 *
                 * This prevents a customer from requesting another
                 * customer's job by changing a URL or query string.
                 */
                DataTable jobs = JobService.GetJobsByCustomerID(WebSession.CustomerID);
                if (jobs == null || jobs.Rows.Count == 0)
                {
                    ShowError("The selected job could not be found.");
                    return;
                }

                DataRow selectedRow = null;
                foreach (DataRow row in jobs.Rows)
                {
                    if (row.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    int currentJobID = GetIntValue(row, "JobID");
                    if (currentJobID == jobID)
                    {
                        selectedRow = row;
                        break;
                    }
                }

                if (selectedRow == null)
                {
                    ShowError("The selected job could not be found.");
                    return;
                }

                // The list projection omits Notes. Load the saved row only after ownership is established.
                DataTable saved = JobService.GetJobByID(jobID);
                if (saved == null || saved.Rows.Count == 0 || GetIntValue(saved.Rows[0], "CustomerID") != WebSession.CustomerID)
                {
                    ShowError("The selected job could not be found.");
                    return;
                }

                DisplaySelectedJob(selectedRow, saved.Rows[0]);
            }
            catch (Exception ex)
            {
                ShowError("We could not load the selected job.");
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void DisplaySelectedJob(DataRow row, DataRow savedJob)
        {
            lblSelectedJobID.Text = GetIntValue(row, "JobID").ToString();
            lblSelectedService.Text = GetSafeString(row, "JobTypeName");
            lblSelectedStatus.Text = GetSafeString(row, "JobStatus");
            lblSelectedDate.Text = FormatDate(GetValue(row, "PromiseDate"));
            string address = GetSafeString(row, "ServiceAddress");
            if (string.IsNullOrWhiteSpace(address))
            {
                address = GetSafeString(row, "FullAddress");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                address = "Service address not available.";
            }

            lblSelectedAddress.Text = Server.HtmlEncode(address);
            string description = GetSafeString(row, "JobDescription");
            if (string.IsNullOrWhiteSpace(description))
            {
                description = GetSafeString(savedJob, "Notes");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                description = "No additional description was provided.";
            }

            const string requestPrefix = "Customer service request: ";
            if (description.StartsWith(requestPrefix, StringComparison.Ordinal))
                description = description.Substring(requestPrefix.Length);
            lblSelectedDescription.Text = Server.HtmlEncode(description);
            // The caller has already located this booking in the current customer's jobs.
            DataTable services = JobService.GetJobDetailsByJobID(GetIntValue(row, "JobID"));
            gvJobServices.DataSource = services;
            gvJobServices.DataBind();
            if (services != null && services.Rows.Count > 1)
                lblSelectedService.Text = services.Rows.Count.ToString() + " service lines";
            else
                lblSelectedService.Text = Server.HtmlEncode(GetSafeString(row, "JobTypeName"));
            pnlSelectedJob.Visible = true;
            pnlSelectedJob.Focus();
        }

        // ============================================================
        // FILTER BUTTONS
        // ============================================================
        private void UpdateFilterButtons()
        {
            SetFilterButtonState(btnAll, "All");
            SetFilterButtonState(btnPending, "Pending");
            SetFilterButtonState(btnInProgress, "In Progress");
            SetFilterButtonState(btnCompleted, "Completed");
            SetFilterButtonState(btnCancelled, "Cancelled");
        }

        private void SetFilterButtonState(Button button, string status)
        {
            button.CssClass = "dashboard-btn " + "dashboard-btn-secondary";
            if (SelectedStatus.Equals(status, StringComparison.OrdinalIgnoreCase))
            {
                button.CssClass = "dashboard-btn " + "dashboard-btn-primary";
            }
        }

        // ============================================================
        // EMPTY STATE
        // ============================================================
        private void ShowEmptyState()
        {
            gvJobs.DataSource = null;
            gvJobs.DataBind();
            pnlJobs.Visible = false;
            pnlEmpty.Visible = true;
            lblJobCount.Text = "0 Jobs";
            UpdateFilterButtons();
        }

        // ============================================================
        // STATUS DISPLAY
        // ============================================================
        protected string GetStatusClass(object statusValue)
        {
            string status = Convert.ToString(statusValue);
            if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                return "dashboard-status dashboard-status-success";
            }

            if (status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                return "dashboard-status dashboard-status-danger";
            }

            if (status.Equals("In Progress", StringComparison.OrdinalIgnoreCase) || status.Equals("In-Progress", StringComparison.OrdinalIgnoreCase))
            {
                return "dashboard-status dashboard-status-info";
            }

            return "dashboard-status dashboard-status-warning";
        }

        // ============================================================
        // DATA HELPERS
        // ============================================================
        private object GetValue(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName))
            {
                return null;
            }

            if (row[columnName] == DBNull.Value)
            {
                return null;
            }

            return row[columnName];
        }

        private string GetSafeString(DataRow row, string columnName)
        {
            object value = GetValue(row, columnName);
            if (value == null)
            {
                return string.Empty;
            }

            return Convert.ToString(value).Trim();
        }

        private int GetIntValue(DataRow row, string columnName)
        {
            object value = GetValue(row, columnName);
            if (value == null)
            {
                return 0;
            }

            int result;
            if (int.TryParse(Convert.ToString(value), out result))
            {
                return result;
            }

            return 0;
        }

        protected string FormatDate(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "Not available";
            }

            DateTime date;
            if (!DateTime.TryParse(Convert.ToString(value), out date))
            {
                return "Not available";
            }

            return date.ToString("dd MMM yyyy");
        }

        // ============================================================
        // ERROR
        // ============================================================
        private void ShowError(string message)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = Server.HtmlEncode(message);
        }
    }
}
