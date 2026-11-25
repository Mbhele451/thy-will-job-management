using System;
using System.Text.RegularExpressions;

namespace MajorProject.Business.Helpers
{
    /// <summary>
    /// Provides validation utilities for common input checks.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates an email address.
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates a South African 10-digit phone number starting with '0'.
        /// </summary>
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;
            string cleaned = phoneNumber.Trim();
            return cleaned.Length == 10 && cleaned.StartsWith("0") && long.TryParse(cleaned, out _);
        }

        /// <summary>
        /// Validates a customer name (letters and spaces only).
        /// </summary>
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            return Regex.IsMatch(name.Trim(), @"^[\p{L}\p{M}]+(?:[ '’.-][\p{L}\p{M}]+)*$");
        }

        /// <summary>
        /// Sanitizes text input to allow letters and spaces only.
        /// </summary>
        public static string SanitizeAlphaOnly(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return Regex.Replace(input, @"[^a-zA-Z\s]", "");
        }

        /// <summary>
        /// Sanitizes text input to allow digits only.
        /// </summary>
        public static string SanitizeDigitsOnly(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return Regex.Replace(input, @"[^\d]", "");
        }

        /// <summary>
        /// Validates a username (alphanumeric and underscores only).
        /// </summary>
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
                return false;
            return Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
        }

        /// <summary>
        /// Validates that a value is not empty or whitespace.
        /// </summary>
        public static bool IsNotEmpty(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Validates that a decimal is positive.
        /// </summary>
        public static bool IsPositive(decimal value)
        {
            return value > 0;
        }

        /// <summary>
        /// Validates that an integer is positive.
        /// </summary>
        public static bool IsPositive(int value)
        {
            return value > 0;
        }

        /// <summary>
        /// Checks if a password meets all requirements:
        /// - Minimum 8 characters
        /// - At least one lowercase letter
        /// - At least one uppercase letter
        /// - At least one digit
        /// - At least one special character
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;
            if (password.Length < 8)
                return false;
            if (!Regex.IsMatch(password, @"[a-z]"))
                return false; // at least one lowercase
            if (!Regex.IsMatch(password, @"[A-Z]"))
                return false; // at least one uppercase
            if (!Regex.IsMatch(password, @"[0-9]"))
                return false; // at least one digit
            if (!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
                return false; // at least one special
            return true;
        }

        /// <summary>
        /// Returns a PasswordStrengthResult with individual checks.
        /// </summary>
        public static PasswordStrengthResult GetPasswordStrength(string password)
        {
            var result = new PasswordStrengthResult();
            if (string.IsNullOrWhiteSpace(password))
            {
                result.IsValid = false;
                return result;
            }

            result.HasMinLength = password.Length >= 8;
            result.HasLowercase = Regex.IsMatch(password, @"[a-z]");
            result.HasUppercase = Regex.IsMatch(password, @"[A-Z]");
            result.HasDigit = Regex.IsMatch(password, @"[0-9]");
            result.HasSpecial = Regex.IsMatch(password, @"[^a-zA-Z0-9]");
            result.IsValid = result.HasMinLength && result.HasLowercase && result.HasUppercase && result.HasDigit && result.HasSpecial;
            return result;
        }
    }

    /// <summary>
    /// Represents the result of a password strength check.
    /// </summary>
    public class PasswordStrengthResult
    {
        public bool HasMinLength { get; set; }
        public bool HasLowercase { get; set; }
        public bool HasUppercase { get; set; }
        public bool HasDigit { get; set; }
        public bool HasSpecial { get; set; }
        public bool IsValid { get; set; }
    }
}
