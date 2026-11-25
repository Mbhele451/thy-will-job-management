using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.UI;

namespace MajorProject.Web.Secure.Manager
{
    public partial class ManagerInvoicePdf : ManagerBasePage
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

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadInvoicePdf();
        }

        private void LoadInvoicePdf()
        {
            int jobID = GetJobID();
            if (jobID <= 0)
            {
                SendNotFound();
                return;
            }

            try
            {
                // Retrieve the job from the database.
                var job = JobService.GetJobManagementDetails(jobID);
                if (job == null || job.Rows.Count == 0)
                {
                    SendNotFound();
                    return;
                }

                // Obtain the customer ID from the database.
                int customerID = Convert.ToInt32(job.Rows[0]["CustomerID"]);
                if (customerID <= 0)
                {
                    SendNotFound();
                    return;
                }

                // Retrieve the invoice using the verified
                // customer ID and selected job ID.
                byte[] pdfData = InvoiceService.GetCustomerInvoicePDFFromDatabase(customerID, jobID);
                if (pdfData == null || pdfData.Length < 5)
                {
                    SendNotFound();
                    return;
                }

                // Validate the PDF signature.
                if (pdfData[0] != 0x25 || pdfData[1] != 0x50 || pdfData[2] != 0x44 || pdfData[3] != 0x46 || pdfData[4] != 0x2D)
                {
                    throw new InvalidOperationException("The invoice does not contain a valid PDF.");
                }

                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.Charset = "";
                Response.AddHeader("Content-Disposition", "inline; filename=INV-" + jobID.ToString("D5") + ".pdf");
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
                System.Diagnostics.Debug.WriteLine("MANAGER INVOICE PDF ERROR: " + ex.ToString());
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
            Response.StatusDescription = "Invoice not found";
            Response.TrySkipIisCustomErrors = true;
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}
