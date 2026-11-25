using System;
using System.Data;
using System.Data.SqlClient;

namespace MajorProject.Data.Data
{
    // All decisions are transactional. The original quote is never rewritten as a cancellation fee.
    public sealed partial class JobChangeDataAccess : DataAccessBase
    {
        public JobChangeDataAccess(string cs) : base(cs)
        {
        }

        private static SqlParameter P(string name, object value) => new SqlParameter(name, value ?? DBNull.Value);
        private static SqlCommand Cmd(SqlConnection c, SqlTransaction t, string sql, params SqlParameter[] p)
        {
            var q = new SqlCommand(sql, c, t);
            q.Parameters.AddRange(p);
            return q;
        }

        private static DataTable Read(SqlConnection c, SqlTransaction t, string sql, params SqlParameter[] p)
        {
            var d = new DataTable();
            using (var q = Cmd(c, t, sql, p))
            using (var a = new SqlDataAdapter(q))
                a.Fill(d);
            return d;
        }

        private static void Actor(SqlConnection c, SqlTransaction t, int actor, bool manager, bool allowAdministrator = false)
        {
            using (var q = Cmd(c, t, @"SELECT COUNT(*) FROM dbo.Staff s WITH(UPDLOCK,HOLDLOCK)
              JOIN dbo.Roles r ON r.RoleID=s.RoleID LEFT JOIN dbo.AppAccountSecurity x WITH(UPDLOCK,HOLDLOCK) ON x.StaffID=s.StaffID
              WHERE s.StaffID=@a AND s.EmploymentStatus='Active' AND COALESCE(x.IsBlocked,0)=0
              AND (r.RoleName=@role OR (@admin=1 AND r.RoleName='Administrator'))", P("@admin", allowAdministrator), P("@a", actor), P("@role", manager ? "Manager" : "Technician")))
                if (Convert.ToInt32(q.ExecuteScalar()) != 1)
                    throw new UnauthorizedAccessException("An active, unblocked account with the required role is needed.");
        }

        private static void Audit(SqlConnection c, SqlTransaction t, int actor, string action, string detail)
        {
            using (var q = Cmd(c, t, "INSERT dbo.AppSecurityAudit(ActorStaffID,ActionName,Detail,IPAddress,OccurredAtUtc) VALUES(@a,@n,@d,'Application',SYSUTCDATETIME())", P("@a", actor), P("@n", action), P("@d", detail.Length > 500 ? detail.Substring(0, 500) : detail)))
                q.ExecuteNonQuery();
        }

        // The technician's existing audit event is the durable indication that the
        // report was submitted. AwaitingMaterials is NEVER stored in the database.
        // Do not purge these audit events while requests still need this status.
        private static bool MaterialReportSubmitted(SqlConnection c, SqlTransaction t, int request)
        {
            const string sql = @"SELECT TOP (1) 1 FROM dbo.AppSecurityAudit
                WHERE ActionName='CancellationMaterials' AND Detail=@detail";
            using (var q = Cmd(c, t, sql,
                P("@detail", "Submitted used quantities for request #" + request)))
                return q.ExecuteScalar() != null;
        }

        private static void AddLocalAwaitingMaterials(DataTable table)
        {
            // This DataColumn exists only in memory and is safe for ASP.NET Eval().
            table.Columns.Add("AwaitingMaterials", typeof(bool));
            foreach (DataRow row in table.Rows)
            {
                bool cancelling = string.Equals(Convert.ToString(row["RequestType"]),
                    "Cancellation", StringComparison.OrdinalIgnoreCase);
                bool pending = string.Equals(Convert.ToString(row["RequestStatus"]),
                    "Pending", StringComparison.OrdinalIgnoreCase);
                bool started = string.Equals(Convert.ToString(row["JobStatus"]),
                    "In Progress", StringComparison.OrdinalIgnoreCase);
                bool submitted = row["MaterialReportSubmitted"] != DBNull.Value
                    && Convert.ToBoolean(row["MaterialReportSubmitted"]);
                row["AwaitingMaterials"] = cancelling && pending && started && !submitted;
            }
            table.Columns.Remove("MaterialReportSubmitted");
        }

        public DataTable Materials(int job, int actor, bool manager)
        {
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction())
            {
                Actor(c, t, actor, manager);
                var d = Read(c, t, @"SELECT m.ItemID,i.ItemName,SUM(m.QuantityUsed) AS AllocatedQuantity,CAST(0 AS int) AS UsedQuantity
              FROM dbo.JobMaterials m JOIN dbo.InventoryItems i ON i.ItemID=m.ItemID JOIN dbo.Jobs j ON j.JobID=m.JobID
              WHERE m.JobID=@j AND (@manager=1 OR j.StaffID=@a) GROUP BY m.ItemID,i.ItemName", P("@j", job), P("@a", actor), P("@manager", manager));
                t.Commit();
                return d;
            }
        }

        public int Request(int job, int actor, string type, string reason, DateTime? date, DataTable used, bool manager = false)
        {
            if (!manager)
                throw new UnauthorizedAccessException("Technicians can report materials on cancellation notices. They cannot request a booking change.");
            if (type == "Cancellation")
                return RequestCancellation(job, actor, false, reason);
            if (type != "Reschedule")
                throw new ArgumentException("Choose cancellation or reschedule.");
            if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length < 10 || reason.Length > 1000)
                throw new ArgumentException("Give a reason between 10 and 1,000 characters.");
            if (type == "Reschedule" && (!date.HasValue || date.Value.Date < DateTime.Today))
                throw new ArgumentException("Choose today or a future date.");
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction(IsolationLevel.Serializable))
            {
                Actor(c, t, actor, manager);
                var jobs = Read(c, t, "SELECT * FROM dbo.Jobs WITH(UPDLOCK,HOLDLOCK) WHERE JobID=@j", P("@j", job));
                if (jobs.Rows.Count != 1)
                    throw new ArgumentException("Job not found.");
                var row = jobs.Rows[0];
                string status = Convert.ToString(row["JobStatus"]);
                if (type == "Reschedule" && status != "Pending")
                    throw new InvalidOperationException("Only jobs that have not started can request rescheduling.");
                if (Read(c, t, "SELECT RequestID FROM dbo.JobChangeRequests WITH(UPDLOCK,HOLDLOCK) WHERE JobID=@j AND RequestStatus='Pending'", P("@j", job)).Rows.Count > 0)
                    throw new InvalidOperationException("This job already has a pending request. A manager must review it first.");
                decimal total = Convert.ToDecimal(row["GrandTotal"]);
                int id;
                using (var q = Cmd(c, t, @"INSERT dbo.JobChangeRequests(JobID,RequestedBy,RequestType,Reason,RequestedDate,OriginalTotal,CancellationFee)
                  OUTPUT INSERTED.RequestID VALUES(@j,@a,@type,@reason,@date,@total,0)", P("@j", job), P("@a", actor), P("@type", type), P("@reason", reason.Trim()), P("@date", date), P("@total", total)))
                    id = Convert.ToInt32(q.ExecuteScalar());

                Audit(c, t, actor, "JobChangeRequested", "Job #" + job + "; request #" + id + "; " + type);
                t.Commit();
                return id;
            }
        }

        public int RequestCustomerReschedule(int job, int customer, DateTime requested, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length < 10 || reason.Length > 1000)
                throw new ArgumentException("Give a reason between 10 and 1,000 characters.");
            using (var connection = CreateOpenConnection())
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                // South African calendar days determine the fee at submission, even if reviewed later.
                const string sql = @"
                    DECLARE @today date = DATEADD(hour,2,SYSUTCDATETIME()), @promise date, @total decimal(18,2), @transport decimal(18,2);
                    SELECT @promise=j.PromiseDate,@total=j.GrandTotal,@transport=j.TransportCost
                    FROM dbo.Jobs j WITH(UPDLOCK,HOLDLOCK) JOIN dbo.Customers c ON c.CustomerID=j.CustomerID
                    LEFT JOIN dbo.AppAccountSecurity x ON x.CustomerID=c.CustomerID
                    WHERE j.JobID=@job AND j.CustomerID=@customer AND j.JobStatus='Pending'
                    AND c.AccountStatus='Active' AND COALESCE(x.IsBlocked,0)=0;
                    IF @promise IS NULL THROW 51202,'Select one of your pending bookings.',1;
                    IF @date<@today OR @date=@promise THROW 51203,'Choose a different date from today onwards.',1;
                    IF EXISTS(SELECT 1 FROM dbo.JobChangeRequests WITH(UPDLOCK,HOLDLOCK) WHERE JobID=@job AND RequestStatus='Pending')
                        THROW 51204,'This booking already has a request awaiting review.',1;
                    INSERT dbo.JobChangeRequests(JobID,RequestedByCustomerID,RequestType,Reason,RequestedDate,OriginalTotal,CancellationFee,ReschedulingFee)
                    OUTPUT INSERTED.RequestID
                    VALUES(@job,@customer,'Reschedule',@reason,@date,@total,0,
                        CASE WHEN DATEDIFF(day,@today,@promise) BETWEEN 0 AND 1 THEN ROUND(@transport*0.05,2) ELSE 0 END);";
                using (var command = Cmd(connection, transaction, sql, P("@job", job), P("@customer", customer), P("@date", requested.Date), P("@reason", reason.Trim())))
                {
                    int id = Convert.ToInt32(command.ExecuteScalar());
                    transaction.Commit();
                    return id;
                }
            }
        }

        public DataTable CustomerRequests(int customer)
        {
            // Compute the column in memory; do not query a physical AwaitingMaterials column.
            var table = ExecuteQuery(@"SELECT r.JobID,r.RequestType,r.RequestedDate,
                r.RequestStatus,r.ReschedulingFee+r.CancellationFee+r.MaterialCharge AS FeeIfApproved,
                r.DecisionNote,j.JobStatus,
                CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM dbo.AppSecurityAudit audit
                    WHERE audit.ActionName='CancellationMaterials'
                    AND audit.Detail='Submitted used quantities for request #' + CONVERT(varchar(20),r.RequestID)
                ) THEN 1 ELSE 0 END AS bit) AS MaterialReportSubmitted
                FROM dbo.JobChangeRequests r JOIN dbo.Jobs j ON j.JobID=r.JobID
                WHERE j.CustomerID=@customer AND
                    (r.RequestedByCustomerID=@customer OR r.RequestType='Cancellation')
                ORDER BY r.RequestID DESC", P("@customer", customer));
            AddLocalAwaitingMaterials(table);
            return table;
        }

        public DataTable Requests(int actor, bool manager)
        {
            if (!manager)
                throw new UnauthorizedAccessException("Booking requests and fees are restricted to the review team.");
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction())
            {
                Actor(c, t, actor, manager, manager);
                var d = Read(c, t, @"SELECT r.*,COALESCE(s.FullName,c.FullName) AS RequestedByName,j.JobStatus,
                    CAST(CASE WHEN EXISTS (
                        SELECT 1 FROM dbo.AppSecurityAudit audit
                        WHERE audit.ActionName='CancellationMaterials'
                        AND audit.Detail='Submitted used quantities for request #' + CONVERT(varchar(20),r.RequestID)
                    ) THEN 1 ELSE 0 END AS bit) AS MaterialReportSubmitted
                    FROM dbo.JobChangeRequests r
                    JOIN dbo.Jobs j ON j.JobID=r.JobID
                    LEFT JOIN dbo.Staff s ON s.StaffID=r.RequestedBy
                    LEFT JOIN dbo.Customers c ON c.CustomerID=r.RequestedByCustomerID
                    WHERE (@m=1 AND (r.RequestType='Reschedule'
                        OR EXISTS(SELECT 1 FROM dbo.Staff a JOIN dbo.Roles role
                                  ON role.RoleID=a.RoleID WHERE a.StaffID=@a AND role.RoleName='Manager')))
                        OR r.RequestedBy=@a
                    ORDER BY CASE WHEN r.RequestStatus='Pending' THEN 0 ELSE 1 END,r.RequestID DESC",
                    P("@m", manager), P("@a", actor));
                AddLocalAwaitingMaterials(d);
                t.Commit();
                return d;
            }
        }

        public DataTable RequestMaterials(int actor, int request)
        {
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction())
            {
                Actor(c, t, actor, true);
                var d = Read(c, t, "SELECT i.ItemName,m.AllocatedQuantity,m.UsedQuantity,m.AllocatedQuantity-m.UsedQuantity AS ReturnQuantity FROM dbo.JobChangeMaterials m JOIN dbo.InventoryItems i ON i.ItemID=m.ItemID WHERE RequestID=@r", P("@r", request));
                t.Commit();
                return d;
            }
        }

        public DataTable ReassignmentQueue(int actor, bool allOpen = false)
        {
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction())
            {
                Actor(c, t, actor, true);
                var d = Read(c, t, @"SELECT j.JobID,j.JobStatus,j.PromiseDate,j.StaffID AS PreviousStaffID,s.FullName AS PreviousTechnician,
          CASE WHEN COALESCE(x.IsBlocked,0)=1 THEN 'Account blocked' WHEN s.StaffID IS NULL THEN 'Unassigned' WHEN s.EmploymentStatus<>'Active' OR r.RoleName<>'Technician' THEN 'Employment inactive or role changed' ELSE 'Assigned' END AS Reason
          FROM dbo.Jobs j LEFT JOIN dbo.Staff s ON s.StaffID=j.StaffID LEFT JOIN dbo.Roles r ON r.RoleID=s.RoleID LEFT JOIN dbo.AppAccountSecurity x ON x.StaffID=s.StaffID
          WHERE j.JobStatus IN('Pending','In Progress') AND (@all=1 OR s.StaffID IS NULL OR s.EmploymentStatus<>'Active' OR r.RoleName<>'Technician' OR COALESCE(x.IsBlocked,0)=1) ORDER BY j.PromiseDate,j.JobID", P("@all", allOpen));
                t.Commit();
                return d;
            }
        }

        public void Decide(int request, int actor, bool approve, string note, int technician, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(note) || note.Length > 1000)
                throw new ArgumentException("Enter a decision note (up to 1,000 characters).");
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction(IsolationLevel.Serializable))
            {
                if (approve && technician > 0)
                    JobDataAccess.AcquireTechnicianDateLock(c, t, date.Date);
                Actor(c, t, actor, true, true);
                var d = Read(c, t, "SELECT * FROM dbo.JobChangeRequests WITH(UPDLOCK,HOLDLOCK) WHERE RequestID=@r AND RequestStatus='Pending'", P("@r", request));
                if (d.Rows.Count != 1)
                    throw new InvalidOperationException("This request has already been reviewed. Refresh the list.");
                var r = d.Rows[0];
                int job = Convert.ToInt32(r["JobID"]);
                if (Convert.ToString(r["RequestType"]) == "Cancellation")
                    Actor(c, t, actor, true);
                var jobs = Read(c, t, "SELECT PromiseDate,JobStatus,GrandTotal FROM dbo.Jobs WITH(UPDLOCK,HOLDLOCK) WHERE JobID=@j", P("@j", job));
                string status = Convert.ToString(jobs.Rows[0]["JobStatus"]);
                if (approve && status != "Pending" && status != "In Progress")
                    throw new InvalidOperationException("This job is no longer open. Reject the stale request.");
                if (approve && Convert.ToString(r["RequestType"]) == "Cancellation")
                {
                    if (status == "In Progress" && !MaterialReportSubmitted(c, t, request))
                        throw new InvalidOperationException("The assigned technician must submit the materials used before approval.");
                    if (Read(c, t, "SELECT TOP(1) JobID FROM dbo.Payments WITH(UPDLOCK,HOLDLOCK) WHERE JobID=@j AND PaymentStatus='Paid'", P("@j", job)).Rows.Count > 0)
                        throw new InvalidOperationException("The original invoice is already paid. Reconcile that payment before approving a cancellation invoice.");
                    if (Convert.ToDecimal(jobs.Rows[0]["GrandTotal"]) != Convert.ToDecimal(r["OriginalTotal"]))
                        throw new InvalidOperationException("The quoted total changed. Reject and request a new review.");
                    if (Read(c, t, @"SELECT COALESCE(a.ItemID,b.ItemID) FROM (SELECT ItemID,SUM(QuantityUsed) Qty FROM dbo.JobMaterials WITH(UPDLOCK,HOLDLOCK) WHERE JobID=@j GROUP BY ItemID) a FULL JOIN (SELECT ItemID,AllocatedQuantity FROM dbo.JobChangeMaterials WHERE RequestID=@r) b ON a.ItemID=b.ItemID WHERE a.ItemID IS NULL OR b.ItemID IS NULL OR a.Qty<>b.AllocatedQuantity", P("@j", job), P("@r", request)).Rows.Count > 0)
                        throw new InvalidOperationException("Material allocation changed. Reject and request a fresh materials record.");
                    using (var q = Cmd(c, t, @"UPDATE i SET QuantityInStock=ISNULL(i.QuantityInStock,0)+m.AllocatedQuantity-m.UsedQuantity,UpdatedAt=GETDATE()
                     FROM dbo.InventoryItems i JOIN dbo.JobChangeMaterials m ON m.ItemID=i.ItemID WHERE m.RequestID=@r;
                     INSERT dbo.InventoryTransactions(ItemID,JobID,TransactionType,Quantity,TransactionDate)
                     SELECT ItemID,@j,'IN',AllocatedQuantity-UsedQuantity,GETDATE() FROM dbo.JobChangeMaterials WHERE RequestID=@r AND AllocatedQuantity>UsedQuantity;
                     UPDATE dbo.Jobs SET JobStatus='Cancelled',UpdatedAt=GETDATE() WHERE JobID=@j;", P("@r", request), P("@j", job)))
                        q.ExecuteNonQuery();
                    using (var q = Cmd(c, t, @"INSERT dbo.JobAllocationNotifications(JobID,StaffID,Message)
                        SELECT JobID,StaffID,'Cancellation approved. This job is no longer scheduled.' FROM dbo.Jobs WHERE JobID=@j AND StaffID IS NOT NULL", P("@j", job)))
                        q.ExecuteNonQuery();
                }
                else if (approve)
                {
                    if (status != "Pending")
                        throw new InvalidOperationException("The job has started. Rescheduling is no longer available.");
                    if (r["RequestedByCustomerID"] != DBNull.Value && date.Date == Convert.ToDateTime(jobs.Rows[0]["PromiseDate"]).Date)
                        throw new ArgumentException("Choose a new promise date for this booking.");
                    Assign(c, t, job, technician, date);
                }

                using (var q = Cmd(c, t, "UPDATE dbo.JobChangeRequests SET RequestStatus=@s,DecidedBy=@a,DecisionNote=@n,DecidedAtUtc=SYSUTCDATETIME() WHERE RequestID=@r", P("@s", approve ? "Approved" : "Rejected"), P("@a", actor), P("@n", note.Trim()), P("@r", request)))
                    q.ExecuteNonQuery();
                if (approve && (Convert.ToString(r["RequestType"]) == "Cancellation" || Convert.ToDecimal(r["ReschedulingFee"]) > 0))
                {
                    using (var q = Cmd(c, t, @"INSERT dbo.JobFeeInvoiceOutbox(RequestID,Recipient)
                        SELECT @r,c.EmailAddress FROM dbo.Jobs j JOIN dbo.Customers c ON c.CustomerID=j.CustomerID
                        WHERE j.JobID=@j AND NULLIF(LTRIM(RTRIM(c.EmailAddress)),'') IS NOT NULL", P("@r", request), P("@j", job)))
                        q.ExecuteNonQuery();
                }
                if (!approve && status == "In Progress" && Convert.ToString(r["RequestType"]) == "Cancellation")
                {
                    using (var q = Cmd(c, t, @"INSERT dbo.JobAllocationNotifications(JobID,StaffID,Message)
                        SELECT JobID,StaffID,'The cancellation was declined. This job remains assigned to you.' FROM dbo.Jobs
                        WHERE JobID=@job AND StaffID IS NOT NULL", P("@job", job)))
                        q.ExecuteNonQuery();
                }
                Audit(c, t, actor, "JobChangeDecision", "Job #" + job + "; request #" + request + "; " + (approve ? "Approved" : "Rejected"));
                t.Commit();
            }
        }

        private static void Assign(SqlConnection c, SqlTransaction t, int job, int technician, DateTime date)
        {
            if (date.Date < DateTime.Today)
                throw new ArgumentException("Choose today or a future date.");
            // Same application lock as booking/rescheduling is supplemented by serializable schedule locks.
            using (var q = Cmd(c, t, @"IF NOT EXISTS(SELECT 1 FROM dbo.Staff s WITH(UPDLOCK,HOLDLOCK) JOIN dbo.Roles r ON r.RoleID=s.RoleID LEFT JOIN dbo.AppAccountSecurity x WITH(UPDLOCK,HOLDLOCK) ON x.StaffID=s.StaffID WHERE s.StaffID=@s AND s.EmploymentStatus='Active' AND s.IsAvailable=1 AND r.RoleName='Technician' AND COALESCE(x.IsBlocked,0)=0) THROW 51200,'Choose an active, available and unblocked technician.',1;
             IF EXISTS(SELECT 1 FROM dbo.Jobs WITH(UPDLOCK,HOLDLOCK) WHERE StaffID=@s AND JobID<>@j AND PromiseDate>=@date AND PromiseDate<DATEADD(day,1,@date) AND JobStatus NOT IN('Completed','Cancelled','Archived')) THROW 51201,'This technician already has a job on that date.',1;
             UPDATE dbo.Jobs SET StaffID=@s,PromiseDate=@date,UpdatedAt=GETDATE() WHERE JobID=@j AND JobStatus IN('Pending','In Progress');", P("@s", technician), P("@j", job), P("@date", date.Date)))
                q.ExecuteNonQuery();
        }

        public void Reassign(int job, int actor, int technician, DateTime date, string note, int? expectedStaff = null)
        {
            if (string.IsNullOrWhiteSpace(note) || note.Length > 1000)
                throw new ArgumentException("Record a handover reason (up to 1,000 characters).");
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction(IsolationLevel.Serializable))
            {
                JobDataAccess.AcquireTechnicianDateLock(c, t, date.Date);
                Actor(c, t, actor, true);
                var d = Read(c, t, "SELECT StaffID,JobStatus FROM dbo.Jobs WITH(UPDLOCK,HOLDLOCK) WHERE JobID=@j", P("@j", job));
                if (d.Rows.Count != 1 || !(Convert.ToString(d.Rows[0]["JobStatus"]) == "Pending" || Convert.ToString(d.Rows[0]["JobStatus"]) == "In Progress"))
                    throw new InvalidOperationException("Only open jobs can be reassigned.");
                var previous = d.Rows[0]["StaffID"];
                int previousId = previous == DBNull.Value ? 0 : Convert.ToInt32(previous);
                if (expectedStaff.HasValue && previousId != expectedStaff.Value)
                    throw new InvalidOperationException("This assignment has changed. Refresh the queue.");
                if (previousId == technician)
                    throw new ArgumentException("Choose a different technician for the handover.");
                if (Read(c, t, "SELECT RequestID FROM dbo.JobChangeRequests WITH(UPDLOCK,HOLDLOCK) WHERE JobID=@j AND RequestType='Cancellation' AND RequestStatus='Pending'", P("@j", job)).Rows.Count > 0)
                    throw new InvalidOperationException("Resolve the open cancellation before handing this job over.");
                Assign(c, t, job, technician, date);
                using (var q = Cmd(c, t, "UPDATE dbo.JobChangeRequests SET RequestStatus='Superseded',DecidedBy=@a,DecisionNote='Superseded by manager reassignment',DecidedAtUtc=SYSUTCDATETIME() WHERE JobID=@j AND RequestStatus='Pending'", P("@j", job), P("@a", actor)))
                    q.ExecuteNonQuery();
                using (var q = Cmd(c, t, "INSERT dbo.JobAssignmentHistory(JobID,PreviousStaffID,NewStaffID,ChangedBy,PromiseDate,HandoverNote) VALUES(@j,@old,@new,@a,@date,@note)", P("@j", job), P("@old", d.Rows[0]["StaffID"]), P("@new", technician), P("@a", actor), P("@date", date.Date), P("@note", note.Trim())))
                    q.ExecuteNonQuery();
                using (var q = Cmd(c, t, @"UPDATE dbo.JobAllocationNotifications SET HandoverNote=@note
                 WHERE NotificationID=(SELECT MAX(NotificationID) FROM dbo.JobAllocationNotifications WHERE JobID=@j AND StaffID=@new)", P("@note", note.Trim()), P("@j", job), P("@new", technician)))
                    q.ExecuteNonQuery();
                Audit(c, t, actor, "JobReassigned", "Job #" + job + "; technician " + d.Rows[0]["StaffID"] + " -> " + technician + ". Handover note recorded in assignment history.");
                t.Commit();
            }
        }

        public DataTable Notifications(int actor)
        {
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction())
            {
                Actor(c, t, actor, false);
                var d = Read(c, t, "SELECT TOP(50) NotificationID,JobID,Message,HandoverNote,CreatedAtUtc FROM dbo.JobAllocationNotifications WHERE StaffID=@a AND ReadAtUtc IS NULL ORDER BY NotificationID DESC", P("@a", actor));
                t.Commit();
                return d;
            }
        }

        public void ReadNotification(int actor, long id)
        {
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction())
            {
                Actor(c, t, actor, false);
                using (var q = Cmd(c, t, "UPDATE dbo.JobAllocationNotifications SET ReadAtUtc=SYSUTCDATETIME() WHERE StaffID=@a AND NotificationID=@id AND ReadAtUtc IS NULL", P("@a", actor), P("@id", id)))
                    q.ExecuteNonQuery();
                t.Commit();
            }
        }

        public DataTable ClaimBookingEmail(Guid lease)
        {
            // A crashed sender releases its claim automatically after five minutes.
            return ExecuteQuery(@";WITH nextEmail AS (
                SELECT TOP (1) * FROM dbo.BookingEmailOutbox WITH (UPDLOCK)
                WHERE SentAtUtc IS NULL AND AvailableAtUtc<=SYSUTCDATETIME() ORDER BY AvailableAtUtc,EmailID)
                UPDATE nextEmail SET LeaseID=@lease,AvailableAtUtc=DATEADD(minute,5,SYSUTCDATETIME())
                OUTPUT inserted.EmailID,inserted.JobID,inserted.Recipient,inserted.PromiseDate,inserted.IsNewBooking;", P("@lease", lease));
        }

        public void MarkBookingEmailSent(long id, Guid lease)
        {
            ExecuteNonQuery(@"UPDATE dbo.BookingEmailOutbox SET SentAtUtc=SYSUTCDATETIME(),LeaseID=NULL
                WHERE EmailID=@id AND LeaseID=@lease AND SentAtUtc IS NULL", P("@id", id), P("@lease", lease));
        }

        public DataTable CustomerFees(int customer)
        {
            return ExecuteQuery(FeeProjection + @" LEFT JOIN dbo.AppAccountSecurity x ON x.CustomerID=c.CustomerID
          WHERE c.CustomerID=@c AND c.AccountStatus='Active' AND COALESCE(x.IsBlocked,0)=0 AND r.ReschedulingFee+r.CancellationFee+r.MaterialCharge>0 AND r.RequestStatus='Approved' ORDER BY r.RequestID DESC", P("@c", customer));
        }

        public void PayDemoFee(int customer, int request)
        {
            using (var c = CreateOpenConnection())
            using (var t = c.BeginTransaction(IsolationLevel.Serializable))
            {
                using (var q = Cmd(c, t, @"UPDATE r SET FeePaymentStatus='DemoPaid',FeePaidAtUtc=SYSUTCDATETIME()
                 FROM dbo.JobChangeRequests r WITH(UPDLOCK,HOLDLOCK) JOIN dbo.Jobs j ON j.JobID=r.JobID JOIN dbo.Customers c ON c.CustomerID=j.CustomerID LEFT JOIN dbo.AppAccountSecurity x ON x.CustomerID=c.CustomerID
                 WHERE r.RequestID=@r AND c.CustomerID=@c AND c.AccountStatus='Active' AND COALESCE(x.IsBlocked,0)=0 AND r.CancellationFee+r.ReschedulingFee+r.MaterialCharge>0 AND r.RequestStatus='Approved' AND r.FeePaymentStatus='Unpaid'; SELECT @@ROWCOUNT;", P("@r", request), P("@c", customer)))
                    if (Convert.ToInt32(q.ExecuteScalar()) != 1)
                        throw new InvalidOperationException("The fee is already settled or unavailable. Refresh the list.");
                using (var q = Cmd(c, t, "INSERT dbo.AppSecurityAudit(TargetCustomerID,ActionName,Detail,IPAddress,OccurredAtUtc) VALUES(@c,'DemoJobChangePayment',@d,'Application',SYSUTCDATETIME())", P("@c", customer), P("@d", "Demo fee settled for request #" + request + ". No money moved.")))
                    q.ExecuteNonQuery();
                t.Commit();
            }
        }
    }
}
