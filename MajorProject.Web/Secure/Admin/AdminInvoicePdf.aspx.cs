using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Web.UI;

namespace MajorProject.Web.Secure.Admin
{
    public partial class AdminInvoicePdf : AdminBasePage
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

        // ============================================================
        // PAGE LOAD
        // ============================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadInvoice();
            }
        }

        // ============================================================
        // LOAD INVOICE PDF
        // ============================================================
        private void LoadInvoice()
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
                // Retrieve the job from the database.
                // Do not accept CustomerID from the query string.
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

                byte[] pdf = InvoiceService.GetCustomerInvoicePDFFromDatabase(customerID, jobID);
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
                Response.AddHeader("Content-Disposition", "inline; filename=INV-" + jobID.ToString("D5") + ".pdf");
                Response.AddHeader("Content-Length", pdf.Length.ToString(CultureInfo.InvariantCulture));
                Response.BinaryWrite(pdf);
                Response.Flush();
                _responseHandled = true;
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Admin invoice PDF error: " + ex);
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
            Response.StatusDescription = "Invoice not found.";
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
