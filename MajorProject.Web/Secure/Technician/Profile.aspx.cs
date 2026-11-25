using MajorProject.Business.Helpers;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Net.Mail;

namespace MajorProject.Web.Secure.Technician
{
    public partial class Profile : TechnicianBasePage
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
                ShowError(HandleUnexpectedError("The technician profile could not be loaded.", ex));
            }
        }

        // =============================================================
        // MESSAGES
        // lblMessage keeps the base "dashboard-message" class and gets
        // a variant class so the CSS can colour it. It is hidden by the
        // CSS while empty, and EnableViewState="false" resets it on
        // every postback.
        // =============================================================
        private void ShowSuccess(string message)
        {
            lblMessage.CssClass = "dashboard-message dashboard-message-success";
            lblMessage.Text = message;
        }

        private void ShowError(string message)
        {
            lblMessage.CssClass = "dashboard-message dashboard-message-error";
            lblMessage.Text = message;
        }

        // =============================================================
        // CURRENT TECHNICIAN
        // =============================================================
        private int GetCurrentStaffID()
        {
            return WebSession.StaffID;
        }

        // =============================================================
        // LOAD PROFILE
        // =============================================================
        private void LoadProfile()
        {
            int staffID = GetCurrentStaffID();
            if (staffID <= 0)
            {
                throw new InvalidOperationException("Your technician session is no longer valid. Please sign in again.");
            }

            DataTable staff = StaffService.GetStaffByID(staffID);
            if (staff == null || staff.Rows.Count == 0)
            {
                throw new InvalidOperationException("Your technician profile could not be found.");
            }

            DataRow row = staff.Rows[0];
            string fullName = Convert.ToString(row["FullName"]);
            string roleName = Convert.ToString(row["RoleName"]);
            string username = Convert.ToString(row["Username"]);
            string email = Convert.ToString(row["EmailAddress"]);
            string phone = Convert.ToString(row["PhoneNumber"]);
            string employmentStatus = Convert.ToString(row["EmploymentStatus"]);
            bool isAvailable = row["IsAvailable"] != DBNull.Value && Convert.ToBoolean(row["IsAvailable"]);
            // =========================================================
            // IDENTITY
            // =========================================================
            lblFullName.Text = fullName;
            lblRole.Text = roleName;
            lblInitials.Text = GetInitials(fullName);
            lblStaffID.Text = Convert.ToString(row["StaffID"]);
            lblUsername.Text = username;
            // =========================================================
            // CONTACT
            // =========================================================
            lblEmail.Text = string.IsNullOrWhiteSpace(email) ? "Not provided" : email;
            lblPhone.Text = string.IsNullOrWhiteSpace(phone) ? "Not provided" : phone;
            // =========================================================
            // EMPLOYMENT
            // =========================================================
            lblAccountStatus.Text = string.IsNullOrWhiteSpace(employmentStatus) ? "Unknown" : employmentStatus;
            if (row["HireDate"] != DBNull.Value)
            {
                DateTime hireDate = Convert.ToDateTime(row["HireDate"]);
                lblHireDate.Text = hireDate.ToString("dd MMMM yyyy");
            }
            else
            {
                lblHireDate.Text = "Not available";
            }

            // =========================================================
            // AVAILABILITY
            // =========================================================
            SetAvailabilityDisplay(isAvailable);
            // =========================================================
            // EDIT FORM
            // =========================================================
            txtFullName.Text = fullName;
            txtPhone.Text = phone;
            txtEmail.Text = email;
            txtUsername.Text = username;
        }

        // =============================================================
        // AVAILABILITY DISPLAY
        // =============================================================
        private void SetAvailabilityDisplay(bool isAvailable)
        {
            if (isAvailable)
            {
                lblAvailability.Text = "Available";
                lblFieldStatus.Text = "You are available for new jobs.";
                btnSetAvailable.Visible = false;
                btnSetUnavailable.Visible = true;
            }
            else
            {
                lblAvailability.Text = "Unavailable";
                lblFieldStatus.Text = "You are currently unavailable.";
                btnSetAvailable.Visible = true;
                btnSetUnavailable.Visible = false;
            }
        }

        // =============================================================
        // SET AVAILABLE
        // =============================================================
        protected void btnSetAvailable_Click(object sender, EventArgs e)
        {
            try
            {
                int staffID = GetCurrentStaffID();
                if (staffID <= 0)
                {
                    ShowError("Your session is no longer valid. Please sign in again.");
                    return;
                }

                StaffService.ChangeAvailability(staffID, true);
                ShowSuccess("You are now marked as available for new jobs.");
                LoadProfile();
            }
            catch (Exception ex)
            {
                ShowError(HandleUnexpectedError("Your availability could not be updated.", ex));
            }
        }

        // =============================================================
        // SET UNAVAILABLE
        // =============================================================
        protected void btnSetUnavailable_Click(object sender, EventArgs e)
        {
            try
            {
                int staffID = GetCurrentStaffID();
                if (staffID <= 0)
                {
                    ShowError("Your session is no longer valid. Please sign in again.");
                    return;
                }

                StaffService.ChangeAvailability(staffID, false);
                ShowSuccess("You are now marked as unavailable for new jobs.");
                LoadProfile();
            }
            catch (Exception ex)
            {
                ShowError(HandleUnexpectedError("Your availability could not be updated.", ex));
            }
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
                    ShowError("Your session is no longer valid. Please sign in again.");
                    return;
                }

                string fullName = txtFullName.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string email = txtEmail.Text.Trim();
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    ShowError("Full name is required.");
                    return;
                }

                if (!IsValidPhone(phone))
                {
                    ShowError("Enter a valid 10-digit South African phone number.");
                    return;
                }

                if (!IsValidEmail(email))
                {
                    ShowError("Enter a valid email address.");
                    return;
                }

                DataTable staff = StaffService.GetStaffByID(staffID);
                if (staff == null || staff.Rows.Count == 0)
                {
                    ShowError("Your technician profile could not be found.");
                    return;
                }

                DataRow row = staff.Rows[0];
                int roleID = Convert.ToInt32(row["RoleID"]);
                decimal salary = row["Salary"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Salary"]);
                /*
                 * We deliberately reuse UpdateStaffDetails
                 * only for the fields the technician is
                 * allowed to change.
                 *
                 * Role and salary are taken directly from
                 * the existing database record.
                 */
                StaffService.UpdateStaffDetails(staffID, roleID, fullName, phone, email, salary);
                // Keep the logged-in session synchronized.
                WebSession.FullName = fullName;
                ShowSuccess("Your profile was successfully updated.");
                LoadProfile();
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                ShowError(HandleUnexpectedError("Your profile could not be updated.", ex));
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
                ShowError(HandleUnexpectedError("You could not be signed out.", ex));
            }
        }

        // =============================================================
        // INITIALS
        // =============================================================
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
