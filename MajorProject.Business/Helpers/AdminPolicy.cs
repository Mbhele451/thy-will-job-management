using System;
using System.Text.RegularExpressions;

namespace MajorProject.Business.Helpers
{
    public static class AdminPolicy
    {
        public static string Text(string value, int max, string name, bool required = true)
        {
            value = (value ?? "").Trim();
            if ((required && value.Length == 0) || value.Length > max || Regex.IsMatch(value, @"[\x00-\x1F\x7F]"))
                throw new ArgumentException(name + " is required and must fit " + max + " characters.");
            return value;
        }

        public static string Username(string value)
        {
            value = Text(value, 50, "Username");
            if (!Regex.IsMatch(value, @"^[A-Za-z0-9_.@-]+$"))
                throw new ArgumentException("Use letters A-Z, digits, dots, underscores, @ or hyphens for usernames.");
            return value;
        }

        public static string Role(string role)
        {
            if (role != "Administrator" && role != "Manager" && role != "Technician" && role != "SysAdmin")
                throw new ArgumentException("Choose a valid staff role.");
            return role;
        }

        public static string PasswordHash(string password, bool optional = false)
        {
            if (optional && string.IsNullOrEmpty(password))
                return null;
            if (password == null || password.Length < 8 || password.Length > 128 || !Regex.IsMatch(password, "[A-Z]") || !Regex.IsMatch(password, "[a-z]") || !Regex.IsMatch(password, "[0-9]") || !Regex.IsMatch(password, "[^a-zA-Z0-9]"))
                throw new ArgumentException("Password requires 8 to 128 characters, uppercase, lowercase, a number and a special character.");
            return CryptographyHelper.HashPassword(password);
        }

        private static string Email(string email)
        {
            email = Text(email, 100, "Email");
            if (!ValidationHelper.IsValidEmail(email))
                throw new ArgumentException("Enter a valid email address.");
            return email;
        }

        public static void DateRange(DateTime from, DateTime to)
        {
            if (to <= from || (to - from).TotalDays > 366)
                throw new ArgumentException("Choose a date range of at most 366 days.");
        }

        public static string ValidateSetting(string key, string value)
        {
            value = Text(value, 500, "Setting value", false);
            int minutes;
            switch (key)
            {
                case "CompanyDisplayName":
                    return Text(value, 100, "Company name");
                case "SupportEmail":
                    return value.Length == 0 ? "" : Email(value);
                case "PublicRegistrationEnabled":
                    if (value != "true" && value != "false")
                        throw new ArgumentException("Use true or false.");
                    return value;
                case "SessionIdleMinutes":
                    if (!int.TryParse(value, out minutes) || minutes < 5 || minutes > 60)
                        throw new ArgumentException("Session idle minutes must be 5 to 60.");
                    return minutes.ToString(System.Globalization.CultureInfo.InvariantCulture);
                default:
                    throw new ArgumentException("Unsupported setting.");
            }
        }
    }
}
