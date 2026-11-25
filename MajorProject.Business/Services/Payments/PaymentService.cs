using MajorProject.Business.Models.Payments;
using MajorProject.Data.Data;
using System;
using System.Data;

namespace MajorProject.Business.Services.Payments
{
    public class PaymentService
    {
        private readonly PaymentDataAccess paymentDataAccess;
        public PaymentService(string connectionString)
        {
            paymentDataAccess = new PaymentDataAccess(connectionString);
        }

        // ==========================================
        // ADD CUSTOMER PAYMENT CARD
        // ==========================================
        public DataTable AddCustomerPaymentCard(int customerID, string cardholderName, string cardBrand, string lastFourDigits, byte expiryMonth, short expiryYear)
        {
            if (customerID <= 0)
                throw new ArgumentException("Invalid customer.");
            if (!PaymentInputValidation.IsValidName((cardholderName ?? string.Empty).Trim()))
                throw new ArgumentException("Enter a valid cardholder name (2 to 100 characters).");
            if (cardBrand != "Visa" && cardBrand != "Mastercard" && cardBrand != "Other")
                throw new ArgumentException("Invalid card brand.");
            if (lastFourDigits == null || !System.Text.RegularExpressions.Regex.IsMatch(lastFourDigits, @"\A[0-9]{4}\z"))
                throw new ArgumentException("Invalid card ending digits.");
            if (!PaymentInputValidation.IsValidExpiry(expiryMonth, expiryYear, DateTime.Today))
                throw new ArgumentException("The card expiry date is invalid or expired.");
            return paymentDataAccess.AddCustomerPaymentCard(customerID, cardholderName.Trim(), cardBrand, lastFourDigits, expiryMonth, expiryYear);
        }

        // ==========================================
        // GET CUSTOMER PAYMENT CARDS
        // ==========================================
        public DataTable GetCustomerPaymentCards(int customerID)
        {
            if (customerID <= 0)
                throw new ArgumentException("Invalid customer.");
            return paymentDataAccess.GetCustomerPaymentCards(customerID);
        }

        // ==========================================
        // GET CARD BY TOKEN
        // ==========================================
        public DataTable GetCustomerPaymentCardByToken(int customerID, Guid cardToken)
        {
            if (customerID <= 0)
                throw new ArgumentException("Invalid customer.");
            if (cardToken == Guid.Empty)
                throw new ArgumentException("Invalid card token.");
            return paymentDataAccess.GetCustomerPaymentCardByToken(customerID, cardToken);
        }

        // ==========================================
        // DEACTIVATE CUSTOMER PAYMENT CARD
        // ==========================================
        public bool DeactivateCustomerPaymentCard(int customerID, Guid cardToken)
        {
            if (customerID <= 0)
                throw new ArgumentException("Invalid customer.");
            if (cardToken == Guid.Empty)
                throw new ArgumentException("Invalid card token.");
            return paymentDataAccess.DeactivateCustomerPaymentCard(customerID, cardToken);
        }

        // ==========================================
        // PROCESS SIMULATED INVOICE PAYMENT
        // ==========================================
        public DataTable ProcessSimulatedInvoicePayment(int jobID, int customerID, string paymentMethod)
        {
            if (jobID <= 0)
                throw new ArgumentException("Invalid invoice.");
            if (customerID <= 0)
                throw new ArgumentException("Invalid customer.");
            string[] allowedMethods =
            {
                "Card",
                "EFT",
                "MobileWallet"
            };
            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new ArgumentException("Select a payment method.");
            bool validMethod = false;
            foreach (string method in allowedMethods)
            {
                if (method.Equals(paymentMethod.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    validMethod = true;
                    paymentMethod = method;
                    break;
                }
            }

            if (!validMethod)
                throw new ArgumentException("Invalid payment method.");
            return paymentDataAccess.ProcessSimulatedInvoicePayment(jobID, customerID, paymentMethod);
        }

        public DataTable GetCustomerInvoicePaymentStatus(int jobID, int customerID)
        {
            if (jobID <= 0)
                throw new ArgumentException("Invalid invoice.");
            if (customerID <= 0)
                throw new ArgumentException("Invalid customer.");
            return paymentDataAccess.GetCustomerInvoicePaymentStatus(jobID, customerID);
        }

        public DataTable GetCustomerInvoicePaymentReceipt(int jobID, int customerID)
        {
            if (jobID <= 0)
                throw new ArgumentException("Invalid invoice ID.");
            if (customerID <= 0)
                throw new ArgumentException("Invalid customer ID.");
            return paymentDataAccess.GetCustomerInvoicePaymentReceipt(jobID, customerID);
        }

        public PaymentResult GetCustomerInvoicePaymentReceiptResult(int jobID, int customerID)
        {
            DataTable receiptData = GetCustomerInvoicePaymentReceipt(jobID, customerID);
            if (receiptData == null || receiptData.Rows.Count == 0)
                return null;
            DataRow row = receiptData.Rows[0];
            PaymentResult result = new PaymentResult();
            result.PaymentID = Convert.ToInt64(row["PaymentID"]);
            result.PaymentAttemptID = Convert.ToInt64(row["PaymentAttemptID"]);
            result.AttemptNumber = Convert.ToInt32(row["AttemptNumber"]);
            result.AmountDue = Convert.ToDecimal(row["AmountDue"]);
            result.PaymentStatus = Convert.ToString(row["PaymentStatus"]);
            result.AttemptStatus = Convert.ToString(row["AttemptStatus"]);
            result.PaymentMethod = Convert.ToString(row["PaymentMethod"]);
            result.ProviderReference = row["ProviderReference"] == DBNull.Value ? null : Convert.ToString(row["ProviderReference"]);
            if (row["PaidAt"] != DBNull.Value)
            {
                result.PaidAt = Convert.ToDateTime(row["PaidAt"]);
            }

            // Cash-payment staff attribution
            if (row["StaffID"] != DBNull.Value)
            {
                result.StaffID = Convert.ToInt32(row["StaffID"]);
            }

            result.StaffFullName = row["StaffFullName"] == DBNull.Value ? null : Convert.ToString(row["StaffFullName"]);
            return result;
        }

        public DataTable ProcessStaffCashPayment(int jobID, decimal amountReceived, int staffID)
        {
            if (jobID <= 0)
                throw new ArgumentException("Invalid job ID.");
            if (staffID <= 0)
                throw new ArgumentException("Invalid staff member.");
            if (amountReceived <= 0)
                throw new ArgumentException("Cash amount must be greater than zero.");
            if (amountReceived > 99999999.99m)
                throw new ArgumentException("Cash amount exceeds the permitted limit.");
            if (decimal.Round(amountReceived, 2) != amountReceived)
                throw new ArgumentException("Cash amount cannot have more than two decimal places.");
            return paymentDataAccess.ProcessStaffCashPayment(jobID, amountReceived, staffID);
        }

        // GET STAFF CASH PAYMENT DETAILS
        public DataTable GetStaffCashPaymentDetails(int jobID)
        {
            if (jobID <= 0)
                throw new ArgumentException("Invalid job ID.");
            return paymentDataAccess.GetStaffCashPaymentDetails(jobID);
        }
    }
}
