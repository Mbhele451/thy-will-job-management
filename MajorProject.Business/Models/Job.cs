using System;

namespace MajorProject.Business.Models
{
    /// <summary>
    /// Represents a job, matching the real Jobs table:
    /// Jobs(JobID, CustomerID, StaffID, PromiseDate, JobStatus,
    ///      TotalMaterialCost, LabourCost, TransportCost, TaxAmount,
    ///      GrandTotal, Notes, CreatedAt, UpdatedAt).
    ///
    /// Fix vs. previous version: removed TechnicianID/JobType/ScheduledDate/
    /// Status/TotalCost, none of which are real columns. Replaced with StaffID,
    /// PromiseDate, JobStatus and the real cost columns.
    ///
    /// NOTE ON STATUS CONSTANTS: AdminForm/ManagerForm's comboBox5 filter uses
    /// the literal string "In-Progress" (hyphen), while this constant uses
    /// "In Progress" (space) to match Job.StatusInProgress from the original
    /// scaffold. These two do not currently match each other anywhere in the
    /// real UI - pick one canonical spelling and update both places once
    /// confirmed, rather than me guessing which is "correct".
    /// </summary>
    public class Job
    {
        public int JobID { get; set; }
        public int CustomerID { get; set; }
        public int StaffID { get; set; }
        public DateTime PromiseDate { get; set; }
        public string JobStatus { get; set; }
        public decimal TotalMaterialCost { get; set; }
        public decimal LabourCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public const string StatusPending = "Pending";
        public const string StatusInProgress = "In Progress"; // see class note above
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusArchived = "Archived";
        public override string ToString()
        {
            return $"Job {JobID} ({JobStatus}) - Grand Total: R{GrandTotal:F2}";
        }
    }
}
