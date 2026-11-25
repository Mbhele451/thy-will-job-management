using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using MajorProject.Business.Services;
using MajorProject.Business.Services.Payments;
using MajorProject.Web.Helpers;

namespace MajorProject.Web.Secure.Customer
{
    public partial class PayInvoice : CustomerBasePage
    {
        private bool invoiceReady;
        private bool invoicePayable;
        private string checkoutModal;
        private string checkoutTab;
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString;
            }
        }

        private int CustomerID
        {
            get
            {
                return WebSession.CustomerID;
            }
        }

        private int JobID
        {
            get
            {
                int jobID;
                if (int.TryParse(Request.QueryString["jobID"], out jobID))
                    return jobID;
                return 0;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!WebSession.IsAuthenticated || !WebSession.HasRole("Customer") || CustomerID <= 0)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (JobID <= 0)
            {
                Response.Redirect("~/Secure/Customer/Invoices.aspx");
                return;
            }

            try
            {
                LoadInvoice();
                if (!IsPostBack)
                {
                    LoadSavedCards();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Invoice checkout failed ({0}).", ex.GetType().Name);
                Response.Redirect("~/Secure/Customer/Invoices.aspx");
            }
        }

        // ==========================================
        // LOAD AND VALIDATE INVOICE
        // ==========================================
        private void LoadInvoice()
        {
            DataTable invoices = InvoiceService.GetInvoicesByCustomerID(CustomerID);
            DataRow invoice = null;
            if (invoices != null)
            {
                foreach (DataRow row in invoices.Rows)
                {
                    if (Convert.ToInt32(row["JobID"]) == JobID)
                    {
                        invoice = row;
                        break;
                    }
                }
            }

            if (invoice == null)
                throw new InvalidOperationException("Invoice not found.");
            string jobStatus = GetSafeString(invoice, "JobStatus");
            string invoiceFilePath = GetSafeString(invoice, "InvoiceFilePath");
            decimal amount = GetDecimalValue(invoice, "GrandTotal");
            if (!jobStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Only completed services can be paid.");
            }

            if (string.IsNullOrWhiteSpace(invoiceFilePath) || amount <= 0)
            {
                throw new InvalidOperationException("This invoice is not eligible for payment.");
            }

            string invoiceReference = "INV-" + JobID.ToString("D6");
            string formattedAmount = FormatCurrency(amount);
            lblInvoiceReference.Text = Server.HtmlEncode(invoiceReference);
            lblPaidInvoiceReference.Text = Server.HtmlEncode(invoiceReference);
            lblAmountDue.Text = Server.HtmlEncode(formattedAmount);
            lblPaidInvoiceAmount.Text = Server.HtmlEncode(formattedAmount);
            lblSavedCardCheckoutAmount.Text = Server.HtmlEncode(formattedAmount);
            lblNewCardCheckoutAmount.Text = Server.HtmlEncode(formattedAmount);
            lblEFTCheckoutAmount.Text = Server.HtmlEncode(formattedAmount);
            lblWalletCheckoutAmount.Text = Server.HtmlEncode(formattedAmount);
            invoiceReady = true;
            LoadPaymentStatus();
        }

        // ==========================================
        // LOAD PAYMENT STATUS AND UPDATE PAGE STATE
        // ==========================================
        private void LoadPaymentStatus()
        {
            DataTable payment = PaymentService.GetCustomerInvoicePaymentStatus(JobID, CustomerID);
            string status = "Unpaid";
            if (payment != null && payment.Rows.Count > 0)
            {
                status = Convert.ToString(payment.Rows[0]["PaymentStatus"]);
                if (string.IsNullOrWhiteSpace(status))
                    status = "Unpaid";
            }

            bool isPaid = string.Equals(status, "Paid", StringComparison.OrdinalIgnoreCase);
            invoicePayable = string.Equals(status, "Unpaid", StringComparison.OrdinalIgnoreCase) || string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase);
            lblPaymentStatus.Text = Server.HtmlEncode(status);
            pnlUnpaidHeader.Visible = !isPaid;
            pnlPaidHeader.Visible = isPaid;
            pnlUnpaidOverview.Visible = !isPaid;
            pnlPaidOverview.Visible = isPaid;
            pnlPaymentMethods.Visible = invoicePayable;
            pnlSuccess.Visible = false;
            if (isPaid)
            {
                LoadPaymentReceiptDetails();
            }
        }

        // ==========================================
        // LOAD PAYMENT RECEIPT DETAILS
        // ==========================================
        private void LoadPaymentReceiptDetails()
        {
            var payment = PaymentService.GetCustomerInvoicePaymentReceiptResult(JobID, CustomerID);
            if (payment == null || !string.Equals(payment.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase) || !string.Equals(payment.AttemptStatus, "Successful", StringComparison.OrdinalIgnoreCase))
            {
                lblPaymentReference.Text = "Receipt details unavailable";
                lblSuccessfulMethod.Text = "Not available";
                lblReceiptMessage.Text = "Your payment is recorded, but the receipt is currently unavailable.";
                lnkViewReceipt.Visible = false;
                lnkDownloadReceipt.Visible = false;
                pnlSuccess.Visible = true;
                return;
            }

            lblPaymentReference.Text = Server.HtmlEncode(string.IsNullOrWhiteSpace(payment.ProviderReference) ? "Not available" : payment.ProviderReference);
            lblSuccessfulMethod.Text = Server.HtmlEncode(string.IsNullOrWhiteSpace(payment.PaymentMethod) ? "Not available" : payment.PaymentMethod);
            lblReceiptMessage.Text = (payment.ProviderReference ?? string.Empty).StartsWith("SIM-", StringComparison.Ordinal) ? "Your simulated payment was recorded successfully. No money was transferred." : "Your payment has been recorded successfully.";
            string receiptUrl = ResolveUrl("~/Secure/Customer/PaymentReceipt.aspx?jobID=" + JobID);
            lnkViewReceipt.NavigateUrl = receiptUrl;
            lnkDownloadReceipt.NavigateUrl = receiptUrl + "&download=1";
            lnkViewReceipt.Visible = true;
            lnkDownloadReceipt.Visible = true;
            pnlSuccess.Visible = true;
        }

        // ==========================================
        // LOAD SAVED CARDS
        // ==========================================
        private void LoadSavedCards()
        {
            DataTable cards = PaymentService.GetCustomerPaymentCards(CustomerID);
            ddlSavedCards.Items.Clear();
            ddlSavedCards.Items.Add(new System.Web.UI.WebControls.ListItem("Select a saved card", ""));
            if (cards != null)
            {
                foreach (DataRow card in cards.Rows)
                {
                    string brand = GetSafeString(card, "CardBrand");
                    string lastFour = GetSafeString(card, "LastFourDigits");
                    string token = GetSafeString(card, "CardToken");
                    var option = new System.Web.UI.WebControls.ListItem(brand + " ending in " + lastFour, token);
                    option.Attributes["data-brand"] = brand;
                    option.Attributes["data-last-four"] = lastFour;
                    int month, year;
                    if (int.TryParse(GetSafeString(card, "ExpiryMonth"), out month) && int.TryParse(GetSafeString(card, "ExpiryYear"), out year))
                        option.Attributes["data-expiry"] = month.ToString("D2") + "/" + (year % 100).ToString("D2");
                    ddlSavedCards.Items.Add(option);
                }
            }

            pnlSavedCardSection.Visible = cards != null && cards.Rows.Count > 0;
        }

        // ==========================================
        // SAVED CARD PAYMENT
        // ==========================================
        private bool CanSubmitPayment()
        {
            if (invoiceReady && invoicePayable)
                return true;
            ShowError("This invoice is not available for payment. Return to your invoices to review its status.");
            return false;
        }

        private void ClearSensitiveCardFields()
        {
            txtCardNumber.Text = string.Empty;
            txtNewCardCvv.Text = string.Empty;
            txtSavedCardCvv.Text = string.Empty;
        }

        protected void btnPaySavedCard_Click(object sender, EventArgs e)
        {
            checkoutModal = "cardModal";
            checkoutTab = "saved";
            try
            {
                if (!CanSubmitPayment())
                    return;
                Guid token;
                if (!Guid.TryParse(ddlSavedCards.SelectedValue, out token) || token == Guid.Empty)
                {
                    ShowError("Please select a saved card.");
                    return;
                }

                if (!PaymentInputValidation.IsValidCvv(txtSavedCardCvv.Text))
                {
                    ShowError("Enter a valid 3 or 4 digit security code.");
                    return;
                }

                DataTable card = PaymentService.GetCustomerPaymentCardByToken(CustomerID, token);
                if (card == null || card.Rows.Count != 1)
                {
                    ShowError("The selected card is unavailable.");
                    return;
                }

                DataRow row = card.Rows[0];
                int month, year;
                if (!int.TryParse(GetSafeString(row, "ExpiryMonth"), out month) || !int.TryParse(GetSafeString(row, "ExpiryYear"), out year) || !PaymentInputValidation.IsValidExpiry(month, year, DateTime.Today) || (row.Table.Columns.Contains("IsActive") && !Convert.ToBoolean(row["IsActive"])))
                {
                    ShowError("This saved card is expired or unavailable. Add another card to continue.");
                    return;
                }

                ProcessPayment("Card");
            }
            catch (Exception ex)
            {
                HandlePaymentException(ex);
            }
            finally
            {
                ClearSensitiveCardFields();
            }
        }

        protected void btnAddCardAndPay_Click(object sender, EventArgs e)
        {
            checkoutModal = "cardModal";
            checkoutTab = "new";
            try
            {
                if (!CanSubmitPayment())
                    return;
                string name = txtCardholderName.Text.Trim();
                string number = PaymentInputValidation.NormalizeCardNumber(txtCardNumber.Text);
                byte month;
                short year;
                if (!PaymentInputValidation.IsValidName(name))
                {
                    ShowError("Enter the name on your card (2 to 100 characters).");
                    return;
                }

                if (!PaymentInputValidation.IsValidCardNumber(number))
                {
                    ShowError("Enter a valid card number.");
                    return;
                }

                if (!PaymentInputValidation.IsValidCvv(txtNewCardCvv.Text))
                {
                    ShowError("Enter a valid 3 or 4 digit security code.");
                    return;
                }

                if (!byte.TryParse(ddlExpiryMonth.SelectedValue, out month) || month < 1 || month > 12)
                {
                    ShowError("Select the card expiry month.");
                    return;
                }

                if (!Regex.IsMatch(txtExpiryYear.Text, @"\A[0-9]{4}\z") || !short.TryParse(txtExpiryYear.Text, out year) || !PaymentInputValidation.IsValidExpiry(month, year, DateTime.Today))
                {
                    ShowError("Enter a valid, unexpired four-digit year.");
                    return;
                }

                // The existing simulation stores masked metadata only. PAN and CVV never reach the data layer.
                PaymentService.AddCustomerPaymentCard(CustomerID, name, PaymentInputValidation.GetCardBrand(number), number.Substring(number.Length - 4), month, year);
                ProcessPayment("Card");
            }
            catch (Exception ex)
            {
                HandlePaymentException(ex);
            }
            finally
            {
                ClearSensitiveCardFields();
            }
        }

        protected void btnPayEFT_Click(object sender, EventArgs e)
        {
            checkoutModal = "eftModal";
            checkoutTab = null;
            try
            {
                if (!CanSubmitPayment())
                    return;
                string bank = ddlEFTBank.SelectedValue, name = txtEFTPayerName.Text.Trim();
                if (Array.IndexOf(new[] { "ABSA", "FNB", "NEDBANK", "STANDARD", "CAPITEC", "OTHER" }, bank) < 0)
                {
                    ShowError("Select your bank.");
                    return;
                }

                if (!PaymentInputValidation.IsValidName(name))
                {
                    ShowError("Enter a valid payer name (2 to 100 characters).");
                    return;
                }

                ProcessPayment("EFT");
            }
            catch (Exception ex)
            {
                HandlePaymentException(ex);
            }
            finally
            {
                ClearSensitiveCardFields();
            }
        }

        protected void btnPayWallet_Click(object sender, EventArgs e)
        {
            checkoutModal = "walletModal";
            checkoutTab = null;
            try
            {
                if (!CanSubmitPayment())
                    return;
                if (Array.IndexOf(new[] { "MobileWallet", "DemoWallet" }, ddlWalletProvider.SelectedValue) < 0)
                {
                    ShowError("Select a wallet provider.");
                    return;
                }

                if (!Regex.IsMatch(txtWalletMobile.Text, @"\A0[6-8][0-9]{8}\z"))
                {
                    ShowError("Enter a valid ten-digit South African mobile number.");
                    return;
                }

                ProcessPayment("MobileWallet");
            }
            catch (Exception ex)
            {
                HandlePaymentException(ex);
            }
            finally
            {
                ClearSensitiveCardFields();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (IsPostBack && pnlMessage.Visible && invoicePayable && !string.IsNullOrEmpty(checkoutModal))
            {
                string message = System.Web.HttpUtility.HtmlDecode(lblMessage.Text);
                string script = "window.addEventListener('load',function(){restorePaymentError(" + System.Web.HttpUtility.JavaScriptStringEncode(checkoutModal, true) + "," + System.Web.HttpUtility.JavaScriptStringEncode(checkoutTab ?? string.Empty, true) + "," + System.Web.HttpUtility.JavaScriptStringEncode(message, true) + ");});";
                ClientScript.RegisterStartupScript(GetType(), "checkout-validation", script, true);
            }
        }

        // ==========================================
        // PROCESS PAYMENT
        // ==========================================
        private void ProcessPayment(string paymentMethod)
        {
            DataTable result = PaymentService.ProcessSimulatedInvoicePayment(JobID, CustomerID, paymentMethod);
            if (result == null || result.Rows.Count == 0)
            {
                ShowError("The payment could not be completed.");
                return;
            }

            DataRow payment = result.Rows[0];
            bool successful = Convert.ToBoolean(payment["IsSuccessful"]);
            if (!successful)
            {
                ShowError("The payment was not successful.");
                return;
            }

            pnlMessage.Visible = false;
            LoadPaymentStatus();
        }

        // ==========================================
        // ERROR HANDLING
        // ==========================================
        private void ShowError(string message)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = Server.HtmlEncode(message);
        }

        private void HandlePaymentException(Exception ex)
        {
            System.Diagnostics.Trace.TraceError("Invoice checkout failed ({0}).", ex.GetType().Name);
            string message = ex.Message;
            if (!string.IsNullOrEmpty(message) && message.IndexOf("already been paid", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ShowError("This invoice has already been paid.");
                LoadPaymentStatus();
            }
            else
            {
                ShowError("We could not complete the payment. Please try again.");
            }
        }

        // ==========================================
        // DATA HELPERS
        // ==========================================
        private object GetValue(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return null;
            }

            return row[column];
        }

        private string GetSafeString(DataRow row, string column)
        {
            object value = GetValue(row, column);
            return value == null ? string.Empty : Convert.ToString(value);
        }

        private decimal GetDecimalValue(DataRow row, string column)
        {
            object value = GetValue(row, column);
            if (value == null)
                return 0;
            return Convert.ToDecimal(value);
        }

        private string FormatCurrency(decimal amount)
        {
            return amount.ToString("C2", new CultureInfo("en-ZA"));
        }

        private string FormatDate(object value)
        {
            if (value == null || value == DBNull.Value)
                return "Not available";
            DateTime date;
            if (DateTime.TryParse(Convert.ToString(value), out date))
            {
                return date.ToString("dd MMMM yyyy");
            }

            return "Not available";
        }
    }
}
