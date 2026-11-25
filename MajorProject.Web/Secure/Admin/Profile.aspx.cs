using MajorProject.Business.Helpers;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Net.Mail;

namespace MajorProject.Web.Secure.Admin
{
    public partial class Profile : AdminBasePage
    {
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString;
            }
        }

        private StaffService StaffService
        {
            get
            {
                return new StaffService(ConnectionString);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    LoadProfile();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("The profile could not be loaded.", ex);
            }
        }

        private int GetCurrentStaffID()
        {
            return WebSession.StaffID;
        }

        private void LoadProfile()
        {
            int staffID = GetCurrentStaffID();
            if (staffID <= 0)
            {
                throw new InvalidOperationException("Your staff session is no longer valid. Please sign in again.");
            }

            DataTable staff = StaffService.GetStaffByID(staffID);
            if (staff == null || staff.Rows.Count == 0)
            {
                throw new InvalidOperationException("Your staff profile could not be found.");
            }

            DataRow row = staff.Rows[0];
            string fullName = Convert.ToString(row["FullName"]);
            string roleName = Convert.ToString(row["RoleName"]);
            string username = Convert.ToString(row["Username"]);
            string email = Convert.ToString(row["EmailAddress"]);
            string phone = Convert.ToString(row["PhoneNumber"]);
            // =========================================================
            // PROFILE HEADER
            // =========================================================
            lblFullName.Text = fullName;
            lblRole.Text = roleName;
            lblStaffID.Text = Convert.ToString(row["StaffID"]);
            lblUsername.Text = username;
            lblEmail.Text = string.IsNullOrWhiteSpace(email) ? "Not provided" : email;
            lblPhone.Text = string.IsNullOrWhiteSpace(phone) ? "Not provided" : phone;
            if (row["HireDate"] != DBNull.Value)
            {
                DateTime hireDate = Convert.ToDateTime(row["HireDate"]);
                lblHireDate.Text = hireDate.ToString("dd MMMM yyyy");
            }
            else
            {
                lblHireDate.Text = "Not available";
            }

            string employmentStatus = Convert.ToString(row["EmploymentStatus"]);
            lblAccountStatus.Text = string.IsNullOrWhiteSpace(employmentStatus) ? "Unknown" : employmentStatus;
            bool isAvailable = row["IsAvailable"] != DBNull.Value && Convert.ToBoolean(row["IsAvailable"]);
            lblAvailability.Text = isAvailable ? "Available" : "Unavailable";
            lblInitials.Text = GetInitials(fullName);
            // =========================================================
            // EDIT FORM
            // =========================================================
            txtFullName.Text = fullName;
            txtPhone.Text = phone;
            txtEmail.Text = email;
            txtUsername.Text = username;
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "?";
            }

            string[] parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                return parts[0].Substring(0, 1).ToUpperInvariant();
            }

            return (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpperInvariant();
        }

        // =============================================================
        // SAVE PROFILE
        // =============================================================
        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            try
            {
                int staffID = GetCurrentStaffID();
                if (staffID <= 0)
                {
                    lblMessage.Text = "Your session is no longer valid. Please sign in again.";
                    return;
                }

                string fullName = txtFullName.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string email = txtEmail.Text.Trim();
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    lblMessage.Text = "Full name is required.";
                    return;
                }

                if (!IsValidPhone(phone))
                {
                    lblMessage.Text = "Enter a valid 10-digit South African phone number.";
                    return;
                }

                if (!IsValidEmail(email))
                {
                    lblMessage.Text = "Enter a valid email address.";
                    return;
                }

                DataTable staff = StaffService.GetStaffByID(staffID);
                if (staff == null || staff.Rows.Count == 0)
                {
                    lblMessage.Text = "Your staff profile could not be found.";
                    return;
                }

                DataRow row = staff.Rows[0];
                int roleID = Convert.ToInt32(row["RoleID"]);
                decimal salary = row["Salary"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Salary"]);
                StaffService.UpdateStaffDetails(staffID, roleID, fullName, phone, email, salary);
                // Keep the logged-in session name
                // synchronized with the database.
                WebSession.FullName = fullName;
                lblMessage.Text = "Your profile was successfully updated.";
                LoadProfile();
            }
            catch (ArgumentException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("Your profile could not be updated.", ex);
            }
        }

        // =============================================================
        // CHANGE PASSWORD
        // =============================================================
        // =============================================================
        // LOGOUT
        // =============================================================
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                WebSession.Clear();
                Response.Redirect(ResolveUrl("~/Account/Login.aspx"), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                lblMessage.Text = HandleUnexpectedError("You could not be signed out.", ex);
            }
        }

        // =============================================================
        // PHONE VALIDATION
        // =============================================================
        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            string digits = phone.Trim();
            if (digits.Length != 10)
            {
                return false;
            }

            foreach (char character in digits)
            {
                if (!char.IsDigit(character))
                {
                    return false;
                }
            }

            return true;
        }

        // =============================================================
        // EMAIL VALIDATION
        // =============================================================
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                MailAddress address = new MailAddress(email);
                return string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        // =============================================================
        // PASSWORD VALIDATION
        // =============================================================
    }
}
