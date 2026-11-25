using System;
using System.Data;
using System.Text.RegularExpressions;
using MajorProject.Business.Helpers;
using MajorProject.Data.Data;

namespace MajorProject.Business.Services
{
    public sealed class SystemAdminService
    {
        private readonly SystemAdminDataAccess data;
        public SystemAdminService(string cs)
        {
            data = new SystemAdminDataAccess(cs);
        }

        public static string Text(string value, int max, string name, bool required = true)
        {
            return AdminPolicy.Text(value, max, name, required);
        }

        public static string Role(string role)
        {
            return AdminPolicy.Role(role);
        }

        public static string PasswordHash(string password, bool optional = false)
        {
            return AdminPolicy.PasswordHash(password, optional);
        }

        private static string Email(string email)
        {
            email = Text(email, 100, "Email");
            if (!ValidationHelper.IsValidEmail(email))
                throw new ArgumentException("Enter a valid email address.");
            return email;
        }

        public DataTable Accounts(int actor, string search)
        {
            return data.Accounts(actor, Text(search, 100, "Search", false));
        }

        public void Block(int actor, bool staff, int id, bool blocked, string reason, string ip)
        {
            if (id <= 0)
                throw new ArgumentException("Select an account.");
            data.Block(actor, staff, id, blocked, Text(reason, 300, "Reason"), Text(ip, 45, "IP"));
        }

        public void Credentials(int actor, bool staff, int id, string username, string email, string password, string role, string ip)
        {
            if (id <= 0)
                throw new ArgumentException("Select an account.");
            data.Credentials(actor, staff, id, staff ? AdminPolicy.Username(username) : "", Email(email), PasswordHash(password, true), staff ? Role(role) : "Customer", Text(ip, 45, "IP"));
        }

        public int CreateStaff(int actor, string name, string phone, string email, string username, string password, string role, decimal salary, string ip)
        {
            phone = Text(phone, 10, "Phone");
            if (!Regex.IsMatch(phone, @"^[0-9]{10}$"))
                throw new ArgumentException("Enter the 10-digit local phone number supported by the current database.");
            if (salary < 0 || salary > 99999999.99m)
                throw new ArgumentException("Salary must be from 0 to 99,999,999.99.");
            int id = data.CreateStaff(actor, Text(name, 100, "Full name"), phone, Email(email), AdminPolicy.Username(username), PasswordHash(password), Role(role), salary, Text(ip, 45, "IP"));
            EmailQueueService.QueueAccountWelcome(email.Trim(), username.Trim());
            return id;
        }

        public static void DateRange(DateTime from, DateTime to)
        {
            AdminPolicy.DateRange(from, to);
        }

        public DataTable Report(int actor, string kind, DateTime from, DateTime to)
        {
            DateRange(from, to);
            return data.Report(actor, kind, from, to);
        }

        public void Revoke(int actor, Guid session, string ip)
        {
            data.Revoke(actor, session, Text(ip, 45, "IP"));
        }

        public DataTable Settings(int actor)
        {
            return data.Settings(actor);
        }

        public string Setting(string key, string fallback)
        {
            if (key != "CompanyDisplayName" && key != "SupportEmail" && key != "PublicRegistrationEnabled" && key != "SessionIdleMinutes")
                throw new ArgumentException("Unsupported setting.");
            return data.Setting(key, fallback);
        }

        public static string ValidateSetting(string key, string value)
        {
            return AdminPolicy.ValidateSetting(key, value);
        }

        public void SaveSetting(int actor, string key, string value, string ip)
        {
            data.SaveSetting(actor, key, ValidateSetting(key, value), Text(ip, 45, "IP"));
        }
    }
}
