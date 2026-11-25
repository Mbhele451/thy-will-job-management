using System;
using System.Security.Cryptography;

namespace MajorProject.Business.Helpers
{
    /// <summary>
    /// Provides secure password hashing and verification utilities.
    ///
    /// Password format:
    /// PBKDF2:iterations:saltBase64:hashBase64
    ///
    /// Example:
    /// PBKDF2:100000:base64Salt:base64Hash
    ///
    /// The entire value is stored in Staff.PasswordHash.
    /// </summary>
    public static class CryptographyHelper
    {
        private const int SaltSizeBytes = 16;
        private const int HashSizeBytes = 32;
        private const int Iterations = 100000;
        /// <summary>
        /// Creates a secure salted PBKDF2 password hash.
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));
            byte[] salt = new byte[SaltSizeBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                hash = pbkdf2.GetBytes(HashSizeBytes);
            }

            return string.Format("PBKDF2:{0}:{1}:{2}", Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        /// <summary>
        /// Verifies a plaintext password against a stored PBKDF2 password hash.
        /// </summary>
        public static bool VerifyPassword(string password, string storedPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;
            if (string.IsNullOrWhiteSpace(storedPasswordHash))
                return false;
            string[] parts = storedPasswordHash.Split(':');
            if (parts.Length != 4)
                return false;
            if (!string.Equals(parts[0], "PBKDF2", StringComparison.Ordinal))
                return false;
            int iterations;
            if (!int.TryParse(parts[1], out iterations))
                return false;
            if (iterations <= 0)
                return false;
            byte[] salt;
            byte[] expectedHash;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expectedHash = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            if (salt.Length != SaltSizeBytes)
                return false;
            if (expectedHash.Length != HashSizeBytes)
                return false;
            byte[] actualHash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                actualHash = pbkdf2.GetBytes(HashSizeBytes);
            }

            return FixedTimeEquals(actualHash, expectedHash);
        }

        /// <summary>
        /// Performs a constant-time byte comparison.
        /// </summary>
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null)
                return false;
            if (a.Length != b.Length)
                return false;
            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }

        /// <summary>
        /// Generates a cryptographically secure random password that always
        /// satisfies the application's password policy:
        /// - Minimum 8 characters
        /// - At least 1 lowercase letter
        /// - At least 1 uppercase letter
        /// - At least 1 digit
        /// - At least 1 special character
        /// </summary>
        public static string GenerateRandomPassword(int length = 12)
        {
            if (length < 8)
                throw new ArgumentException("Password length must be at least 8 characters.", nameof(length));
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string special = "!@#$%^&*";
            const string allCharacters = lowercase + uppercase + digits + special;
            char[] result = new char[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                // Guarantee at least one character from each required category.
                result[0] = GetRandomCharacter(lowercase, rng);
                result[1] = GetRandomCharacter(uppercase, rng);
                result[2] = GetRandomCharacter(digits, rng);
                result[3] = GetRandomCharacter(special, rng);
                // Fill the remaining positions from all allowed characters.
                for (int i = 4; i < length; i++)
                {
                    result[i] = GetRandomCharacter(allCharacters, rng);
                }

                // Shuffle the password so the required characters are not always
                // in the same positions.
                Shuffle(result, rng);
            }

            return new string (result);
        }

        /// <summary>
        /// Gets a cryptographically secure random character from the supplied character set.
        /// Uses rejection sampling to avoid modulo bias.
        /// </summary>
        private static char GetRandomCharacter(string characters, RandomNumberGenerator rng)
        {
            byte[] buffer = new byte[4];
            uint maxUnbiasedValue = uint.MaxValue - (uint.MaxValue % (uint)characters.Length);
            uint value;
            do
            {
                rng.GetBytes(buffer);
                value = BitConverter.ToUInt32(buffer, 0);
            }
            while (value >= maxUnbiasedValue);
            return characters[(int)(value % (uint)characters.Length)];
        }

        /// <summary>
        /// Cryptographically shuffles the characters using Fisher-Yates.
        /// </summary>
        private static void Shuffle(char[] array, RandomNumberGenerator rng)
        {
            byte[] buffer = new byte[4];
            for (int i = array.Length - 1; i > 0; i--)
            {
                uint maxUnbiasedValue = uint.MaxValue - (uint.MaxValue % (uint)(i + 1));
                uint value;
                do
                {
                    rng.GetBytes(buffer);
                    value = BitConverter.ToUInt32(buffer, 0);
                }
                while (value >= maxUnbiasedValue);
                int j = (int)(value % (uint)(i + 1));
                char temporary = array[i];
                array[i] = array[j];
                array[j] = temporary;
            }
        }

        public static string GenerateOtp()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999).ToString("D6");
        }
    }
}
