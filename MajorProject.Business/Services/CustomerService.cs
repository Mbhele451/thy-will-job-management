using System;
using System.Data;
using System.Text.RegularExpressions;
using MajorProject.Business.Helpers;
using MajorProject.Data.Data;

namespace MajorProject.Business.Services
{
    public class CustomerService
    {
        private readonly CustomerDataAccess _dataAccess;
        private readonly string connectionString;
        // ============================================================
        // CONSTRUCTOR
        // ============================================================
        public CustomerService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Database connection string is required.", nameof(connectionString));
            }

            this.connectionString = connectionString;
            _dataAccess = new CustomerDataAccess(connectionString);
        }

        // ============================================================
        // GET ALL CUSTOMERS
        // ============================================================
        public DataTable GetAllCustomers()
        {
            return _dataAccess.GetAllCustomers();
        }

        // ============================================================
        // GET CUSTOMER BY ID
        // ============================================================
        public DataTable GetCustomerByID(int customerID)
        {
            if (customerID <= 0)
            {
                throw new ArgumentException("Invalid customer ID.");
            }

            return _dataAccess.GetCustomerByID(customerID);
        }

        // ============================================================
        // SEARCH CUSTOMERS
        // ============================================================
        public DataTable SearchCustomersByName(string name)
        {
            return _dataAccess.SearchCustomersByName(name);
        }

        // ============================================================
        // FIND CUSTOMER BY EMAIL
        // ============================================================
        public bool TryFindCustomerByEmail(string email, out int customerID, out string fullName)
        {
            customerID = 0;
            fullName = string.Empty;
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string normalizedEmail = email.Trim();
            DataTable dt = _dataAccess.GetCustomerByEmail(normalizedEmail);
            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }

            DataRow row = dt.Rows[0];
            customerID = Convert.ToInt32(row["CustomerID"]);
            fullName = row["FullName"]?.ToString() ?? string.Empty;
            return true;
        }

        // ============================================================
        // CREATE CUSTOMER - ADMIN VERSION
        // ============================================================
        /// <summary>
        /// Creates a customer account administratively.
        ///
        /// A secure temporary password is generated.
        /// The plaintext password is returned to the caller
        /// so the application can send it to the customer.
        /// </summary>
        public int CreateCustomer(string fullName, string phoneNumber, string email, string address, string area, string city, string province, string customerType, string southAfricanIDNumber, string companyRegistrationNumber, out string temporaryPassword)
        {
            temporaryPassword = CryptographyHelper.GenerateRandomPassword();
            int id = CreateCustomer(fullName, phoneNumber, email, temporaryPassword, address, area, city, province, customerType, southAfricanIDNumber, companyRegistrationNumber);
            EmailQueueService.QueueAccountWelcome(email.Trim(), email.Trim());
            return id;
        }

        public int CreateCustomer(string fullName, string phoneNumber, string email, string address, string area, string city, string province, string customerType, string sa, string company, string passport, string country, out string temporaryPassword)
        {
            temporaryPassword = CryptographyHelper.GenerateRandomPassword();
            int id = CreateCustomer(fullName, phoneNumber, email, temporaryPassword, address, area, city, province, customerType, sa, company, passport, country);
            EmailQueueService.QueueAccountWelcome(email.Trim(), email.Trim());
            return id;
        }

        // ============================================================
        // CREATE CUSTOMER - CUSTOMER REGISTRATION
        // ============================================================
        public int CreateCustomer(string fullName, string phoneNumber, string email, string password, string address, string area, string city, string province, string customerType, string southAfricanIDNumber, string companyRegistrationNumber)
        {
            return CreateCustomer(fullName, phoneNumber, email, password, address, area, city, province, customerType, southAfricanIDNumber, companyRegistrationNumber, null, null);
        }

        public int CreateCustomer(string fullName, string phoneNumber, string email, string password, string address, string area, string city, string province, string customerType, string southAfricanIDNumber, string companyRegistrationNumber, string passportNumber, string passportCountry)
        {
            return CreateCustomerCore(fullName, phoneNumber, email, password, address, area, city, province, customerType, southAfricanIDNumber, companyRegistrationNumber, passportNumber, passportCountry, 0, null);
        }

        public int CreateCustomerBySysAdmin(int actor, string ip, string fullName, string phoneNumber, string email, string password, string address, string area, string city, string province, string customerType, string southAfricanIDNumber, string companyRegistrationNumber, string passportNumber, string passportCountry)
        {
            if (actor <= 0)
                throw new UnauthorizedAccessException("SysAdmin access is required.");
            int id = CreateCustomerCore(fullName, phoneNumber, email, password, address, area, city, province, customerType, southAfricanIDNumber, companyRegistrationNumber, passportNumber, passportCountry, actor, ip);
            EmailQueueService.QueueAccountWelcome(email.Trim(), email.Trim());
            return id;
        }

        private int CreateCustomerCore(string fullName, string phoneNumber, string email, string password, string address, string area, string city, string province, string customerType, string southAfricanIDNumber, string companyRegistrationNumber, string passportNumber, string passportCountry, int actor, string ip)
        {
            // --------------------------------------------------------
            // NORMALIZE INPUT
            // --------------------------------------------------------
            fullName = (fullName ?? string.Empty).Trim();
            phoneNumber = Regex.Replace(phoneNumber ?? string.Empty, @"\D", string.Empty);
            email = (email ?? string.Empty).Trim();
            password = password ?? string.Empty;
            address = (address ?? string.Empty).Trim();
            area = (area ?? string.Empty).Trim();
            city = (city ?? string.Empty).Trim();
            province = (province ?? string.Empty).Trim();
            customerType = (customerType ?? string.Empty).Trim();
            southAfricanIDNumber = (southAfricanIDNumber ?? string.Empty).Trim();
            companyRegistrationNumber = (companyRegistrationNumber ?? string.Empty).Trim();
            // --------------------------------------------------------
            // REQUIRED FIELDS
            // --------------------------------------------------------
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Customer name is required.");
            }

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new ArgumentException("Phone number is required.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email address is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Full address is required.");
            }

            if (string.IsNullOrWhiteSpace(area))
            {
                throw new ArgumentException("Service area is required.");
            }

            if (string.IsNullOrWhiteSpace(customerType))
            {
                throw new ArgumentException("Customer type is required.");
            }

            // --------------------------------------------------------
            // FULL NAME
            // --------------------------------------------------------
            if (!ValidationHelper.IsValidName(fullName))
            {
                throw new ArgumentException("Please enter a valid full name.");
            }

            if (fullName.Length > 100)
            {
                throw new ArgumentException("Full name is too long.");
            }

            // --------------------------------------------------------
            // PHONE
            // --------------------------------------------------------
            if (!ValidationHelper.IsValidPhoneNumber(phoneNumber))
            {
                throw new ArgumentException("Invalid phone number. Please enter a valid South African 10-digit number.");
            }

            // --------------------------------------------------------
            // EMAIL
            // --------------------------------------------------------
            if (!ValidationHelper.IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email address.");
            }

            if (email.Length > 100)
            {
                throw new ArgumentException("Email address is too long.");
            }

            // --------------------------------------------------------
            // PASSWORD
            // --------------------------------------------------------
            if (!ValidationHelper.IsValidPassword(password))
            {
                throw new ArgumentException("Password must contain at least 8 characters, " + "including an uppercase letter, lowercase letter, " + "number and special character.");
            }

            // --------------------------------------------------------
            // ADDRESS
            // --------------------------------------------------------
            if (address.Length < 5)
            {
                throw new ArgumentException("Please enter a valid full address.");
            }

            if (address.Length > 255)
            {
                throw new ArgumentException("Address is too long.");
            }

            // --------------------------------------------------------
            // CUSTOMER TYPE
            // --------------------------------------------------------
            bool isResidential = customerType.Equals("Residential", StringComparison.OrdinalIgnoreCase);
            bool isBusiness = customerType.Equals("Business", StringComparison.OrdinalIgnoreCase);
            if (!isResidential && !isBusiness)
            {
                throw new ArgumentException("Invalid customer type.");
            }

            // --------------------------------------------------------
            // IDENTITY VALIDATION
            // --------------------------------------------------------
            passportNumber = (passportNumber ?? string.Empty).Trim().ToUpperInvariant();
            passportCountry = (passportCountry ?? string.Empty).Trim().ToUpperInvariant();
            MajorProject.Business.Models.CustomerIdentity.Validate(customerType, southAfricanIDNumber, companyRegistrationNumber, passportNumber, passportCountry);
            // --------------------------------------------------------
            // CITY
            // --------------------------------------------------------
            if (!city.Equals("Durban", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid city.");
            }

            // --------------------------------------------------------
            // PROVINCE
            // --------------------------------------------------------
            if (!province.Equals("KwaZulu-Natal", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid province.");
            }

            // --------------------------------------------------------
            // SERVICE AREA
            // --------------------------------------------------------
            if (!IsValidServiceArea(area))
            {
                throw new ArgumentException("Invalid service area.");
            }

            // --------------------------------------------------------
            // DUPLICATE EMAIL
            // --------------------------------------------------------
            if (_dataAccess.EmailExists(email))
            {
                throw new InvalidOperationException("An account with this email address already exists.");
            }

            // --------------------------------------------------------
            // DUPLICATE PHONE
            // --------------------------------------------------------
            // Phone numbers are not unique identifiers; shared household contacts are allowed.
            // --------------------------------------------------------
            // DUPLICATE SA ID
            // --------------------------------------------------------
            if (isResidential && !string.IsNullOrWhiteSpace(southAfricanIDNumber) && _dataAccess.SouthAfricanIDExists(southAfricanIDNumber))
            {
                throw new InvalidOperationException("This South African ID number is already registered.");
            }

            // --------------------------------------------------------
            // DUPLICATE COMPANY NUMBER
            // --------------------------------------------------------
            if (isBusiness && _dataAccess.CompanyRegistrationNumberExists(companyRegistrationNumber))
            {
                throw new InvalidOperationException("This company registration number is already registered.");
            }

            // --------------------------------------------------------
            // HASH PASSWORD
            // --------------------------------------------------------
            string passwordHash = CryptographyHelper.HashPassword(password);
            // --------------------------------------------------------
            // CREATE CUSTOMER
            // --------------------------------------------------------
            if (actor > 0)
                return new SystemAdminDataAccess(connectionString).CreateCustomer(actor, ip, fullName, phoneNumber, email, address, area, city, province, customerType, southAfricanIDNumber, companyRegistrationNumber, passwordHash, passportNumber, passportCountry);
            try
            {
                int customerID = _dataAccess.CreateCustomer(fullName, phoneNumber, email, address, area, city, province, customerType, southAfricanIDNumber, companyRegistrationNumber, passwordHash, DateTime.Now, passportNumber, passportCountry);
                return customerID;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                    throw new InvalidOperationException("This email or identity document is already registered.");
                throw;
            }
        }

        // ============================================================
        // UPDATE CUSTOMER PROFILE
        // ============================================================
        /// <summary>
        /// Updates editable customer profile information.
        ///
        /// Customer type and identity information are deliberately
        /// not changed from the profile page.
        /// </summary>
        public void UpdateCustomer(int customerID, string phoneNumber, string email, string address, string area)
        {
            if (customerID <= 0)
            {
                throw new ArgumentException("Invalid customer ID.");
            }

            phoneNumber = Regex.Replace(phoneNumber ?? string.Empty, @"\D", string.Empty);
            email = (email ?? string.Empty).Trim();
            address = (address ?? string.Empty).Trim();
            area = (area ?? string.Empty).Trim();
            // --------------------------------------------------------
            // REQUIRED FIELDS
            // --------------------------------------------------------
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new ArgumentException("Phone number is required.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email address is required.");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Full address is required.");
            }

            if (string.IsNullOrWhiteSpace(area))
            {
                throw new ArgumentException("Service area is required.");
            }

            // --------------------------------------------------------
            // PHONE
            // --------------------------------------------------------
            if (!ValidationHelper.IsValidPhoneNumber(phoneNumber))
            {
                throw new ArgumentException("Invalid phone number.");
            }

            // --------------------------------------------------------
            // EMAIL
            // --------------------------------------------------------
            if (!ValidationHelper.IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email address.");
            }

            if (email.Length > 100)
            {
                throw new ArgumentException("Email address is too long.");
            }

            // --------------------------------------------------------
            // ADDRESS
            // --------------------------------------------------------
            if (address.Length < 5)
            {
                throw new ArgumentException("Please enter a valid full address.");
            }

            if (address.Length > 255)
            {
                throw new ArgumentException("Address is too long.");
            }

            // --------------------------------------------------------
            // SERVICE AREA
            // --------------------------------------------------------
            if (!IsValidServiceArea(area))
            {
                throw new ArgumentException("Invalid service area.");
            }

            // --------------------------------------------------------
            // DUPLICATE PHONE
            // --------------------------------------------------------
            // Phone numbers are not unique identifiers; shared household contacts are allowed.
            // --------------------------------------------------------
            // DUPLICATE EMAIL
            // --------------------------------------------------------
            if (_dataAccess.IsEmailExistsForOther(email, customerID))
            {
                throw new InvalidOperationException("This email address is already registered to another customer.");
            }

            // --------------------------------------------------------
            // UPDATE
            // --------------------------------------------------------
            int affectedRows = _dataAccess.UpdateCustomer(customerID, phoneNumber, email, address, area);
            if (affectedRows == 0)
            {
                throw new InvalidOperationException("The customer profile could not be updated.");
            }
        }

        // ============================================================
        // DEACTIVATE CUSTOMER
        // ============================================================
        public void DeactivateCustomer(int customerID)
        {
            if (customerID <= 0)
            {
                throw new ArgumentException("Invalid customer ID.");
            }

            ValidateAccountStatus("Deactivated");
            int affectedRows = _dataAccess.UpdateAccountStatus(customerID, "Deactivated");
            if (affectedRows == 0)
            {
                throw new InvalidOperationException("The customer could not be deactivated.");
            }
        }

        // ============================================================
        // ACTIVATE CUSTOMER
        // ============================================================
        public void ActivateCustomer(int customerID)
        {
            if (customerID <= 0)
            {
                throw new ArgumentException("Invalid customer ID.");
            }

            ValidateAccountStatus("Active");
            int affectedRows = _dataAccess.UpdateAccountStatus(customerID, "Active");
            if (affectedRows == 0)
            {
                throw new InvalidOperationException("The customer could not be activated.");
            }
        }

        // ============================================================
        // CHANGE PASSWORD
        // ============================================================
        /// <summary>
        /// Changes the password for an authenticated customer.
        ///
        /// The current password is verified before the new password
        /// is hashed and stored.
        /// </summary>
        public void ChangePassword(int customerID, string currentPassword, string newPassword)
        {
            // --------------------------------------------------------
            // CUSTOMER ID
            // --------------------------------------------------------
            if (customerID <= 0)
            {
                throw new ArgumentException("Invalid customer ID.");
            }

            // --------------------------------------------------------
            // CURRENT PASSWORD
            // --------------------------------------------------------
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                throw new ArgumentException("Current password is required.");
            }

            // --------------------------------------------------------
            // NEW PASSWORD
            // --------------------------------------------------------
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentException("New password is required.");
            }

            // --------------------------------------------------------
            // GET CURRENT HASH
            // --------------------------------------------------------
            string storedPasswordHash = _dataAccess.GetCustomerPasswordHash(customerID);
            if (string.IsNullOrWhiteSpace(storedPasswordHash))
            {
                throw new InvalidOperationException("The customer password could not be found.");
            }

            // --------------------------------------------------------
            // VERIFY CURRENT PASSWORD
            // --------------------------------------------------------
            bool currentPasswordIsValid = CryptographyHelper.VerifyPassword(currentPassword, storedPasswordHash);
            if (!currentPasswordIsValid)
            {
                throw new InvalidOperationException("The current password is incorrect.");
            }

            // --------------------------------------------------------
            // VALIDATE NEW PASSWORD
            // --------------------------------------------------------
            if (!ValidationHelper.IsValidPassword(newPassword))
            {
                throw new ArgumentException("Password must contain at least 8 characters, " + "including an uppercase letter, lowercase letter, " + "number and special character.");
            }

            // --------------------------------------------------------
            // PREVENT PASSWORD REUSE
            // --------------------------------------------------------
            if (CryptographyHelper.VerifyPassword(newPassword, storedPasswordHash))
            {
                throw new ArgumentException("Your new password must be different from your current password.");
            }

            // --------------------------------------------------------
            // HASH NEW PASSWORD
            // --------------------------------------------------------
            string newPasswordHash = CryptographyHelper.HashPassword(newPassword);
            // --------------------------------------------------------
            // UPDATE PASSWORD
            // --------------------------------------------------------
            int affectedRows = _dataAccess.UpdateCustomerPassword(customerID, newPasswordHash);
            if (affectedRows == 0)
            {
                throw new InvalidOperationException("Your password could not be changed.");
            }
        }

        // ============================================================
        // VALIDATE SERVICE AREA
        // ============================================================
        private bool IsValidServiceArea(string area)
        {
            string[] allowedAreas =
            {
                "Umhlanga",
                "Bearea",
                "Morningside",
                "KwaMashu",
                "Durban North",
                "PineTown"
            };
            foreach (string allowedArea in allowedAreas)
            {
                if (area.Equals(allowedArea, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // ============================================================
        // VALIDATE ACCOUNT STATUS
        // ============================================================
        private static void ValidateAccountStatus(string accountStatus)
        {
            if (!accountStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) && !accountStatus.Equals("Deactivated", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid customer account status.");
            }
        }
    }
}
