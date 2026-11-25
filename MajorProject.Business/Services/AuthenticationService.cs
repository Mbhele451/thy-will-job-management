using System;
using System.Data;
using System.Threading.Tasks;
using MajorProject.Business.Helpers;
using MajorProject.Data.Data;

namespace MajorProject.Business.Services
{
    public class AuthenticationService : IDisposable
    {
        private readonly AuthenticationDataAccess _dataAccess;
        private readonly SystemAdminDataAccess _security;
        private bool _disposed;
        public AuthenticationService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));
            }

            _dataAccess = new AuthenticationDataAccess(connectionString);
            _security = new SystemAdminDataAccess(connectionString);
        }

        // ============================================================
        // STAFF AUTHENTICATION
        // ============================================================
        public bool AuthenticateUser(string username, string password, out int staffID, out string fullName, out string roleName)
        {
            staffID = 0;
            fullName = string.Empty;
            roleName = string.Empty;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            DataTable dt = _dataAccess.GetStaffForAuthentication(username);
            if (dt == null || dt.Rows.Count != 1)
            {
                return false;
            }

            DataRow row = dt.Rows[0];
            string storedHash = row["PasswordHash"]?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(storedHash))
            {
                return false;
            }

            if (!CryptographyHelper.VerifyPassword(password, storedHash))
            {
                return false;
            }

            if (!_security.LoginAllowed(true, Convert.ToInt32(row["StaffID"])))
                return false;
            staffID = Convert.ToInt32(row["StaffID"]);
            fullName = row["FullName"]?.ToString() ?? string.Empty;
            roleName = row["RoleName"]?.ToString() ?? string.Empty;
            return true;
        }

        // ============================================================
        // STAFF RETRIEVAL
        // ============================================================
        public DataTable GetStaffByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new DataTable();
            }

            return _dataAccess.GetStaffByEmail(email);
        }

        public bool TryGetStaffByEmail(string email, out int staffID, out string fullName)
        {
            staffID = 0;
            fullName = string.Empty;
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            DataTable dt = _dataAccess.GetStaffByEmail(email);
            if (dt == null || dt.Rows.Count != 1)
            {
                return false;
            }

            DataRow row = dt.Rows[0];
            staffID = Convert.ToInt32(row["StaffID"]);
            fullName = row["FullName"]?.ToString() ?? string.Empty;
            return true;
        }

        // ============================================================
        // STAFF PASSWORD UPDATE
        // ============================================================
        public void UpdatePassword(int staffID, string newPassword)
        {
            ValidateNewPassword(newPassword);
            string hash = CryptographyHelper.HashPassword(newPassword);
            int rowsAffected = _dataAccess.UpdatePasswordHash(staffID, hash);
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException("The staff password could not be updated.");
            }
        }

        // ============================================================
        // STAFF OTP MANAGEMENT
        // ============================================================
        /// <summary>
        /// Generates an OTP and queues the email for sending.
        /// </summary>
        public async Task<bool> GenerateAndSendOtpAsync(string email)
        {
            try
            {
                DataTable dt = _dataAccess.GetStaffByEmail(email);
                if (dt == null || dt.Rows.Count != 1)
                {
                    Logger.Warning($"OTP request for non-existent email: {email}");
                    return false;
                }

                DataRow row = dt.Rows[0];
                int staffID = Convert.ToInt32(row["StaffID"]);
                string fullName = row["FullName"]?.ToString() ?? string.Empty;
                Logger.Info($"Generating OTP for StaffID {staffID} ({email})");
                string otp = CryptographyHelper.GenerateOtp();
                DateTime expiry = DateTime.Now.AddMinutes(5);
                int rowsAffected = _dataAccess.SaveOtp(staffID, otp, expiry);
                if (rowsAffected == 0)
                {
                    Logger.Error($"SaveOtp failed for StaffID {staffID}");
                    return false;
                }

                Logger.Info($"SaveOtp returned {rowsAffected} rows affected for StaffID {staffID}");
                EmailQueueService.Instance.EnqueueOtpEmail(email, fullName, otp);
                Logger.Info($"OTP queued for {email}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Unexpected error in GenerateAndSendOtpAsync for {email}", ex);
                return false;
            }
        }

        public bool ValidateOtp(int staffID, string otp)
        {
            if (staffID <= 0 || string.IsNullOrWhiteSpace(otp))
            {
                return false;
            }

            DataTable dt = _dataAccess.GetOtpData(staffID);
            if (dt == null || dt.Rows.Count != 1)
            {
                return false;
            }

            DataRow row = dt.Rows[0];
            string storedOtp = row["ResetOTP"]?.ToString() ?? string.Empty;
            if (!DateTime.TryParse(row["ResetOTPExpiry"]?.ToString(), out DateTime expiry))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(storedOtp))
            {
                return false;
            }

            if (expiry < DateTime.Now)
            {
                return false;
            }

            return storedOtp == otp;
        }

        /// <summary>
        /// Resets a staff password after successful OTP validation.
        /// Password strength is validated in the Business layer.
        /// </summary>
        public void ResetPassword(int staffID, string newPassword)
        {
            if (staffID <= 0)
            {
                throw new ArgumentException("Invalid staff account.", nameof(staffID));
            }

            ValidateNewPassword(newPassword);
            string hash = CryptographyHelper.HashPassword(newPassword);
            int rowsAffected = _dataAccess.UpdatePasswordHash(staffID, hash);
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException("The staff password could not be reset.");
            }

            _dataAccess.ClearOtp(staffID);
        }

        // ============================================================
        // CUSTOMER PASSWORD RESET
        // ============================================================
        public DataTable GetCustomerByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new DataTable();
            }

            return _dataAccess.GetCustomerByEmail(email);
        }

        /// <summary>
        /// Generates an OTP and queues it for the customer.
        /// </summary>
        public async Task<bool> GenerateAndSendCustomerOtpAsync(string email)
        {
            try
            {
                DataTable dt = _dataAccess.GetCustomerByEmail(email);
                if (dt == null || dt.Rows.Count != 1)
                {
                    Logger.Warning($"Customer OTP request for non-existent email: {email}");
                    return false;
                }

                DataRow row = dt.Rows[0];
                int customerID = Convert.ToInt32(row["CustomerID"]);
                string fullName = row["FullName"]?.ToString() ?? string.Empty;
                string customerEmail = row["EmailAddress"]?.ToString() ?? email;
                Logger.Info($"Generating OTP for CustomerID {customerID} ({customerEmail})");
                string otp = CryptographyHelper.GenerateOtp();
                DateTime expiry = DateTime.Now.AddMinutes(5);
                int rowsAffected = _dataAccess.SaveCustomerOtp(customerID, otp, expiry);
                if (rowsAffected == 0)
                {
                    Logger.Error($"SaveCustomerOtp failed for CustomerID {customerID}");
                    return false;
                }

                Logger.Info($"Customer OTP saved for CustomerID {customerID}");
                EmailQueueService.Instance.EnqueueOtpEmail(customerEmail, fullName, otp);
                Logger.Info($"Customer OTP queued for {customerEmail}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Unexpected error generating customer OTP for {email}", ex);
                return false;
            }
        }

        public bool ValidateCustomerOtp(int customerID, string otp)
        {
            if (customerID <= 0 || string.IsNullOrWhiteSpace(otp))
            {
                return false;
            }

            DataTable dt = _dataAccess.GetCustomerOtpData(customerID);
            if (dt == null || dt.Rows.Count != 1)
            {
                return false;
            }

            DataRow row = dt.Rows[0];
            string storedOtp = row["ResetOTP"]?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(storedOtp))
            {
                return false;
            }

            if (!DateTime.TryParse(row["ResetOTPExpiry"]?.ToString(), out DateTime expiry))
            {
                return false;
            }

            if (expiry < DateTime.Now)
            {
                return false;
            }

            return storedOtp == otp;
        }

        /// <summary>
        /// Resets a customer password after successful OTP validation.
        /// Password strength is validated in the Business layer.
        /// </summary>
        public void ResetCustomerPassword(int customerID, string newPassword)
        {
            if (customerID <= 0)
            {
                throw new ArgumentException("Invalid customer account.", nameof(customerID));
            }

            ValidateNewPassword(newPassword);
            string hash = CryptographyHelper.HashPassword(newPassword);
            int rowsAffected = _dataAccess.UpdateCustomerPasswordHash(customerID, hash);
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException("The customer password could not be reset.");
            }

            _dataAccess.ClearCustomerOtp(customerID);
        }

        // ============================================================
        // CUSTOMER AUTHENTICATION
        // ============================================================
        public bool AuthenticateCustomer(string email, string password, out int customerID, out string fullName, out string customerEmail)
        {
            customerID = 0;
            fullName = string.Empty;
            customerEmail = string.Empty;
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            DataTable dt = _dataAccess.GetCustomerForAuthentication(email);
            if (dt == null || dt.Rows.Count != 1)
            {
                return false;
            }

            DataRow row = dt.Rows[0];
            string storedHash = row["PasswordHash"]?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(storedHash))
            {
                return false;
            }

            if (!CryptographyHelper.VerifyPassword(password, storedHash))
            {
                return false;
            }

            if (!_security.LoginAllowed(false, Convert.ToInt32(row["CustomerID"])))
                return false;
            customerID = Convert.ToInt32(row["CustomerID"]);
            fullName = row["FullName"]?.ToString() ?? string.Empty;
            customerEmail = row["EmailAddress"]?.ToString() ?? string.Empty;
            return true;
        }

        // ============================================================
        // PASSWORD VALIDATION
        // ============================================================
        /// <summary>
        /// Validates a password before it is hashed and stored.
        /// Uses the same password rules as registration and profile
        /// password changes.
        /// </summary>
        private void ValidateNewPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty.", nameof(password));
            }

            if (!ValidationHelper.IsValidPassword(password))
            {
                throw new ArgumentException("Password must be at least 8 characters long and contain " + "an uppercase letter, a lowercase letter, a number, " + "and a special character.", nameof(password));
            }
        }

        // ============================================================
        // IDisposable
        // ============================================================
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                // AuthenticationDataAccess does not require
                // explicit disposal here.
                // EmailQueueService is shared and must not be disposed.
                }

                _disposed = true;
            }
        }
    }
}
