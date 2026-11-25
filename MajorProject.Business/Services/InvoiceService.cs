using MajorProject.Business.Helpers;
using MajorProject.Data.Data;
using MajorProject.Business.Models.DTOs;
using System;
using System.Data;

namespace MajorProject.Business.Services
{
    /// <summary>
    /// Provides invoice-related operations for jobs.
    ///
    /// There is no separate Invoices table in the confirmed database schema.
    /// Invoice financial information is stored on the Jobs table.
    ///
    /// Job financial values are created and owned by JobService. This service
    /// therefore does not recalculate job totals using a hard-coded tax rate.
    /// It reads the authoritative values already saved against the job.
    /// </summary>
    public class InvoiceService
    {
        private readonly InvoiceDataAccess _dataAccess;
        public InvoiceService(string connectionString)
        {
            _dataAccess = new InvoiceDataAccess(connectionString);
        }

        /// <summary>
        /// Gets the invoice-shaped financial data for all jobs.
        ///
        /// Intended for trusted staff/admin operations.
        /// </summary>
        public DataTable GetAllInvoices()
        {
            return _dataAccess.GetAllJobInvoiceData();
        }

        /// <summary>
        /// Gets the invoice-shaped financial data for one job.
        ///
        /// Intended for trusted staff/admin operations.
        /// </summary>
        public DataTable GetInvoiceByJobID(int jobID)
        {
            if (jobID <= 0)
                throw new ArgumentException("A valid job ID is required.");
            return _dataAccess.GetJobInvoiceDataByJobID(jobID);
        }

        /// <summary>
        /// Gets generated invoice data for all jobs belonging to a specific
        /// customer.
        ///
        /// Only jobs with generated invoice files are returned.
        /// </summary>
        public DataTable GetInvoicesByCustomerID(int customerID)
        {
            if (customerID <= 0)
                throw new ArgumentException("A valid customer ID is required.");
            return _dataAccess.GetJobInvoiceDataByCustomerID(customerID);
        }

        /// <summary>
        /// Gets one generated invoice belonging to a specific customer.
        ///
        /// Both CustomerID and JobID are checked by the data-access layer.
        /// This prevents a customer from accessing another customer's invoice.
        /// </summary>
        public DataTable GetCustomerInvoice(int customerID, int jobID)
        {
            if (customerID <= 0)
                throw new ArgumentException("A valid customer ID is required.");
            if (jobID <= 0)
                throw new ArgumentException("A valid job ID is required.");
            return _dataAccess.GetCustomerInvoiceData(customerID, jobID);
        }

        /// <summary>
        /// Calculates the subtotal from the three job cost components.
        ///
        /// This method does not calculate tax. Tax is calculated during
        /// job creation using the TaxPercentage configured for each service
        /// and the resulting TaxAmount is saved to Jobs.
        /// </summary>
        public decimal CalculateSubtotal(decimal labourCost, decimal transportCost, decimal materialCost)
        {
            return labourCost + transportCost + materialCost;
        }

        /// <summary>
        /// Calculates a total from already-known authoritative values.
        ///
        /// No tax percentage is assumed here. The caller supplies the actual
        /// TaxAmount calculated for the saved job.
        /// </summary>
        public decimal CalculateGrandTotal(decimal labourCost, decimal transportCost, decimal materialCost, decimal taxAmount)
        {
            decimal subtotal = CalculateSubtotal(labourCost, transportCost, materialCost);
            return subtotal + taxAmount;
        }

        /// <summary>
        /// Saves the generated invoice PDF to the Jobs table.
        ///
        /// This method stores the PDF and its server-side file path.
        /// It does NOT mark the invoice as emailed. Email delivery is recorded
        /// separately only after Mailjet confirms successful delivery.
        /// </summary>
        public void SaveInvoiceToDatabase(int jobID, byte[] pdfBytes, string filePath)
        {
            try
            {
                if (jobID <= 0)
                    throw new ArgumentException("A valid job ID is required.");
                if (pdfBytes == null || pdfBytes.Length == 0)
                    throw new ArgumentException("PDF data cannot be empty.");
                _dataAccess.SaveInvoiceData(jobID, pdfBytes, filePath);
                Logger.Info($"Invoice PDF saved to DB for Job {jobID}. Path: {filePath}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to save invoice PDF to DB for Job {jobID}", ex);
                throw;
            }
        }

        /// <summary>
        /// Records a successful invoice email after Mailjet confirms
        /// that the email was accepted successfully.
        /// </summary>
        public void MarkInvoiceAsSent(int jobID, string customerEmail)
        {
            try
            {
                if (jobID <= 0)
                    throw new ArgumentException("A valid job ID is required.");
                if (string.IsNullOrWhiteSpace(customerEmail))
                    throw new ArgumentException("Customer email is required.");
                _dataAccess.MarkInvoiceAsSent(jobID, customerEmail);
                Logger.Info($"Invoice email marked as sent for Job {jobID} to {customerEmail}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to mark invoice email as sent for Job {jobID}", ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the generated invoice PDF from the database.
        ///
        /// Intended for trusted staff/admin operations.
        /// Customer-facing pages should use GetCustomerInvoicePDFFromDatabase()
        /// so that customer ownership is checked.
        /// </summary>
        public byte[] GetInvoicePDFFromDatabase(int jobID)
        {
            if (jobID <= 0)
                throw new ArgumentException("A valid job ID is required.");
            return _dataAccess.GetInvoicePDFData(jobID);
        }

        /// <summary>
        /// Retrieves a customer's invoice PDF from the database.
        ///
        /// The data-access layer verifies both CustomerID and JobID before
        /// returning the PDF.
        /// </summary>
        public byte[] GetCustomerInvoicePDFFromDatabase(int customerID, int jobID)
        {
            if (customerID <= 0)
                throw new ArgumentException("A valid customer ID is required.");
            if (jobID <= 0)
                throw new ArgumentException("A valid job ID is required.");
            return _dataAccess.GetCustomerInvoicePDFData(customerID, jobID);
        }

        public InvoiceDto GetCustomerInvoiceDto(int customerID, int jobID)
        {
            DataTable invoiceData = GetCustomerInvoice(customerID, jobID);
            if (invoiceData == null || invoiceData.Rows.Count == 0)
                return null;
            DataRow row = invoiceData.Rows[0];
            InvoiceDto invoice = new InvoiceDto();
            invoice.JobId = Convert.ToInt32(row["JobID"]);
            invoice.CustomerName = row["CustomerName"] == DBNull.Value ? string.Empty : Convert.ToString(row["CustomerName"]);
            invoice.CustomerEmail = row["CustomerEmail"] == DBNull.Value ? string.Empty : Convert.ToString(row["CustomerEmail"]);
            invoice.InvoiceDate = Convert.ToDateTime(row["CreatedAt"]);
            invoice.LabourCost = Convert.ToDecimal(row["LabourCost"]);
            invoice.TransportCost = Convert.ToDecimal(row["TransportCost"]);
            invoice.MaterialCost = Convert.ToDecimal(row["TotalMaterialCost"]);
            invoice.TaxAmount = Convert.ToDecimal(row["TaxAmount"]);
            invoice.GrandTotal = Convert.ToDecimal(row["GrandTotal"]);
            return invoice;
        }
    }
}
