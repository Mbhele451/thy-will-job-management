using MajorProject.Data.Data;
using MajorProject.Business.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Net;

namespace MajorProject.Business.Services
{
    public class JobService
    {
        private readonly JobDataAccess _jobDataAccess;
        private readonly StaffService _staffService;
        private readonly string _connectionString;
        private const int MaxQuantityPerLine = 1000;
        private const int MaxCartLines = 50;
        private const int MaxJobNoteLength = 255;
        public JobService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));
            }

            _connectionString = connectionString;
            _jobDataAccess = new JobDataAccess(connectionString);
            _staffService = new StaffService(connectionString);
        }

        // ============================================================
        // JOB RETRIEVAL
        // ============================================================
        public DataTable GetAllJobs()
        {
            return _jobDataAccess.GetAllJobs();
        }

        public DataTable GetJobByID(int jobID)
        {
            ValidateJobID(jobID);
            return _jobDataAccess.GetJobByID(jobID);
        }

        public DataTable GetJobByCustomerID(int customerID, int jobID)
        {
            ValidateCustomerID(customerID);
            ValidateJobID(jobID);
            return _jobDataAccess.GetJobByCustomerID(customerID, jobID);
        }

        public DataTable GetJobsByStaffID(int staffID)
        {
            ValidateStaffID(staffID);
            return _jobDataAccess.GetJobsByStaffID(staffID);
        }

        public DataTable GetJobsByStatus(string status)
        {
            ValidateStatus(status);
            return _jobDataAccess.GetJobsByStatus(status.Trim());
        }

        public DataTable GetJobViews()
        {
            return _jobDataAccess.GetJobViews();
        }

        public DataTable GetJobsByCustomerID(int customerID)
        {
            ValidateCustomerID(customerID);
            return _jobDataAccess.GetJobsByCustomerID(customerID);
        }

        public int GetJobCountByStatus(int staffID, string status)
        {
            ValidateStaffID(staffID);
            ValidateStatus(status);
            return _jobDataAccess.GetJobCountByStatus(staffID, status.Trim());
        }

        public int GetTotalJobCountByStatus(string status)
        {
            ValidateStatus(status);
            return _jobDataAccess.GetTotalJobCountByStatus(status.Trim());
        }

        // ============================================================
        // STATUS VALIDATION
        // ============================================================
        public bool IsValidStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            string normalizedStatus = status.Trim();
            return normalizedStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) || normalizedStatus.Equals("In Progress", StringComparison.OrdinalIgnoreCase) || normalizedStatus.Equals("In-Progress", StringComparison.OrdinalIgnoreCase) || normalizedStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) || normalizedStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) || normalizedStatus.Equals("Archived", StringComparison.OrdinalIgnoreCase) || normalizedStatus.Equals("All", StringComparison.OrdinalIgnoreCase);
        }

        // ============================================================
        // TECHNICIAN-SAFE JOB RETRIEVAL
        // ============================================================
        public DataTable GetTechnicianJobs(int staffID)
        {
            ValidateStaffID(staffID);
            return _jobDataAccess.GetTechnicianJobs(staffID);
        }

        public DataTable GetTechnicianJobByID(int jobID, int staffID)
        {
            ValidateJobID(jobID);
            ValidateStaffID(staffID);
            return _jobDataAccess.GetTechnicianJobByID(jobID, staffID);
        }

        // ============================================================
        // LEGACY TECHNICIAN AVAILABILITY LOOKUP
        // ============================================================
        // Retained for existing screens.
        // Do not use this method to allocate a technician during booking.
        // ============================================================
        public int FindAvailableTechnician(DateTime promiseDate)
        {
            ValidatePromiseDate(promiseDate);
            int technicianID = _staffService.FindAvailableTechnician(promiseDate.Date);
            return technicianID > 0 ? technicianID : 0;
        }

        // ============================================================
        // AUTOMATIC TECHNICIAN ASSIGNMENT AND JOB CREATION
        // ============================================================
        // This is the method BookJob.aspx.cs must call.
        //
        // Technician selection and job creation happen together inside
        // JobDataAccess, under the date-level database allocation lock.
        // ============================================================
        public int CreateJobWithDetailsAndMaterials(int customerID, DateTime promiseDate, string notes, DataTable cart)
        {
            ValidateCustomerID(customerID);
            ValidatePromiseDate(promiseDate);
            ValidateCart(cart);
            notes = ValidateAndNormalizeNotes(notes);
            int jobID = _jobDataAccess.CreateJobWithDetailsAndMaterials(customerID, promiseDate.Date, notes, cart);
            if (jobID <= 0)
            {
                throw new InvalidOperationException("The job could not be created or no technician is available.");
            }

            return jobID;
        }

        // ============================================================
        // EXPLICIT TECHNICIAN ASSIGNMENT
        // ============================================================
        // Retained for administrative and existing internal workflows.
        // JobDataAccess performs the allocation lock and conflict check.
        // ============================================================
        public int CreateJobWithDetailsAndMaterials(int customerID, int staffID, DateTime promiseDate, string notes, DataTable cart)
        {
            ValidateCustomerID(customerID);
            ValidateStaffID(staffID);
            ValidatePromiseDate(promiseDate);
            ValidateCart(cart);
            notes = ValidateAndNormalizeNotes(notes);
            int jobID = _jobDataAccess.CreateJobWithDetailsAndMaterials(customerID, staffID, promiseDate.Date, notes, cart);
            if (jobID <= 0)
            {
                throw new InvalidOperationException("The job could not be created.");
            }

            return jobID;
        }

        // ============================================================
        // GENERAL JOB STATUS
        // ============================================================
        public void UpdateJobStatus(int jobID, string status, string notes = null)
        {
            ValidateJobID(jobID);
            ValidateStatus(status);
            string normalizedStatus = status.Trim();
            if (normalizedStatus.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("'All' is not a valid job status for an update.", nameof(status));
            }

            notes = ValidateAndNormalizeNotes(notes);
            int affectedRows = _jobDataAccess.UpdateJobStatus(jobID, normalizedStatus, notes);
            if (affectedRows != 1)
            {
                throw new InvalidOperationException("The job status could not be updated.");
            }
        }

        // ============================================================
        // ARCHIVE JOB
        // ============================================================
        public void ArchiveJob(string currentStatus, int jobID)
        {
            ValidateJobID(jobID);
            if (string.IsNullOrWhiteSpace(currentStatus))
            {
                throw new ArgumentException("Current job status is required.", nameof(currentStatus));
            }

            string normalizedStatus = currentStatus.Trim();
            if (!normalizedStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) && !normalizedStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Only Completed or Cancelled jobs can be archived.");
            }

            int affectedRows = _jobDataAccess.ArchiveJob(jobID);
            if (affectedRows != 1)
            {
                throw new InvalidOperationException("The job could not be archived.");
            }
        }

        // ============================================================
        // TECHNICIAN START JOB
        // ============================================================
        public void StartTechnicianJob(int jobID, int staffID)
        {
            ValidateJobID(jobID);
            ValidateStaffID(staffID);
            DataTable jobTable = _jobDataAccess.GetTechnicianJobByID(jobID, staffID);
            if (jobTable == null || jobTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("The selected job does not belong to this technician.");
            }

            DataRow jobRow = jobTable.Rows[0];
            string status = Convert.ToString(jobRow["JobStatus"]).Trim();
            if (!status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Only Pending jobs can be started.");
            }

            if (jobRow["PromiseDate"] == DBNull.Value)
            {
                throw new InvalidOperationException("The job does not have a valid promise date.");
            }

            DateTime promiseDate = Convert.ToDateTime(jobRow["PromiseDate"]).Date;
            if (promiseDate > DateTime.Today)
            {
                throw new InvalidOperationException("The job cannot be started before its promise date.");
            }

            int affectedRows = _jobDataAccess.StartTechnicianJob(jobID, staffID);
            if (affectedRows != 1)
            {
                throw new InvalidOperationException("The job could not be started. Its status or assignment may have changed.");
            }
        }

        // ============================================================
        // TECHNICIAN COMPLETE JOB
        // ============================================================
        public void CompleteTechnicianJob(int jobID, int staffID)
        {
            ValidateJobID(jobID);
            ValidateStaffID(staffID);
            DataTable jobTable = _jobDataAccess.GetTechnicianJobByID(jobID, staffID);
            if (jobTable == null || jobTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("The selected job does not belong to this technician.");
            }

            DataRow jobRow = jobTable.Rows[0];
            string status = Convert.ToString(jobRow["JobStatus"]).Trim();
            if (!status.Equals("In Progress", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Only In Progress jobs can be completed.");
            }

            int affectedRows = _jobDataAccess.CompleteTechnicianJob(jobID, staffID);
            if (affectedRows != 1)
            {
                throw new InvalidOperationException("The job could not be completed. Its status or assignment may have changed.");
            }

            // Completion is authoritative in the database. Email delivery is queued
            // separately so a slow provider or temporary outage never rolls back the job.
            QueueCompletionEmail(jobRow);
        }

        private void QueueCompletionEmail(DataRow jobRow)
        {
            try
            {
                int jobID = Convert.ToInt32(jobRow["JobID"]);
                int customerID = Convert.ToInt32(jobRow["CustomerID"]);
                var invoice = new InvoiceService(_connectionString).GetCustomerInvoiceDto(customerID, jobID);
                if (invoice == null || string.IsNullOrWhiteSpace(invoice.CustomerEmail))
                {
                    Logger.Warning("Job completion email skipped: customer email or invoice data is unavailable for Job " + jobID + ".");
                    return;
                }

                string customer = WebUtility.HtmlEncode(invoice.CustomerName ?? "Customer");
                string html = "<html><body>" + "<h2>Job completed - THY WILL</h2>" + "<p>Dear " + customer + ",</p>" + "<p>Your technician has completed Job <strong>#" + jobID + "</strong>. Thank you for choosing THY WILL.</p>" + "<p><strong>Demo invoice total:</strong> R" + invoice.GrandTotal.ToString("N2") + "</p>" + "<h3>Simulated payment</h3><p>This is a demonstration system. No real payment is due and no bank charge is made. Do not transfer money or enter real banking details. Use the simulated checkout in your customer portal.</p>" + "<p>You can review the invoice and payment status in your customer portal. Please keep this email for your records.</p>" + "<p>Kind regards,<br/>THY WILL Electrical &amp; Construction</p></body></html>";
                EmailQueueService.Instance.Enqueue(invoice.CustomerEmail, invoice.CustomerName, "Job completed - payment details for Job #" + jobID, html);
                Logger.Info("Queued job completion email for Job " + jobID + " to " + invoice.CustomerEmail + ".");
            }
            catch (Exception ex)
            {
                // Never report a completed job as failed because notification infrastructure is down.
                Logger.Error("Unable to queue customer completion email.", ex);
            }
        }

        private static string AppSetting(string key, string fallback)
        {
            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        // ============================================================
        // TECHNICIAN CANCEL JOB
        // ============================================================
        public bool CancelTechnicianJob(int jobID, int staffID, out bool isLateCancellation)
        {
            ValidateJobID(jobID);
            ValidateStaffID(staffID);
            DataTable jobTable = _jobDataAccess.GetTechnicianJobByID(jobID, staffID);
            if (jobTable == null || jobTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("The selected job does not belong to this technician.");
            }

            DataRow jobRow = jobTable.Rows[0];
            string status = Convert.ToString(jobRow["JobStatus"]).Trim();
            if (!status.Equals("Pending", StringComparison.OrdinalIgnoreCase) && !status.Equals("In Progress", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Only Pending or In Progress jobs can be cancelled.");
            }

            return _jobDataAccess.CancelTechnicianJob(jobID, staffID, out isLateCancellation);
        }

        // ============================================================
        // JOB DETAILS
        // ============================================================
        public DataTable GetJobDetailsWithType()
        {
            return _jobDataAccess.GetJobDetailsWithType();
        }

        public DataTable GetJobDetailsByJobID(int jobID)
        {
            ValidateJobID(jobID);
            return _jobDataAccess.GetJobDetailsByJobID(jobID);
        }

        // ============================================================
        // RESCHEDULE
        // ============================================================
        public void RescheduleJob(int jobID, DateTime newPromiseDate, int newTechnicianID)
        {
            ValidateJobID(jobID);
            ValidateStaffID(newTechnicianID);
            ValidatePromiseDate(newPromiseDate);
            _jobDataAccess.RescheduleJob(jobID, newPromiseDate.Date, newTechnicianID);
        }

        // ============================================================
        // ADMIN CANCELLATION
        // ============================================================
        public bool CancelAdminJob(int jobID, out bool isLateCancellation)
        {
            ValidateJobID(jobID);
            return _jobDataAccess.CancelJobAndRestoreInventory(jobID, out isLateCancellation);
        }

        // ============================================================
        // JOB SEARCH
        // ============================================================
        public DataTable SearchJobs(string searchText, string status)
        {
            string normalizedStatus = string.IsNullOrWhiteSpace(status) ? "All" : status.Trim();
            ValidateStatus(normalizedStatus);
            return _jobDataAccess.SearchJobs(searchText, normalizedStatus);
        }

        // ============================================================
        // JOB MANAGEMENT
        // ============================================================
        public DataTable GetJobManagementDetails(int jobID)
        {
            ValidateJobID(jobID);
            return _jobDataAccess.GetJobManagementDetails(jobID);
        }

        // ============================================================
        // CART VALIDATION
        // ============================================================
        private void ValidateCart(DataTable cart)
        {
            if (cart == null || cart.Rows.Count == 0)
            {
                throw new ArgumentException("At least one service is required.", nameof(cart));
            }

            if (cart.Rows.Count > MaxCartLines)
            {
                throw new ArgumentException("A job cannot contain more than " + MaxCartLines + " service lines.", nameof(cart));
            }

            if (!cart.Columns.Contains("JobTypeID"))
            {
                throw new ArgumentException("The cart does not contain the required JobTypeID column.");
            }

            if (!cart.Columns.Contains("Quantity"))
            {
                throw new ArgumentException("The cart does not contain the required Quantity column.");
            }

            int validCartLines = 0;
            foreach (DataRow row in cart.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                GetJobTypeID(row);
                GetQuantity(row);
                validCartLines++;
            }

            if (validCartLines == 0)
            {
                throw new ArgumentException("At least one valid service is required.", nameof(cart));
            }
        }

        private int GetJobTypeID(DataRow row)
        {
            if (!row.Table.Columns.Contains("JobTypeID"))
            {
                throw new ArgumentException("The cart does not contain the required JobTypeID column.");
            }

            if (row["JobTypeID"] == DBNull.Value)
            {
                throw new ArgumentException("Every service must have a valid service ID.");
            }

            int jobTypeID;
            if (!int.TryParse(Convert.ToString(row["JobTypeID"]), out jobTypeID) || jobTypeID <= 0)
            {
                throw new ArgumentException("Every service must have a valid service ID.");
            }

            return jobTypeID;
        }

        private int GetQuantity(DataRow row)
        {
            if (!row.Table.Columns.Contains("Quantity"))
            {
                throw new ArgumentException("The cart does not contain the required Quantity column.");
            }

            if (row["Quantity"] == DBNull.Value)
            {
                throw new ArgumentException("Every service must have a valid quantity.");
            }

            int quantity;
            if (!int.TryParse(Convert.ToString(row["Quantity"]), out quantity) || quantity <= 0)
            {
                throw new ArgumentException("Every service quantity must be a valid positive whole number.");
            }

            if (quantity > MaxQuantityPerLine)
            {
                throw new ArgumentException("A service quantity cannot exceed " + MaxQuantityPerLine + ".");
            }

            return quantity;
        }

        // ============================================================
        // VALIDATION HELPERS
        // ============================================================
        private void ValidateJobID(int jobID)
        {
            if (jobID <= 0)
            {
                throw new ArgumentException("Invalid job ID.", nameof(jobID));
            }
        }

        private void ValidateCustomerID(int customerID)
        {
            if (customerID <= 0)
            {
                throw new ArgumentException("Invalid customer ID.", nameof(customerID));
            }
        }

        private void ValidateStaffID(int staffID)
        {
            if (staffID <= 0)
            {
                throw new ArgumentException("Invalid staff ID.", nameof(staffID));
            }
        }

        private void ValidatePromiseDate(DateTime promiseDate)
        {
            if (promiseDate.Date < DateTime.Today)
            {
                throw new ArgumentException("The job date cannot be in the past.", nameof(promiseDate));
            }
        }

        private void ValidateStatus(string status)
        {
            if (!IsValidStatus(status))
            {
                throw new ArgumentException("Invalid job status.", nameof(status));
            }
        }

        private string ValidateAndNormalizeNotes(string notes)
        {
            string normalizedNotes = (notes ?? string.Empty).Trim();
            if (normalizedNotes.Length > MaxJobNoteLength)
            {
                throw new ArgumentException("The job note cannot exceed " + MaxJobNoteLength + " characters.", nameof(notes));
            }

            return normalizedNotes;
        }
    }
}
