using System;
using System.Text.RegularExpressions;

namespace MajorProject.Business.Services.Payments
{
    // Format checks for the existing simulated checkout. No issuer verification or bank authorization.
    public static class PaymentInputValidation
    {
        public static bool IsValidName(string value)
        {
            return value != null && Regex.IsMatch(value, @"\A[\p{L}\p{M} '.-]{2,100}\z") && Regex.IsMatch(value, @"\p{L}");
        }

        public static string NormalizeCardNumber(string value)
        {
            return value != null && Regex.IsMatch(value, @"\A[0-9 -]{13,25}\z") ? value.Replace(" ", "").Replace("-", "") : string.Empty;
        }

        public static bool IsValidCardNumber(string value)
        {
            if (value == null || !Regex.IsMatch(value, @"\A[0-9]{13,19}\z") || Regex.IsMatch(value, @"\A([0-9])\1+\z"))
                return false;
            int sum = 0;
            bool doubleDigit = false;
            for (int i = value.Length - 1; i >= 0; i--)
            {
                int digit = value[i] - '0';
                if (doubleDigit)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
                doubleDigit = !doubleDigit;
            }

            return sum % 10 == 0;
        }

        public static bool IsValidCvv(string value)
        {
            return value != null && Regex.IsMatch(value, @"\A[0-9]{3,4}\z");
        }

        public static bool IsValidExpiry(int month, int year, DateTime today)
        {
            return month >= 1 && month <= 12 && year >= 2000 && year <= 9999 && (year > today.Year || (year == today.Year && month >= today.Month));
        }

        public static string GetCardBrand(string value)
        {
            if (value.StartsWith("4", StringComparison.Ordinal))
                return "Visa";
            int prefix;
            if (value.Length >= 4 && int.TryParse(value.Substring(0, 4), out prefix) && ((prefix >= 5100 && prefix <= 5599) || (prefix >= 2221 && prefix <= 2720)))
                return "Mastercard";
            return "Other";
        }
    }
}
