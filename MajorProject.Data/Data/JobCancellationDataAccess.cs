using System;
using System.Data;
using System.Data.SqlClient;

namespace MajorProject.Data.Data
{
    // Place in the same project as JobChangeDataAccess.cs. Requires the existing
    // JobChangeRequests, JobChangeMaterials and AppSecurityAudit tables.
    public sealed partial class JobChangeDataAccess
    {
        public int RequestCancellation(int job, int actor, bool customer, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length < 10 || reason.Length > 1000)
                throw new ArgumentException("Give a reason between 10 and 1,000 characters.");
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction(IsolationLevel.Serializable))
            {
                if (!customer)
                    Actor(c, t, actor, true, true);
                var jobs = Read(c, t, @"SELECT j.JobStatus,j.StaffID,j.GrandTotal,
                    DATEDIFF(day,CONVERT(date,DATEADD(hour,2,SYSUTCDATETIME())),j.PromiseDate) AS DaysToVisit
                    FROM dbo.Jobs j WITH(UPDLOCK,HOLDLOCK) JOIN dbo.Customers c ON c.CustomerID=j.CustomerID
                    LEFT JOIN dbo.AppAccountSecurity x ON x.CustomerID=c.CustomerID
                    WHERE j.JobID=@job AND j.JobStatus IN ('Pending','In Progress')
                    AND (@customer=0 OR (j.CustomerID=@actor AND c.AccountStatus='Active' AND COALESCE(x.IsBlocked,0)=0))",
                    P("@job", job), P("@customer", customer), P("@actor", actor));
                if (jobs.Rows.Count != 1)
                    throw new InvalidOperationException("Choose an open booking that you can manage.");
                if (Read(c, t, @"SELECT RequestID FROM dbo.JobChangeRequests WITH(UPDLOCK,HOLDLOCK)
                    WHERE JobID=@job AND RequestStatus='Pending'", P("@job", job)).Rows.Count > 0)
                    throw new InvalidOperationException("This booking already has a request awaiting review.");

                var row = jobs.Rows[0];
                bool started = Convert.ToString(row["JobStatus"]) == "In Progress";
                if (started && row.IsNull("StaffID"))
                    throw new InvalidOperationException("Assign the technician responsible for this work before requesting a material report.");
                bool late = !row.IsNull("DaysToVisit") &&
                    (Convert.ToInt32(row["DaysToVisit"]) == 0 || Convert.ToInt32(row["DaysToVisit"]) == 1);
                decimal total = Convert.ToDecimal(row["GrandTotal"]);
                decimal fee = started || late ? Math.Round(total * 0.05m, 2, MidpointRounding.AwayFromZero) : 0m;
                string explanation = started
                    ? "Work had started. The cancellation charge is 5% of the original job total plus the value of materials used."
                    : late ? "Cancellation was requested on the promise date or the calendar day before. The charge is 5% of the original job total."
                    : "Cancellation was outside the late window and work had not started. No cancellation fee applies.";
                int request;
                using (var q = Cmd(c, t, @"INSERT dbo.JobChangeRequests
                    (JobID,RequestedBy,RequestedByCustomerID,RequestType,Reason,OriginalTotal,CancellationFee,FeeReason)
                    OUTPUT inserted.RequestID VALUES(@job,@staff,@customer,'Cancellation',@reason,@total,@fee,@explanation)",
                    P("@job", job), P("@staff", customer ? (object)null : actor),
                    P("@customer", customer ? (object)actor : null),
                    P("@reason", reason.Trim()), P("@total", total),
                    P("@fee", fee), P("@explanation", explanation)))
                    request = Convert.ToInt32(q.ExecuteScalar());
                using (var q = Cmd(c, t, @"INSERT dbo.JobChangeMaterials(RequestID,ItemID,AllocatedQuantity,UsedQuantity)
                    SELECT @r,ItemID,SUM(QuantityUsed),0 FROM dbo.JobMaterials WITH(UPDLOCK,HOLDLOCK)
                    WHERE JobID=@job GROUP BY ItemID", P("@r", request), P("@job", job)))
                    q.ExecuteNonQuery();
                if (started)
                {
                    using (var q = Cmd(c, t, @"INSERT dbo.JobAllocationNotifications(JobID,StaffID,Message)
                        VALUES(@job,@staff,'Cancellation requested. Submit the quantities of materials used for manager review.')",
                        P("@job", job), P("@staff", row["StaffID"])))
                        q.ExecuteNonQuery();
                }
                if (!customer)
                    Audit(c, t, actor, "JobChangeRequested", "In-person cancellation for job #" + job + ", request #" + request);
                t.Commit();
                return request;
            }
        }

        public DataTable TechnicianMaterialReports(int actor)
        {
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction())
            {
                Actor(c, t, actor, false);
                // The missing column is replaced by a check for an existing audit event.
                var rows = Read(c, t, @"SELECT r.RequestID,j.JobID,j.PromiseDate
                    FROM dbo.JobChangeRequests r JOIN dbo.Jobs j ON j.JobID=r.JobID
                    WHERE r.RequestStatus='Pending' AND r.RequestType='Cancellation'
                    AND j.JobStatus='In Progress' AND j.StaffID=@actor
                    AND NOT EXISTS (
                        SELECT 1 FROM dbo.AppSecurityAudit a
                        WHERE a.ActionName='CancellationMaterials'
                        AND a.Detail='Submitted used quantities for request #' + CONVERT(varchar(20),r.RequestID)
                    )
                    ORDER BY r.RequestID", P("@actor", actor));
                t.Commit();
                return rows;
            }
        }

        public void SubmitMaterials(int request, int actor, DataTable used)
        {
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction(IsolationLevel.Serializable))
            {
                Actor(c, t, actor, false);
                var jobs = Read(c, t, @"SELECT j.JobID FROM dbo.JobChangeRequests r WITH(UPDLOCK,HOLDLOCK)
                    JOIN dbo.Jobs j WITH(UPDLOCK,HOLDLOCK) ON j.JobID=r.JobID
                    WHERE r.RequestID=@r AND r.RequestStatus='Pending' AND r.RequestType='Cancellation'
                    AND j.StaffID=@a AND j.JobStatus='In Progress'", P("@r", request), P("@a", actor));
                if (jobs.Rows.Count != 1 || MaterialReportSubmitted(c, t, request))
                    throw new InvalidOperationException("This report has been submitted or is no longer assigned to you.");

                var allocated = Read(c, t, "SELECT ItemID,AllocatedQuantity FROM dbo.JobChangeMaterials WHERE RequestID=@r", P("@r", request));
                if (used == null || used.Rows.Count != allocated.Rows.Count)
                    throw new ArgumentException("Report the quantity used for every allocated item, including zero.");
                foreach (DataRow item in allocated.Rows)
                {
                    var matches = used.Select("ItemID=" + Convert.ToInt32(item["ItemID"]));
                    int quantity;
                    if (matches.Length != 1 || !int.TryParse(Convert.ToString(matches[0]["UsedQuantity"]), out quantity)
                        || quantity < 0 || quantity > Convert.ToInt32(item["AllocatedQuantity"]))
                        throw new ArgumentException("Used quantities must be whole numbers between zero and the allocated quantity.");
                    using (var q = Cmd(c, t, "UPDATE dbo.JobChangeMaterials SET UsedQuantity=@used WHERE RequestID=@r AND ItemID=@i",
                        P("@used", quantity), P("@r", request), P("@i", item["ItemID"])))
                        q.ExecuteNonQuery();
                }
                using (var q = Cmd(c, t, @"UPDATE dbo.JobChangeRequests SET
                    MaterialCharge=COALESCE((SELECT SUM(ROUND(a.Cost * m.UsedQuantity / NULLIF(a.Qty,0),2))
                    FROM dbo.JobChangeMaterials m JOIN
                    (SELECT ItemID,SUM(TotalCost) AS Cost,SUM(QuantityUsed) AS Qty
                     FROM dbo.JobMaterials WHERE JobID=@job GROUP BY ItemID) a
                    ON a.ItemID=m.ItemID WHERE m.RequestID=@r),0) WHERE RequestID=@r",
                    P("@job", jobs.Rows[0]["JobID"]), P("@r", request)))
                    q.ExecuteNonQuery();
                // This event is already part of the existing workflow. It proves
                // submission even when the technician reports zero materials used.
                Audit(c, t, actor, "CancellationMaterials", "Submitted used quantities for request #" + request);
                t.Commit();
            }
        }

        public DataTable ClaimFeeEmail(Guid lease)
        {
            return ExecuteQuery(@";WITH ready AS (SELECT TOP(1) * FROM dbo.JobFeeInvoiceOutbox WITH(UPDLOCK)
                WHERE SentAtUtc IS NULL AND AvailableAtUtc<=SYSUTCDATETIME() ORDER BY AvailableAtUtc,RequestID)
                UPDATE ready SET LeaseID=@lease,AvailableAtUtc=DATEADD(minute,5,SYSUTCDATETIME())
                OUTPUT inserted.RequestID,inserted.Recipient;", P("@lease", lease));
        }

        public void MarkFeeEmailSent(int request, Guid lease)
        {
            ExecuteNonQuery("UPDATE dbo.JobFeeInvoiceOutbox SET SentAtUtc=SYSUTCDATETIME(),LeaseID=NULL WHERE RequestID=@r AND LeaseID=@lease", P("@r", request), P("@lease", lease));
        }

        internal const string FeeProjection = @"SELECT r.RequestID,r.JobID,r.RequestType,c.FullName AS CustomerName,
            r.OriginalTotal,r.CancellationFee,r.ReschedulingFee,r.MaterialCharge,
            r.CancellationFee+r.ReschedulingFee+r.MaterialCharge AS Fee,r.FeePaymentStatus,r.FeePaidAtUtc,r.DecidedAtUtc,
            COALESCE(r.FeeReason,CASE WHEN r.RequestType='Reschedule' THEN 'Late rescheduling charge of 5% of transport cost.'
            ELSE 'Cancellation charge agreed under the original booking terms.' END) AS FeeReason
            FROM dbo.JobChangeRequests r JOIN dbo.Jobs j ON j.JobID=r.JobID JOIN dbo.Customers c ON c.CustomerID=j.CustomerID ";

        public DataTable FeeEmailDetails(int request, Guid lease)
        {
            return ExecuteQuery(FeeProjection + @" JOIN dbo.JobFeeInvoiceOutbox e ON e.RequestID=r.RequestID
                WHERE r.RequestID=@r AND r.RequestStatus='Approved' AND e.LeaseID=@lease AND e.SentAtUtc IS NULL", P("@r", request), P("@lease", lease));
        }
    }
}
