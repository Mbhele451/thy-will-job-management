using System;
using System.Data;
using System.Data.SqlClient;

namespace MajorProject.Data.Data
{
    public class CustomerDataAccess : DataAccessBase
    {
        public CustomerDataAccess(string connectionString) : base(connectionString)
        {
        }

        // ============================================================
        // GET ALL CUSTOMERS
        // ============================================================
        public DataTable GetAllCustomers()
        {
            const string query = @"
            SELECT
                CustomerID,
                FullName,
                PhoneNumber,
                EmailAddress,
                FullAddress,
                Area,
                City,
                Province,
                CustomerType,
                SouthAfricanIDNumber,
                CompanyRegistrationNumber,
                PassportNumber,
                PassportCountry,
                DateRegistered,
                AccountStatus
            FROM Customers
            ORDER BY FullName";
            return ExecuteQuery(query);
        }

        // ============================================================
        // GET CUSTOMER BY ID
        // ============================================================
        public DataTable GetCustomerByID(int customerID)
        {
            const string query = @"
            SELECT
                CustomerID,
                FullName,
                PhoneNumber,
                EmailAddress,
                FullAddress,
                Area,
                City,
                Province,
                CustomerType,
                SouthAfricanIDNumber,
                CompanyRegistrationNumber,
                PassportNumber,
                PassportCountry,
                DateRegistered,
                AccountStatus
            FROM Customers
            WHERE CustomerID = @CustomerID";
            return ExecuteQuery(query, new SqlParameter("@CustomerID", customerID));
        }

        // ============================================================
        // GET CUSTOMER BY EMAIL
        // ============================================================
        public DataTable GetCustomerByEmail(string email)
        {
            const string query = @"
            SELECT TOP 1
                CustomerID,
                FullName,
                PhoneNumber,
                EmailAddress,
                FullAddress,
                Area,
                City,
                Province,
                CustomerType,
                SouthAfricanIDNumber,
                CompanyRegistrationNumber,
                PassportNumber,
                PassportCountry,
                DateRegistered,
                AccountStatus,
                PasswordHash,
                ResetOTP,
                ResetOTPExpiry
            FROM Customers
            WHERE EmailAddress = @Email";
            return ExecuteQuery(query, new SqlParameter("@Email", email));
        }

        // ============================================================
        // SEARCH CUSTOMERS
        // ============================================================
        public DataTable SearchCustomersByName(string name)
        {
            const string query = @"
            SELECT
                CustomerID,
                FullName,
                PhoneNumber,
                EmailAddress,
                FullAddress,
                Area,
                City,
                Province,
                CustomerType,
                SouthAfricanIDNumber,
                CompanyRegistrationNumber,
                PassportNumber,
                PassportCountry,
                DateRegistered,
                AccountStatus
            FROM Customers
            WHERE FullName LIKE @Name
            ORDER BY FullName";
            return ExecuteQuery(query, new SqlParameter("@Name", "%" + (name ?? string.Empty).Trim() + "%"));
        }

        // ============================================================
        // EMAIL EXISTS
        // ============================================================
        public bool EmailExists(string email)
        {
            const string query = @"
            SELECT COUNT(*)
            FROM Customers
            WHERE EmailAddress = @EmailAddress";
            object result = ExecuteScalar(query, new SqlParameter("@EmailAddress", email));
            return result != null && Convert.ToInt32(result) > 0;
        }

        // ============================================================
        // EMAIL EXISTS FOR ANOTHER CUSTOMER
        // ============================================================
        public bool IsEmailExistsForOther(string email, int excludeCustomerID)
        {
            const string query = @"
            SELECT COUNT(*)
            FROM Customers
            WHERE EmailAddress = @EmailAddress
              AND CustomerID <> @ExcludeID";
            object result = ExecuteScalar(query, new SqlParameter("@EmailAddress", email), new SqlParameter("@ExcludeID", excludeCustomerID));
            return result != null && Convert.ToInt32(result) > 0;
        }

        // ============================================================
        // PHONE EXISTS
        // ============================================================
        public bool PhoneNumberExists(string phoneNumber)
        {
            const string query = @"
            SELECT COUNT(*)
            FROM Customers
            WHERE PhoneNumber = @PhoneNumber";
            object result = ExecuteScalar(query, new SqlParameter("@PhoneNumber", phoneNumber));
            return result != null && Convert.ToInt32(result) > 0;
        }

        // ============================================================
        // PHONE EXISTS FOR ANOTHER CUSTOMER
        // ============================================================
        public bool IsPhoneNumberExistsForOther(string phoneNumber, int excludeCustomerID)
        {
            const string query = @"
            SELECT COUNT(*)
            FROM Customers
            WHERE PhoneNumber = @PhoneNumber
              AND CustomerID <> @ExcludeID";
            object result = ExecuteScalar(query, new SqlParameter("@PhoneNumber", phoneNumber), new SqlParameter("@ExcludeID", excludeCustomerID));
            return result != null && Convert.ToInt32(result) > 0;
        }

        // ============================================================
        // SOUTH AFRICAN ID EXISTS
        // ============================================================
        public bool SouthAfricanIDExists(string southAfricanIDNumber)
        {
            const string query = @"
            SELECT COUNT(*)
            FROM Customers
            WHERE SouthAfricanIDNumber = @SouthAfricanIDNumber";
            object result = ExecuteScalar(query, new SqlParameter("@SouthAfricanIDNumber", southAfricanIDNumber));
            return result != null && Convert.ToInt32(result) > 0;
        }

        // ============================================================
        // SOUTH AFRICAN ID EXISTS FOR ANOTHER CUSTOMER
        // ============================================================
        public bool IsSouthAfricanIDExistsForOther(string southAfricanIDNumber, int excludeCustomerID)
        {
            const string query = @"
            SELECT COUNT(*)
            FROM Customers
            WHERE SouthAfricanIDNumber = @SouthAfricanIDNumber
              AND CustomerID <> @ExcludeID";
            object result = ExecuteScalar(query, new SqlParameter("@SouthAfricanIDNumber", southAfricanIDNumber), new SqlParameter("@ExcludeID", excludeCustomerID));
            return result != null && Convert.ToInt32(result) > 0;
        }

        // ============================================================
        // COMPANY REGISTRATION EXISTS
        // ============================================================
        public bool CompanyRegistrationNumberExists(string companyRegistrationNumber)
        {
            const string query = @"
            SELECT COUNT(*)
            FROM Customers
            WHERE CompanyRegistrationNumber =
                  @CompanyRegistrationNumber";
            object result = ExecuteScalar(query, new SqlParameter("@CompanyRegistrationNumber", companyRegistrationNumber));
            return result != null && Convert.ToInt32(result) > 0;
        }

        // ============================================================
        // COMPANY REGISTRATION EXISTS FOR ANOTHER CUSTOMER
        // ============================================================
        public bool IsCompanyRegistrationNumberExistsForOther(string companyRegistrationNumber, int excludeCustomerID)
        {
            const string query = @"
            SELECT COUNT(*)
            FROM Customers
            WHERE CompanyRegistrationNumber =
                  @CompanyRegistrationNumber
              AND CustomerID <> @ExcludeID";
            object result = ExecuteScalar(query, new SqlParameter("@CompanyRegistrationNumber", companyRegistrationNumber), new SqlParameter("@ExcludeID", excludeCustomerID));
            return result != null && Convert.ToInt32(result) > 0;
        }

        // ============================================================
        // CREATE CUSTOMER
        // ============================================================
        public int CreateCustomer(string fullName, string phoneNumber, string email, string address, string area, string city, string province, string customerType, string southAfricanIDNumber, string companyRegistrationNumber, string passwordHash, DateTime dateRegistered)
        {
            return CreateCustomer(fullName, phoneNumber, email, address, area, city, province, customerType, southAfricanIDNumber, companyRegistrationNumber, passwordHash, dateRegistered, null, null);
        }

        public int CreateCustomer(string fullName, string phoneNumber, string email, string address, string area, string city, string province, string customerType, string southAfricanIDNumber, string companyRegistrationNumber, string passwordHash, DateTime dateRegistered, string passportNumber, string passportCountry)
        {
            const string query = @"
            INSERT INTO Customers
            (
                FullName,
                PhoneNumber,
                EmailAddress,
                FullAddress,
                Area,
                City,
                Province,
                CustomerType,
                SouthAfricanIDNumber,
                CompanyRegistrationNumber,
                PassportNumber,
                PassportCountry,
                DateRegistered,
                PasswordHash,
                AccountStatus
            )
            VALUES
            (
                @FullName,
                @PhoneNumber,
                @EmailAddress,
                @FullAddress,
                @Area,
                @City,
                @Province,
                @CustomerType,
                @SouthAfricanIDNumber,
                @CompanyRegistrationNumber,
                @PassportNumber,
                @PassportCountry,
                @DateRegistered,
                @PasswordHash,
                @AccountStatus
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);";
            string guarded = "SET XACT_ABORT ON; BEGIN TRY BEGIN TRANSACTION; IF EXISTS(SELECT 1 FROM dbo.Staff WITH(UPDLOCK,HOLDLOCK) WHERE EmailAddress=@EmailAddress OR Username=@EmailAddress) THROW 51123,'This email conflicts with an existing staff login.',1; " + query + " COMMIT; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;";
            object result = ExecuteScalar(guarded, new SqlParameter("@FullName", fullName), new SqlParameter("@PhoneNumber", phoneNumber), new SqlParameter("@EmailAddress", email), new SqlParameter("@FullAddress", address), new SqlParameter("@Area", area), new SqlParameter("@City", city), new SqlParameter("@Province", province), new SqlParameter("@CustomerType", customerType), new SqlParameter("@SouthAfricanIDNumber", string.IsNullOrWhiteSpace(southAfricanIDNumber) ? (object)DBNull.Value : southAfricanIDNumber), new SqlParameter("@CompanyRegistrationNumber", string.IsNullOrWhiteSpace(companyRegistrationNumber) ? (object)DBNull.Value : companyRegistrationNumber), new SqlParameter("@PassportNumber", SqlDbType.VarChar, 20) { Value = string.IsNullOrWhiteSpace(passportNumber) ? (object)DBNull.Value : passportNumber }, new SqlParameter("@PassportCountry", SqlDbType.NVarChar, 50) { Value = string.IsNullOrWhiteSpace(passportCountry) ? (object)DBNull.Value : passportCountry }, new SqlParameter("@DateRegistered", dateRegistered), new SqlParameter("@PasswordHash", passwordHash), new SqlParameter("@AccountStatus", "Active"));
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // ============================================================
        // UPDATE CUSTOMER
        // ============================================================
        public int UpdateCustomer(int customerID, string phoneNumber, string email, string address, string area)
        {
            const string query = @"
            UPDATE Customers
            SET
                PhoneNumber = @PhoneNumber,
                EmailAddress = @EmailAddress,
                FullAddress = @FullAddress,
                Area = @Area
            WHERE CustomerID = @CustomerID";
            string guarded = "SET XACT_ABORT ON; BEGIN TRY BEGIN TRANSACTION; IF EXISTS(SELECT 1 FROM dbo.Staff WITH(UPDLOCK,HOLDLOCK) WHERE EmailAddress=@EmailAddress OR Username=@EmailAddress) THROW 51123,'This email conflicts with an existing staff login.',1; " + query + "; COMMIT; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;";
            return ExecuteNonQuery(guarded, new SqlParameter("@PhoneNumber", phoneNumber), new SqlParameter("@EmailAddress", (object)email ?? DBNull.Value), new SqlParameter("@FullAddress", (object)address ?? DBNull.Value), new SqlParameter("@Area", (object)area ?? DBNull.Value), new SqlParameter("@CustomerID", customerID));
        }

        // ============================================================
        // ACCOUNT STATUS
        // ============================================================
        public int UpdateAccountStatus(int customerID, string accountStatus)
        {
            const string query = @"
            UPDATE Customers
            SET AccountStatus = @AccountStatus
            WHERE CustomerID = @CustomerID";
            return ExecuteNonQuery(query, new SqlParameter("@AccountStatus", accountStatus), new SqlParameter("@CustomerID", customerID));
        }

        // ============================================================
        // PASSWORD
        // ============================================================
        public string GetCustomerPasswordHash(int customerID)
        {
            const string query = @"
            SELECT PasswordHash
            FROM Customers
            WHERE CustomerID = @CustomerID";
            object result = ExecuteScalar(query, new SqlParameter("@CustomerID", customerID));
            if (result == null || result == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToString(result);
        }

        public int UpdateCustomerPassword(int customerID, string passwordHash)
        {
            const string query = @"
            UPDATE Customers
            SET PasswordHash = @PasswordHash
            WHERE CustomerID = @CustomerID; UPDATE dbo.AppAccountSecurity SET SecurityVersion=SecurityVersion+1,UpdatedAtUtc=SYSUTCDATETIME() WHERE CustomerID=@CustomerID; UPDATE dbo.AppUserSessions SET EndedAtUtc=SYSUTCDATETIME(),EndReason='Password changed' WHERE CustomerID=@CustomerID AND EndedAtUtc IS NULL";
            return ExecuteNonQuery(query, new SqlParameter("@PasswordHash", passwordHash), new SqlParameter("@CustomerID", customerID));
        }
    }
}
