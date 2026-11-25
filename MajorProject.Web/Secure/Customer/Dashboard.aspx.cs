using MajorProject.Business.Helpers;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;

namespace MajorProject.Web.Secure.Customer
{
    public partial class Dashboard : CustomerBasePage
    {
        // ============================================================
        // CONNECTION STRING
        // ============================================================
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

        // ============================================================
        // SERVICES
        // ============================================================
        private CustomerService CustomerService
        {
            get
            {
                return new CustomerService(ConnectionString);
            }
        }

        private JobService JobService
        {
            get
            {
                return new JobService(ConnectionString);
            }
        }

        private InvoiceService InvoiceService
        {
            get
            {
                return new InvoiceService(ConnectionString);
            }
        }

        // ============================================================
        // PAGE LOAD
        // ============================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!WebSession.IsAuthenticated || !WebSession.HasRole("Customer") || WebSession.CustomerID <= 0)
            {
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }

        // ============================================================
        // LOAD DASHBOARD
        // ============================================================
        private void LoadDashboard()
        {
            try
            {
                pnlError.Visible = false;
                DataTable jobs = JobService.GetJobsByCustomerID(WebSession.CustomerID);
                DataTable invoices = InvoiceService.GetInvoicesByCustomerID(WebSession.CustomerID);
                LoadCustomerInformation();
                var changeInvoices = new JobChangeService(ConnectionString).CustomerFees(WebSession.CustomerID);
                LoadStatistics(jobs, invoices.Rows.Count + changeInvoices.Rows.Count);
                LoadNextService(jobs);
                LoadRecentJobs(jobs);
                LoadAccountActivity(jobs, invoices);
            }
            catch (ConfigurationErrorsException)
            {
                ShowError("The application database configuration is unavailable.");
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to load customer dashboard.", ex);
                ShowError("Unable to load your dashboard right now. Please try again.");
            }
        }

        // ============================================================
        // CUSTOMER INFORMATION
        // ============================================================
        private void LoadCustomerInformation()
        {
            DataTable customer = CustomerService.GetCustomerByID(WebSession.CustomerID);
            if (customer == null || customer.Rows.Count == 0)
            {
                lblCustomerName.Text = "Customer";
                return;
            }

            DataRow row = customer.Rows[0];
            string fullName = GetSafeString(row, "FullName", "Customer");
            lblCustomerName.Text = Server.HtmlEncode(fullName);
        }

        // ============================================================
        // STATISTICS
        // ============================================================
        private void LoadStatistics(DataTable jobs, int invoiceCount)
        {
            int activeJobs = 0;
            int completedJobs = 0;
            if (jobs != null)
            {
                foreach (DataRow row in jobs.Rows)
                {
                    string status = GetSafeString(row, "JobStatus", "");
                    if (IsActiveStatus(status))
                    {
                        activeJobs++;
                    }

                    if (IsCompletedStatus(status))
                    {
                        completedJobs++;
                    }
                }
            }

            lblActiveJobsCount.Text = activeJobs.ToString();
            lblCompletedJobsCount.Text = completedJobs.ToString();
            lblInvoiceCount.Text = invoiceCount.ToString();
        }

        // ============================================================
        // NEXT SERVICE
        // ============================================================
        private void LoadNextService(DataTable jobs)
        {
            DataRow nextJob = FindNextService(jobs);
            if (nextJob == null)
            {
                pnlNextService.Visible = false;
                pnlNoNextService.Visible = true;
                return;
            }

            pnlNextService.Visible = true;
            pnlNoNextService.Visible = false;
            lblNextServiceJobID.Text = GetSafeString(nextJob, "JobID", "0");
            lblNextServiceType.Text = Server.HtmlEncode(GetSafeString(nextJob, "JobTypeName", "Service"));
            lblNextServiceArea.Text = Server.HtmlEncode(GetSafeString(nextJob, "Area", "Not available"));
            lblNextServiceStatus.Text = Server.HtmlEncode(GetSafeString(nextJob, "JobStatus", "Pending"));
            lblNextServiceDate.Text = FormatDate(nextJob, "PromiseDate");
            lnkNextService.NavigateUrl = "~/Secure/Customer/MyJobs.aspx";
        }

        private DataRow FindNextService(DataTable jobs)
        {
            if (jobs == null || jobs.Rows.Count == 0)
            {
                return null;
            }

            DataRow nextJob = null;
            DateTime selectedDate = DateTime.MaxValue;
            bool selectedHasDate = false;
            foreach (DataRow row in jobs.Rows)
            {
                string status = GetSafeString(row, "JobStatus", "");
                if (!IsActiveStatus(status))
                {
                    continue;
                }

                DateTime currentDate = GetDateValue(row, "PromiseDate");
                bool currentHasDate = currentDate != DateTime.MinValue;
                // An overdue active job is still shown in My Jobs, but is not
                // the customer's next scheduled service.
                if (currentHasDate && currentDate.Date < DateTime.Today)
                    continue;
                if (nextJob == null)
                {
                    nextJob = row;
                    selectedHasDate = currentHasDate;
                    if (currentHasDate)
                    {
                        selectedDate = currentDate;
                    }

                    continue;
                }

                if (currentHasDate && !selectedHasDate)
                {
                    nextJob = row;
                    selectedDate = currentDate;
                    selectedHasDate = true;
                    continue;
                }

                if (!currentHasDate)
                {
                    continue;
                }

                if (currentDate < selectedDate)
                {
                    nextJob = row;
                    selectedDate = currentDate;
                    selectedHasDate = true;
                    continue;
                }

                if (currentDate == selectedDate)
                {
                    int currentJobID = GetIntValue(row, "JobID");
                    int selectedJobID = GetIntValue(nextJob, "JobID");
                    if (currentJobID < selectedJobID)
                    {
                        nextJob = row;
                    }
                }
            }

            return nextJob;
        }

        // ============================================================
        // RECENT JOBS
        // ============================================================
        private void LoadRecentJobs(DataTable jobs)
        {
            DataTable recentJobs = CreateRecentJobsTable();
            if (jobs != null)
            {
                int numberOfRows = Math.Min(jobs.Rows.Count, 5);
                for (int i = 0; i < numberOfRows; i++)
                {
                    DataRow source = jobs.Rows[i];
                    DataRow target = recentJobs.NewRow();
                    target["JobID"] = GetIntValue(source, "JobID");
                    target["JobTypeName"] = GetSafeString(source, "JobTypeName", "Service");
                    target["JobStatus"] = GetSafeString(source, "JobStatus", "Unknown");
                    DateTime promiseDate = GetDateValue(source, "PromiseDate");
                    if (promiseDate == DateTime.MinValue)
                    {
                        target["PromiseDate"] = DBNull.Value;
                    }
                    else
                    {
                        target["PromiseDate"] = promiseDate;
                    }

                    recentJobs.Rows.Add(target);
                }
            }

            gvRecentJobs.DataSource = recentJobs;
            gvRecentJobs.DataBind();
        }

        private DataTable CreateRecentJobsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("JobID", typeof(int));
            table.Columns.Add("JobTypeName", typeof(string));
            table.Columns.Add("JobStatus", typeof(string));
            table.Columns.Add("PromiseDate", typeof(DateTime));
            return table;
        }

        // ============================================================
        // ACCOUNT ACTIVITY
        // ============================================================
        private void LoadAccountActivity(DataTable jobs, DataTable invoices)
        {
            List<ActivityItem> activities = new List<ActivityItem>();
            AddJobActivities(activities, jobs);
            AddInvoiceActivities(activities, invoices);
            activities.Sort(delegate (ActivityItem first, ActivityItem second)
            {
                return second.Date.CompareTo(first.Date);
            });
            if (activities.Count == 0)
            {
                pnlNoActivity.Visible = true;
                rptActivity.DataSource = null;
                rptActivity.DataBind();
                return;
            }

            pnlNoActivity.Visible = false;
            if (activities.Count > 6)
            {
                activities = activities.GetRange(0, 6);
            }

            rptActivity.DataSource = activities;
            rptActivity.DataBind();
        }

        private void AddJobActivities(List<ActivityItem> activities, DataTable jobs)
        {
            if (jobs == null)
            {
                return;
            }

            foreach (DataRow row in jobs.Rows)
            {
                string status = GetSafeString(row, "JobStatus", "Unknown");
                DateTime activityDate = GetDateValue(row, "UpdatedAt");
                if (activityDate == DateTime.MinValue)
                {
                    activityDate = GetDateValue(row, "CreatedAt");
                }

                string jobID = GetSafeString(row, "JobID", "");
                string service = GetSafeString(row, "JobTypeName", "Service");
                ActivityItem item = new ActivityItem();
                item.Date = activityDate;
                item.DateText = FormatActivityDate(activityDate);
                if (IsCompletedStatus(status))
                {
                    item.Title = "Job completed";
                    item.Description = "Job #" + jobID + " — " + service;
                    item.IconClass = "bi bi-check-circle";
                }
                else if (IsInProgressStatus(status))
                {
                    item.Title = "Job in progress";
                    item.Description = "Job #" + jobID + " — " + service;
                    item.IconClass = "bi bi-tools";
                }
                else if (IsPendingStatus(status))
                {
                    item.Title = "Job received";
                    item.Description = "Job #" + jobID + " — " + service;
                    item.IconClass = "bi bi-clock";
                }
                else if (IsCancelledStatus(status))
                {
                    item.Title = "Job cancelled";
                    item.Description = "Job #" + jobID + " — " + service;
                    item.IconClass = "bi bi-x-circle";
                }
                else
                {
                    item.Title = "Job updated";
                    item.Description = "Job #" + jobID + " — " + service;
                    item.IconClass = "bi bi-info-circle";
                }

                activities.Add(item);
            }
        }

        private void AddInvoiceActivities(List<ActivityItem> activities, DataTable invoices)
        {
            if (invoices == null)
            {
                return;
            }

            foreach (DataRow row in invoices.Rows)
            {
                string jobID = GetSafeString(row, "JobID", "");
                decimal amount = GetDecimalValue(row, "GrandTotal");
                DateTime createdDate = GetDateValue(row, "CreatedAt");
                DateTime sentDate = GetDateValue(row, "InvoiceSentDate");
                ActivityItem item = new ActivityItem();
                if (sentDate != DateTime.MinValue)
                {
                    item.Date = sentDate;
                    item.Title = "Invoice sent";
                    item.Description = "Invoice for Job #" + jobID + " — " + FormatCurrency(amount);
                    item.IconClass = "bi bi-send";
                }
                else
                {
                    item.Date = createdDate;
                    item.Title = "Invoice generated";
                    item.Description = "Invoice for Job #" + jobID + " — " + FormatCurrency(amount);
                    item.IconClass = "bi bi-receipt";
                }

                item.DateText = FormatActivityDate(item.Date);
                activities.Add(item);
            }
        }

        // ============================================================
        // RECENT JOB BUTTON
        // ============================================================
        protected void gvRecentJobs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "ViewJob", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            int jobID;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out jobID))
            {
                ShowError("The selected job could not be opened.");
                return;
            }

            if (jobID <= 0)
            {
                ShowError("The selected job is invalid.");
                return;
            }

            /*
             * The customer dashboard does not use a JobDetails.aspx
             * page. MyJobs.aspx is the customer job-management page.
             *
             * The job ID is intentionally not placed into the URL.
             * MyJobs.aspx performs its own customer-scoped data loading.
             */
            Response.Redirect("~/Secure/Customer/MyJobs.aspx");
        }

        // ============================================================
        // STATUS HELPERS
        // ============================================================
        private bool IsActiveStatus(string status)
        {
            return IsPendingStatus(status) || IsInProgressStatus(status);
        }

        private bool IsPendingStatus(string status)
        {
            return string.Equals(status == null ? "" : status.Trim(), "Pending", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsInProgressStatus(string status)
        {
            string normalized = status == null ? "" : status.Trim();
            return string.Equals(normalized, "In Progress", StringComparison.OrdinalIgnoreCase) || string.Equals(normalized, "In-Progress", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsCompletedStatus(string status)
        {
            return string.Equals(status == null ? "" : status.Trim(), "Completed", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsCancelledStatus(string status)
        {
            return string.Equals(status == null ? "" : status.Trim(), "Cancelled", StringComparison.OrdinalIgnoreCase);
        }

        // ============================================================
        // DATA HELPERS
        // ============================================================
        private string GetSafeString(DataRow row, string columnName, string fallback)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName))
            {
                return fallback;
            }

            if (row[columnName] == DBNull.Value)
            {
                return fallback;
            }

            string value = Convert.ToString(row[columnName]);
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            return value;
        }

        private int GetIntValue(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName))
            {
                return 0;
            }

            if (row[columnName] == DBNull.Value)
            {
                return 0;
            }

            int value;
            if (int.TryParse(Convert.ToString(row[columnName]), out value))
            {
                return value;
            }

            return 0;
        }

        private decimal GetDecimalValue(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName))
            {
                return 0m;
            }

            if (row[columnName] == DBNull.Value)
            {
                return 0m;
            }

            decimal value;
            if (decimal.TryParse(Convert.ToString(row[columnName]), out value))
            {
                return value;
            }

            return 0m;
        }

        private DateTime GetDateValue(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName))
            {
                return DateTime.MinValue;
            }

            if (row[columnName] == DBNull.Value)
            {
                return DateTime.MinValue;
            }

            DateTime value;
            if (DateTime.TryParse(Convert.ToString(row[columnName]), out value))
            {
                return value;
            }

            return DateTime.MinValue;
        }

        private string FormatDate(DataRow row, string columnName)
        {
            DateTime date = GetDateValue(row, columnName);
            if (date == DateTime.MinValue)
            {
                return "Not scheduled";
            }

            return date.ToString("dd MMM yyyy");
        }

        private string FormatActivityDate(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                return "Date unavailable";
            }

            return date.ToString("dd MMM yyyy, HH:mm");
        }

        private string FormatCurrency(decimal amount)
        {
            return amount.ToString("C2", CultureInfo.GetCultureInfo("en-ZA"));
        }

        // ============================================================
        // ERROR HANDLING
        // ============================================================
        private void ShowError(string message)
        {
            lblError.Text = Server.HtmlEncode(message ?? "Unable to load the dashboard.");
            pnlError.Visible = true;
        }

        // ============================================================
        // ACTIVITY MODEL
        // ============================================================
        private class ActivityItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string IconClass { get; set; }
            public string DateText { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
