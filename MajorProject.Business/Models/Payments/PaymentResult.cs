using System;

namespace MajorProject.Business.Models.Payments
{
    public class PaymentResult
    {
        public long PaymentID { get; set; }
        public long PaymentAttemptID { get; set; }
        public int AttemptNumber { get; set; }
        public decimal AmountDue { get; set; }
        public string PaymentStatus { get; set; }
        public string AttemptStatus { get; set; }
        public string ProviderReference { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? PaidAt { get; set; }
        public string CardBrand { get; set; }
        public string LastFourDigits { get; set; }
        // Staff attribution for cash payments
        public int? StaffID { get; set; }
        public string StaffFullName { get; set; }

        public bool IsSuccessful
        {
            get
            {
                return AttemptStatus == "Successful";
            }
        }

        public string Message
        {
            get
            {
                if (IsSuccessful)
                    return "Your simulated payment was successful.";
                return "Your simulated payment was declined. You can try again.";
            }
        }
    }
}
