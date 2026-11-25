using System;
using System.Data;
using System.Data.SqlClient;

namespace MajorProject.Data.Data
{
    /// <summary>
    /// Handles authentication-related database operations.
    /// Shared between Desktop (WinForms) and Web (ASP.NET) applications.
    /// </summary>
    public class AuthenticationDataAccess : DataAccessBase
    {
        public AuthenticationDataAccess(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Tests the database connection.
        /// </summary>
        public void TestConnection()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
            }
        }

        // ============================================================
        // STAFF AUTHENTICATION
        // ============================================================
        /// <summary>
        /// Retrieves a staff account using the unique username.
        /// Only active staff accounts are returned.
        /// </summary>
        public DataTable GetStaffForAuthentication(string username)
        {
            const string query = @"
                SELECT
                    s.StaffID,
                    s.RoleID,
                    s.FullName,
                    s.PhoneNumber,
                    s.EmailAddress,
                    s.HireDate,
                    s.Salary,
                    s.EmploymentStatus,
                    s.Username,
                    s.PasswordHash,
                    s.IsAvailable,
                    r.RoleName
                FROM Staff AS s
                INNER JOIN Roles AS r
                    ON s.RoleID = r.RoleID
                WHERE (s.Username = @Username OR s.EmailAddress = @Username)
                  AND s.EmploymentStatus = 'Active';";
            return ExecuteQuery(query, new SqlParameter("@Username", SqlDbType.VarChar, 100) { Value = username });
        }

        /// <summary>
        /// Retrieves staff information by StaffID.
        /// </summary>
        public DataTable GetStaffByID(int staffID)
        {
            const string query = @"
                SELECT
                    s.StaffID,
                    s.RoleID,
                    s.FullName,
                    s.PhoneNumber,
                    s.EmailAddress,
                    s.HireDate,
                    s.Salary,
                    s.EmploymentStatus,
                    s.Username,
                    s.IsAvailable,
                    r.RoleName
                FROM Staff AS s
                INNER JOIN Roles AS r
                    ON s.RoleID = r.RoleID
                WHERE s.StaffID = @StaffID;";
            return ExecuteQuery(query, new SqlParameter("@StaffID", SqlDbType.Int) { Value = staffID });
        }

        /// <summary>
        /// Retrieves a staff member using their email address.
        /// </summary>
        public DataTable GetStaffByEmail(string email)
        {
            const string query = @"
                SELECT
                    s.StaffID,
                    s.FullName,
                    s.EmailAddress,
                    s.Username,
                    s.EmploymentStatus,
                    r.RoleName
                FROM Staff AS s
                INNER JOIN Roles AS r
                    ON s.RoleID = r.RoleID
                WHERE s.EmailAddress = @Email;";
            return ExecuteQuery(query, new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = email });
        }

        /// <summary>
        /// Updates the stored password hash for a staff member.
        /// </summary>
        public int UpdatePasswordHash(int staffID, string passwordHash)
        {
            const string query = @"
                UPDATE Staff
                SET PasswordHash = @PasswordHash
                WHERE StaffID = @StaffID; UPDATE dbo.AppAccountSecurity SET SecurityVersion=SecurityVersion+1,UpdatedAtUtc=SYSUTCDATETIME() WHERE StaffID=@StaffID; UPDATE dbo.AppUserSessions SET EndedAtUtc=SYSUTCDATETIME(),EndReason='Password changed' WHERE StaffID=@StaffID AND EndedAtUtc IS NULL;";
            return ExecuteNonQuery(query, new SqlParameter("@PasswordHash", SqlDbType.VarChar, 255) { Value = passwordHash }, new SqlParameter("@StaffID", SqlDbType.Int) { Value = staffID });
        }

        // ============================================================
        // STAFF OTP MANAGEMENT
        // ============================================================
        /// <summary>
        /// Saves the OTP and its expiry time for a staff member.
        /// </summary>
        public int SaveOtp(int staffID, string otp, DateTime expiry)
        {
            const string query = @"
                UPDATE Staff
                SET ResetOTP = @Otp,
                    ResetOTPExpiry = @Expiry
                WHERE StaffID = @StaffID;";
            return ExecuteNonQuery(query, new SqlParameter("@Otp", SqlDbType.NVarChar, 10) { Value = otp }, new SqlParameter("@Expiry", SqlDbType.DateTime) { Value = expiry }, new SqlParameter("@StaffID", SqlDbType.Int) { Value = staffID });
        }

        /// <summary>
        /// Retrieves the OTP and expiry for a staff member.
        /// </summary>
        public DataTable GetOtpData(int staffID)
        {
            const string query = @"
                SELECT
                    ResetOTP,
                    ResetOTPExpiry
                FROM Staff
                WHERE StaffID = @StaffID;";
            return ExecuteQuery(query, new SqlParameter("@StaffID", SqlDbType.Int) { Value = staffID });
        }

        /// <summary>
        /// Clears the OTP fields for a staff member.
        /// </summary>
        public int ClearOtp(int staffID)
        {
            const string query = @"
                UPDATE Staff
                SET ResetOTP = NULL,
                    ResetOTPExpiry = NULL
                WHERE StaffID = @StaffID;";
            return ExecuteNonQuery(query, new SqlParameter("@StaffID", SqlDbType.Int) { Value = staffID });
        }

        // ============================================================
        // CUSTOMER PASSWORD RESET
        // ============================================================
        /// <summary>
        /// Retrieves a customer account using their email address.
        /// Used by the password-reset flow.
        /// </summary>
        public DataTable GetCustomerByEmail(string email)
        {
            const string query = @"
                SELECT
                    CustomerID,
                    FullName,
                    EmailAddress,
                    PasswordHash,
                    AccountStatus
                FROM Customers
                WHERE EmailAddress = @Email;";
            return ExecuteQuery(query, new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = email });
        }

        /// <summary>
        /// Saves a password-reset OTP for a customer.
        /// </summary>
        public int SaveCustomerOtp(int customerID, string otp, DateTime expiry)
        {
            const string query = @"
                UPDATE Customers
                SET ResetOTP = @Otp,
                    ResetOTPExpiry = @Expiry
                WHERE CustomerID = @CustomerID;";
            return ExecuteNonQuery(query, new SqlParameter("@Otp", SqlDbType.NVarChar, 10) { Value = otp }, new SqlParameter("@Expiry", SqlDbType.DateTime) { Value = expiry }, new SqlParameter("@CustomerID", SqlDbType.Int) { Value = customerID });
        }

        /// <summary>
        /// Retrieves the password-reset OTP for a customer.
        /// </summary>
        public DataTable GetCustomerOtpData(int customerID)
        {
            const string query = @"
                SELECT
                    ResetOTP,
                    ResetOTPExpiry
                FROM Customers
                WHERE CustomerID = @CustomerID;";
            return ExecuteQuery(query, new SqlParameter("@CustomerID", SqlDbType.Int) { Value = customerID });
        }

        /// <summary>
        /// Updates the customer's password hash.
        /// </summary>
        public int UpdateCustomerPasswordHash(int customerID, string passwordHash)
        {
            const string query = @"
                UPDATE Customers
                SET PasswordHash = @PasswordHash
                WHERE CustomerID = @CustomerID; UPDATE dbo.AppAccountSecurity SET SecurityVersion=SecurityVersion+1,UpdatedAtUtc=SYSUTCDATETIME() WHERE CustomerID=@CustomerID; UPDATE dbo.AppUserSessions SET EndedAtUtc=SYSUTCDATETIME(),EndReason='Password changed' WHERE CustomerID=@CustomerID AND EndedAtUtc IS NULL;";
            return ExecuteNonQuery(query, new SqlParameter("@PasswordHash", SqlDbType.VarChar, 255) { Value = passwordHash }, new SqlParameter("@CustomerID", SqlDbType.Int) { Value = customerID });
        }

        /// <summary>
        /// Clears the customer's password-reset OTP.
        /// </summary>
        public int ClearCustomerOtp(int customerID)
        {
            const string query = @"
                UPDATE Customers
                SET ResetOTP = NULL,
                    ResetOTPExpiry = NULL
                WHERE CustomerID = @CustomerID;";
            return ExecuteNonQuery(query, new SqlParameter("@CustomerID", SqlDbType.Int) { Value = customerID });
        }

        /// <summary>
        /// Retrieves an active customer account for authentication.
        /// </summary>
        public DataTable GetCustomerForAuthentication(string email)
        {
            const string query = @"
                SELECT
                    CustomerID,
                    FullName,
                    EmailAddress,
                    PasswordHash,
                    AccountStatus
                FROM Customers
                WHERE EmailAddress = @Email
                  AND AccountStatus = 'Active';";
            return ExecuteQuery(query, new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = email });
        }
    }
}
