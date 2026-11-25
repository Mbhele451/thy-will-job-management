using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Web;

namespace MajorProject.Web.Secure.Customer
{
    public partial class InvoicePdf : CustomerBasePage
    {
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadInvoicePdf();
            }
        }

        private void LoadInvoicePdf()
        {
            int jobID = GetJobIDFromRequest();
            int requestID = 0;
            bool feeInvoice = Request.QueryString["fee"] != null;
            if (feeInvoice ? !int.TryParse(Request.QueryString["fee"], out requestID) || requestID <= 0 : jobID <= 0)
            {
                SendNotFound();
                return;
            }

            int customerID = WebSession.CustomerID;
            if (customerID <= 0)
            {
                SendUnauthorized();
                return;
            }

            try
            {
                byte[] pdfData = feeInvoice ? new JobChangeService(ConnectionString).GetFeeInvoicePdf(customerID, requestID) :
                    InvoiceService.GetCustomerInvoicePDFFromDatabase(customerID, jobID);
                if (pdfData == null || pdfData.Length == 0)
                {
                    SendNotFound();
                    return;
                }

                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.ContentType = "application/pdf";
                string filename = feeInvoice ? "FEE-" + requestID : "INV-" + jobID;
                Response.AddHeader("Content-Disposition", "inline; filename=" + filename + ".pdf");
                Response.AddHeader("Content-Length", pdfData.Length.ToString());
                Response.BinaryWrite(pdfData);
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Invoice PDF error: " + ex.Message);
                SendNotFound();
            }
        }

        private int GetJobIDFromRequest()
        {
            string value = Request.QueryString["jobID"];
            int jobID;
            if (!int.TryParse(value, out jobID))
            {
                return 0;
            }

            return jobID;
        }

        private void SendUnauthorized()
        {
            Response.Clear();
            Response.StatusCode = 401;
            Response.StatusDescription = "Unauthorized";
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private void SendNotFound()
        {
            Response.Clear();
            Response.StatusCode = 404;
            Response.StatusDescription = "Invoice not found";
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}
