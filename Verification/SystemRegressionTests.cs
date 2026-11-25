using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using MajorProject.Business.Models;
using MajorProject.Business.Helpers;
using MajorProject.Business.Services.Payments;
using MajorProject.Web.Helpers;

internal static class SystemRegressionTests
{
    private static int checks;
    private static int failures;
    private static void Check(string name, bool condition)
    {
        checks++;
        if (!condition)
        {
            failures++;
            Console.WriteLine("FAIL " + name);
        }
    }

    private static void Reject(string name, Action action)
    {
        try
        {
            action();
            Check(name, false);
        }
        catch (ArgumentException)
        {
            Check(name, true);
        }
        catch (InvalidOperationException)
        {
            Check(name, true);
        }
    }

    public static int Main()
    {
        try
        {
            Carts();
            Identities();
            Payments();
            Policies();
            Ledger();
            Dashboard();
        }
        catch (Exception ex)
        {
            failures++;
            Console.WriteLine(ex);
        }

        Console.WriteLine("Assertions: " + checks + "; failures: " + failures);
        return failures == 0 ? 0 : 1;
    }

    private static void Carts()
    {
        Reject("invalid customer", () => new BookingCart(0));
        var cart = new BookingCart(1);
        Reject("empty cart cannot submit", () => cart.ToDataTable());
        foreach (int value in new[]
        {
            int.MinValue,
            -1,
            0,
            1001,
            int.MaxValue
        }

        )
            Reject("invalid quantity " + value, () => cart.AddService(1, "Wiring", value));
        Reject("invalid service id", () => cart.AddService(0, "Wiring", 1));
        Reject("empty service name", () => cart.AddService(1, " ", 1));
        Reject("long service name", () => cart.AddService(1, new string ('a', 101), 1));
        cart.AddService(1, "Wiring", 999);
        cart.AddService(1, "Wiring", 1);
        Check("duplicate service merged at limit", cart.ServiceCount == 1 && cart.TotalQuantity == 1000);
        Reject("duplicate overflow", () => cart.AddService(1, "Wiring", 1));
        Check("overflow preserves cart", cart.TotalQuantity == 1000);
        Reject("selection rollback", () => cart.AddSelection(new[] { new BookingCartLine(2, "Geyser", 1), new BookingCartLine(1, "Wiring", 1) }));
        Check("failed selection is atomic", cart.ServiceCount == 1 && cart.TotalQuantity == 1000);
        Reject("null selection", () => cart.AddSelection(null));
        Reject("empty selection", () => cart.AddSelection(new BookingCartLine[0]));
        Reject("null selected line", () => cart.AddSelection(new BookingCartLine[] { null }));
        Reject("missing quantity target", () => cart.SetQuantity(2, 1));
        Check("missing removal harmless", !cart.RemoveService(2));
        for (int id = 2; id <= 50; id++)
            cart.AddService(id, "Service " + id, 1);
        Reject("51st service rejected", () => cart.AddService(51, "Extra", 1));
        Check("50 lines submit", cart.ToDataTable().Rows.Count == 50);
        cart.Clear();
        Check("clear removes every line", cart.ServiceCount == 0);
        foreach (string value in new[]
        {
            null,
            "",
            "   ",
            new string ('a', 231)
        }

        )
            Reject("invalid description", () => BookingCart.NormalizeDescription(value));
        Check("description trimmed", BookingCart.NormalizeDescription("  Work  ") == "Work");
        Check("description boundary", BookingCart.NormalizeDescription(new string ('a', 230)).Length == 230);
    }

    private static void Identities()
    {
        for (int mask = 0; mask < 16; mask++)
        {
            string sa = (mask & 1) != 0 ? "9001015009087" : null;
            string company = (mask & 2) != 0 ? "2020/1234567" : null;
            string passport = (mask & 4) != 0 ? "AB123456" : null;
            string country = (mask & 8) != 0 ? "FRANCE" : null;
            foreach (string type in new[]
            {
                "Residential",
                "Business"
            }

            )
            {
                bool expected = type == "Residential" ? mask == 1 || mask == 12 : mask == 2;
                bool actual = true;
                try
                {
                    CustomerIdentity.Validate(type, sa, company, passport, country);
                }
                catch (ArgumentException)
                {
                    actual = false;
                }

                Check("identity " + type + " " + mask, actual == expected);
            }
        }

        Reject("passport too long", () => CustomerIdentity.Validate("Residential", null, null, new string ('a', 21), "FRANCE"));
        Reject("country too long", () => CustomerIdentity.Validate("Residential", null, null, "AB123", new string ('a', 51)));
        Reject("non ASCII ID", () => CustomerIdentity.Validate("Residential", "٩٠٠١٠١٥٠٠٩٠٨٧", null, null, null));
        Reject("unknown identity type", () => CustomerIdentity.Validate("Other", null, null, null, null));
        CustomerIdentity.Validate("Residential", null, null, "AB123", "中国");
        Check("Unicode issuing country", true);
    }

    private static void Payments()
    {
        foreach (string value in new[]
        {
            null,
            "",
            "0",
            "0000000000000000",
            "4111111111111112",
            "４１１１１１１１１１１１１１１１",
            "4111111111111111\n"
        }

        )
            Check("invalid card rejected", !PaymentInputValidation.IsValidCardNumber(value));
        Check("Visa test number", PaymentInputValidation.IsValidCardNumber("4111111111111111"));
        Check("Mastercard test number", PaymentInputValidation.IsValidCardNumber("5555555555554444"));
        Check("card normalization", PaymentInputValidation.NormalizeCardNumber("4111-1111 1111-1111") == "4111111111111111");
        Check("bad separators rejected", PaymentInputValidation.NormalizeCardNumber("4111x111111111111") == "");
        foreach (string value in new[]
        {
            null,
            "",
            "12",
            "12345",
            "abc",
            "١٢٣",
            "123\n"
        }

        )
            Check("invalid CVV", !PaymentInputValidation.IsValidCvv(value));
        Check("3 digit CVV", PaymentInputValidation.IsValidCvv("123"));
        Check("4 digit CVV", PaymentInputValidation.IsValidCvv("1234"));
        DateTime today = new DateTime(2026, 11, 16);
        for (int month = 0; month <= 13; month++)
            Check("expiry month " + month, PaymentInputValidation.IsValidExpiry(month, 2026, today) == (month == 11 || month == 12));
        Check("previous year expired", !PaymentInputValidation.IsValidExpiry(12, 2025, today));
        Check("future year", PaymentInputValidation.IsValidExpiry(1, 2027, today));
        Check("valid cardholder", PaymentInputValidation.IsValidName("Élodie O'Brien"));
        Check("punctuation only name rejected", !PaymentInputValidation.IsValidName(".."));
    }

    private static void Policies()
    {
        foreach (string value in new[]
        {
            null,
            "",
            "weak",
            "abcdefgh",
            "ABCDEFGH",
            "Abcdefgh",
            "Abcdefg1"
        }

        )
            Check("weak password rejected", !ValidationHelper.IsValidPassword(value));
        Check("strong password", ValidationHelper.IsValidPassword("Sample123!"));
        foreach (string value in new[]
        {
            "Élodie Dupont",
            "李 明",
            "O'Brien",
            "Anne-Marie"
        }

        )
            Check("international name", ValidationHelper.IsValidName(value));
        foreach (string value in new[]
        {
            "123",
            "<script>",
            "",
            "  "
        }

        )
            Check("invalid name", !ValidationHelper.IsValidName(value));
        foreach (string value in new[]
        {
            "5",
            "20",
            "60"
        }

        )
            Check("valid idle timeout", AdminPolicy.ValidateSetting("SessionIdleMinutes", value) == value);
        foreach (string value in new[]
        {
            "4",
            "61",
            "1.5",
            "abc"
        }

        )
            Reject("invalid idle timeout", () => AdminPolicy.ValidateSetting("SessionIdleMinutes", value));
        Reject("unknown setting", () => AdminPolicy.ValidateSetting("ConnectionString", "secret"));
        Reject("excess date range", () => AdminPolicy.DateRange(DateTime.Today, DateTime.Today.AddDays(367)));
        Reject("reversed date range", () => AdminPolicy.DateRange(DateTime.Today, DateTime.Today.AddDays(-1)));
        foreach (string value in new[]
        {
            "=1+1",
            " +SUM(A1)",
            "-2",
            "@cmd"
        }

        )
            Check("CSV formula escaped", CsvReportWriter.Field(value).StartsWith("\"'"));
        Check("CSV quoting", CsvReportWriter.Field("a,\"b") == "\"a,\"\"b\"");
        Check("CSV null", CsvReportWriter.Field(DBNull.Value) == "\"\"");
        string hash = CryptographyHelper.HashPassword("Sample123!");
        Check("password roundtrip", CryptographyHelper.VerifyPassword("Sample123!", hash));
        Check("wrong password", !CryptographyHelper.VerifyPassword("Wrong123!", hash));
        foreach (string value in new[]
        {
            null,
            "",
            "invalid",
            "PBKDF2:0:AA==:AA==",
            "PBKDF2:100000:bad:bad"
        }

        )
            Check("malformed hash", !CryptographyHelper.VerifyPassword("Sample123!", value));
    }

    private static void Ledger()
    {
        var ledger = new BookingSubmissionLedger();
        int job;
        string token = ledger.Issue(1);
        Check("new token", ledger.TryGetResult(token, 1, out job) && job == 0);
        Check("token owner enforced", !ledger.TryGetResult(token, 2, out job));
        ledger.Complete(token, 1, 20);
        Check("duplicate result retained", ledger.TryGetResult(token, 1, out job) && job == 20);
        Reject("different completion rejected", () => ledger.Complete(token, 1, 21));
        for (int i = 0; i < 40; i++)
            ledger.Issue(1);
        Check("old token eviction", !ledger.TryGetResult(token, 1, out job));
    }

    private static void Dashboard()
    {
        var assembly = Assembly.LoadFrom(Path.GetFullPath("MajorProject.Web/bin/MajorProject.Web.dll"));
        Type type = assembly.GetType("MajorProject.Web.Secure.Customer.Dashboard", true);
        object page = Activator.CreateInstance(type);
        MethodInfo method = type.GetMethod("FindNextService", BindingFlags.NonPublic | BindingFlags.Instance);
        var jobs = new DataTable();
        jobs.Columns.Add("JobID", typeof(int));
        jobs.Columns.Add("JobStatus");
        jobs.Columns.Add("PromiseDate", typeof(DateTime));
        jobs.Rows.Add(1, "Pending", DateTime.Today.AddDays(-5));
        jobs.Rows.Add(2, "Pending", DateTime.Today.AddDays(1));
        jobs.Rows.Add(3, "Completed", DateTime.Today);
        var selected = (DataRow)method.Invoke(page, new object[] { jobs });
        Check("overdue service does not hide upcoming service", (int)selected["JobID"] == 2);
        jobs.Rows.Add(4, "In Progress", DateTime.Today);
        selected = (DataRow)method.Invoke(page, new object[] { jobs });
        Check("today precedes tomorrow", (int)selected["JobID"] == 4);
        jobs.Rows.Clear();
        jobs.Rows.Add(1, "Pending", DateTime.Today.AddDays(-1));
        Check("overdue only means no upcoming service", method.Invoke(page, new object[] { jobs }) == null);
    }
}
