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

namespace MajorProject.Web.Secure.Manager
{
    public partial class ManagerPaymentReceipt : ManagerBasePage
    {
        private bool _responseHandled;
        private string ConnectionString
        {
            get
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"];
                if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    throw new ConfigurationErrorsException("The database connection string is unavailable.");
                }

                return settings.ConnectionString;
            }
        }

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

        protected void Page_Load(object sender, EventArgs e)
        {
            GenerateReceipt();
        }

        private void GenerateReceipt()
        {
            int jobID = GetJobID();
            if (jobID <= 0)
            {
                SendNotFound();
                return;
            }

            try
            {
                // ----------------------------------------------------
                // VERIFY THE JOB
                // ----------------------------------------------------
                var job = JobService.GetJobManagementDetails(jobID);
                if (job == null || job.Rows.Count == 0)
                {
                    SendNotFound();
                    return;
                }

                // Customer ID is obtained from the database,
                // never from the URL.
                int customerID = Convert.ToInt32(job.Rows[0]["CustomerID"]);
                if (customerID <= 0)
                {
                    SendNotFound();
                    return;
                }

                // ----------------------------------------------------
                // RETRIEVE THE INVOICE
                // ----------------------------------------------------
                InvoiceDto invoice = InvoiceService.GetCustomerInvoiceDto(customerID, jobID);
                if (invoice == null)
                {
                    SendNotFound();
                    return;
                }

                // ----------------------------------------------------
                // VERIFY SUCCESSFUL PAYMENT
                // ----------------------------------------------------
                PaymentResult payment = PaymentService.GetCustomerInvoicePaymentReceiptResult(jobID, customerID);
                if (payment == null || !string.Equals(payment.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase) || !string.Equals(payment.AttemptStatus, "Successful", StringComparison.OrdinalIgnoreCase))
                {
                    SendNotFound();
                    return;
                }

                // ----------------------------------------------------
                // GENERATE RECEIPT
                // ----------------------------------------------------
                byte[] pdfData = ReceiptPdfService.GeneratePaymentReceiptPdf(invoice, payment);
                if (pdfData == null || pdfData.Length < 5)
                {
                    SendNotFound();
                    return;
                }

                // Validate PDF signature.
                if (pdfData[0] != 0x25 || pdfData[1] != 0x50 || pdfData[2] != 0x44 || pdfData[3] != 0x46 || pdfData[4] != 0x2D)
                {
                    throw new InvalidOperationException("The generated receipt is not a valid PDF.");
                }

                // ----------------------------------------------------
                // SEND PDF
                // ----------------------------------------------------
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.Charset = "";
                Response.AddHeader("Content-Disposition", "inline; filename=REC-" + payment.PaymentID.ToString(CultureInfo.InvariantCulture) + ".pdf");
                Response.AddHeader("Content-Length", pdfData.Length.ToString(CultureInfo.InvariantCulture));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                Response.TrySkipIisCustomErrors = true;
                Response.BinaryWrite(pdfData);
                _responseHandled = true;
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MANAGER PAYMENT RECEIPT ERROR: " + ex.ToString());
                SendNotFound();
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (_responseHandled)
            {
                return;
            }

            base.Render(writer);
        }

        private int GetJobID()
        {
            int jobID;
            if (!int.TryParse(Request.QueryString["jobID"], out jobID))
            {
                return 0;
            }

            return jobID;
        }

        private void SendNotFound()
        {
            _responseHandled = true;
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.StatusCode = 404;
            Response.StatusDescription = "Payment receipt not found";
            Response.TrySkipIisCustomErrors = true;
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}
