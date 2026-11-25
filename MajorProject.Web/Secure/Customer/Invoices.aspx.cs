using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MajorProject.Web.Secure.Customer
{
    public partial class Invoices : CustomerBasePage
    {
        private string ConnectionString
        {
            get
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"];
                if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    throw new ConfigurationErrorsException("Connection string 'GroupWst34ConnectionString' was not found.");
                }

                return settings.ConnectionString;
            }
        }

        private InvoiceService InvoiceService
        {
            get
            {
                return new InvoiceService(ConnectionString);
            }
        }

        private int SelectedJobID
        {
            get
            {
                object value = ViewState["SelectedInvoiceJobID"];
                int jobID;
                if (value != null && int.TryParse(Convert.ToString(value), out jobID))
                {
                    return jobID;
                }

                return 0;
            }

            set
            {
                ViewState["SelectedInvoiceJobID"] = value;
            }
        }

        // ============================================================
        // PAGE LOAD
        // ============================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!WebSession.IsAuthenticated || !WebSession.HasRole("Customer") || WebSession.CustomerID <= 0)
            {
                return;
            }

            if (!IsPostBack)
            {
                LoadInvoices();
            }
        }

        // ============================================================
        // LOAD INVOICES
        // ============================================================
        private void LoadInvoices()
        {
            try
            {
                DataTable invoices = InvoiceService.GetInvoicesByCustomerID(WebSession.CustomerID);
                var changeInvoices = new JobChangeService(ConnectionString).CustomerFees(WebSession.CustomerID);
                feeInvoices.DataSource = changeInvoices;
                feeInvoices.DataBind();
                feeSection.Visible = changeInvoices.Rows.Count > 0;
                if (invoices.Rows.Count == 0)
                {
                    ShowEmptyState();
                }
                else
                {
                    gvInvoices.DataSource = invoices;
                    gvInvoices.DataBind();
                    pnlInvoices.Visible = true;
                    pnlEmpty.Visible = false;
                }
                lblInvoiceCount.Text = (invoices.Rows.Count + changeInvoices.Rows.Count).ToString();
                decimal totalBilled = CalculateTotalBilled(invoices);
                decimal outstanding = CalculateOutstandingBalance(invoices);
                foreach (DataRow fee in changeInvoices.Rows)
                {
                    decimal amount = (decimal)fee["Fee"];
                    totalBilled += amount;
                    if ((string)fee["FeePaymentStatus"] == "Unpaid")
                        outstanding += amount;
                }
                lblTotalBilled.Text = FormatCurrency(totalBilled);
                lblOutstandingBalance.Text = FormatCurrency(outstanding);
            }
            catch (Exception ex)
            {
                ShowError("We could not load your invoices. Please try again.");
                pnlInvoices.Visible = false;
                pnlEmpty.Visible = true;
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        // ============================================================
        // FINANCIAL SUMMARY
        // ============================================================
        private decimal CalculateTotalBilled(DataTable invoices)
        {
            decimal total = 0m;
            if (invoices == null)
                return total;
            foreach (DataRow row in invoices.Rows)
            {
                if (IsInvoicePaid(row) || GetSafeString(row, "JobStatus").Equals("Completed", StringComparison.OrdinalIgnoreCase))
                    total += GetDecimalValue(row, "GrandTotal");
            }

            return total;
        }

        private decimal CalculateOutstandingBalance(DataTable invoices)
        {
            decimal outstanding = 0m;
            if (invoices == null)
                return outstanding;
            foreach (DataRow row in invoices.Rows)
            {
                if (!IsInvoicePaid(row) && GetSafeString(row, "JobStatus").Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    outstanding += GetDecimalValue(row, "GrandTotal");
                }
            }

            return outstanding;
        }

        // ============================================================
        // PAYMENT STATUS
        // ============================================================
        private string GetPaymentStatus(DataRow invoice)
        {
            string status = GetSafeString(invoice, "PaymentStatus");
            if (string.IsNullOrWhiteSpace(status))
            {
                return "Unpaid";
            }

            return status;
        }

        private bool IsInvoicePaid(DataRow invoice)
        {
            return GetPaymentStatus(invoice).Equals("Paid", StringComparison.OrdinalIgnoreCase);
        }

        // ============================================================
        // VIEW INVOICE
        // ============================================================
        protected void gvInvoices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("ViewInvoice", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            int jobID;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out jobID) || jobID <= 0)
            {
                ShowError("The selected invoice is invalid.");
                return;
            }

            LoadSelectedInvoice(jobID);
        }

        private void LoadSelectedInvoice(int jobID)
        {
            try
            {
                DataTable invoices = InvoiceService.GetInvoicesByCustomerID(WebSession.CustomerID);
                DataRow invoice = FindInvoice(invoices, jobID);
                if (invoice == null)
                {
                    ShowError("The selected invoice could not be found.");
                    return;
                }

                // Set the selected job before displaying the invoice.
                // The receipt URL uses this value.
                SelectedJobID = jobID;
                DisplayInvoice(invoice);
                // Invoice ownership has been established above; retrieve every saved service line.
                DataTable services = new JobService(ConnectionString).GetJobDetailsByJobID(jobID);
                gvInvoiceServices.DataSource = services;
                gvInvoiceServices.DataBind();
                if (services != null && services.Rows.Count > 1)
                    lblSelectedService.Text = services.Rows.Count.ToString(CultureInfo.InvariantCulture) + " service lines";
                LoadInvoicePdf(jobID);
            }
            catch (Exception ex)
            {
                ShowError("We could not load the selected invoice.");
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private DataRow FindInvoice(DataTable invoices, int jobID)
        {
            if (invoices == null)
                return null;
            foreach (DataRow row in invoices.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                    continue;
                if (GetIntValue(row, "JobID") == jobID)
                {
                    return row;
                }
            }

            return null;
        }

        // ============================================================
        // DISPLAY SELECTED INVOICE
        // ============================================================
        private void DisplayInvoice(DataRow invoice)
        {
            lblSelectedInvoiceNumber.Text = Server.HtmlEncode(GetInvoiceNumber(GetValue(invoice, "JobID")));
            lblSelectedService.Text = Server.HtmlEncode(GetSafeString(invoice, "JobTypeName"));
            lblSelectedDate.Text = FormatDate(GetValue(invoice, "PromiseDate"));
            decimal labour = GetDecimalValue(invoice, "LabourCost");
            decimal transport = GetDecimalValue(invoice, "TransportCost");
            decimal materials = GetDecimalValue(invoice, "TotalMaterialCost");
            decimal tax = GetDecimalValue(invoice, "TaxAmount");
            decimal grandTotal = GetDecimalValue(invoice, "GrandTotal");
            lblLabourCost.Text = FormatCurrency(labour);
            lblTransportCost.Text = FormatCurrency(transport);
            lblMaterialCost.Text = FormatCurrency(materials);
            lblTaxAmount.Text = FormatCurrency(tax);
            lblGrandTotal.Text = FormatCurrency(grandTotal);
            lblSelectedTotal.Text = FormatCurrency(grandTotal);
            // ========================================================
            // EMAIL INFORMATION
            // ========================================================
            object sentDate = GetValue(invoice, "InvoiceSentDate");
            string sentEmail = GetSafeString(invoice, "InvoiceSentToEmail");
            if (sentDate != null && !string.IsNullOrWhiteSpace(sentEmail))
            {
                lblEmailStatus.Text = "Invoice sent to " + Server.HtmlEncode(sentEmail) + " on " + FormatDate(sentDate) + ".";
            }
            else
            {
                lblEmailStatus.Text = "This invoice has not been recorded as emailed.";
            }

            // ========================================================
            // PAYMENT INFORMATION
            // ========================================================
            string paymentStatus = GetPaymentStatus(invoice);
            bool isPaid = IsInvoicePaid(invoice);
            string jobStatus = GetSafeString(invoice, "JobStatus");
            bool receiptAvailable = isPaid && (jobStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase)
                || jobStatus.Equals("Archived", StringComparison.OrdinalIgnoreCase));
            string paymentMethod = GetSafeString(invoice, "PaymentMethod");
            string reference = GetSafeString(invoice, "PaymentReference");
            object paymentDate = GetValue(invoice, "PaymentDate");
            lblSelectedPaymentStatus.Text = Server.HtmlEncode(paymentStatus);
            lblPaymentReceipt.Visible = false;
            lblPaymentReceipt.Text = string.Empty;
            if (receiptAvailable)
            {
                string receiptText = "Payment received.";
                if (!string.IsNullOrWhiteSpace(paymentMethod))
                {
                    receiptText += " Method: " + Server.HtmlEncode(paymentMethod) + ".";
                }

                if (!string.IsNullOrWhiteSpace(reference))
                {
                    receiptText += " Receipt reference: " + Server.HtmlEncode(reference) + ".";
                }
                else if (paymentMethod.Equals("Cash", StringComparison.OrdinalIgnoreCase))
                {
                    receiptText += " Cash payment recorded successfully.";
                }

                if (paymentDate != null)
                {
                    receiptText += " Payment date: " + FormatDate(paymentDate) + ".";
                }

                lblPaymentReceipt.Text = receiptText;
                lblPaymentReceipt.Visible = true;
            }

            // ========================================================
            // PAYMENT RECEIPT TAB
            // ========================================================
            ConfigurePaymentReceipt(receiptAvailable);
            // ========================================================
            // PAYMENT ELIGIBILITY
            // ========================================================
            string invoiceFilePath = GetSafeString(invoice, "InvoiceFilePath");
            bool canPay = !isPaid && jobStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(invoiceFilePath) && grandTotal > 0;
            btnPayInvoice.Visible = canPay;
            // Display the selected invoice.
            pnlSelectedInvoice.Visible = true;
            pnlSelectedInvoice.Attributes["data-invoice-loaded"] = "true";
        }

        // ============================================================
        // CONFIGURE PAYMENT RECEIPT TAB
        // ============================================================
        private void ConfigurePaymentReceipt(bool isPaid)
        {
            // The receipt tab is available only for paid invoices.
            tabReceipt.Visible = isPaid;
            pnlReceiptTabContent.Visible = isPaid;
            pnlReceiptPreview.Visible = isPaid;
            if (!isPaid)
            {
                // Clear any previous invoice's receipt information.
                receiptPdfFrame.Attributes.Remove("src");
                lnkDownloadReceipt.NavigateUrl = string.Empty;
                return;
            }

            if (SelectedJobID <= 0)
            {
                tabReceipt.Visible = false;
                pnlReceiptTabContent.Visible = false;
                pnlReceiptPreview.Visible = false;
                receiptPdfFrame.Attributes.Remove("src");
                lnkDownloadReceipt.NavigateUrl = string.Empty;
                return;
            }

            // Receipt preview.
            receiptPdfFrame.Attributes["src"] = ResolveUrl("~/Secure/Customer/PaymentReceipt.aspx?jobID=" + SelectedJobID);
            // Receipt download.
            lnkDownloadReceipt.NavigateUrl = ResolveUrl("~/Secure/Customer/PaymentReceipt.aspx?jobID=" + SelectedJobID + "&download=1");
        }

        // ============================================================
        // PAY INVOICE
        // ============================================================
        protected void btnPayInvoice_Click(object sender, EventArgs e)
        {
            if (!WebSession.IsAuthenticated || !WebSession.HasRole("Customer") || WebSession.CustomerID <= 0)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (SelectedJobID <= 0)
            {
                ShowError("Please select an invoice first.");
                return;
            }

            try
            {
                DataTable invoices = InvoiceService.GetInvoicesByCustomerID(WebSession.CustomerID);
                DataRow invoice = FindInvoice(invoices, SelectedJobID);
                if (invoice == null)
                {
                    ShowError("The selected invoice could not be found.");
                    return;
                }

                if (IsInvoicePaid(invoice))
                {
                    ShowError("This invoice has already been paid.");
                    LoadSelectedInvoice(SelectedJobID);
                    return;
                }

                string jobStatus = GetSafeString(invoice, "JobStatus");
                string invoiceFilePath = GetSafeString(invoice, "InvoiceFilePath");
                decimal amount = GetDecimalValue(invoice, "GrandTotal");
                if (!jobStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    ShowError("Only completed services can be paid.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(invoiceFilePath) || amount <= 0)
                {
                    ShowError("This invoice is not eligible for payment.");
                    return;
                }

                Response.Redirect(ResolveUrl("~/Secure/Customer/PayInvoice.aspx?jobID=" + SelectedJobID), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                ShowError("We could not open the payment page. Please try again.");
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        // ============================================================
        // PDF PREVIEW
        // ============================================================
        private void LoadInvoicePdf(int jobID)
        {
            byte[] pdfData = InvoiceService.GetCustomerInvoicePDFFromDatabase(WebSession.CustomerID, jobID);
            if (pdfData == null || pdfData.Length == 0)
            {
                pnlPdfPreview.Visible = false;
                pnlPdfUnavailable.Visible = true;
                invoicePdfFrame.Attributes.Remove("src");
                return;
            }

            string pdfUrl = ResolveUrl("~/Secure/Customer/InvoicePdf.aspx?jobID=" + jobID);
            invoicePdfFrame.Attributes["src"] = pdfUrl;
            pnlPdfPreview.Visible = true;
            pnlPdfUnavailable.Visible = false;
        }

        // ============================================================
        // DOWNLOAD PDF
        // ============================================================
        protected void btnDownloadInvoice_Click(object sender, EventArgs e)
        {
            if (!WebSession.IsAuthenticated || !WebSession.HasRole("Customer") || WebSession.CustomerID <= 0)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (SelectedJobID <= 0)
            {
                ShowError("No invoice has been selected.");
                return;
            }

            try
            {
                DataTable invoices = InvoiceService.GetInvoicesByCustomerID(WebSession.CustomerID);
                DataRow invoice = FindInvoice(invoices, SelectedJobID);
                if (invoice == null)
                {
                    ShowError("The selected invoice could not be found.");
                    return;
                }

                byte[] pdfData = InvoiceService.GetCustomerInvoicePDFFromDatabase(WebSession.CustomerID, SelectedJobID);
                if (pdfData == null || pdfData.Length == 0)
                {
                    ShowError("The invoice PDF is not available.");
                    return;
                }

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + GetInvoiceNumber(SelectedJobID) + ".pdf");
                Response.AddHeader("Content-Length", pdfData.Length.ToString());
                Response.BinaryWrite(pdfData);
                Response.End();
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                ShowError("The invoice could not be downloaded.");
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        // ============================================================
        // PAGING
        // ============================================================
        protected void gvInvoices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInvoices.PageIndex = e.NewPageIndex;
            LoadInvoices();
        }

        // ============================================================
        // INVOICE NUMBER
        // ============================================================
        protected string GetInvoiceNumber(object jobID)
        {
            int id;
            if (jobID == null || !int.TryParse(Convert.ToString(jobID), out id) || id <= 0)
            {
                return "Invoice";
            }

            return "INV-" + id.ToString("D5");
        }

        // ============================================================
        // DATE FORMATTING
        // ============================================================
        protected string FormatDate(object value)
        {
            if (value == null || value == DBNull.Value)
                return "Not available";
            DateTime date;
            if (value is DateTime)
            {
                date = (DateTime)value;
            }
            else if (!DateTime.TryParse(Convert.ToString(value), out date))
            {
                return "Not available";
            }

            return date.ToString("dd MMM yyyy");
        }

        // ============================================================
        // CURRENCY FORMATTING
        // ============================================================
        protected string FormatCurrency(object value)
        {
            decimal amount = GetDecimalValue(value);
            return amount.ToString("C2", CultureInfo.GetCultureInfo("en-ZA"));
        }

        // ============================================================
        // EMAIL HELPERS
        // ============================================================
        protected string GetEmailStatus(object value)
        {
            if (value == null || value == DBNull.Value)
                return "Not sent";
            DateTime sentDate;
            if (!DateTime.TryParse(Convert.ToString(value), out sentDate))
            {
                return "Not sent";
            }

            return "Sent";
        }

        protected string GetEmailStatusClass(object value)
        {
            string status = GetEmailStatus(value);
            if (status.Equals("Sent", StringComparison.OrdinalIgnoreCase))
            {
                return "dashboard-status dashboard-status-success";
            }

            return "dashboard-status dashboard-status-warning";
        }

        // ============================================================
        // DATABASE VALUE HELPERS
        // ============================================================
        private decimal GetDecimalValue(DataRow row, string columnName)
        {
            return GetDecimalValue(GetValue(row, columnName));
        }

        private decimal GetDecimalValue(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0m;
            if (value is decimal)
                return (decimal)value;
            if (value is int || value is long || value is double || value is float)
            {
                return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            }

            decimal result;
            if (decimal.TryParse(Convert.ToString(value), NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }

            if (decimal.TryParse(Convert.ToString(value), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            return 0m;
        }

        private int GetIntValue(DataRow row, string columnName)
        {
            object value = GetValue(row, columnName);
            if (value == null)
                return 0;
            int result;
            if (int.TryParse(Convert.ToString(value), out result))
            {
                return result;
            }

            return 0;
        }

        private string GetSafeString(DataRow row, string columnName)
        {
            object value = GetValue(row, columnName);
            if (value == null)
                return string.Empty;
            return Convert.ToString(value).Trim();
        }

        private object GetValue(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName))
            {
                return null;
            }

            if (row[columnName] == DBNull.Value)
                return null;
            return row[columnName];
        }

        // ============================================================
        // EMPTY STATE
        // ============================================================
        private void ShowEmptyState()
        {
            gvInvoices.DataSource = null;
            gvInvoices.DataBind();
            pnlInvoices.Visible = false;
            pnlEmpty.Visible = true;
            pnlSelectedInvoice.Visible = false;
            // Clear selected invoice and receipt state.
            SelectedJobID = 0;
            tabReceipt.Visible = false;
            pnlReceiptTabContent.Visible = false;
            pnlReceiptPreview.Visible = false;
            receiptPdfFrame.Attributes.Remove("src");
            lnkDownloadReceipt.NavigateUrl = string.Empty;
            lblInvoiceCount.Text = "0";
            lblTotalBilled.Text = FormatCurrency(0m);
            lblOutstandingBalance.Text = FormatCurrency(0m);
        }

        // ============================================================
        // ERROR
        // ============================================================
        private void ShowError(string message)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = Server.HtmlEncode(message);
        }

        // ============================================================
        // GRID PAYMENT STATUS HELPERS
        // ============================================================
        protected string GetDisplayPaymentStatus(object value)
        {
            string status = Convert.ToString(value).Trim();
            return string.IsNullOrWhiteSpace(status) ? "Unpaid" : status;
        }

        protected string GetPaymentStatusClass(object value)
        {
            string status = GetDisplayPaymentStatus(value);
            if (status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
                return "invoice-status invoice-status-paid";
            if (status.Equals("Processing", StringComparison.OrdinalIgnoreCase))
                return "invoice-status invoice-status-processing";
            if (status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                return "invoice-status invoice-status-pending";
            if (status.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                return "invoice-status invoice-status-failed";
            return "invoice-status invoice-status-unpaid";
        }

        protected bool CanPayInvoice(object paymentStatus, object jobStatus, object invoiceFilePath, object grandTotal)
        {
            string payment = Convert.ToString(paymentStatus).Trim();
            string job = Convert.ToString(jobStatus).Trim();
            string filePath = Convert.ToString(invoiceFilePath).Trim();
            decimal amount;
            decimal.TryParse(Convert.ToString(grandTotal), NumberStyles.Any, CultureInfo.InvariantCulture, out amount);
            return !payment.Equals("Paid", StringComparison.OrdinalIgnoreCase) && !payment.Equals("Processing", StringComparison.OrdinalIgnoreCase) && job.Equals("Completed", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(filePath) && amount > 0;
        }
    }
}
