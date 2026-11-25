using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using MajorProject.Business.Models.Payments;
using MajorProject.Business.Services.Payments;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;

namespace MajorProject.Web.Secure.Admin
{
    public partial class Jobs : AdminBasePage
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
        private JobService JobService
        {
            get
            {
                return new JobService(ConnectionString);
            }
        }

        private StaffService StaffService
        {
            get
            {
                return new StaffService(ConnectionString);
            }
        }

        private PaymentService PaymentService
        {
            get
            {
                return new PaymentService(ConnectionString);
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
            if (!IsPostBack)
            {
                InitializePage();
                LoadJobs();
            }
        }

        private void InitializePage()
        {
            txtNewPromiseDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
            ClearSelectedJob();
        }

        // ============================================================
        // LOAD JOBS
        // ============================================================
        private void LoadJobs()
        {
            try
            {
                DataTable jobs = JobService.SearchJobs(txtSearch.Text, ddlStatus.SelectedValue);
                // ----------------------------------------------------
                // Searching/filtering ALWAYS clears the selection.
                // The user must explicitly click View.
                // ----------------------------------------------------
                dgvJobs.SelectedIndex = -1;
                ClearSelectedJob();
                if (jobs == null || jobs.Rows.Count == 0)
                {
                    dgvJobs.DataSource = null;
                    dgvJobs.DataBind();
                    lblMessage.Text = "No jobs found.";
                    return;
                }

                dgvJobs.DataSource = jobs;
                dgvJobs.DataBind();
                lblMessage.Text = jobs.Rows.Count + " job(s) found. Select a job using the View button.";
            }
            catch (Exception ex)
            {
                dgvJobs.DataSource = null;
                dgvJobs.DataBind();
                dgvJobs.SelectedIndex = -1;
                ClearSelectedJob();
                lblMessage.Text = HandleUnexpectedError("Failed to load Admin jobs.", ex);
            }
        }

        // ============================================================
        // SEARCH
        // ============================================================
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dgvJobs.SelectedIndex = -1;
            LoadJobs();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            if (ddlStatus.Items.FindByValue("All") != null)
            {
                ddlStatus.SelectedValue = "All";
            }

            dgvJobs.SelectedIndex = -1;
            LoadJobs();
        }

        // ============================================================
        // JOB SELECTION
        // ============================================================
        protected void dgvJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedJob();
        }

        private int GetSelectedJobID()
        {
            if (dgvJobs.SelectedIndex < 0)
            {
                return 0;
            }

            if (dgvJobs.DataKeys == null || dgvJobs.DataKeys.Count <= dgvJobs.SelectedIndex)
            {
                return 0;
            }

            DataKey key = dgvJobs.DataKeys[dgvJobs.SelectedIndex];
            if (key == null || key.Value == null)
            {
                return 0;
            }

            int jobID;
            if (!int.TryParse(Convert.ToString(key.Value), out jobID))
            {
                return 0;
            }

            return jobID;
        }

        private bool HasSelectedJob()
        {
            return GetSelectedJobID() > 0;
        }

        // ============================================================
        // DISPLAY SELECTED JOB
        // ============================================================
        private void ShowSelectedJob()
        {
            try
            {
                int jobID = GetSelectedJobID();
                if (jobID <= 0)
                {
                    ClearSelectedJob();
                    lblMessage.Text = "Select a job from the list to view its details.";
                    return;
                }

                DataTable job = JobService.GetJobManagementDetails(jobID);
                if (job == null || job.Rows.Count == 0)
                {
                    ClearSelectedJob();
                    lblMessage.Text = "The selected job could not be found.";
                    return;
                }

                DataRow row = job.Rows[0];
                ConfigureCustomerDocuments(row);
                // ----------------------------------------------------
                // BASIC JOB INFORMATION
                // ----------------------------------------------------
                lblJobID.Text = Convert.ToString(row["JobID"]);
                lblCustomer.Text = GetDatabaseString(row, "CustomerName");
                lblTechnician.Text = GetDatabaseString(row, "TechnicianName");
                lblService.Text = GetDatabaseString(row, "Services");
                lblPromiseDate.Text = GetDatabaseDate(row, "PromiseDate");
                lblStatus.Text = GetDatabaseString(row, "JobStatus");
                // ----------------------------------------------------
                // FINANCIAL INFORMATION
                // ----------------------------------------------------
                lblLabour.Text = GetDatabaseMoney(row, "LabourCost");
                lblMaterials.Text = GetDatabaseMoney(row, "TotalMaterialCost");
                lblTransport.Text = GetDatabaseMoney(row, "TransportCost");
                lblTax.Text = GetDatabaseMoney(row, "TaxAmount");
                lblTotal.Text = GetDatabaseMoney(row, "GrandTotal");
                // ----------------------------------------------------
                // PROMISE DATE / TECHNICIAN
                // ----------------------------------------------------
                DateTime promiseDate = GetDateFromJob(row);
                txtNewPromiseDate.Text = GetDateForInput(promiseDate);
                txtNewPromiseDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
                LoadTechniciansForDate(promiseDate);
                // ----------------------------------------------------
                // TRY TO PRESERVE CURRENT TECHNICIAN
                // ----------------------------------------------------
                int currentTechnicianID = GetDatabaseInt(row, "StaffID");
                if (currentTechnicianID > 0)
                {
                    SelectTechnicianIfAvailable(currentTechnicianID);
                }

                // ----------------------------------------------------
                // CANCELLATION STATE
                // ----------------------------------------------------
                ConfigureCancellationState(promiseDate, GetDatabaseString(row, "JobStatus"));
                // ----------------------------------------------------
                // ENABLE JOB MANAGEMENT
                // ----------------------------------------------------
                bool canReschedule = !GetDatabaseString(row, "JobStatus").Equals("Completed", StringComparison.OrdinalIgnoreCase) && !GetDatabaseString(row, "JobStatus").Equals("Cancelled", StringComparison.OrdinalIgnoreCase) && !GetDatabaseString(row, "JobStatus").Equals("Archived", StringComparison.OrdinalIgnoreCase);
                SetJobManagementEnabled(canReschedule);
                ConfigureCashCollection(jobID);
                lblMessage.Text = "Job #" + jobID + " selected.";
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("Failed to display the selected job.", ex);
            }
        }

        // ============================================================
        // VIEW ADMIN INVOICE
        // ============================================================
        protected void btnViewAdminInvoice_Click(object sender, EventArgs e)
        {
            int jobID = GetSelectedJobID();
            if (jobID <= 0)
            {
                lblMessage.Text = "Select a job first.";
                return;
            }

            Response.Redirect(ResolveUrl("~/Secure/Admin/AdminInvoicePdf.aspx?jobID=" + jobID), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        // ============================================================
        // VIEW ADMIN PAYMENT RECEIPT
        // ============================================================
        protected void btnViewAdminReceipt_Click(object sender, EventArgs e)
        {
            int jobID = GetSelectedJobID();
            if (jobID <= 0)
            {
                lblMessage.Text = "Select a job first.";
                return;
            }

            Response.Redirect(ResolveUrl("~/Secure/Admin/AdminPaymentReceipt.aspx?jobID=" + jobID), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        // ============================================================
        // DATABASE VALUE HELPERS
        // ============================================================
        private DateTime GetDateFromJob(DataRow row)
        {
            if (row == null || !row.Table.Columns.Contains("PromiseDate") || row["PromiseDate"] == DBNull.Value)
            {
                return DateTime.Today;
            }

            return Convert.ToDateTime(row["PromiseDate"]).Date;
        }

        private string GetDateForInput(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        private string GetDatabaseString(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
            {
                return "—";
            }

            if (row[columnName] == DBNull.Value)
            {
                return "—";
            }

            string value = Convert.ToString(row[columnName]);
            if (string.IsNullOrWhiteSpace(value))
            {
                return "—";
            }

            return value;
        }

        private int GetDatabaseInt(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
            {
                return 0;
            }

            if (row[columnName] == DBNull.Value)
            {
                return 0;
            }

            int value;
            if (!int.TryParse(Convert.ToString(row[columnName]), out value))
            {
                return 0;
            }

            return value;
        }

        private string GetDatabaseDate(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
            {
                return "—";
            }

            if (row[columnName] == DBNull.Value)
            {
                return "—";
            }

            return Convert.ToDateTime(row[columnName]).ToString("dd MMM yyyy");
        }

        private string GetDatabaseMoney(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
            {
                return "0.00";
            }

            if (row[columnName] == DBNull.Value)
            {
                return "0.00";
            }

            return Convert.ToDecimal(row[columnName]).ToString("N2");
        }

        // ============================================================
        // TECHNICIANS
        // ============================================================
        private void LoadTechniciansForDate(DateTime date)
        {
            ddlTechnician.Items.Clear();
            ddlTechnician.Items.Add(new ListItem("-- Select technician --", ""));
            DataTable technicians = StaffService.GetAvailableTechnicians(date.Date);
            if (technicians == null || technicians.Rows.Count == 0)
            {
                return;
            }

            foreach (DataRow row in technicians.Rows)
            {
                if (!row.Table.Columns.Contains("StaffID") || !row.Table.Columns.Contains("FullName"))
                {
                    continue;
                }

                string staffID = Convert.ToString(row["StaffID"]);
                string fullName = Convert.ToString(row["FullName"]);
                if (string.IsNullOrWhiteSpace(staffID))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    continue;
                }

                ddlTechnician.Items.Add(new ListItem(fullName, staffID));
            }
        }

        private void SelectTechnicianIfAvailable(int technicianID)
        {
            if (technicianID <= 0)
            {
                return;
            }

            ListItem item = ddlTechnician.Items.FindByValue(technicianID.ToString());
            if (item != null)
            {
                ddlTechnician.ClearSelection();
                item.Selected = true;
            }
        }

        private bool IsTechnicianAvailableForDate(int technicianID, DateTime date)
        {
            if (technicianID <= 0)
            {
                return false;
            }

            DataTable technicians = StaffService.GetAvailableTechnicians(date.Date);
            if (technicians == null)
            {
                return false;
            }

            foreach (DataRow row in technicians.Rows)
            {
                if (!row.Table.Columns.Contains("StaffID"))
                {
                    continue;
                }

                int availableID;
                if (int.TryParse(Convert.ToString(row["StaffID"]), out availableID) && availableID == technicianID)
                {
                    return true;
                }
            }

            return false;
        }

        // ============================================================
        // CANCELLATION STATE
        // ============================================================
        private void ConfigureCancellationState(DateTime promiseDate, string status)
        {
            bool active = status.Equals("Pending", StringComparison.OrdinalIgnoreCase) || status.Equals("In Progress", StringComparison.OrdinalIgnoreCase);
            btnCancelJob.Visible = active;
            if (!active)
            {
                pnlLateCancellation.Visible = false;
                return;
            }

            int daysUntilJob = (promiseDate.Date - DateTime.UtcNow.AddHours(2).Date).Days;
            pnlLateCancellation.Visible = status.Equals("In Progress", StringComparison.OrdinalIgnoreCase) || daysUntilJob == 0 || daysUntilJob == 1;
        }

        // ============================================================
        // RESCHEDULE
        // ============================================================
        protected void btnReschedule_Click(object sender, EventArgs e)
        {
            try
            {
                int jobID = GetSelectedJobID();
                if (jobID <= 0)
                {
                    lblMessage.Text = "Select a job first.";
                    return;
                }

                // ----------------------------------------------------
                // Read current job
                // ----------------------------------------------------
                DataTable job = JobService.GetJobManagementDetails(jobID);
                if (job == null || job.Rows.Count == 0)
                {
                    lblMessage.Text = "The selected job could not be found.";
                    return;
                }

                DataRow jobRow = job.Rows[0];
                string currentStatus = GetDatabaseString(jobRow, "JobStatus");
                // ----------------------------------------------------
                // Only active jobs can be rescheduled.
                // ----------------------------------------------------
                if (currentStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) || currentStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) || currentStatus.Equals("Archived", StringComparison.OrdinalIgnoreCase))
                {
                    lblMessage.Text = "Completed, Cancelled or Archived jobs cannot be rescheduled.";
                    return;
                }

                // ----------------------------------------------------
                // Validate new date
                // ----------------------------------------------------
                DateTime newDate;
                if (!DateTime.TryParseExact(txtNewPromiseDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out newDate))
                {
                    lblMessage.Text = "Enter a valid promise date.";
                    return;
                }

                newDate = newDate.Date;
                if (newDate < DateTime.Today)
                {
                    lblMessage.Text = "Promise date cannot be earlier than today.";
                    return;
                }

                // ----------------------------------------------------
                // Validate technician
                // ----------------------------------------------------
                if (string.IsNullOrWhiteSpace(ddlTechnician.SelectedValue))
                {
                    lblMessage.Text = "Select a technician.";
                    return;
                }

                int technicianID;
                if (!int.TryParse(ddlTechnician.SelectedValue, out technicianID) || technicianID <= 0)
                {
                    lblMessage.Text = "The selected technician is invalid.";
                    return;
                }

                // ----------------------------------------------------
                // Confirm technician is available for new date
                // ----------------------------------------------------
                if (!IsTechnicianAvailableForDate(technicianID, newDate))
                {
                    lblMessage.Text = "The selected technician is not available on the new promise date. Please choose another technician.";
                    LoadTechniciansForDate(newDate);
                    return;
                }

                // ----------------------------------------------------
                // Transactional reschedule
                // ----------------------------------------------------
                JobService.RescheduleJob(jobID, newDate, technicianID);
                lblMessage.Text = "Job #" + jobID + " was successfully rescheduled.";
                // ----------------------------------------------------
                // Refresh job list.
                //
                // This intentionally clears selection because the
                // user should explicitly select a job again.
                // ----------------------------------------------------
                LoadJobs();
            }
            catch (InvalidOperationException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (ArgumentException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("The job could not be rescheduled.", ex);
            }
        }

        // ============================================================
        // NEW DATE CHANGE
        // ============================================================
        protected void txtNewPromiseDate_TextChanged(object sender, EventArgs e)
        {
            // --------------------------------------------------------
            // A date cannot be changed unless a job was explicitly
            // selected.
            // --------------------------------------------------------
            if (!HasSelectedJob())
            {
                lblMessage.Text = "Select a job before changing the promise date.";
                return;
            }

            DateTime newDate;
            if (!DateTime.TryParseExact(txtNewPromiseDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out newDate))
            {
                ddlTechnician.Items.Clear();
                ddlTechnician.Items.Add(new ListItem("-- Select a valid date first --", ""));
                lblMessage.Text = "Enter a valid promise date.";
                return;
            }

            newDate = newDate.Date;
            if (newDate < DateTime.Today)
            {
                ddlTechnician.Items.Clear();
                ddlTechnician.Items.Add(new ListItem("-- Date cannot be in the past --", ""));
                lblMessage.Text = "Promise date cannot be earlier than today.";
                return;
            }

            LoadTechniciansForDate(newDate);
            lblMessage.Text = "Technicians updated for " + newDate.ToString("dd MMM yyyy") + ".";
        }

        // ============================================================
        // CANCEL JOB
        // ============================================================
        protected void btnCancelJob_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Secure/JobChanges.aspx?job=" + GetSelectedJobID()), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        // ============================================================
        // ENABLE / DISABLE JOB MANAGEMENT
        // ============================================================
        private void SetJobManagementEnabled(bool enabled)
        {
            txtNewPromiseDate.Enabled = enabled;
            ddlTechnician.Enabled = enabled;
            btnReschedule.Enabled = enabled;
            btnCancelJob.Enabled = enabled;
        }

        // ============================================================
        // CLEAR SELECTED JOB
        // ============================================================
        private void ClearSelectedJob()
        {
            lblJobID.Text = "—";
            lblCustomer.Text = "—";
            lblService.Text = "—";
            lblTechnician.Text = "—";
            lblPromiseDate.Text = "—";
            lblStatus.Text = "—";
            lblLabour.Text = "0.00";
            lblMaterials.Text = "0.00";
            lblTransport.Text = "0.00";
            lblTax.Text = "0.00";
            lblTotal.Text = "0.00";
            txtNewPromiseDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtNewPromiseDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
            ddlTechnician.Items.Clear();
            ddlTechnician.Items.Add(new ListItem("-- Select technician --", ""));
            btnCancelJob.Visible = false;
            pnlLateCancellation.Visible = false;
            SetJobManagementEnabled(false);
            lblDocumentPaymentStatus.Text = "—";
            btnViewAdminInvoice.Visible = false;
            btnViewAdminReceipt.Visible = false;
            btnCollectCash.Visible = false;
        }

        // ============================================================
        // CUSTOMER DOCUMENTS
        // ============================================================
        private void ConfigureCustomerDocuments(DataRow jobRow)
        {
            btnViewAdminInvoice.Visible = false;
            btnViewAdminReceipt.Visible = false;
            lblDocumentPaymentStatus.Text = "Payment information unavailable.";
            int jobID = GetDatabaseInt(jobRow, "JobID");
            int customerID = GetDatabaseInt(jobRow, "CustomerID");
            if (jobID <= 0 || customerID <= 0)
            {
                return;
            }

            // --------------------------------------------------------
            // CHECK INVOICE AVAILABILITY
            // --------------------------------------------------------
            try
            {
                byte[] invoicePdf = InvoiceService.GetCustomerInvoicePDFFromDatabase(customerID, jobID);
                btnViewAdminInvoice.Visible = invoicePdf != null && invoicePdf.Length > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Admin invoice availability error: " + ex);
                btnViewAdminInvoice.Visible = false;
            }

            // --------------------------------------------------------
            // CHECK PAYMENT STATUS
            // --------------------------------------------------------
            try
            {
                PaymentResult payment = PaymentService.GetCustomerInvoicePaymentReceiptResult(jobID, customerID);
                bool successfulPayment = payment != null && string.Equals(payment.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase) && string.Equals(payment.AttemptStatus, "Successful", StringComparison.OrdinalIgnoreCase);
                if (successfulPayment)
                {
                    lblDocumentPaymentStatus.Text = "Paid — successful payment recorded.";
                    btnViewAdminReceipt.Visible = true;
                }
                else
                {
                    lblDocumentPaymentStatus.Text = "No successful payment receipt is available.";
                    btnViewAdminReceipt.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Admin payment status error: " + ex);
                lblDocumentPaymentStatus.Text = "Payment information unavailable.";
                btnViewAdminReceipt.Visible = false;
            }
        }

        private void ConfigureCashCollection(int jobID)
        {
            btnCollectCash.Visible = false;
            if (jobID <= 0)
                return;
            try
            {
                DataTable paymentDetails = PaymentService.GetStaffCashPaymentDetails(jobID);
                if (paymentDetails == null || paymentDetails.Rows.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Cash collection: No payment record for Job " + jobID);
                    return;
                }

                DataRow row = paymentDetails.Rows[0];
                string jobStatus = Convert.ToString(row["JobStatus"]).Trim();
                string paymentStatus = Convert.ToString(row["PaymentStatus"]).Trim();
                bool completed = jobStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase);
                bool pending = paymentStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase);
                btnCollectCash.Visible = completed && pending;
                System.Diagnostics.Debug.WriteLine("Admin Cash Collection | Job: " + jobID + " | Job Status: " + jobStatus + " | Payment Status: " + paymentStatus + " | Button Visible: " + btnCollectCash.Visible);
            }
            catch (Exception ex)
            {
                btnCollectCash.Visible = false;
                System.Diagnostics.Debug.WriteLine("Admin cash collection error: " + ex);
            }
        }

        protected void btnCollectCash_Click(object sender, EventArgs e)
        {
            int jobID = GetSelectedJobID();
            if (jobID <= 0)
            {
                lblMessage.Text = "Select a job first.";
                return;
            }

            string url = ResolveUrl("~/Secure/Staff/CashPayment.aspx?jobID=" + jobID);
            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
