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

namespace MajorProject.Web.Secure.Customer
{
    public partial class PaymentReceipt : CustomerBasePage
    {
        private bool _responseHandled;
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString;
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
            int customerID = WebSession.CustomerID;
            int jobID = GetJobID();
            if (customerID <= 0 || jobID <= 0)
            {
                SendNotFound();
                return;
            }

            try
            {
                // Retrieve the customer's invoice.
                InvoiceDto invoice = InvoiceService.GetCustomerInvoiceDto(customerID, jobID);
                if (invoice == null)
                {
                    SendNotFound();
                    return;
                }

                // Retrieve the successful payment.
                PaymentResult payment = PaymentService.GetCustomerInvoicePaymentReceiptResult(jobID, customerID);
                if (payment == null || payment.PaymentStatus != "Paid" || payment.AttemptStatus != "Successful")
                {
                    SendNotFound();
                    return;
                }

                // Generate the PDF.
                byte[] pdfData = ReceiptPdfService.GeneratePaymentReceiptPdf(invoice, payment);
                if (pdfData == null || pdfData.Length == 0)
                {
                    SendNotFound();
                    return;
                }

                // Verify that the generated content is actually a PDF.
                if (pdfData.Length < 5 || pdfData[0] != 0x25 || pdfData[1] != 0x50 || pdfData[2] != 0x44 || pdfData[3] != 0x46 || pdfData[4] != 0x2D)
                {
                    throw new InvalidOperationException("The generated receipt does not have a valid PDF header.");
                }

                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.Charset = "";
                string disposition = Request.QueryString["download"] == "1" ? "attachment" : "inline";
                Response.AddHeader("Content-Disposition", disposition + "; filename=REC-" + payment.PaymentID + ".pdf");
                Response.AddHeader("Content-Length", pdfData.Length.ToString(CultureInfo.InvariantCulture));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                Response.TrySkipIisCustomErrors = true;
                // Write only the PDF bytes.
                Response.BinaryWrite(pdfData);
                // Prevent Web Forms from appending page HTML.
                _responseHandled = true;
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PAYMENT RECEIPT ERROR: " + ex.ToString());
                SendNotFound();
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // Do not render the ASPX page after sending the PDF.
            if (_responseHandled)
                return;
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
