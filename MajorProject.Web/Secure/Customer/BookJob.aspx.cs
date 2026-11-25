using MajorProject.Business.Helpers;
using MajorProject.Business.Models;
using MajorProject.Business.Models.DTOs;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace MajorProject.Web.Secure.Customer
{
    public partial class BookJob : CustomerBasePage
    {
        private JobService _jobService;
        private JobTypeService _jobTypeService;
        private CustomerService _customerService;
        private InvoiceService _invoiceService;
        private DataTable _catalog;
        private bool _servicesReady;
        private readonly List<HtmlInputCheckBox> _serviceChecks = new List<HtmlInputCheckBox>();
        private BookingCart Cart
        {
            get
            {
                var cart = ViewState["CustomerBookingCart"] as BookingCart;
                if (cart == null || cart.CustomerID != WebSession.CustomerID)
                {
                    cart = new BookingCart(WebSession.CustomerID);
                    ViewState["CustomerBookingCart"] = cart;
                }

                return cart;
            }

            set
            {
                ViewState["CustomerBookingCart"] = value;
            }
        }

        private BookingSubmissionLedger Ledger
        {
            get
            {
                var ledger = Session["CustomerBookingSubmissions"] as BookingSubmissionLedger;
                if (ledger == null)
                    Session["CustomerBookingSubmissions"] = ledger = new BookingSubmissionLedger();
                return ledger;
            }
        }

        private string IssueBookingToken()
        {
            BookingSubmissionLedger ledger = Ledger;
            string token = ledger.Issue(WebSession.CustomerID);
            Session["CustomerBookingSubmissions"] = ledger;
            return token;
        }

        private string BookingToken
        {
            get
            {
                return Convert.ToString(ViewState["BookingSubmissionToken"]);
            }

            set
            {
                ViewState["BookingSubmissionToken"] = value;
            }
        }

        private string ConnectionString
        {
            get
            {
                var settings = ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"];
                if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                    throw new ConfigurationErrorsException("The database connection is not configured.");
                return settings.ConnectionString;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            AccessControl.RequireRole("Customer");
            if (!WebSession.IsLoggedIn || !WebSession.HasRole("Customer") || WebSession.CustomerID <= 0)
                return;
            try
            {
                _jobService = new JobService(ConnectionString);
                _jobTypeService = new JobTypeService(ConnectionString);
                _customerService = new CustomerService(ConnectionString);
                _invoiceService = new InvoiceService(ConnectionString);
                // Recreate dynamic controls during Init so checked values survive postbacks.
                LoadServiceTypes();
            }
            catch (Exception ex)
            {
                Logger.Error("Loading the customer booking service catalogue.", ex);
                ShowError("The booking service is unavailable. Please try again later.");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (WebSession.CustomerID <= 0)
            {
                ShowError("Your customer session has expired. Please sign in again.");
                return;
            }

            if (!IsPostBack)
            {
                BookingToken = IssueBookingToken();
                if (_customerService != null)
                    LoadCustomerInformation();
            }

            txtPromiseDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (WebSession.CustomerID <= 0)
            {
                btnSubmit.Enabled = false;
                btnAddSelected.Enabled = false;
                return;
            }

            gvBookingCart.DataSource = Cart.Lines;
            gvBookingCart.DataBind();
            lblCartCount.Text = Cart.ServiceCount.ToString(CultureInfo.InvariantCulture);
            lblCartUnits.Text = Cart.TotalQuantity.ToString(CultureInfo.InvariantCulture);
            btnAddSelected.Enabled = _servicesReady;
            btnSubmit.Enabled = _servicesReady;
            foreach (HtmlInputCheckBox box in _serviceChecks)
            {
                var label = box.Parent as HtmlGenericControl;
                if (label != null)
                    label.Attributes["class"] = "service-type-card" + (box.Checked ? " selected" : "");
            }
        }

        private void LoadServiceTypes()
        {
            phServiceTypeCards.Controls.Clear();
            _serviceChecks.Clear();
            _catalog = _jobTypeService.GetAllJobTypes();
            if (_catalog == null || _catalog.Rows.Count == 0)
            {
                ShowError("No services are currently available. Please try again later.");
                return;
            }

            var grid = new HtmlGenericControl("div");
            grid.Attributes["class"] = "service-type-grid";
            grid.Attributes["data-service-catalog"] = "true";
            foreach (DataRow row in _catalog.Rows)
            {
                int id;
                if (!int.TryParse(Convert.ToString(row["JobTypeID"]), out id) || id <= 0)
                    continue;
                string name = Convert.ToString(row["JobTypeName"]);
                var label = new HtmlGenericControl("label");
                label.Attributes["class"] = "service-type-card";
                var check = new HtmlInputCheckBox
                {
                    ID = "svc_" + id,
                    Value = id.ToString(CultureInfo.InvariantCulture)
                };
                var title = new HtmlGenericControl("span");
                title.Attributes["class"] = "card-title";
                title.InnerText = name;
                var desc = new HtmlGenericControl("span");
                desc.Attributes["class"] = "card-desc";
                desc.InnerText = "Select to add this service to your booking.";
                label.Controls.Add(check);
                label.Controls.Add(title);
                label.Controls.Add(desc);
                grid.Controls.Add(label);
                _serviceChecks.Add(check);
            }

            phServiceTypeCards.Controls.Add(grid);
            _servicesReady = _serviceChecks.Count > 0;
        }

        protected void cvServiceType_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = _servicesReady && _serviceChecks.Any(check => check.Checked);
        }

        protected void cvCart_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = WebSession.CustomerID > 0 && Cart.ServiceCount > 0;
        }

        private BookingCart ReadEditedCartQuantities(int? removedServiceID = null)
        {
            var draft = new BookingCart(WebSession.CustomerID);
            foreach (BookingCartLine line in Cart.Lines)
                draft.AddService(line.JobTypeID, line.ServiceName, line.Quantity);
            foreach (GridViewRow row in gvBookingCart.Rows)
            {
                if (row.RowType != DataControlRowType.DataRow)
                    continue;
                if (row.RowIndex >= gvBookingCart.DataKeys.Count)
                    throw new ArgumentException("Your cart could not be read. Refresh the page and try again.");
                int id = Convert.ToInt32(gvBookingCart.DataKeys[row.RowIndex].Value, CultureInfo.InvariantCulture);
                if (removedServiceID.HasValue && id == removedServiceID.Value)
                    continue;
                var input = row.FindControl("txtCartQuantity") as TextBox;
                int quantity;
                if (input == null || !int.TryParse(input.Text, NumberStyles.None, CultureInfo.InvariantCulture, out quantity))
                    throw new ArgumentException("Enter a whole-number quantity from 1 to 1000 for every cart service.");
                draft.SetQuantity(id, quantity);
            }

            return draft;
        }

        protected void btnAddSelected_Click(object sender, EventArgs e)
        {
            ClearMessage();
            Page.Validate("CartSelection");
            if (!Page.IsValid)
                return;
            try
            {
                var selection = new List<BookingCartLine>();
                foreach (HtmlInputCheckBox box in _serviceChecks.Where(check => check.Checked))
                {
                    int id = int.Parse(box.Value, CultureInfo.InvariantCulture);
                    DataTable service = _jobTypeService.GetJobTypeByID(id);
                    if (service == null || service.Rows.Count == 0)
                        throw new ArgumentException("A selected service is no longer available. Please refresh the service list.");
                    selection.Add(new BookingCartLine(id, Convert.ToString(service.Rows[0]["JobTypeName"]), 1));
                }

                BookingCart draft = ReadEditedCartQuantities();
                draft.AddSelection(selection); // Atomic: a failed selection does not partially change the draft.
                Cart = draft;
                foreach (HtmlInputCheckBox box in _serviceChecks)
                    box.Checked = false;
                ShowSuccess("Selected services added. Review the quantities in your booking cart.");
                pnlCart.Focus();
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Adding selected services to a customer cart.", ex);
                ShowError("The services could not be added. Please try again.");
            }
        }

        protected void gvBookingCart_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "SetQuantity" && e.CommandName != "RemoveLine")
                return;
            ClearMessage();
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id) || id <= 0)
            {
                ShowError("The selected cart line is invalid.");
                return;
            }

            try
            {
                BookingCart draft = ReadEditedCartQuantities(e.CommandName == "RemoveLine" ? (int? )id : null);
                if (e.CommandName == "RemoveLine")
                {
                    if (!draft.RemoveService(id))
                        throw new ArgumentException("This service is no longer in your cart.");
                }
                else
                {
                    var control = e.CommandSource as System.Web.UI.Control;
                    var row = control == null ? null : control.NamingContainer as GridViewRow;
                    var input = row == null ? null : row.FindControl("txtCartQuantity") as TextBox;
                    int quantity;
                    if (input == null || !int.TryParse(input.Text, NumberStyles.None, CultureInfo.InvariantCulture, out quantity))
                        throw new ArgumentException("Enter a whole-number quantity from 1 to 1000, then choose Update.");
                    draft.SetQuantity(id, quantity);
                }

                Cart = draft;
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
        }

        private void LoadCustomerInformation()
        {
            try
            {
                DataTable table = _customerService.GetCustomerByID(WebSession.CustomerID);
                if (table == null || table.Rows.Count == 0)
                {
                    ShowError("Your customer profile could not be loaded.");
                    return;
                }

                DataRow row = table.Rows[0];
                txtAddress.Text = Convert.ToString(row["FullAddress"]);
                txtArea.Text = Convert.ToString(row["Area"]);
                txtCity.Text = Convert.ToString(row["City"]);
                txtProvince.Text = Convert.ToString(row["Province"]);
                litAddress.Text = Server.HtmlEncode(txtAddress.Text);
                litAreaCityProvince.Text = Server.HtmlEncode(string.Join(", ", new[] { txtArea.Text, txtCity.Text, txtProvince.Text }.Where(value => !string.IsNullOrWhiteSpace(value))));
            }
            catch (Exception ex)
            {
                Logger.Error("Loading customer booking address.", ex);
                ShowError("Your saved address could not be loaded. Please try again.");
            }
        }

        protected async void btnSubmit_Click(object sender, EventArgs e)
        {
            ClearMessage();
            int customerID = WebSession.CustomerID;
            int previousJob;
            if (customerID <= 0 || !Ledger.TryGetResult(BookingToken, customerID, out previousJob))
            {
                ShowError("This booking form has expired. Refresh the page and try again.");
                return;
            }

            if (previousJob > 0)
            {
                ClearBookingForm();
                ShowSuccess("This booking was already submitted. Your reference is Job #" + previousJob + ".");
                return;
            }

            try
            {
                Cart = ReadEditedCartQuantities();
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
                return;
            }

            Page.Validate("Booking");
            if (!Page.IsValid)
                return;
            if (!_servicesReady)
            {
                ShowError("The booking service is unavailable. Please try again later.");
                return;
            }

            try
            {
                DateTime promiseDate;
                if (!DateTime.TryParseExact(txtPromiseDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out promiseDate) || promiseDate.Date < DateTime.Today)
                    throw new ArgumentException("Choose today or a future service date.");
                string description = BookingCart.NormalizeDescription(txtDescription.Text);
                DataTable customer = _customerService.GetCustomerByID(customerID);
                if (customer == null || customer.Rows.Count == 0)
                    throw new ArgumentException("Your customer profile could not be loaded. Please sign in again.");
                BookingCart draft = Cart;
                if (draft.CustomerID != customerID)
                    throw new InvalidOperationException("This cart does not belong to your customer account.");
                foreach (BookingCartLine line in draft.Lines)
                {
                    DataTable service = _jobTypeService.GetJobTypeByID(line.JobTypeID);
                    if (service == null || service.Rows.Count == 0)
                        throw new ArgumentException("A service in your cart is no longer available. Remove it and choose another service.");
                }

                DataTable cart = draft.ToDataTable();
                // One call creates one parent booking with every service line. Existing Data access
                // remains responsible for stock, price calculation and technician allocation.
                int jobID = _jobService.CreateJobWithDetailsAndMaterials(customerID, promiseDate.Date, description, cart);
                // Record completion before PDF/email work, so ordinary duplicate posts do not create another job.
                BookingSubmissionLedger ledger = Ledger;
                ledger.Complete(BookingToken, customerID, jobID);
                Session["CustomerBookingSubmissions"] = ledger;
                ClearBookingForm();
                bool invoiceProcessed = await ProcessInvoiceAfterJobCreationAsync(jobID);
                ShowSuccess("Booking submitted. Your reference is Job #" + jobID + ". " + (invoiceProcessed ? "Your invoice was emailed to you." : "Your booking is saved. Your invoice email could not be sent; check Invoices or contact the team."));
            }
            catch (ArgumentException ex)
            {
                ShowBusinessError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowBusinessError(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Creating a customer cart booking.", ex);
                ShowError("Your booking could not be submitted. Your cart is still available; please try again.");
            }
        }

        private void ClearBookingForm()
        {
            Cart = new BookingCart(WebSession.CustomerID);
            txtDescription.Text = string.Empty;
            txtPromiseDate.Text = string.Empty;
            BookingToken = IssueBookingToken();
            foreach (HtmlInputCheckBox box in _serviceChecks)
                box.Checked = false;
        }

        private async Task<bool> ProcessInvoiceAfterJobCreationAsync(int jobID)
        {
            InvoiceDto invoice;
            try
            {
                invoice = BuildInvoiceFromSavedJob(jobID);
            }
            catch (Exception ex)
            {
                Logger.Error("Building invoice for customer-created Job " + jobID, ex);
                return false;
            }

            byte[] pdfBytes;
            try
            {
                InvoicePdfService pdfService = new InvoicePdfService();
                pdfBytes = pdfService.GenerateInvoicePdf(invoice);
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    throw new InvalidOperationException("The invoice PDF generator returned empty data.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Generating invoice PDF for customer-created Job " + jobID, ex);
                return false;
            }

            string filePath;
            try
            {
                filePath = SaveInvoicePdf(invoice, pdfBytes);
            }
            catch (Exception ex)
            {
                Logger.Error("Saving invoice PDF for customer-created Job " + jobID, ex);
                return false;
            }

            try
            {
                _invoiceService.SaveInvoiceToDatabase(jobID, pdfBytes, filePath);
            }
            catch (Exception ex)
            {
                Logger.Error("Saving invoice PDF to database for customer-created Job " + jobID, ex);
                return false;
            }

            if (string.IsNullOrWhiteSpace(invoice.CustomerEmail))
            {
                Logger.Error("Customer-created Job " + jobID + " has no customer email address.");
                return false;
            }

            bool emailSent;
            try
            {
                MailjetEmailService mailjet = CreateMailjetEmailService();
                emailSent = await mailjet.SendEmailWithAttachmentAsync(invoice.CustomerEmail, invoice.CustomerName, "THY WILL Invoice - Job #" + invoice.JobId, BuildInvoiceEmailBody(invoice), pdfBytes, "Invoice-" + invoice.JobId + ".pdf");
            }
            catch (Exception ex)
            {
                Logger.Error("Sending invoice email for customer-created Job " + jobID, ex);
                return false;
            }

            if (!emailSent)
            {
                Logger.Error("Mailjet returned false while sending invoice email for customer-created Job " + jobID);
                return false;
            }

            try
            {
                _invoiceService.MarkInvoiceAsSent(jobID, invoice.CustomerEmail);
            }
            catch (Exception ex)
            {
                Logger.Error("Marking invoice as sent for customer-created Job " + jobID, ex);
                // The email was already sent successfully.
                return true;
            }

            return true;
        }

        // ============================================================
        // BUILD INVOICE FROM SAVED JOB
        // ============================================================
        private InvoiceDto BuildInvoiceFromSavedJob(int jobID)
        {
            DataTable jobTable = _jobService.GetJobByID(jobID);
            if (jobTable == null || jobTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("The newly created job could not be retrieved.");
            }

            DataRow jobRow = jobTable.Rows[0];
            int customerID = Convert.ToInt32(jobRow["CustomerID"]);
            DataTable customerTable = _customerService.GetCustomerByID(customerID);
            if (customerTable == null || customerTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("The customer for the new job could not be retrieved.");
            }

            DataRow customerRow = customerTable.Rows[0];
            InvoiceDto invoice = new InvoiceDto();
            invoice.JobId = Convert.ToInt32(jobRow["JobID"]);
            invoice.InvoiceDate = Convert.ToDateTime(jobRow["CreatedAt"]);
            invoice.CustomerName = Convert.ToString(customerRow["FullName"]);
            invoice.CustomerEmail = Convert.ToString(customerRow["EmailAddress"]);
            invoice.LabourCost = Convert.ToDecimal(jobRow["LabourCost"]);
            invoice.TransportCost = Convert.ToDecimal(jobRow["TransportCost"]);
            invoice.MaterialCost = Convert.ToDecimal(jobRow["TotalMaterialCost"]);
            invoice.TaxAmount = Convert.ToDecimal(jobRow["TaxAmount"]);
            invoice.GrandTotal = Convert.ToDecimal(jobRow["GrandTotal"]);
            DataTable details = _jobService.GetJobDetailsByJobID(jobID);
            if (details != null)
            {
                foreach (DataRow detailRow in details.Rows)
                {
                    InvoiceItemDto item = new InvoiceItemDto();
                    item.ServiceName = Convert.ToString(detailRow["JobTypeName"]);
                    item.Quantity = Convert.ToInt32(detailRow["Quantity"]);
                    invoice.Items.Add(item);
                }
            }

            return invoice;
        }

        // ============================================================
        // SAVE INVOICE PDF
        // ============================================================
        private string SaveInvoicePdf(InvoiceDto invoice, byte[] pdfBytes)
        {
            if (invoice == null)
            {
                throw new ArgumentNullException(nameof(invoice));
            }

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                throw new InvalidOperationException("The generated invoice PDF is empty.");
            }

            string invoiceDirectory = Server.MapPath("~/App_Data/Invoices");
            if (string.IsNullOrWhiteSpace(invoiceDirectory))
            {
                throw new InvalidOperationException("The invoice storage directory could not be resolved.");
            }

            if (!Directory.Exists(invoiceDirectory))
            {
                Directory.CreateDirectory(invoiceDirectory);
            }

            string fileName = "Invoice-" + invoice.JobId + ".pdf";
            string filePath = Path.Combine(invoiceDirectory, fileName);
            File.WriteAllBytes(filePath, pdfBytes);
            return "~/App_Data/Invoices/" + fileName;
        }

        // ============================================================
        // MAILJET CONFIGURATION
        // ============================================================
        private MailjetEmailService CreateMailjetEmailService()
        {
            string apiKey = ConfigurationManager.AppSettings["MailjetApiKey"];
            string apiSecret = ConfigurationManager.AppSettings["MailjetApiSecret"];
            string fromEmail = ConfigurationManager.AppSettings["MailjetFromEmail"];
            string fromName = ConfigurationManager.AppSettings["MailjetFromName"];
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret) || string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(fromName))
            {
                throw new ConfigurationErrorsException("Mailjet configuration is incomplete.");
            }

            return new MailjetEmailService(apiKey, apiSecret, fromEmail, fromName);
        }

        // ============================================================
        // INVOICE EMAIL BODY
        // ============================================================
        private string BuildInvoiceEmailBody(InvoiceDto invoice)
        {
            string customerName = Server.HtmlEncode(invoice.CustomerName ?? "Customer");
            return "<p>Dear " + customerName + ",</p>" + "<p>Thank you for choosing " + "THY WILL Electrical &amp; Construction.</p>" + "<p>Your invoice for Job #" + invoice.JobId + " is attached to this email.</p>" + "<p><strong>Invoice total:</strong> R" + invoice.GrandTotal.ToString("N2", CultureInfo.InvariantCulture) + "</p>" + "<p>Please keep this invoice for your records.</p>" + "<p>Kind regards,<br />" + "THY WILL Electrical &amp; Construction</p>";
        }

        private void ShowBusinessError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                ShowError("We could not submit your service request. Please try again.");
                return;
            }

            if (message.IndexOf("already assigned", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ShowError("The selected date is no longer available. Please choose another date and try again.");
                return;
            }

            if (message.IndexOf("no longer active", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ShowError("Your account is no longer active. Please contact THY WILL for assistance.");
                return;
            }

            if (message.IndexOf("Insufficient stock", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ShowError("The selected service cannot currently be booked because the required materials are unavailable. Please choose another service or contact THY WILL.");
                return;
            }

            if (message.IndexOf("Inventory", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ShowError("We could not complete the service request because of an inventory issue. Please try again later or contact THY WILL.");
                return;
            }

            if (message.IndexOf("technician", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ShowError("We could not assign a technician for the selected date. Please choose another date and try again.");
                return;
            }

            ShowError(message);
        }

        // ============================================================
        // MESSAGE HELPERS
        // ============================================================
        private void ShowSuccess(string message)
        {
            pnlMessage.Visible = true;
            pnlMessage.Attributes["role"] = "status";
            pnlMessage.CssClass = "alert alert-success booking-message";
            lblMessage.Text = Server.HtmlEncode(message);
        }

        private void ShowError(string message)
        {
            pnlMessage.Visible = true;
            pnlMessage.Attributes["role"] = "alert";
            pnlMessage.CssClass = "alert alert-danger booking-message";
            lblMessage.Text = Server.HtmlEncode(message);
        }

        private void ClearMessage()
        {
            pnlMessage.Visible = false;
            lblMessage.Text = string.Empty;
        }
    }
}
