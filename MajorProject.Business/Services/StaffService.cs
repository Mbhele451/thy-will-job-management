using MajorProject.Business.Helpers;
using System;
using System.Data;
using MajorProject.Data.Data;

namespace MajorProject.Business.Services
{
    public class StaffService
    {
        private readonly StaffDataAccess _dataAccess;
        public StaffService(string connectionString)
        {
            _dataAccess = new StaffDataAccess(connectionString);
        }

        // ============================================================
        // RETRIEVAL
        // ============================================================
        public DataTable GetAllStaff()
        {
            return _dataAccess.GetAllStaff();
        }

        public DataTable GetStaffByID(int staffID)
        {
            return _dataAccess.GetStaffByID(staffID);
        }

        public DataTable GetAvailableTechnicians(DateTime date)
        {
            return _dataAccess.GetAvailableTechnicians(date);
        }

        public DataTable GetAvailableTechnicians()
        {
            return _dataAccess.GetAvailableTechnicians();
        }

        // ============================================================
        // INSERT
        // ============================================================
        public void InsertStaff(int roleID, string fullName, string phone, string email, decimal salary, string username, string password)
        {
            if (roleID <= 0)
                throw new ArgumentException("A valid role is required.");
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full Name required.");
            if (!ValidationHelper.IsValidPhoneNumber(phone))
                throw new ArgumentException("Invalid phone.");
            if (!ValidationHelper.IsValidEmail(email))
                throw new ArgumentException("Invalid email.");
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username required.");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password required.");
            if (salary <= 0)
                throw new ArgumentException("Salary must be positive.");
            string hash = CryptographyHelper.HashPassword(password);
            _dataAccess.InsertStaff(roleID, fullName, phone, email, salary, username, hash);
            EmailQueueService.QueueAccountWelcome(email, username);
        }

        // ============================================================
        // UPDATE DETAILS
        // ============================================================
        public void UpdateStaffDetails(int staffID, int roleID, string fullName, string phone, string email, decimal salary)
        {
            if (staffID <= 0)
                throw new ArgumentException("Invalid staff member.");
            if (roleID <= 0)
                throw new ArgumentException("A valid role is required.");
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full Name required.");
            if (!ValidationHelper.IsValidPhoneNumber(phone))
                throw new ArgumentException("Invalid phone.");
            if (!ValidationHelper.IsValidEmail(email))
                throw new ArgumentException("Invalid email.");
            if (salary <= 0)
                throw new ArgumentException("Salary must be positive.");
            _dataAccess.UpdateStaffDetails(staffID, roleID, fullName, phone, email, salary);
        }

        // ============================================================
        // PASSWORD
        // ============================================================
        public void ChangePassword(int staffID, string newPassword)
        {
            if (staffID <= 0)
                throw new ArgumentException("Invalid staff member.");
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Password required.");
            if (!ValidationHelper.IsValidPassword(newPassword))
                throw new ArgumentException("Password requires at least 8 characters, uppercase and lowercase letters, a number, and a special character.");
            string hash = CryptographyHelper.HashPassword(newPassword);
            _dataAccess.UpdatePasswordHash(staffID, hash);
        }

        // ============================================================
        // EMPLOYMENT STATUS
        // ============================================================
        public void ChangeEmploymentStatus(int staffID, string employmentStatus)
        {
            if (staffID <= 0)
                throw new ArgumentException("Invalid staff member.");
            if (string.IsNullOrWhiteSpace(employmentStatus))
                throw new ArgumentException("Employment status required.");
            if (!string.Equals(employmentStatus, "Active", StringComparison.OrdinalIgnoreCase) && !string.Equals(employmentStatus, "Inactive", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid employment status.");
            }

            if (_dataAccess.UpdateEmploymentStatus(staffID, employmentStatus) != 1)
                throw new InvalidOperationException("The staff member could not be updated.");
        }

        // ============================================================
        // AVAILABILITY
        // ============================================================
        public void ChangeAvailability(int staffID, bool isAvailable)
        {
            if (staffID <= 0)
                throw new ArgumentException("Invalid staff member.");
            if (_dataAccess.UpdateAvailability(staffID, isAvailable) != 1)
                throw new InvalidOperationException("Blocked or inactive staff cannot be marked available.");
        }

        // ============================================================
        // ARCHIVE
        // ============================================================
        public void ArchiveStaff(int staffID)
        {
            if (staffID <= 0)
                throw new ArgumentException("Invalid staff member.");
            _dataAccess.ArchiveStaff(staffID);
        }

        // ============================================================
        // SEARCH
        // ============================================================
        public DataTable SearchStaffByName(string name)
        {
            return _dataAccess.SearchStaffByName(name);
        }

        public DataTable SearchStaffByPhone(string phone)
        {
            return _dataAccess.SearchStaffByPhone(phone);
        }

        // ============================================================
        // FIND AVAILABLE TECHNICIAN
        // ============================================================
        public int FindAvailableTechnician(DateTime date)
        {
            if (date.Date < DateTime.Today)
            {
                throw new ArgumentException("The job date cannot be in the past.");
            }

            DataTable technicians = _dataAccess.GetAvailableTechnicians(date.Date);
            if (technicians == null || technicians.Rows.Count == 0)
            {
                return 0;
            }

            // StaffDataAccess already orders technicians
            // by the lowest completed-job workload.
            DataRow selectedTechnician = technicians.Rows[0];
            return Convert.ToInt32(selectedTechnician["StaffID"]);
        }

        public DataTable GetAvailableTechniciansForJob(DateTime date, int jobID)
        {
            if (jobID <= 0)
            {
                throw new ArgumentException("Invalid job ID.", nameof(jobID));
            }

            return _dataAccess.GetAvailableTechniciansForJob(date.Date, jobID);
        }
    }
}
