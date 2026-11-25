using System;

namespace MajorProject.Business.Models
{
    /// <summary>
    /// Represents a staff member from the Staff table.
    ///
    /// Database columns:
    /// StaffID
    /// RoleID
    /// FullName
    /// PhoneNumber
    /// EmailAddress
    /// HireDate
    /// Salary
    /// EmploymentStatus
    /// Username
    /// PasswordHash
    /// IsAvailable
    ///
    /// RoleName is populated from the Roles table through a JOIN.
    /// It is not a physical column in Staff.
    /// </summary>
    public class Staff
    {
        public int StaffID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public string EmploymentStatus { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsAvailable { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", FullName ?? string.Empty, RoleName ?? string.Empty);
        }
    }
}
