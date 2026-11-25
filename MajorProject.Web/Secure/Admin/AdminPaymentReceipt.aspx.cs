using MajorProject.Business.Models.DTOs;
using MajorProject.Business.Models.Payments;
using MajorProject.Business.Services;
using MajorProject.Business.Services.Payments;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Web.UI;

namespace MajorProject.Web.Secure.Admin
{
    public partial class AdminPaymentReceipt : AdminBasePage
    {
        private bool _responseHandled;
        // ============================================================
        // CONNECTION STRING
        // ============================================================
        private string ConnectionString
        {
            get
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"];
                if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    throw new ConfigurationErrorsException("Connection string was not found.");
                }

                return settings.ConnectionString;
            }
        }

        // ============================================================
        // SERVICES
        // ============================================================
        private JobService JobService
        {
            get
            {
                return new JobService(ConnectionString);
            }
        }

        private InvoiceService InvoiceService
        {
            get
            {
                return new InvoiceService(ConnectionString);
            }
        }

        private PaymentService PaymentService
        {
            get
            {
                return new PaymentService(ConnectionString);
            }
        }

        private PaymentReceiptPdfService ReceiptPdfService
        {
            get
            {
                return new PaymentReceiptPdfService();
            }
        }

        // ============================================================
        // PAGE LOAD
        // ============================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPaymentReceipt();
            }
        }

        // ============================================================
        // LOAD PAYMENT RECEIPT
        // ============================================================
        private void LoadPaymentReceipt()
        {
            try
            {
                int jobID;
                if (!int.TryParse(Request.QueryString["jobID"], out jobID) || jobID <= 0)
                {
                    NotFound();
                    return;
                }

                // ----------------------------------------------------
                // Retrieve the job and its customer ID from database.
                // ----------------------------------------------------
                var job = JobService.GetJobManagementDetails(jobID);
                if (job == null || job.Rows.Count == 0)
                {
                    NotFound();
                    return;
                }

                int customerID = Convert.ToInt32(job.Rows[0]["CustomerID"]);
                if (customerID <= 0)
                {
                    NotFound();
                    return;
                }

                // ----------------------------------------------------
                // Retrieve invoice details.
                // ----------------------------------------------------
                InvoiceDto invoice = InvoiceService.GetCustomerInvoiceDto(customerID, jobID);
                if (invoice == null)
                {
                    NotFound();
                    return;
                }

                // ----------------------------------------------------
                // Retrieve payment details.
                // ----------------------------------------------------
                PaymentResult payment = PaymentService.GetCustomerInvoicePaymentReceiptResult(jobID, customerID);
                bool successfulPayment = payment != null && string.Equals(payment.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase) && string.Equals(payment.AttemptStatus, "Successful", StringComparison.OrdinalIgnoreCase);
                if (!successfulPayment)
                {
                    NotFound();
                    return;
                }

                // ----------------------------------------------------
                // GENERATE RECEIPT PDF
                // ----------------------------------------------------
                byte[] pdf = ReceiptPdfService.GeneratePaymentReceiptPdf(invoice, payment);
                if (pdf == null || pdf.Length == 0 || !HasPdfSignature(pdf))
                {
                    NotFound();
                    return;
                }

                // ----------------------------------------------------
                // STREAM PDF
                // ----------------------------------------------------
                Response.Clear();
                Response.ClearHeaders();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "inline; filename=REC-" + payment.PaymentID.ToString() + ".pdf");
                Response.AddHeader("Content-Length", pdf.Length.ToString(CultureInfo.InvariantCulture));
                Response.BinaryWrite(pdf);
                Response.Flush();
                _responseHandled = true;
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Admin payment receipt error: " + ex);
                NotFound();
            }
        }

        // ============================================================
        // PDF VALIDATION
        // ============================================================
        private bool HasPdfSignature(byte[] pdf)
        {
            if (pdf == null || pdf.Length < 5)
            {
                return false;
            }

            return pdf[0] == 0x25 && pdf[1] == 0x50 && pdf[2] == 0x44 && pdf[3] == 0x46 && pdf[4] == 0x2D;
        }

        // ============================================================
        // NOT FOUND
        // ============================================================
        private void NotFound()
        {
            _responseHandled = true;
            Response.Clear();
            Response.StatusCode = 404;
            Response.StatusDescription = "Payment receipt not found.";
            Response.SuppressContent = true;
            Context.ApplicationInstance.CompleteRequest();
        }

        // ============================================================
        // PREVENT WEB FORMS HTML FROM APPENDING TO PDF
        // ============================================================
        protected override void Render(HtmlTextWriter writer)
        {
            if (_responseHandled)
            {
                return;
            }

            base.Render(writer);
        }
    }
}
