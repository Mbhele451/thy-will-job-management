using MajorProject.Business.Models.DTOs;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;

namespace MajorProject.Web.Manager
{
    public partial class WorkOrders : ManagerBasePage
    {
        // ============================================================
        // VALIDATION LIMITS
        // ============================================================
        private const int MaxQuantityPerLine = 1000;
        private const int MaxCartLines = 50;
        private const int MaxJobNoteLength = 255;
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
                    throw new ConfigurationErrorsException("Connection string 'GroupWst34ConnectionString' was not found.");
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

        private JobTypeService JobTypeService
        {
            get
            {
                return new JobTypeService(ConnectionString);
            }
        }

        private StaffService StaffService
        {
            get
            {
                return new StaffService(ConnectionString);
            }
        }

        private CustomerService CustomerService
        {
            get
            {
                return new CustomerService(ConnectionString);
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
        // MAILJET
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
        // BUILD INVOICE FROM COMMITTED DATABASE DATA
        // ============================================================
        private InvoiceDto BuildInvoiceFromSavedJob(int jobID)
        {
            DataTable jobTable = JobService.GetJobByID(jobID);
            if (jobTable == null || jobTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("The newly created job could not be retrieved.");
            }

            DataRow jobRow = jobTable.Rows[0];
            int customerID = Convert.ToInt32(jobRow["CustomerID"]);
            DataTable customerTable = CustomerService.GetCustomerByID(customerID);
            if (customerTable == null || customerTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("The customer for the new job could not be retrieved.");
            }

            DataRow customerRow = customerTable.Rows[0];
            InvoiceDto invoice = new InvoiceDto();
            // --------------------------------------------------------
            // Job information
            // --------------------------------------------------------
            invoice.JobId = Convert.ToInt32(jobRow["JobID"]);
            invoice.InvoiceDate = Convert.ToDateTime(jobRow["CreatedAt"]);
            // --------------------------------------------------------
            // Customer information
            // --------------------------------------------------------
            invoice.CustomerName = Convert.ToString(customerRow["FullName"]);
            invoice.CustomerEmail = Convert.ToString(customerRow["EmailAddress"]);
            // --------------------------------------------------------
            // IMPORTANT:
            //
            // These values come from the committed Jobs row.
            //
            // Do not recalculate them from JobTypes or InventoryItems.
            // --------------------------------------------------------
            invoice.LabourCost = Convert.ToDecimal(jobRow["LabourCost"]);
            invoice.TransportCost = Convert.ToDecimal(jobRow["TransportCost"]);
            invoice.MaterialCost = Convert.ToDecimal(jobRow["TotalMaterialCost"]);
            invoice.TaxAmount = Convert.ToDecimal(jobRow["TaxAmount"]);
            invoice.GrandTotal = Convert.ToDecimal(jobRow["GrandTotal"]);
            // --------------------------------------------------------
            // Job service details
            // --------------------------------------------------------
            DataTable details = JobService.GetJobDetailsByJobID(jobID);
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
        // SAVE INVOICE PDF TO SERVER
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
        // INVOICE EMAIL BODY
        // ============================================================
        private string BuildInvoiceEmailBody(InvoiceDto invoice)
        {
            string customerName = Server.HtmlEncode(invoice.CustomerName ?? "Customer");
            return "<p>Dear " + customerName + ",</p>" + "<p>Thank you for choosing THY WILL Electrical &amp; Construction.</p>" + "<p>Your invoice for Job #" + invoice.JobId + " is attached to this email.</p>" + "<p><strong>Invoice total:</strong> R" + invoice.GrandTotal.ToString("N2", CultureInfo.InvariantCulture) + "</p>" + "<p>Please keep this invoice for your records.</p>" + "<p>Kind regards,<br />" + "THY WILL Electrical &amp; Construction</p>";
        }

        // ============================================================
        // PROCESS INVOICE AFTER JOB CREATION
        // ============================================================
        private async System.Threading.Tasks.Task<bool> ProcessInvoiceAfterJobCreationAsync(int jobID)
        {
            InvoiceDto invoice;
            // --------------------------------------------------------
            // 1. Retrieve committed job and build invoice
            // --------------------------------------------------------
            try
            {
                invoice = BuildInvoiceFromSavedJob(jobID);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Building invoice for Job " + jobID, ex);
                return false;
            }

            // --------------------------------------------------------
            // 2. Generate PDF
            // --------------------------------------------------------
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
                HandleUnexpectedError("Generating invoice PDF for Job " + jobID, ex);
                return false;
            }

            // --------------------------------------------------------
            // 3. Save PDF
            // --------------------------------------------------------
            string filePath;
            try
            {
                filePath = SaveInvoicePdf(invoice, pdfBytes);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Saving invoice PDF for Job " + jobID, ex);
                return false;
            }

            // --------------------------------------------------------
            // 4. Save invoice PDF to database
            // --------------------------------------------------------
            try
            {
                InvoiceService.SaveInvoiceToDatabase(jobID, pdfBytes, filePath);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Saving invoice PDF to database for Job " + jobID, ex);
                return false;
            }

            // --------------------------------------------------------
            // 5. Customer email validation
            // --------------------------------------------------------
            if (string.IsNullOrWhiteSpace(invoice.CustomerEmail))
            {
                return false;
            }

            // --------------------------------------------------------
            // 6. Send invoice through Mailjet
            // --------------------------------------------------------
            bool emailSent;
            try
            {
                MailjetEmailService mailjet = CreateMailjetEmailService();
                emailSent = await mailjet.SendEmailWithAttachmentAsync(invoice.CustomerEmail, invoice.CustomerName, "THY WILL Invoice - Job #" + invoice.JobId, BuildInvoiceEmailBody(invoice), pdfBytes, "Invoice-" + invoice.JobId + ".pdf");
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Sending invoice email for Job " + jobID, ex);
                return false;
            }

            // --------------------------------------------------------
            // 7. Mark invoice as sent
            // --------------------------------------------------------
            if (!emailSent)
            {
                return false;
            }

            try
            {
                InvoiceService.MarkInvoiceAsSent(jobID, invoice.CustomerEmail);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Marking invoice as sent for Job " + jobID, ex);
                // Email was already sent successfully.
                return true;
            }

            return true;
        }

        // ============================================================
        // VIEWSTATE / CART
        // ============================================================
        private DataTable Cart
        {
            get
            {
                if (ViewState["JobCart"] == null)
                {
                    ViewState["JobCart"] = CreateCartTable();
                }

                return (DataTable)ViewState["JobCart"];
            }

            set
            {
                ViewState["JobCart"] = value;
            }
        }

        private int SelectedCustomerID
        {
            get
            {
                if (ViewState["SelectedCustomerID"] == null)
                {
                    return 0;
                }

                return Convert.ToInt32(ViewState["SelectedCustomerID"]);
            }

            set
            {
                ViewState["SelectedCustomerID"] = value;
            }
        }

        private string CustomerSortExpression
        {
            get
            {
                return ViewState["CustomerSortExpression"] as string ?? "FullName ASC";
            }

            set
            {
                ViewState["CustomerSortExpression"] = value;
            }
        }

        // ============================================================
        // PAGE LOAD
        // ============================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitialisePage();
            }
        }

        private void InitialisePage()
        {
            HideMessages();
            txtQuantity.Text = "1";
            SetPromiseDateMinimum();
            txtPromiseDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            LoadServiceTypes();
            LoadCustomers();
            LoadTechnicians(DateTime.Today);
            BindCart();
            pnlSelectedCustomer.Visible = false;
        }

        // ============================================================
        // CUSTOMER GRID
        // ============================================================
        private void LoadCustomers()
        {
            DataTable customers;
            string search = (txtCustomerSearch.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(search))
            {
                customers = CustomerService.GetAllCustomers();
            }
            else
            {
                customers = CustomerService.SearchCustomersByName(search);
            }

            if (customers == null)
            {
                customers = new DataTable();
            }

            DataView view = customers.DefaultView;
            if (view.Table.Columns.Contains("AccountStatus"))
            {
                view.RowFilter = "AccountStatus = 'Active'";
            }

            view.Sort = GetSafeCustomerSortExpression(CustomerSortExpression);
            gvCustomers.DataSource = view;
            gvCustomers.DataBind();
            RestoreCustomerSelection();
        }

        private string GetSafeCustomerSortExpression(string sortExpression)
        {
            string[] allowedColumns =
            {
                "FullName",
                "PhoneNumber",
                "EmailAddress",
                "Area",
                "CustomerType",
                "DateRegistered"
            };
            string requested = sortExpression ?? string.Empty;
            string[] parts = requested.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return "FullName ASC";
            }

            string column = parts[0];
            bool allowed = false;
            foreach (string allowedColumn in allowedColumns)
            {
                if (allowedColumn.Equals(column, StringComparison.OrdinalIgnoreCase))
                {
                    allowed = true;
                    break;
                }
            }

            if (!allowed)
            {
                return "FullName ASC";
            }

            string direction = parts.Length > 1 ? parts[1] : "ASC";
            if (!direction.Equals("ASC", StringComparison.OrdinalIgnoreCase) && !direction.Equals("DESC", StringComparison.OrdinalIgnoreCase))
            {
                direction = "ASC";
            }

            return column + " " + direction;
        }

        private void RestoreCustomerSelection()
        {
            if (SelectedCustomerID <= 0)
            {
                return;
            }

            for (int i = 0; i < gvCustomers.Rows.Count; i++)
            {
                DataKey key = gvCustomers.DataKeys[i];
                if (key == null)
                {
                    continue;
                }

                int customerID = Convert.ToInt32(key.Value);
                if (customerID == SelectedCustomerID)
                {
                    gvCustomers.SelectedIndex = i;
                    break;
                }
            }
        }

        protected void btnSearchCustomers_Click(object sender, EventArgs e)
        {
            gvCustomers.PageIndex = 0;
            LoadCustomers();
        }

        protected void btnClearCustomerSearch_Click(object sender, EventArgs e)
        {
            txtCustomerSearch.Text = string.Empty;
            gvCustomers.PageIndex = 0;
            LoadCustomers();
        }

        protected void gvCustomers_Sorting(object sender, GridViewSortEventArgs e)
        {
            string current = CustomerSortExpression;
            string column = e.SortExpression;
            string safeColumn = GetSafeCustomerSortExpression(column + " ASC").Split(' ')[0];
            if (current.StartsWith(safeColumn + " ", StringComparison.OrdinalIgnoreCase))
            {
                if (current.EndsWith(" ASC", StringComparison.OrdinalIgnoreCase))
                {
                    CustomerSortExpression = safeColumn + " DESC";
                }
                else
                {
                    CustomerSortExpression = safeColumn + " ASC";
                }
            }
            else
            {
                CustomerSortExpression = safeColumn + " ASC";
            }

            LoadCustomers();
        }

        protected void gvCustomers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCustomers.PageIndex = e.NewPageIndex;
            LoadCustomers();
        }

        protected void gvCustomers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "SelectCustomer")
            {
                return;
            }

            int customerID;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out customerID) || customerID <= 0)
            {
                ShowError("The selected customer is invalid.");
                return;
            }

            SelectCustomer(customerID);
        }

        private void SelectCustomer(int customerID)
        {
            DataTable customer = CustomerService.GetCustomerByID(customerID);
            if (customer == null || customer.Rows.Count == 0)
            {
                ShowError("The selected customer could not be found.");
                return;
            }

            DataRow row = customer.Rows[0];
            string accountStatus = Convert.ToString(row["AccountStatus"]);
            if (!string.Equals(accountStatus, "Active", StringComparison.OrdinalIgnoreCase))
            {
                ShowError("Only active customers can be assigned to a new job.");
                return;
            }

            SelectedCustomerID = customerID;
            lblSelectedCustomerName.Text = Convert.ToString(row["FullName"]);
            string phone = Convert.ToString(row["PhoneNumber"]);
            string email = Convert.ToString(row["EmailAddress"]);
            string area = Convert.ToString(row["Area"]);
            lblSelectedCustomerDetails.Text = string.Format("{0} • {1} • {2}", phone, email, area);
            pnlSelectedCustomer.Visible = true;
            ShowSuccess("Customer selected successfully.");
        }

        // ============================================================
        // SERVICE TYPES
        // ============================================================
        private void LoadServiceTypes()
        {
            DataTable jobTypes = JobTypeService.GetAllJobTypes();
            ddlServiceType.DataSource = jobTypes;
            ddlServiceType.DataTextField = "JobTypeName";
            ddlServiceType.DataValueField = "JobTypeID";
            ddlServiceType.DataBind();
            ddlServiceType.Items.Insert(0, new ListItem("-- Select service type --", ""));
        }

        // ============================================================
        // TECHNICIANS
        // ============================================================
        private void LoadTechnicians(DateTime promiseDate)
        {
            ddlTechnician.Items.Clear();
            ddlTechnician.Items.Add(new ListItem("-- Select technician --", ""));
            DataTable technicians = StaffService.GetAvailableTechnicians(promiseDate);
            if (technicians == null)
            {
                return;
            }

            foreach (DataRow row in technicians.Rows)
            {
                string staffID = Convert.ToString(row["StaffID"]);
                string fullName = Convert.ToString(row["FullName"]);
                ddlTechnician.Items.Add(new ListItem(fullName, staffID));
            }
        }

        protected void txtPromiseDate_TextChanged(object sender, EventArgs e)
        {
            DateTime promiseDate;
            if (!TryGetPromiseDate(out promiseDate))
            {
                ddlTechnician.Items.Clear();
                ddlTechnician.Items.Add(new ListItem("-- Select a valid date first --", ""));
                return;
            }

            LoadTechnicians(promiseDate.Date);
            HideMessages();
        }

        // ============================================================
        // DATE VALIDATION
        // ============================================================
        private bool TryGetPromiseDate(out DateTime promiseDate)
        {
            promiseDate = DateTime.MinValue;
            if (!DateTime.TryParseExact(txtPromiseDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out promiseDate))
            {
                return false;
            }

            if (promiseDate.Date < DateTime.Today)
            {
                ShowError("Promise date cannot be earlier than today.");
                return false;
            }

            return true;
        }

        // ============================================================
        // CART
        // ============================================================
        private DataTable CreateCartTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CartLineID", typeof(int));
            table.Columns.Add("JobTypeID", typeof(int));
            table.Columns.Add("JobTypeName", typeof(string));
            table.Columns.Add("Quantity", typeof(int));
            return table;
        }

        private int GetNextCartLineID()
        {
            DataTable cart = Cart;
            if (cart.Rows.Count == 0)
            {
                return 1;
            }

            int highestID = 0;
            foreach (DataRow row in cart.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                int cartLineID = Convert.ToInt32(row["CartLineID"]);
                if (cartLineID > highestID)
                {
                    highestID = cartLineID;
                }
            }

            return highestID + 1;
        }

        private void BindCart()
        {
            gvCart.DataSource = Cart;
            gvCart.DataBind();
        }

        // ============================================================
        // ADD SERVICE
        // ============================================================
        protected void btnAddService_Click(object sender, EventArgs e)
        {
            HideMessages();
            if (!ValidateServiceEntry())
            {
                return;
            }

            DataTable cart = Cart;
            if (cart.Rows.Count >= MaxCartLines)
            {
                ShowError("A job cannot contain more than " + MaxCartLines + " service lines.");
                return;
            }

            int jobTypeID = Convert.ToInt32(ddlServiceType.SelectedValue);
            int quantity;
            if (!int.TryParse(txtQuantity.Text.Trim(), out quantity))
            {
                ShowError("Quantity must be a whole number.");
                return;
            }

            // --------------------------------------------------------
            // We only retrieve the service to verify that it still
            // exists and obtain its display name.
            //
            // We do NOT copy pricing into the cart.
            // --------------------------------------------------------
            DataTable serviceData = JobTypeService.GetJobCostsAndMaterials(jobTypeID);
            if (serviceData == null || serviceData.Rows.Count == 0)
            {
                ShowError("The selected service type could not be found.");
                return;
            }

            DataRow service = serviceData.Rows[0];
            DataRow cartRow = cart.NewRow();
            cartRow["CartLineID"] = GetNextCartLineID();
            cartRow["JobTypeID"] = jobTypeID;
            cartRow["JobTypeName"] = Convert.ToString(service["JobTypeName"]);
            cartRow["Quantity"] = quantity;
            cart.Rows.Add(cartRow);
            Cart = cart;
            BindCart();
            ddlServiceType.SelectedIndex = 0;
            txtQuantity.Text = "1";
            ShowSuccess("Service added to the job.");
        }

        private bool ValidateServiceEntry()
        {
            if (SelectedCustomerID <= 0)
            {
                ShowError("Please select an active customer first.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ddlServiceType.SelectedValue))
            {
                ShowError("Please select a service type.");
                return false;
            }

            int quantity;
            if (!int.TryParse(txtQuantity.Text.Trim(), out quantity) || quantity <= 0)
            {
                ShowError("Quantity must be a whole number greater than zero.");
                return false;
            }

            if (quantity > MaxQuantityPerLine)
            {
                ShowError("Quantity cannot exceed " + MaxQuantityPerLine + " per service line.");
                return false;
            }

            DateTime promiseDate;
            if (!TryGetPromiseDate(out promiseDate))
            {
                ShowError("Please select a valid promise date.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ddlTechnician.SelectedValue))
            {
                ShowError("Please assign a technician.");
                return false;
            }

            return true;
        }

        // ============================================================
        // REMOVE SERVICE
        // ============================================================
        protected void gvCart_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "RemoveService")
            {
                return;
            }

            int cartLineID;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out cartLineID) || cartLineID <= 0)
            {
                ShowError("The selected service could not be removed.");
                return;
            }

            DataTable cart = Cart;
            DataRow[] rows = cart.Select("CartLineID = " + cartLineID);
            if (rows.Length == 0)
            {
                ShowError("The selected service could not be found.");
                return;
            }

            cart.Rows.Remove(rows[0]);
            Cart = cart;
            BindCart();
            ShowSuccess("Service removed from the job.");
        }

        // ============================================================
        // SAVE JOB
        // ============================================================
        protected async void btnSaveJob_Click(object sender, EventArgs e)
        {
            HideMessages();
            // --------------------------------------------------------
            // 1. Validate page data
            // --------------------------------------------------------
            if (!ValidateJob())
            {
                return;
            }

            DateTime promiseDate;
            if (!TryGetPromiseDate(out promiseDate))
            {
                ShowError("Please select a valid promise date.");
                return;
            }

            int technicianID;
            if (!int.TryParse(ddlTechnician.SelectedValue, out technicianID) || technicianID <= 0)
            {
                ShowError("The selected technician is invalid.");
                return;
            }

            // --------------------------------------------------------
            // 2. Refresh service names and validate cart
            //
            // No pricing is calculated here.
            // The database remains authoritative.
            // --------------------------------------------------------
            if (!RefreshCartFromDatabase())
            {
                return;
            }

            if (!ValidateCartContents())
            {
                return;
            }

            int jobID;
            // --------------------------------------------------------
            // 3. CREATE JOB
            //
            // JobDataAccess performs the authoritative transaction:
            //
            // Jobs
            // JobDetails
            // JobMaterials
            // inventory validation
            // inventory deduction
            // inventory transactions
            // financial calculations
            // --------------------------------------------------------
            try
            {
                jobID = JobService.CreateJobWithDetailsAndMaterials(SelectedCustomerID, technicianID, promiseDate.Date, (txtJobNote.Text ?? string.Empty).Trim(), Cart);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
                return;
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                ShowError(HandleUnexpectedError("Creating job from Manager WorkOrders page.", ex));
                return;
            }

            // --------------------------------------------------------
            // IMPORTANT:
            //
            // The job transaction has committed at this point.
            //
            // Invoice processing happens afterwards.
            // --------------------------------------------------------
            bool invoiceProcessed = await ProcessInvoiceAfterJobCreationAsync(jobID);
            // --------------------------------------------------------
            // 4. FINAL MESSAGE
            // --------------------------------------------------------
            if (invoiceProcessed)
            {
                ShowSuccess("Job #" + jobID + " was created successfully and the invoice was emailed to the customer.");
            }
            else
            {
                ShowSuccess("Job #" + jobID + " was created successfully, but the invoice could not be emailed to the customer.");
            }

            // --------------------------------------------------------
            // 5. Reset form
            // --------------------------------------------------------
            ResetJobForm();
        }

        // ============================================================
        // JOB VALIDATION
        // ============================================================
        private bool ValidateJob()
        {
            // --------------------------------------------------------
            // CUSTOMER
            // --------------------------------------------------------
            if (SelectedCustomerID <= 0)
            {
                ShowError("Please select an active customer.");
                return false;
            }

            DataTable customer = CustomerService.GetCustomerByID(SelectedCustomerID);
            if (customer == null || customer.Rows.Count == 0)
            {
                ShowError("The selected customer no longer exists.");
                SelectedCustomerID = 0;
                pnlSelectedCustomer.Visible = false;
                return false;
            }

            string accountStatus = Convert.ToString(customer.Rows[0]["AccountStatus"]);
            if (!string.Equals(accountStatus, "Active", StringComparison.OrdinalIgnoreCase))
            {
                ShowError("The selected customer is no longer active. Please select an active customer.");
                SelectedCustomerID = 0;
                pnlSelectedCustomer.Visible = false;
                LoadCustomers();
                return false;
            }

            // --------------------------------------------------------
            // CART
            // --------------------------------------------------------
            if (Cart == null || Cart.Rows.Count == 0)
            {
                ShowError("Please add at least one service to the job.");
                return false;
            }

            if (Cart.Rows.Count > MaxCartLines)
            {
                ShowError("A job cannot contain more than " + MaxCartLines + " service lines.");
                return false;
            }

            if (!ValidateCartContents())
            {
                return false;
            }

            // --------------------------------------------------------
            // PROMISE DATE
            // --------------------------------------------------------
            DateTime promiseDate;
            if (!TryGetPromiseDate(out promiseDate))
            {
                ShowError("Please select a valid promise date.");
                return false;
            }

            // --------------------------------------------------------
            // TECHNICIAN
            // --------------------------------------------------------
            int technicianID;
            if (!int.TryParse(ddlTechnician.SelectedValue, out technicianID) || technicianID <= 0)
            {
                ShowError("Please assign a valid technician.");
                return false;
            }

            if (!ValidateSelectedTechnician(technicianID, promiseDate.Date))
            {
                return false;
            }

            // --------------------------------------------------------
            // NOTE
            // --------------------------------------------------------
            string note = (txtJobNote.Text ?? string.Empty).Trim();
            if (note.Length > MaxJobNoteLength)
            {
                ShowError("The job note cannot exceed " + MaxJobNoteLength + " characters.");
                return false;
            }

            return true;
        }

        // ============================================================
        // CART VALIDATION
        // ============================================================
        private bool ValidateCartContents()
        {
            if (Cart == null)
            {
                ShowError("The job cart is invalid.");
                return false;
            }

            if (Cart.Rows.Count == 0)
            {
                ShowError("Please add at least one service to the job.");
                return false;
            }

            if (Cart.Rows.Count > MaxCartLines)
            {
                ShowError("A job cannot contain more than " + MaxCartLines + " service lines.");
                return false;
            }

            if (!Cart.Columns.Contains("JobTypeID") || !Cart.Columns.Contains("Quantity"))
            {
                ShowError("The job cart is missing required service information.");
                return false;
            }

            int validCartLines = 0;
            foreach (DataRow row in Cart.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                int jobTypeID;
                if (!int.TryParse(Convert.ToString(row["JobTypeID"]), out jobTypeID) || jobTypeID <= 0)
                {
                    ShowError("The job contains an invalid service type.");
                    return false;
                }

                int quantity;
                if (!int.TryParse(Convert.ToString(row["Quantity"]), out quantity) || quantity <= 0)
                {
                    ShowError("Every service quantity must be greater than zero.");
                    return false;
                }

                if (quantity > MaxQuantityPerLine)
                {
                    ShowError("A service quantity cannot exceed " + MaxQuantityPerLine + ".");
                    return false;
                }

                validCartLines++;
            }

            if (validCartLines == 0)
            {
                ShowError("At least one valid service is required.");
                return false;
            }

            return true;
        }

        // ============================================================
        // REFRESH SERVICE DATA
        // ============================================================
        //
        // This method is only responsible for confirming that each
        // service still exists and refreshing its display name.
        //
        // It does NOT calculate or store financial values.
        // ============================================================
        private bool RefreshCartFromDatabase()
        {
            DataTable cart = Cart;
            if (cart == null || cart.Rows.Count == 0)
            {
                ShowError("The job cart is empty.");
                return false;
            }

            foreach (DataRow cartRow in cart.Rows)
            {
                if (cartRow.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                int jobTypeID;
                if (!int.TryParse(Convert.ToString(cartRow["JobTypeID"]), out jobTypeID) || jobTypeID <= 0)
                {
                    ShowError("The job contains an invalid service type.");
                    return false;
                }

                DataTable serviceData = JobTypeService.GetJobCostsAndMaterials(jobTypeID);
                if (serviceData == null || serviceData.Rows.Count == 0)
                {
                    ShowError("One of the selected service types is no longer available.");
                    return false;
                }

                DataRow service = serviceData.Rows[0];
                cartRow["JobTypeName"] = Convert.ToString(service["JobTypeName"]);
            }

            Cart = cart;
            return true;
        }

        // ============================================================
        // TECHNICIAN VALIDATION
        // ============================================================
        private bool ValidateSelectedTechnician(int technicianID, DateTime promiseDate)
        {
            DataTable technician = StaffService.GetStaffByID(technicianID);
            if (technician == null || technician.Rows.Count == 0)
            {
                ShowError("The selected technician no longer exists.");
                return false;
            }

            DataRow row = technician.Rows[0];
            string employmentStatus = Convert.ToString(row["EmploymentStatus"]);
            if (!string.Equals(employmentStatus, "Active", StringComparison.OrdinalIgnoreCase))
            {
                ShowError("The selected technician is no longer active.");
                return false;
            }

            bool isAvailable = false;
            if (row.Table.Columns.Contains("IsAvailable") && row["IsAvailable"] != DBNull.Value)
            {
                isAvailable = Convert.ToBoolean(row["IsAvailable"]);
            }

            if (!isAvailable)
            {
                ShowError("The selected technician is currently unavailable.");
                return false;
            }

            DataTable availableTechnicians = StaffService.GetAvailableTechnicians(promiseDate);
            bool technicianAvailableForDate = false;
            if (availableTechnicians != null)
            {
                foreach (DataRow availableRow in availableTechnicians.Rows)
                {
                    int availableID;
                    if (int.TryParse(Convert.ToString(availableRow["StaffID"]), out availableID) && availableID == technicianID)
                    {
                        technicianAvailableForDate = true;
                        break;
                    }
                }
            }

            if (!technicianAvailableForDate)
            {
                ShowError("The selected technician is no longer available for the selected promise date. Please choose another technician.");
                LoadTechnicians(promiseDate);
                return false;
            }

            return true;
        }

        // ============================================================
        // CANCEL
        // ============================================================
        protected void btnCancelJob_Click(object sender, EventArgs e)
        {
            ResetJobForm();
            ShowSuccess("Job creation has been cancelled.");
        }

        // ============================================================
        // RESET FORM
        // ============================================================
        private void ResetJobForm()
        {
            SelectedCustomerID = 0;
            CustomerSortExpression = "FullName ASC";
            txtCustomerSearch.Text = string.Empty;
            txtQuantity.Text = "1";
            txtPromiseDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            SetPromiseDateMinimum();
            txtJobNote.Text = string.Empty;
            ddlServiceType.SelectedIndex = 0;
            LoadTechnicians(DateTime.Today);
            Cart = CreateCartTable();
            BindCart();
            pnlSelectedCustomer.Visible = false;
            lblSelectedCustomerName.Text = string.Empty;
            lblSelectedCustomerDetails.Text = string.Empty;
            gvCustomers.PageIndex = 0;
            LoadCustomers();
        }

        // ============================================================
        // MESSAGES
        // ============================================================
        private void ShowSuccess(string message)
        {
            pnlError.Visible = false;
            litError.Text = string.Empty;
            pnlSuccess.Visible = true;
            litSuccess.Text = Server.HtmlEncode(message);
        }

        private void ShowError(string message)
        {
            pnlSuccess.Visible = false;
            litSuccess.Text = string.Empty;
            pnlError.Visible = true;
            litError.Text = Server.HtmlEncode(message);
        }

        private void HideMessages()
        {
            pnlSuccess.Visible = false;
            pnlError.Visible = false;
            litSuccess.Text = string.Empty;
            litError.Text = string.Empty;
        }

        private void SetPromiseDateMinimum()
        {
            txtPromiseDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
        }
    }
}
