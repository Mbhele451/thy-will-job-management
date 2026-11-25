using System;

namespace MajorProject.Business.Models
{
    /// <summary>
    /// Represents a job's financial breakdown in "invoice" shape. There is no
    /// separate Invoices table in the confirmed schema (see InvoiceDataAccess),
    /// so this model mirrors the relevant columns straight from Jobs rather than
    /// an InvoiceID/DueDate/separate-Status that don't exist anywhere.
    ///
    /// If a real Invoices table is added later (for payment tracking distinct
    /// from job status), this model should be rebuilt from its actual columns
    /// rather than extended to guess at fields.
    /// </summary>
    public class Invoice
    {
        public int JobID { get; set; }
        public int CustomerID { get; set; }
        public int StaffID { get; set; }
        public DateTime PromiseDate { get; set; }
        public string JobStatus { get; set; }
        public decimal LabourCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal TotalMaterialCost { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return $"Invoice for Job {JobID} - R{GrandTotal:F2} ({JobStatus})";
        }
    }
}
