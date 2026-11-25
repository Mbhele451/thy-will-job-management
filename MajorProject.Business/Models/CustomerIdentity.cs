using System;
using System.Text.RegularExpressions;

namespace MajorProject.Business.Models
{
    public static class CustomerIdentity
    {
        public static void Validate(string type, string sa, string company, string passport, string country)
        {
            bool a = !string.IsNullOrWhiteSpace(sa), b = !string.IsNullOrWhiteSpace(company), c = !string.IsNullOrWhiteSpace(passport), d = !string.IsNullOrWhiteSpace(country);
            if (type == "Business")
            {
                if (!b || a || c || d || !Regex.IsMatch(company, @"^[0-9]{4}/[0-9/]{7,15}$") || company.Length > 20)
                    throw new ArgumentException("Business customers require a company registration number only (12 to 20 digits/slashes, starting YYYY/).");
                return;
            }

            if (type != "Residential")
                throw new ArgumentException("Choose Residential or Business.");
            if (b)
                throw new ArgumentException("Residential customers cannot have a company registration number.");
            if (a)
            {
                if (c || d || !Regex.IsMatch(sa, @"^[0-9]{13}$"))
                    throw new ArgumentException("Enter exactly 13 digits for an SA ID and leave passport fields empty.");
                return;
            }

            if (!c || !d || passport.Length > 20 || country.Length > 50 || Regex.IsMatch(passport, @"[^\x20-\x7E]") || Regex.IsMatch(country, @"[\x00-\x1F\x7F]"))
                throw new ArgumentException("Enter a passport number (up to 20 characters) and issuing country (up to 50 characters).");
        }
    }
}
