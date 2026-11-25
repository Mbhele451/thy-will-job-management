using System;
using System.Data;
using System.Data.SqlClient;

namespace MajorProject.Data.Data
{
    public class PaymentDataAccess
    {
        private readonly string connectionString;
        public PaymentDataAccess(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is required.");
            this.connectionString = connectionString;
        }

        // ==========================================
        // ADD CUSTOMER PAYMENT CARD
        // ==========================================
        public DataTable AddCustomerPaymentCard(int customerID, string cardholderName, string cardBrand, string lastFourDigits, byte expiryMonth, short expiryYear)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("dbo.usp_AddCustomerPaymentCard", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;
                command.Parameters.Add("@CardholderName", SqlDbType.NVarChar, 100).Value = cardholderName;
                command.Parameters.Add("@CardBrand", SqlDbType.VarChar, 20).Value = cardBrand;
                command.Parameters.Add("@LastFourDigits", SqlDbType.Char, 4).Value = lastFourDigits;
                command.Parameters.Add("@ExpiryMonth", SqlDbType.TinyInt).Value = expiryMonth;
                command.Parameters.Add("@ExpiryYear", SqlDbType.SmallInt).Value = expiryYear;
                DataTable result = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }

                return result;
            }
        }

        // ==========================================
        // GET CUSTOMER PAYMENT CARDS
        // ==========================================
        public DataTable GetCustomerPaymentCards(int customerID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("dbo.usp_GetCustomerPaymentCards", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;
                DataTable result = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }

                return result;
            }
        }

        // ==========================================
        // GET CARD BY TOKEN
        // ==========================================
        public DataTable GetCustomerPaymentCardByToken(int customerID, Guid cardToken)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("dbo.usp_GetCustomerPaymentCardByToken", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;
                command.Parameters.Add("@CardToken", SqlDbType.UniqueIdentifier).Value = cardToken;
                DataTable result = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }

                return result;
            }
        }

        // ==========================================
        // DEACTIVATE CUSTOMER PAYMENT CARD
        // ==========================================
        public bool DeactivateCustomerPaymentCard(int customerID, Guid cardToken)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("dbo.usp_DeactivateCustomerPaymentCard", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;
                command.Parameters.Add("@CardToken", SqlDbType.UniqueIdentifier).Value = cardToken;
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Convert.ToBoolean(reader["IsSuccessful"]);
                    }
                }
            }

            return false;
        }

        // ==========================================
        // PROCESS SIMULATED INVOICE PAYMENT
        // ==========================================
        public DataTable ProcessSimulatedInvoicePayment(int jobID, int customerID, string paymentMethod)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("dbo.usp_ProcessSimulatedInvoicePayment", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@JobID", SqlDbType.Int).Value = jobID;
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;
                command.Parameters.Add("@PaymentMethod", SqlDbType.VarChar, 30).Value = paymentMethod;
                return ExecuteServiceInvoiceOperation(command, jobID, customerID, false);
            }
        }

        public DataTable GetCustomerInvoicePaymentStatus(int jobID, int customerID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("dbo.usp_GetCustomerInvoicePaymentStatus", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@JobID", SqlDbType.Int).Value = jobID;
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;
                DataTable result = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }

                return result;
            }
        }

        public DataTable GetCustomerInvoicePaymentReceipt(int jobID, int customerID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("dbo.usp_GetCustomerInvoicePaymentReceipt", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@JobID", SqlDbType.Int).Value = jobID;
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;
                return ExecuteServiceInvoiceOperation(command, jobID, customerID, true);
            }
        }

        // PROCESS STAFF CASH PAYMENT
        public DataTable ProcessStaffCashPayment(int jobID, decimal amountReceived, int staffID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("dbo.usp_ProcessStaffCashPayment", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@JobID", SqlDbType.Int).Value = jobID;
                SqlParameter amountParameter = command.Parameters.Add("@AmountReceived", SqlDbType.Decimal);
                amountParameter.Precision = 10;
                amountParameter.Scale = 2;
                amountParameter.Value = amountReceived;
                command.Parameters.Add("@StaffID", SqlDbType.Int).Value = staffID;
                return ExecuteServiceInvoiceOperation(command, jobID, null, false);
            }
        }

        private static DataTable ExecuteServiceInvoiceOperation(SqlCommand command, int jobID, int? customerID, bool receipt)
        {
            SqlConnection connection = command.Connection;
            connection.Open();
            using (SqlTransaction transaction = connection.BeginTransaction())
            using (SqlCommand check = new SqlCommand(@"SELECT j.JobStatus,
                CASE WHEN EXISTS(SELECT 1 FROM dbo.Payments p WHERE p.JobID=j.JobID AND p.PaymentStatus='Paid') THEN 1 ELSE 0 END
                FROM dbo.Jobs j WITH(UPDLOCK,HOLDLOCK)
                WHERE j.JobID=@job AND (@customer IS NULL OR j.CustomerID=@customer)", connection, transaction))
            {
                check.Parameters.Add("@job", SqlDbType.Int).Value = jobID;
                check.Parameters.Add("@customer", SqlDbType.Int).Value = (object)customerID ?? DBNull.Value;
                using (SqlDataReader row = check.ExecuteReader())
                {
                    if (!row.Read())
                        throw new InvalidOperationException("Invoice not found.");
                    string status = row.GetString(0);
                    bool paid = row.GetInt32(1) == 1;
                    if (receipt)
                    {
                        if (!paid || (status != "Completed" && status != "Archived"))
                            return new DataTable();
                    }
                    else
                    {
                        if (status != "Completed")
                            throw new InvalidOperationException("Only completed jobs can be paid.");
                        if (paid)
                            throw new InvalidOperationException("This invoice has already been paid.");
                    }
                }

                command.Transaction = transaction;
                var result = new DataTable();
                using (var adapter = new SqlDataAdapter(command))
                    adapter.Fill(result);
                transaction.Commit();
                return result;
            }
        }
        // GET STAFF CASH PAYMENT DETAILS
        public DataTable GetStaffCashPaymentDetails(int jobID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("dbo.usp_GetStaffCashPaymentDetails", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@JobID", SqlDbType.Int).Value = jobID;
                DataTable result = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }

                return result;
            }
        }
    }
}
