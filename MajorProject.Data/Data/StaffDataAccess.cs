using System;
using System.Data;
using System.Data.SqlClient;

namespace MajorProject.Data.Data
{
    public class StaffDataAccess : DataAccessBase
    {
        private static string GuardLegacyWrite(string sql, bool role, bool target)
        {
            string guard = "SET XACT_ABORT ON; BEGIN TRY BEGIN TRANSACTION; ";
            if (role)
                guard += "IF EXISTS(SELECT 1 FROM dbo.Roles WITH(UPDLOCK,HOLDLOCK) WHERE RoleID=@RoleID AND RoleName='SysAdmin') THROW 51120,'Use the SysAdmin account-management screen for this role.',1; ";
            if (role)
            {
                string exclude = target ? "StaffID<>@StaffID AND " : "";
                string extra = target ? "" : " OR Username=@Username OR EmailAddress=@Username";
                guard += "IF EXISTS(SELECT 1 FROM dbo.Staff WITH(UPDLOCK,HOLDLOCK) WHERE " + exclude + "(Username=@EmailAddress OR EmailAddress=@EmailAddress" + extra + ")) OR EXISTS(SELECT 1 FROM dbo.Customers WITH(UPDLOCK,HOLDLOCK) WHERE EmailAddress=@EmailAddress" + (target ? "" : " OR EmailAddress=@Username") + ") THROW 51122,'The username or email conflicts with another account.',1; ";
            }

            if (target)
                guard += "IF EXISTS(SELECT 1 FROM dbo.Staff s WITH(UPDLOCK,HOLDLOCK) JOIN dbo.Roles r ON r.RoleID=s.RoleID WHERE s.StaffID=@StaffID AND r.RoleName='SysAdmin') THROW 51121,'SysAdmin accounts are protected from ordinary staff management.',1; ";
            return guard + sql + "; COMMIT; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;";
        }

        public StaffDataAccess(string connectionString) : base(connectionString)
        {
        }

        // ============================================================
        // STAFF RETRIEVAL
        // ============================================================
        // Returns all staff for the Manager Staff Management page.
        // RoleName is included so the UI does not need to perform
        // another lookup.
        //
        // Sensitive authentication fields such as PasswordHash,
        // ResetOTP and ResetOTPExpiry are intentionally excluded.
        public DataTable GetAllStaff()
        {
            const string query = @"
                SELECT
                    s.StaffID,
                    s.RoleID,
                    r.RoleName,
                    s.FullName,
                    s.PhoneNumber,
                    s.EmailAddress,
                    s.HireDate,
                    s.Salary,
                    s.EmploymentStatus,
                    s.Username,
                    s.IsAvailable
                FROM Staff AS s
                INNER JOIN Roles AS r
                    ON s.RoleID = r.RoleID
                ORDER BY s.FullName";
            return ExecuteQuery(query);
        }

        // Returns one staff member without exposing password or OTP data.
        public DataTable GetStaffByID(int staffID)
        {
            const string query = @"
                SELECT
                    s.StaffID,
                    s.RoleID,
                    r.RoleName,
                    s.FullName,
                    s.PhoneNumber,
                    s.EmailAddress,
                    s.HireDate,
                    s.Salary,
                    s.EmploymentStatus,
                    s.Username,
                    s.IsAvailable
                FROM Staff AS s
                INNER JOIN Roles AS r
                    ON s.RoleID = r.RoleID
                WHERE s.StaffID = @StaffID";
            return ExecuteQuery(query, new SqlParameter("@StaffID", staffID));
        }

        // ============================================================
        // TECHNICIAN RETRIEVAL
        // ============================================================
        // Returns technicians who are eligible for a new job
        // on the selected date.
        //
        // A technician is eligible when:
        // - They are a Technician.
        // - Their employment status is Active.
        // - They are generally available.
        // - They do not currently have an active job
        //   on the selected date.
        //
        // Completed, Cancelled and Archived jobs do not
        // block the technician from receiving another job.
        //
        // The number of completed jobs on the selected date
        // is returned as CompletedJobs.
        //
        // Technicians are ordered by the lowest workload first.
        public DataTable GetAvailableTechnicians(DateTime date)
        {
            const string query = @"
        SELECT
            s.StaffID,
            s.FullName,

            (
                SELECT COUNT(*)
                FROM Jobs AS completedJobs
                WHERE completedJobs.StaffID = s.StaffID
                  AND completedJobs.PromiseDate IS NOT NULL
                  AND CAST(completedJobs.PromiseDate AS DATE) = @SelectedDate
                  AND completedJobs.JobStatus = 'Completed'
            ) AS CompletedJobs

        FROM Staff AS s

        INNER JOIN Roles AS r
            ON s.RoleID = r.RoleID

        LEFT JOIN AppAccountSecurity AS security
            ON security.StaffID = s.StaffID

        WHERE r.RoleName = 'Technician'
          AND s.EmploymentStatus = 'Active'
          AND s.IsAvailable = 1
          AND COALESCE(security.IsBlocked, 0) = 0

          AND NOT EXISTS
          (
              SELECT 1
              FROM Jobs AS activeJobs
              WHERE activeJobs.StaffID = s.StaffID
                AND activeJobs.PromiseDate IS NOT NULL
                AND CAST(activeJobs.PromiseDate AS DATE) = @SelectedDate
                AND activeJobs.JobStatus NOT IN
                (
                    'Completed',
                    'Cancelled',
                    'Archived'
                )
          )

        ORDER BY
            CompletedJobs ASC,
            s.FullName ASC";
            return ExecuteQuery(query, new SqlParameter("@SelectedDate", date.Date));
        }

        // Returns available technicians with fewer than
        // three non-completed jobs.
        public DataTable GetAvailableTechnicians()
        {
            const string query = @"
                SELECT
                    s.StaffID,
                    s.FullName
                FROM Staff AS s
                INNER JOIN Roles AS r
                    ON s.RoleID = r.RoleID
                LEFT JOIN AppAccountSecurity AS security
                    ON security.StaffID = s.StaffID
                WHERE r.RoleName = 'Technician'
                  AND s.EmploymentStatus = 'Active'
                  AND s.IsAvailable = 1
                  AND COALESCE(security.IsBlocked, 0) = 0
                  AND
                  (
                      SELECT COUNT(*)
                      FROM Jobs AS j
                      WHERE j.StaffID = s.StaffID
                        AND j.JobStatus <> 'Completed'
                  ) < 3
                ORDER BY s.FullName";
            return ExecuteQuery(query);
        }

        // ============================================================
        // STAFF INSERT
        // ============================================================
        public int InsertStaff(int roleID, string fullName, string phone, string email, decimal salary, string username, string passwordHash)
        {
            const string query = @"
                INSERT INTO Staff
                (
                    RoleID,
                    FullName,
                    PhoneNumber,
                    EmailAddress,
                    HireDate,
                    Salary,
                    EmploymentStatus,
                    Username,
                    PasswordHash,
                    IsAvailable
                )
                VALUES
                (
                    @RoleID,
                    @FullName,
                    @PhoneNumber,
                    @EmailAddress,
                    @HireDate,
                    @Salary,
                    'Active',
                    @Username,
                    @PasswordHash,
                    1
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            object result = ExecuteScalar(GuardLegacyWrite(query, true, false), new SqlParameter("@RoleID", roleID), new SqlParameter("@FullName", fullName), new SqlParameter("@PhoneNumber", phone), new SqlParameter("@EmailAddress", (object)email ?? DBNull.Value), new SqlParameter("@HireDate", DateTime.Now), new SqlParameter("@Salary", salary), new SqlParameter("@Username", username), new SqlParameter("@PasswordHash", passwordHash));
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // ============================================================
        // STAFF DETAILS UPDATE
        // ============================================================
        // Updates normal staff information.
        //
        // Password changes are deliberately NOT handled here.
        public int UpdateStaffDetails(int staffID, int roleID, string fullName, string phone, string email, decimal salary)
        {
            const string query = @"
                UPDATE Staff
                SET
                    RoleID = @RoleID,
                    FullName = @FullName,
                    PhoneNumber = @PhoneNumber,
                    EmailAddress = @EmailAddress,
                    Salary = @Salary
                WHERE StaffID = @StaffID";
            return ExecuteNonQuery(GuardLegacyWrite(query, true, true), new SqlParameter("@RoleID", roleID), new SqlParameter("@FullName", fullName), new SqlParameter("@PhoneNumber", phone), new SqlParameter("@EmailAddress", (object)email ?? DBNull.Value), new SqlParameter("@Salary", salary), new SqlParameter("@StaffID", staffID));
        }

        // ============================================================
        // PASSWORD
        // ============================================================
        // Updates only the password hash.
        public int UpdatePasswordHash(int staffID, string passwordHash)
        {
            const string query = @"
                UPDATE Staff
                SET PasswordHash = @PasswordHash
                WHERE StaffID = @StaffID; UPDATE dbo.AppAccountSecurity SET SecurityVersion=SecurityVersion+1,UpdatedAtUtc=SYSUTCDATETIME() WHERE StaffID=@StaffID; UPDATE dbo.AppUserSessions SET EndedAtUtc=SYSUTCDATETIME(),EndReason='Password changed' WHERE StaffID=@StaffID AND EndedAtUtc IS NULL";
            return ExecuteNonQuery(GuardLegacyWrite(query, false, true), new SqlParameter("@PasswordHash", passwordHash), new SqlParameter("@StaffID", staffID));
        }

        // ============================================================
        // EMPLOYMENT STATUS
        // ============================================================
        // Changes the employment status.
        //
        // Example:
        // Active -> Inactive
        // Inactive -> Active
        public int UpdateEmploymentStatus(int staffID, string employmentStatus)
        {
            const string query = @"
                UPDATE Staff
                SET EmploymentStatus = @EmploymentStatus,
                    IsAvailable = CASE WHEN @EmploymentStatus = 'Active' THEN IsAvailable ELSE 0 END
                WHERE StaffID = @StaffID";
            return ExecuteNonQuery(GuardLegacyWrite(query, false, true), new SqlParameter("@EmploymentStatus", employmentStatus), new SqlParameter("@StaffID", staffID));
        }

        // ============================================================
        // TECHNICIAN AVAILABILITY
        // ============================================================
        public int UpdateAvailability(int staffID, bool isAvailable)
        {
            const string query = @"
                UPDATE Staff
                SET IsAvailable = @IsAvailable
                WHERE StaffID = @StaffID
                  AND EmploymentStatus = 'Active'
                  AND COALESCE((SELECT IsBlocked FROM dbo.AppAccountSecurity WHERE StaffID = @StaffID), 0) = 0";
            return ExecuteNonQuery(GuardLegacyWrite(query, false, true), new SqlParameter("@IsAvailable", isAvailable), new SqlParameter("@StaffID", staffID));
        }

        // ============================================================
        // ARCHIVE
        // ============================================================
        // Archives a staff member without deleting the database record.
        //
        // The employee becomes inactive and unavailable.
        public int ArchiveStaff(int staffID)
        {
            const string query = @"
                UPDATE Staff
                SET
                    EmploymentStatus = 'Inactive',
                    IsAvailable = 0
                WHERE StaffID = @StaffID";
            return ExecuteNonQuery(GuardLegacyWrite(query, false, true), new SqlParameter("@StaffID", staffID));
        }

        // ============================================================
        // SEARCH
        // ============================================================
        public DataTable SearchStaffByName(string name)
        {
            const string query = @"
                SELECT
                    s.StaffID,
                    s.RoleID,
                    r.RoleName,
                    s.FullName,
                    s.PhoneNumber,
                    s.EmailAddress,
                    s.HireDate,
                    s.Salary,
                    s.EmploymentStatus,
                    s.Username,
                    s.IsAvailable
                FROM Staff AS s
                INNER JOIN Roles AS r
                    ON s.RoleID = r.RoleID
                WHERE s.FullName LIKE @Name
                ORDER BY s.FullName";
            return ExecuteQuery(query, new SqlParameter("@Name", "%" + (name ?? string.Empty) + "%"));
        }

        public DataTable SearchStaffByPhone(string phone)
        {
            const string query = @"
                SELECT
                    s.StaffID,
                    s.RoleID,
                    r.RoleName,
                    s.FullName,
                    s.PhoneNumber,
                    s.EmailAddress,
                    s.HireDate,
                    s.Salary,
                    s.EmploymentStatus,
                    s.Username,
                    s.IsAvailable
                FROM Staff AS s
                INNER JOIN Roles AS r
                    ON s.RoleID = r.RoleID
                WHERE s.PhoneNumber LIKE @Phone
                ORDER BY s.FullName";
            return ExecuteQuery(query, new SqlParameter("@Phone", "%" + (phone ?? string.Empty) + "%"));
        }

        public DataTable GetAvailableTechniciansForJob(DateTime date, int jobID)
        {
            if (jobID <= 0)
            {
                throw new ArgumentException("Invalid job ID.", nameof(jobID));
            }

            const string query = @"
        SELECT
            s.StaffID,
            s.FullName
        FROM Staff AS s
        INNER JOIN Roles AS r
            ON s.RoleID = r.RoleID
        LEFT JOIN AppAccountSecurity AS security
            ON security.StaffID = s.StaffID
        WHERE r.RoleName = 'Technician'
          AND s.EmploymentStatus = 'Active'
          AND s.IsAvailable = 1
          AND COALESCE(security.IsBlocked, 0) = 0
          AND NOT EXISTS
          (
              SELECT 1
              FROM Jobs AS j
              WHERE j.StaffID = s.StaffID
                AND j.PromiseDate IS NOT NULL
                AND CAST(j.PromiseDate AS DATE) = @SelectedDate
                AND j.JobID <> @JobID
                AND j.JobStatus NOT IN
                (
                    'Completed',
                    'Cancelled',
                    'Archived'
                )
          )
        ORDER BY s.FullName;";
            return ExecuteQuery(query, new SqlParameter("@SelectedDate", date.Date), new SqlParameter("@JobID", jobID));
        }
    }
}
