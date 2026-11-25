using Major_Project.Core;
using Major_Project.UI.Forms.Common;
using Major_Project.Utilities;
using MajorProject.Business.Helpers;
using MajorProject.Business.Models.DTOs;
using MajorProject.Business.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Major_Project.UI.Forms.Admin
{
    public partial class AdminForm : Form
    {
        // ---- Services ----
        private readonly JobService _jobService;
        private readonly InventoryService _inventoryService;
        private readonly CustomerService _customerService;
        private readonly StaffService _staffService;
        private readonly JobTypeService _jobTypeService;
        // ---- Form state ----
        private readonly int _staffID;
        private readonly string _fullName;
        private bool isUpdating = false;
        private bool _savingJob;
        private int currentEditingJobID = -1;
        private int selectedJobCustomerID = -1;
        private DataTable cartTable = new DataTable();
        private BindingSource jobsViewBindingSource = new BindingSource();
        // ---- Help ----
        private HelpManager helpManager;
        private readonly CustomerIdentityFields _customerIdentity;
        internal WorkspaceTabs NavigationTabs => tabControl1;

        public AdminForm()
        {
            InitializeComponent();
        }

        public AdminForm(int staffID, string fullName)
        {
            InitializeComponent();
            GridPresentation.Attach(this);
            btnUpdate.Enabled = false;
            btnDelJob.Enabled = false;
            _customerIdentity = new CustomerIdentityFields(cmbCustomerType, txtSouthAfricanIDNumber, lblSA_ID, txtCompanyRegistrationNumber, lblCompanyRegNo, cmbIdentityDocument, txtPassportNumber, txtPassportCountry, lblIdentityDocument, lblPassportNumber, lblPassportCountry);
            StaffWorkspacePages.AddCustomerDirectory(tabControl1, () => LoadCustomers());
            StaffWorkspacePages.AddInventory(tabControl1);
            JobChangeWorkspace.Add(tabControl1, true, rescheduleReviewOnly: true);
            StaffWorkspacePages.AddProfile(tabControl1);
            StaffJobActions.Bind(this, dgvJobs, LoadJobsGrid, btnCancelSelectedJob, btnCollectCash, btnSaveInvoicePdf, btnSaveReceiptPdf, btnViewInvoicePdf, btnViewReceiptPdf);
            dataGridView6.CellDoubleClick += (sender, e) =>
            {
                if (e.ColumnIndex >= 0)
                    SelectCustomerForJob(e.RowIndex);
            };
            _staffID = staffID;
            _fullName = fullName;
            string conn = Properties.Settings.Default.GroupWst34ConnectionString;
            _jobService = new JobService(conn);
            _inventoryService = new InventoryService(conn);
            _customerService = new CustomerService(conn);
            _staffService = new StaffService(conn);
            _jobTypeService = new JobTypeService(conn);
            helpManager = new HelpManager();
            RegisterAdminHelp();
        }

        // ════════════════════════════════════════════════════════════════════
        //  HELP SYSTEM
        // ════════════════════════════════════════════════════════════════════
        private void RegisterAdminHelp()
        {
            helpManager.RegisterControlSection("Customer Details", (txtCustName, "Customer Name", "Enter the customer's full legal name (letters and spaces only)."), (txtPhone, "Phone Number", "Enter a valid 10-digit South African phone number starting with '0'."), (txtEmail, "Email Address", "Enter the customer's primary contact email address."), (txtFAddress, "Physical Address", "Enter the customer's complete physical delivery/service address."), (cmbSevArea, "Service Area", "Select the regional service zone for electrical/construction works."), (cmbCustomerType, "Customer Type", "Select the entity classification (e.g., Commercial, Residential)."));
            helpManager.RegisterControlSection("Customer Management", (textBox3, "Customer Search", "Type a name or contact number to filter customer records."), (dataGridView6, "Customer List", "Double-click a record to assign the customer to a new job slot."));
            helpManager.RegisterControlSection("Job Details", (dateTimePicker3, "Promise Date", "Select target completion date (cannot be earlier than today)."), (comboBox4, "Technician Assignment", "Assign an active field technician available for the chosen date."), (comboBox2, "Service Type", "Select electrical or construction service tasks to perform."), (numericUpDown3, "Service Quantity", "Specify the units/hours needed for the selected service item."), (textBox1, "Job Notes", "Enter job site instructions, structural logs, or specialized requests."), (dataGridViewCart, "Service Cart", "Review queued work order line items and pricing before saving."));
            helpManager.RegisterControlSection("Job Management", (btnUpdate, "Update Job", "Edit the selected pending booking."), (btnDelJob, "Archive", "Remove a completed or cancelled job from the Jobs list and retain its record."), (dgvJobs, "Jobs Table", "Select a job to view details, process updates, or archive finished records."), (comboBox5, "Job Status Filter", "Filter the job list by status (Pending, In-Progress, Completed, All)."), (textBox2, "Job Overview", "Renders formatted cost breakdowns and party assignments for active selections."), (textBox5, "Invoice Total", "Displays subtotal, transport, material, VAT (15%), and grand totals."));
            helpManager.AttachHelpButton(btnHelpJobs, defaultControl: comboBox2);
            helpManager.AttachHelpButton(btnJobManagement, defaultControl: dgvJobs);
            helpManager.RegisterContainer(this);
        }

        // ════════════════════════════════════════════════════════════════════
        //  FORM LOAD
        // ════════════════════════════════════════════════════════════════════
        private void Admin_Load(object sender, EventArgs e)
        {
            if (_jobService == null)
                return; // Designer-only constructor has no database services.
            try
            {
                dateTimePicker3.MinDate = DateTime.Today;
                dateTimePicker3.Value = DateTime.Today;
                // Setup cart table
                cartTable.Columns.Clear();
                cartTable.Rows.Clear();
                cartTable = new DataTable();
                cartTable.Columns.Add("JobTypeID", typeof(int));
                cartTable.Columns.Add("JobTypeName", typeof(string));
                cartTable.Columns.Add("Quantity", typeof(int));
                cartTable.Columns.Add("BaseLabourCost", typeof(decimal));
                cartTable.Columns.Add("BaseTransportCost", typeof(decimal));
                cartTable.Columns.Add("MaterialCost", typeof(decimal));
                cartTable.Columns.Add("TaxPercentage", typeof(decimal));
                dataGridViewCart.DataSource = cartTable;
                if (dataGridViewCart.Columns.Contains("JobTypeID"))
                    dataGridViewCart.Columns["JobTypeID"].Visible = false;
                // Populate status filter
                comboBox5.Items.Clear();
                comboBox5.Items.Add("All");
                comboBox5.Items.Add("Pending");
                comboBox5.Items.Add("In Progress");
                comboBox5.Items.Add("Completed");
                comboBox5.Items.Add("Cancelled");
                comboBox5.SelectedIndex = 0;
                // Load data
                LoadJobTypes();
                LoadCustomers();
                LoadStaff();
                LoadInventoryGrid();
                LoadJobsGrid();
                UpdateStatButtons();
                PopulateTechnicianCombo(DateTime.Today);
                // Grid formatting
                if (dgvJobs != null)
                {
                    dgvJobs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvJobs.AllowUserToAddRows = false;
                    dgvJobs.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);
                }

                lblAdminGreeting.Text = "Welcome, " + _fullName;
                textBox5.Text = "R 0.00";
                if (dgvJobs.Rows.Count > 0)
                {
                    dgvJobs.ClearSelection();
                    dgvJobs.Rows[0].Selected = true;
                    dgvJobs.CurrentCell = dgvJobs.Rows[0].Cells[0];
                }

                numericUpDown3.Minimum = 1;
                numericUpDown3.Maximum = 1000;
                numericUpDown3.Value = 1;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load the admin dashboard. Please check your database connection and try again. Error: " + ex.Message);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  DATA LOADING
        // ════════════════════════════════════════════════════════════════════
        private void LoadJobTypes()
        {
            try
            {
                DataTable dt = _jobTypeService.GetAllJobTypes();
                comboBox2.DataSource = dt;
                comboBox2.DisplayMember = "JobTypeName";
                comboBox2.ValueMember = "JobTypeID";
                comboBox2.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Unable to load job types. Please verify your database connection. Error: " + ex.Message);
            }
        }

        private void LoadCustomers(string filter = null)
        {
            try
            {
                DataTable dt;
                if (string.IsNullOrWhiteSpace(filter))
                    dt = _customerService.GetAllCustomers();
                else
                    dt = _customerService.SearchCustomersByName(filter);
                dataGridView6.DataSource = dt;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load customer list. Error: " + ex.Message);
            }
        }

        private void LoadStaff()
        {
        // Staff data is loaded when needed for technicians
        }

        private void LoadInventoryGrid()
        {
            try
            {
                DataTable dt = _inventoryService.GetAllInventoryItems();
            // If you have an inventory grid, bind it here.
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load inventory. Error: " + ex.Message);
            }
        }

        private void LoadJobsGrid()
        {
            try
            {
                DataTable jobs = _jobService.GetJobViews(); // new method using JobsView query
                // Filter out archived (we'll do it in the BindingSource)
                DataTable filtered = jobs.Clone();
                foreach (DataRow row in jobs.Rows)
                {
                    if (row["JobStatus"].ToString() != "Archived")
                        filtered.ImportRow(row);
                }

                GridPresentation.BindJobs(dgvJobs, jobsViewBindingSource, filtered);
                // Set friendly column headers
                if (dgvJobs.Columns.Contains("CustomerName"))
                    dgvJobs.Columns["CustomerName"].HeaderText = "Customer";
                if (dgvJobs.Columns.Contains("TechnicianName"))
                    dgvJobs.Columns["TechnicianName"].HeaderText = "Technician";
                if (dgvJobs.Columns.Contains("JobStatus"))
                    dgvJobs.Columns["JobStatus"].HeaderText = "Status";
                if (dgvJobs.Columns.Contains("PromiseDate"))
                    dgvJobs.Columns["PromiseDate"].HeaderText = "Promise Date";
                if (dgvJobs.Columns.Contains("TotalMaterialCost"))
                    dgvJobs.Columns["TotalMaterialCost"].HeaderText = "Materials";
                if (dgvJobs.Columns.Contains("LabourCost"))
                    dgvJobs.Columns["LabourCost"].HeaderText = "Labour";
                if (dgvJobs.Columns.Contains("TransportCost"))
                    dgvJobs.Columns["TransportCost"].HeaderText = "Transport";
                if (dgvJobs.Columns.Contains("GrandTotal"))
                    dgvJobs.Columns["GrandTotal"].HeaderText = "Grand Total";
                dgvJobs_SelectionChanged(dgvJobs, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load jobs. Please check your database connection. Error: " + ex.Message);
            }
        }

        private void PopulateTechnicianCombo(DateTime date)
        {
            try
            {
                DataTable techs = _staffService.GetAvailableTechnicians(date);
                DesktopRecordPresentation.BindTechnicians(comboBox4, techs, lblTechnicianAvailability);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Unable to load available technicians for the selected date. Error: " + ex.Message);
            }
        }

        private void UpdateStatButtons()
        {
            try
            {
                RefreshDashboardLists();
                button1.Text = $"Pending\n{_jobService.GetTotalJobCountByStatus("Pending")}";
                button9.Text = $"In progress\n{_jobService.GetTotalJobCountByStatus("In Progress")}";
                button2.Text = $"Completed\n{_jobService.GetTotalJobCountByStatus("Completed")}";
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to update dashboard statistics. Error: " + ex.Message);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        // CUSTOMER OPERATIONS
        // ════════════════════════════════════════════════════════════════════
        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            try
            {
                string customerType = cmbCustomerType.Text.Trim();
                // Basic validation
                if (!ValidationHelper.IsValidName(txtCustName.Text))
                {
                    UIHelper.ShowWarning("Please enter a valid customer name (letters and spaces only).");
                    return;
                }

                if (!ValidationHelper.IsValidPhoneNumber(txtPhone.Text))
                {
                    UIHelper.ShowWarning("Please enter a valid 10-digit South African phone number starting with '0'.");
                    return;
                }

                if (!ValidationHelper.IsValidEmail(txtEmail.Text))
                {
                    UIHelper.ShowWarning("Please enter a valid email address.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtFAddress.Text))
                {
                    UIHelper.ShowWarning("Please enter the customer's full address.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(cmbSevArea.Text))
                {
                    UIHelper.ShowWarning("Please select a service area.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(customerType))
                {
                    UIHelper.ShowWarning("Please select a customer type.");
                    return;
                }

                _customerIdentity.Validate();
                // The system generates the password.
                // The Admin never sees the password.
                string temporaryPassword;
                if (!UserSession.Check())
                    return;
                int customerID = _customerService.CreateCustomer(txtCustName.Text.Trim(), txtPhone.Text.Trim(), txtEmail.Text.Trim(), txtFAddress.Text.Trim(), cmbSevArea.Text.Trim(), "Durban", "KwaZulu-Natal", customerType, _customerIdentity.SouthAfricanID, _customerIdentity.Company, _customerIdentity.Passport, _customerIdentity.Country, out temporaryPassword);
                // Send the generated password directly to the customer.
                UIHelper.ShowSuccess("Customer registered successfully. The customer can use Forgot Password with their registered email address.");
                ClearCustomerInputFields();
                LoadCustomers();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Unable to save customer profile. " + "Please check all fields and try again. Error: " + ex.Message);
            }
        }

        private void cmbCustomerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _customerIdentity?.Reset();
        }

        private void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            Control[] fields =
            {
                txtCustName,
                txtEmail,
                txtPhone,
                txtFAddress,
                cmbSevArea,
                cmbCustomerType,
                txtSouthAfricanIDNumber,
                txtCompanyRegistrationNumber
            };
            foreach (var field in fields)
            {
                field.BackColor = Color.White;
            }

            // Full name
            if (!ValidationHelper.IsNotEmpty(txtCustName.Text))
            {
                txtCustName.BackColor = Color.LightPink;
                txtCustName.Focus();
                return;
            }

            // Email
            if (!ValidationHelper.IsValidEmail(txtEmail.Text))
            {
                txtEmail.BackColor = Color.LightPink;
                txtEmail.Focus();
                return;
            }

            // Phone
            if (!ValidationHelper.IsValidPhoneNumber(txtPhone.Text))
            {
                txtPhone.BackColor = Color.LightPink;
                UIHelper.ShowWarning("Phone number must be 10 digits and start with '0'.");
                txtPhone.Focus();
                return;
            }

            // Address
            if (!ValidationHelper.IsNotEmpty(txtFAddress.Text))
            {
                txtFAddress.BackColor = Color.LightPink;
                txtFAddress.Focus();
                return;
            }

            // Service area
            if (cmbSevArea.SelectedIndex == -1)
            {
                cmbSevArea.BackColor = Color.LightPink;
                cmbSevArea.Focus();
                return;
            }

            // Customer type
            if (cmbCustomerType.SelectedItem == null)
            {
                cmbCustomerType.BackColor = Color.LightPink;
                cmbCustomerType.Focus();
                return;
            }

            // Make sure a customer has actually been selected
            if (this.Tag == null)
            {
                UIHelper.ShowWarning("Please select a customer to update.");
                return;
            }

            int customerID = (int)this.Tag;
            try
            {
                if (!UserSession.Check())
                    return;
                _customerService.UpdateCustomer(customerID, txtPhone.Text.Trim(), txtEmail.Text.Trim(), txtFAddress.Text.Trim(), cmbSevArea.Text.Trim());
                UIHelper.ShowSuccess("Customer updated successfully.");
                LoadCustomers();
                ClearCustomerInputFields();
                btnSaveProfile.Enabled = true;
                btnUpdateProfile.Enabled = false;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Unable to update customer. " + "Please check the details and try again. Error: " + ex.Message);
            }
        }

        private void ClearCustomerInputFields()
        {
            txtCustName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtFAddress.Clear();
            _customerIdentity.Reset();
            cmbCustomerType.SelectedIndex = -1;
            if (cmbSevArea.Items.Count > 0)
            {
                cmbSevArea.SelectedIndex = 0;
            }

            this.Tag = null;
            btnUpdateProfile.Enabled = false;
        }

        // ════════════════════════════════════════════════════════════════════
        //  CART & JOB CREATION
        // ════════════════════════════════════════════════════════════════════
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (isUpdating)
                {
                    UIHelper.ShowWarning("Services cannot be changed while rescheduling an existing job.");
                    return;
                }

                if (comboBox2.SelectedItem == null)
                {
                    UIHelper.ShowWarning("Please select a service.");
                    return;
                }

                if (selectedJobCustomerID <= 0)
                {
                    UIHelper.ShowWarning("Please select a customer by double-clicking a customer record.");
                    return;
                }

                DataRowView selected = comboBox2.SelectedItem as DataRowView;
                if (selected == null)
                {
                    UIHelper.ShowWarning("Unable to read the selected service.");
                    return;
                }

                int jobTypeID = Convert.ToInt32(selected["JobTypeID"]);
                string jobTypeName = selected["JobTypeName"].ToString();
                int qty = (int)numericUpDown3.Value;
                if (jobTypeID <= 0)
                {
                    UIHelper.ShowWarning("Invalid service selected.");
                    return;
                }

                if (qty <= 0 || qty > 1000)
                {
                    UIHelper.ShowWarning("Service quantity must be between 1 and 1000.");
                    return;
                }

                DataRow[] existing = cartTable.Select($"JobTypeID = {jobTypeID}");
                if (existing.Length > 0)
                {
                    int existingQty = Convert.ToInt32(existing[0]["Quantity"]);
                    int newQty = existingQty + qty;
                    if (newQty > 1000)
                    {
                        UIHelper.ShowWarning("The total quantity for a service cannot exceed 1000.");
                        return;
                    }

                    existing[0]["Quantity"] = newQty;
                }
                else
                {
                    DataTable costTable = _jobTypeService.GetJobCostsAndMaterials(jobTypeID);
                    if (costTable == null || costTable.Rows.Count == 0)
                    {
                        UIHelper.ShowWarning("Unable to load pricing information for the selected service.");
                        return;
                    }

                    DataRow costRow = costTable.Rows[0];
                    decimal labourCost = costRow["BaseLabourCost"] != DBNull.Value ? Convert.ToDecimal(costRow["BaseLabourCost"]) : 0m;
                    decimal transportCost = costRow["BaseTransportCost"] != DBNull.Value ? Convert.ToDecimal(costRow["BaseTransportCost"]) : 0m;
                    decimal materialCost = costRow["MaterialCost"] != DBNull.Value ? Convert.ToDecimal(costRow["MaterialCost"]) : 0m;
                    decimal taxPercentage = costRow["TaxPercentage"] != DBNull.Value ? Convert.ToDecimal(costRow["TaxPercentage"]) : 0m;
                    DataRow newRow = cartTable.NewRow();
                    newRow["JobTypeID"] = jobTypeID;
                    newRow["JobTypeName"] = jobTypeName;
                    newRow["Quantity"] = qty;
                    newRow["BaseLabourCost"] = labourCost;
                    newRow["BaseTransportCost"] = transportCost;
                    newRow["MaterialCost"] = materialCost;
                    newRow["TaxPercentage"] = taxPercentage;
                    cartTable.Rows.Add(newRow);
                }

                RecalculateTotal();
                numericUpDown3.Value = 1;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to add service to cart. Error: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e) // Remove from cart
        {
            try
            {
                if (isUpdating)
                {
                    UIHelper.ShowWarning("Services cannot be changed while rescheduling an existing job.");
                    return;
                }

                if (dataGridViewCart.CurrentRow != null && !dataGridViewCart.CurrentRow.IsNewRow)
                {
                    DataRowView bound = (DataRowView)dataGridViewCart.CurrentRow.DataBoundItem;
                    bound.Row.Delete();
                    cartTable.AcceptChanges();
                    RecalculateTotal();
                    if (cartTable.Rows.Count == 0)
                    {
                        textBox5.Text = "R 0.00";
                        numericUpDown3.Value = 1;
                        textBox1.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to remove item from cart. Error: " + ex.Message);
            }
        }

        private async void button6_Click(object sender, EventArgs e) // Save/Update Job
        {
            if (_savingJob)
                return;
            _savingJob = true;
            button6.Enabled = false;
            try
            {
                // ============================================================
                // UPDATE EXISTING JOB
                // ============================================================
                // ============================================================
                // UPDATE / RESCHEDULE EXISTING JOB
                // ============================================================
                if (isUpdating)
                {
                    if (currentEditingJobID <= 0)
                    {
                        WorkflowUi.ShowError(this, "No job is currently selected.");
                        return;
                    }

                    // --------------------------------------------------------
                    // Technician
                    // --------------------------------------------------------
                    if (!DesktopRecordPresentation.HasTechnician(comboBox4))
                    {
                        WorkflowUi.ShowError(this, "Please assign a technician.");
                        return;
                    }

                    int technician_ID;
                    if (!int.TryParse(Convert.ToString(comboBox4.SelectedValue), out technician_ID) || technician_ID <= 0)
                    {
                        WorkflowUi.ShowError(this, "The selected technician is invalid.");
                        return;
                    }

                    // --------------------------------------------------------
                    // Promise date
                    // --------------------------------------------------------
                    DateTime newPromiseDate = dateTimePicker3.Value.Date;
                    if (newPromiseDate < DateTime.Today)
                    {
                        WorkflowUi.ShowError(this, "Promise date cannot be earlier than today.");
                        return;
                    }

                    // --------------------------------------------------------
                    // Read current job
                    // --------------------------------------------------------
                    DataTable job = _jobService.GetJobManagementDetails(currentEditingJobID);
                    if (job == null || job.Rows.Count == 0)
                    {
                        WorkflowUi.ShowError(this, "The selected job could not be found.");
                        return;
                    }

                    DataRow jobRow = job.Rows[0];
                    string currentStatus = jobRow.Table.Columns.Contains("JobStatus") && jobRow["JobStatus"] != DBNull.Value ? Convert.ToString(jobRow["JobStatus"]) : string.Empty;
                    // --------------------------------------------------------
                    // Only Pending / In Progress jobs can be rescheduled.
                    // Completed, Cancelled and Archived are locked.
                    // --------------------------------------------------------
                    if (currentStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) || currentStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) || currentStatus.Equals("Archived", StringComparison.OrdinalIgnoreCase))
                    {
                        WorkflowUi.ShowError(this, "Completed, Cancelled or Archived jobs cannot be rescheduled.");
                        return;
                    }

                    // --------------------------------------------------------
                    // The technician combo is already populated by:
                    //
                    // dateTimePicker3_ValueChanged()
                    //
                    // so simply verify that the selected technician exists
                    // in the available list for this date.
                    // --------------------------------------------------------
                    bool technicianAvailable = false;
                    DataTable availableTechnicians = _staffService.GetAvailableTechnicians(newPromiseDate);
                    if (availableTechnicians != null)
                    {
                        foreach (DataRow row in availableTechnicians.Rows)
                        {
                            if (!row.Table.Columns.Contains("StaffID") || row["StaffID"] == DBNull.Value)
                            {
                                continue;
                            }

                            int availableTechnicianID;
                            if (int.TryParse(Convert.ToString(row["StaffID"]), out availableTechnicianID) && availableTechnicianID == technician_ID)
                            {
                                technicianAvailable = true;
                                break;
                            }
                        }
                    }

                    if (!technicianAvailable)
                    {
                        WorkflowUi.ShowError(this, "The selected technician is not available on the selected promise date. Please choose another technician.");
                        return;
                    }

                    // --------------------------------------------------------
                    // Perform transactional reschedule.
                    // --------------------------------------------------------
                    if (!UserSession.Check())
                        return;
                    _jobService.RescheduleJob(currentEditingJobID, newPromiseDate, technician_ID);
                    string successMessage = $"Job #{currentEditingJobID} was successfully rescheduled.";
                    ResetAfterSave();
                    WorkflowUi.Notify(this, successMessage);
                    return;
                }

                // ============================================================
                // CREATE NEW JOB
                // ============================================================
                if (selectedJobCustomerID <= 0)
                {
                    WorkflowUi.ShowError(this, "Please select a customer by double-clicking a row in the customer list.");
                    return;
                }

                if (!DesktopRecordPresentation.HasTechnician(comboBox4))
                {
                    WorkflowUi.ShowError(this, "Please assign a technician.");
                    return;
                }

                if (cartTable == null || cartTable.Rows.Count == 0)
                {
                    WorkflowUi.ShowError(this, "Cart is empty. Please add at least one service.");
                    return;
                }

                // ------------------------------------------------------------
                // Validate maximum number of cart lines
                // ------------------------------------------------------------
                if (cartTable.Rows.Count > 50)
                {
                    WorkflowUi.ShowError(this, "A job cannot contain more than 50 service lines.");
                    return;
                }

                // ------------------------------------------------------------
                // Create the job using the AUTHORITATIVE Business/Data layer.
                //
                // Do NOT calculate stock requirements here.
                // Do NOT send totals here.
                // JobDataAccess calculates:
                //   - labour
                //   - transport
                //   - materials
                //   - VAT
                //   - grand total
                //   - stock requirements
                //   - inventory deduction
                // ------------------------------------------------------------
                int technicianID = Convert.ToInt32(comboBox4.SelectedValue);
                if (!UserSession.Check())
                    return;
                int newJobID = _jobService.CreateJobWithDetailsAndMaterials(selectedJobCustomerID, technicianID, dateTimePicker3.Value.Date, (textBox1.Text ?? string.Empty).Trim(), cartTable);
                // ============================================================
                // IMPORTANT:
                // The job has now been committed successfully.
                //
                // If invoice processing fails below, the job MUST remain
                // created in the database.
                // ============================================================
                // Display the newly-created job information.
                WorkflowUi.Notify(this, $"Job #{newJobID} saved. Preparing its invoice...");
                // ------------------------------------------------------------
                // Display saved job summary.
                //
                // This reads the actual saved database values rather than
                // treating the cart calculation as authoritative.
                // ------------------------------------------------------------
                try
                {
                    var savedJob = _jobService.GetJobByID(newJobID);
                    if (savedJob != null && savedJob.Rows.Count > 0)
                    {
                        DataRow jobRow = savedJob.Rows[0];
                        decimal materialCost = Convert.ToDecimal(jobRow["TotalMaterialCost"]);
                        decimal labourCost = Convert.ToDecimal(jobRow["LabourCost"]);
                        decimal transportCost = Convert.ToDecimal(jobRow["TransportCost"]);
                        decimal taxAmount = Convert.ToDecimal(jobRow["TaxAmount"]);
                        decimal grandTotal = Convert.ToDecimal(jobRow["GrandTotal"]);
                        DisplayJobSummaryInvoice(newJobID, materialCost, labourCost, transportCost, taxAmount, grandTotal, textBox2);
                    }
                }
                catch (Exception summaryEx)
                {
                    // The job itself already exists, so failure to display the
                    // summary should not make the operation look like a failed
                    // job creation.
                    Logger.Error($"Failed to display saved job summary for job {newJobID}.", summaryEx);
                }

                // ============================================================
                // PROCESS INVOICE AFTER JOB CREATION
                // ============================================================
                bool invoiceProcessed = false;
                try
                {
                    invoiceProcessed = await ProcessInvoiceAfterJobCreationAsync(newJobID);
                }
                catch (Exception invoiceEx)
                {
                    Logger.Error($"Invoice processing failed for job {newJobID}.", invoiceEx);
                }

                // ------------------------------------------------------------
                // Return to the destination page before displaying the outcome.
                ResetAfterSave();
                if (invoiceProcessed)
                    WorkflowUi.Notify(this, $"Job #{newJobID} saved successfully. The invoice was emailed to the customer.");
                else
                    WorkflowUi.Warn(this, $"Job #{newJobID} saved successfully. Invoice email was not confirmed; the job remains saved. Check the invoice before retrying email.");
            }
            catch (ArgumentException argEx)
            {
                WorkflowUi.ShowError(this, "Validation error: " + argEx.Message);
            }
            catch (InvalidOperationException invEx)
            {
                WorkflowUi.ShowError(this, "Job creation error: " + invEx.Message);
            }
            catch (Exception ex)
            {
                WorkflowUi.ShowError(this, "Failed to save job. Please check all fields and try again. Error: " + ex.Message);
            }
            finally
            {
                _savingJob = false;
                button6.Enabled = true;
            }
        }

        private InvoiceDto BuildInvoiceFromSavedJob(int jobID)
        {
            DataTable jobTable = _jobService.GetJobByID(jobID);
            if (jobTable == null || jobTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("The saved job could not be found.");
            }

            DataRow jobRow = jobTable.Rows[0];
            int customerID = Convert.ToInt32(jobRow["CustomerID"]);
            DataTable customerTable = _customerService.GetCustomerByID(customerID);
            if (customerTable == null || customerTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("The customer associated with the job could not be found.");
            }

            DataRow customerRow = customerTable.Rows[0];
            var invoice = new InvoiceDto
            {
                JobId = Convert.ToInt32(jobRow["JobID"]),
                InvoiceDate = Convert.ToDateTime(jobRow["CreatedAt"]),
                CustomerName = Convert.ToString(customerRow["FullName"]),
                CustomerEmail = Convert.ToString(customerRow["EmailAddress"]),
                LabourCost = Convert.ToDecimal(jobRow["LabourCost"]),
                TransportCost = Convert.ToDecimal(jobRow["TransportCost"]),
                MaterialCost = Convert.ToDecimal(jobRow["TotalMaterialCost"]),
                TaxAmount = Convert.ToDecimal(jobRow["TaxAmount"]),
                GrandTotal = Convert.ToDecimal(jobRow["GrandTotal"]),
                Items = new List<InvoiceItemDto>()
            };
            // ------------------------------------------------------------
            // Get the services from the SAVED job.
            // ------------------------------------------------------------
            DataTable detailsTable = _jobService.GetJobDetailsByJobID(jobID);
            if (detailsTable != null)
            {
                foreach (DataRow detailRow in detailsTable.Rows)
                {
                    if (detailRow.RowState == DataRowState.Deleted)
                        continue;
                    invoice.Items.Add(new InvoiceItemDto { ServiceName = Convert.ToString(detailRow["JobTypeName"]), Quantity = Convert.ToInt32(detailRow["Quantity"]) });
                }
            }

            return invoice;
        }

        private string SaveInvoicePdf(InvoiceDto invoice, byte[] pdfBytes)
        {
            string invoiceFolder = Path.Combine(Application.StartupPath, "Invoices");
            Directory.CreateDirectory(invoiceFolder);
            string fileName = $"Invoice-{invoice.JobId}.pdf";
            string filePath = Path.Combine(invoiceFolder, fileName);
            File.WriteAllBytes(filePath, pdfBytes);
            return filePath;
        }

        private string BuildInvoiceEmailBody(InvoiceDto invoice)
        {
            string customerName = WebUtility.HtmlEncode(invoice.CustomerName);
            return $@"
        <h2>THY WILL Electrical & Construction</h2>

        <p>Dear {customerName},</p>

        <p>
            Thank you for choosing THY WILL Electrical & Construction.
        </p>

        <p>
            Please find attached the invoice for Job
            <strong>#{invoice.JobId}</strong>.
        </p>

        <p>
            Invoice Total:
            <strong>R {invoice.GrandTotal:N2}</strong>
        </p>

        <p>
            Please keep this invoice for your records.
        </p>

        <p>
            Kind regards,<br />
            THY WILL Electrical & Construction
        </p>";
        }

        private async Task<bool> ProcessInvoiceAfterJobCreationAsync(int jobID)
        {
            // ============================================================
            // 1. BUILD INVOICE FROM SAVED DATABASE DATA
            // ============================================================
            InvoiceDto invoice = BuildInvoiceFromSavedJob(jobID);
            // ============================================================
            // 2. GENERATE PDF
            // ============================================================
            var pdfService = new InvoicePdfService();
            byte[] pdfBytes = pdfService.GenerateInvoicePdf(invoice);
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                throw new InvalidOperationException("Invoice PDF generation returned no data.");
            }

            // ============================================================
            // 3. SAVE PDF TO DESKTOP DISK
            // ============================================================
            string pdfPath = SaveInvoicePdf(invoice, pdfBytes);
            // ============================================================
            // 4. SAVE INVOICE TO DATABASE
            // ============================================================
            var invoiceService = new InvoiceService(Properties.Settings.Default.GroupWst34ConnectionString);
            invoiceService.SaveInvoiceToDatabase(jobID, pdfBytes, pdfPath);
            // ============================================================
            // 5. CUSTOMER MUST HAVE AN EMAIL ADDRESS
            // ============================================================
            if (string.IsNullOrWhiteSpace(invoice.CustomerEmail))
            {
                Logger.Error($"Invoice for job {jobID} was saved, but the customer has no email address.");
                return false;
            }

            // ============================================================
            // 6. SEND EMAIL
            // ============================================================
            var mailjet = new MailjetEmailService(Properties.Settings.Default.MailjetApiKey, Properties.Settings.Default.MailjetApiSecret, Properties.Settings.Default.MailjetFromEmail, Properties.Settings.Default.MailjetFromName);
            string emailSubject = $"THY WILL Invoice - Job #{invoice.JobId}";
            string emailBody = BuildInvoiceEmailBody(invoice);
            string pdfFileName = $"Invoice-{invoice.JobId}.pdf";
            bool emailSent = await mailjet.SendEmailWithAttachmentAsync(invoice.CustomerEmail, invoice.CustomerName, emailSubject, emailBody, pdfBytes, pdfFileName);
            if (!emailSent)
            {
                Logger.Error($"Invoice email failed for job {jobID}.");
                return false;
            }

            // ============================================================
            // 7. MARK INVOICE AS SENT
            // ============================================================
            try
            {
                invoiceService.MarkInvoiceAsSent(jobID, invoice.CustomerEmail);
            }
            catch (Exception markSentEx)
            {
                // Email has already been successfully delivered.
                // Therefore do not report the whole invoice process as failed.
                Logger.Error($"Invoice email was sent for job {jobID}, but the invoice could not be marked as sent.", markSentEx);
                return true;
            }

            return true;
        }

        // ════════════════════════════════════════════════════════════════════
        //  JOB MANAGEMENT
        // ════════════════════════════════════════════════════════════════════
        private async void btnDelJob_Click(object sender, EventArgs e) // Archive
        {
            DataRow row = GetSelectedJobRow();
            if (row == null)
            {
                UIHelper.ShowWarning("Please select a job from the list.");
                return;
            }

            string currentStatus = row["JobStatus"].ToString();
            int jobID = Convert.ToInt32(row["JobID"]);
            if (!await WorkflowUi.ConfirmAsync(this, $"Archive this {currentStatus} job? This action cannot be undone."))
                return;
            try
            {
                if (!UserSession.Check())
                    return;
                _jobService.ArchiveJob(currentStatus, jobID);
                UIHelper.ShowSuccess("Job archived successfully.");
                LoadJobsGrid();
                UpdateStatButtons();
            }
            catch (InvalidOperationException ex)
            {
                UIHelper.ShowWarning(ex.Message);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to archive job. Error: " + ex.Message);
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e) // Status filter
        {
            string selected = Convert.ToString(comboBox5.SelectedItem);
            if (string.IsNullOrWhiteSpace(selected))
                selected = "All";
            string baseFilter = "JobStatus <> 'Archived'";
            if (selected == "All")
                jobsViewBindingSource.Filter = baseFilter;
            else
                jobsViewBindingSource.Filter = $"{baseFilter} AND JobStatus = '{selected}'";
        }

        private void dgvJobs_SelectionChanged(object sender, EventArgs e)
        {
            GridPresentation.JobActions(dgvJobs, btnUpdate, btnDelJob);
            var row = (dgvJobs.CurrentRow?.DataBoundItem as DataRowView)?.Row;
            textBox2.Text = DesktopRecordPresentation.JobDetails(row);
            btnOpenJobDetails.Enabled = row != null;
        }

        private void OpenJobDetails_Click(object sender, EventArgs e)
        {
            if (dgvJobs.CurrentRow?.DataBoundItem is DataRowView)
                tabControl1.SelectedTab = tabJobDetails;
        }

        private void BackToJobs_Click(object sender, EventArgs e) => tabControl1.SelectedTab = tabPage3;
        private void Jobs_DoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                OpenJobDetails_Click(sender, EventArgs.Empty);
        }

        private void btnInvoice_Click(object sender, EventArgs e) // Edit / Reschedule job
        {
            DataRow row = GetSelectedJobRow();
            if (row == null)
            {
                UIHelper.ShowWarning("Please select a job.");
                return;
            }

            string status = row["JobStatus"] != DBNull.Value ? Convert.ToString(row["JobStatus"]).Trim() : string.Empty;
            // ============================================================
            // ONLY PENDING AND IN-PROGRESS JOBS MAY BE RESCHEDULED
            // ============================================================
            if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase) || status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) || status.Equals("Archived", StringComparison.OrdinalIgnoreCase))
            {
                UIHelper.ShowWarning($"A {status} job cannot be rescheduled.");
                return;
            }

            try
            {
                // ============================================================
                // GET JOB ID
                // ============================================================
                int jobID = Convert.ToInt32(row["JobID"]);
                if (jobID <= 0)
                {
                    UIHelper.ShowWarning("The selected job has an invalid Job ID.");
                    return;
                }

                // ============================================================
                // ENTER UPDATE MODE FIRST
                //
                // This MUST happen before changing dateTimePicker3.Value
                // because changing the date can trigger:
                //
                //     dateTimePicker3_ValueChanged()
                //
                // The event needs to know that we are editing an existing job.
                // ============================================================
                SetJobEditMode(true);
                currentEditingJobID = jobID;
                // ============================================================
                // LOAD CUSTOMER INFORMATION
                //
                // Customer information is display-only during rescheduling.
                // Rescheduling does NOT change the customer.
                // ============================================================
                txtCustName.Text = row["CustomerName"] != DBNull.Value ? Convert.ToString(row["CustomerName"]) : string.Empty;
                txtPhone.Text = row["PhoneNumber"] != DBNull.Value ? Convert.ToString(row["PhoneNumber"]) : string.Empty;
                txtEmail.Text = row["EmailAddress"] != DBNull.Value ? Convert.ToString(row["EmailAddress"]) : string.Empty;
                txtFAddress.Text = row["FullAddress"] != DBNull.Value ? Convert.ToString(row["FullAddress"]) : string.Empty;
                cmbSevArea.Text = row["ServiceArea"] != DBNull.Value ? Convert.ToString(row["ServiceArea"]) : string.Empty;
                // Customer type is not required for rescheduling.
                // The jobs DataTable does not necessarily contain this column.
                cmbCustomerType.SelectedIndex = -1;
                // ============================================================
                // LOAD CURRENT PROMISE DATE
                // ============================================================
                DateTime promiseDate = DateTime.Today;
                if (row["PromiseDate"] != DBNull.Value)
                {
                    promiseDate = Convert.ToDateTime(row["PromiseDate"]).Date;
                }

                // Protect against a date outside the DateTimePicker range.
                if (promiseDate < dateTimePicker3.MinDate)
                {
                    promiseDate = dateTimePicker3.MinDate;
                }

                if (promiseDate > dateTimePicker3.MaxDate)
                {
                    promiseDate = dateTimePicker3.MaxDate;
                }

                // ============================================================
                // GET CURRENT TECHNICIAN ID
                // ============================================================
                int currentTechnicianID = -1;
                if (row["StaffID"] != DBNull.Value)
                {
                    currentTechnicianID = Convert.ToInt32(row["StaffID"]);
                }

                // ============================================================
                // SET THE DATE
                //
                // isUpdating is already TRUE, so if the ValueChanged event
                // fires, it will use the rescheduling-specific technician
                // availability logic.
                // ============================================================
                dateTimePicker3.Value = promiseDate;
                // ============================================================
                // LOAD TECHNICIANS FOR RESCHEDULING
                //
                // IMPORTANT:
                //
                // GetAvailableTechniciansForJob() is different from the normal
                // GetAvailableTechnicians().
                //
                // NEW JOB:
                //     Exclude technicians with any active job on that date.
                //
                // RESCHEDULING:
                //     Ignore the current job being edited, but still exclude
                //     technicians who have another active job on that date.
                //
                // This means the current technician remains selectable when
                // appropriate, without manually inserting them into the combo.
                // ============================================================
                DataTable technicians = _staffService.GetAvailableTechniciansForJob(promiseDate, currentEditingJobID);
                DesktopRecordPresentation.BindTechnicians(comboBox4, technicians, lblTechnicianAvailability);
                // ============================================================
                // SELECT THE CURRENT TECHNICIAN
                // ============================================================
                if (currentTechnicianID > 0)
                {
                    bool technicianFound = false;
                    if (technicians != null)
                    {
                        foreach (DataRow technicianRow in technicians.Rows)
                        {
                            if (technicianRow["StaffID"] == DBNull.Value)
                                continue;
                            int staffID;
                            if (int.TryParse(Convert.ToString(technicianRow["StaffID"]), out staffID) && staffID == currentTechnicianID)
                            {
                                technicianFound = true;
                                break;
                            }
                        }
                    }

                    if (technicianFound)
                    {
                        comboBox4.SelectedValue = currentTechnicianID;
                    }
                    else
                    {
                        // The current technician has another active job
                        // on this date and therefore cannot be selected.
                        comboBox4.SelectedIndex = comboBox4.Items.Count > 0 ? 0 : -1;
                        UIHelper.ShowWarning("The current technician is not available for this date because " + "they have another active job. Please select another technician.");
                    }
                }
                else
                {
                    comboBox4.SelectedIndex = comboBox4.Items.Count > 0 ? 0 : -1;
                }

                // ============================================================
                // CART IS NOT EDITABLE DURING RESCHEDULING
                //
                // Only these values may change:
                //
                //     PromiseDate
                //     StaffID
                //
                // Services, materials, quantities and financial values remain
                // exactly as they were when the job was created.
                // ============================================================
                // ============================================================
                // OPEN THE JOB / RESCHEDULING TAB
                // ============================================================
                tabControl1.SelectedTab = tabPage2;
            }
            catch (Exception ex)
            {
                // ============================================================
                // RESTORE CREATE MODE IF EDITING COULD NOT BE INITIALIZED
                // ============================================================
                SetJobEditMode(false);
                currentEditingJobID = -1;
                UIHelper.ShowError("Failed to load job details for editing. Error: " + ex.Message);
                Logger.Error("Failed to load job " + "for rescheduling in AdminForm.", ex);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  HELPERS
        // ════════════════════════════════════════════════════════════════════
        private void SetJobEditMode(bool editing)
        {
            isUpdating = editing;
            // ============================================================
            // CUSTOMER DETAILS
            // ============================================================
            // Customer information is displayed during job editing,
            // but cannot be changed as part of a job reschedule.
            txtCustName.ReadOnly = editing;
            txtPhone.ReadOnly = editing;
            txtEmail.ReadOnly = editing;
            txtFAddress.ReadOnly = editing;
            cmbSevArea.Enabled = !editing;
            cmbCustomerType.Enabled = !editing;
            // Identity fields are never edited from the job screen.
            txtSouthAfricanIDNumber.Enabled = !editing;
            txtCompanyRegistrationNumber.Enabled = !editing;
            // ============================================================
            // JOB DATE / TECHNICIAN
            // ============================================================
            // These are the ONLY job fields editable during rescheduling.
            dateTimePicker3.Enabled = true;
            comboBox4.Enabled = true;
            // ============================================================
            // SERVICE CART
            // ============================================================
            // Services cannot be added, removed or changed while editing
            // an existing job.
            comboBox2.Enabled = !editing;
            numericUpDown3.Enabled = !editing;
            button5.Enabled = !editing;
            button4.Enabled = !editing;
            dataGridViewCart.Enabled = !editing;
            // Notes are only supplied when creating the job.
            textBox1.Enabled = !editing;
            // Customer profile save must not be available while editing
            // a job.
            btnSaveProfile.Enabled = !editing;
        }

        private void RecalculateTotal()
        {
            textBox5.Text = DesktopRecordPresentation.CartTotal(cartTable).ToString("C2", GridPresentation.Rand);
            GridPresentation.FormatColumns(dataGridViewCart);
        }

        private void DisplayJobSummaryInvoice(int jobID, decimal materials, decimal labor, decimal transport, decimal tax, decimal total, Control targetTextBox = null)
        {
            try
            {
                List<string> services = new List<string>();
                // ============================================================
                // READ SERVICES FROM THE SAVED JOB
                // ============================================================
                DataTable detailsTable = _jobService.GetJobDetailsByJobID(jobID);
                if (detailsTable != null)
                {
                    foreach (DataRow row in detailsTable.Rows)
                    {
                        if (row.RowState == DataRowState.Deleted)
                            continue;
                        string name = row["JobTypeName"] != DBNull.Value ? Convert.ToString(row["JobTypeName"]) : "Service";
                        int quantity = row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0;
                        services.Add($"{name} (x{quantity})");
                    }
                }

                string combinedServices = services.Count > 0 ? string.Join(", ", services) : "No services recorded.";
                // ============================================================
                // CUSTOMER / TECHNICIAN DISPLAY
                // ============================================================
                string customerName = txtCustName.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string area = cmbSevArea.Text.Trim();
                string technician = comboBox4.Text.Trim();
                DateTime promiseDate = dateTimePicker3.Value.Date;
                string idDisplay = jobID > 0 ? jobID.ToString() : "NEW RECORD (Draft)";
                // ============================================================
                // BUILD SUMMARY
                // ============================================================
                string invoice = "── JOB INVOICE SUMMARY ───────────\r\n" + $"Job ID       : {idDisplay}\r\n" + "Status       : Pending\r\n" + $"Promise Date : {promiseDate:dd MMM yyyy}\r\n" + "── CUSTOMER DETAILS ─────────────\r\n" + $"Name         : {customerName}\r\n" + $"Phone        : {phone}\r\n" + $"Area         : {area}\r\n" + "── JOB ALLOCATION ───────────────\r\n" + $"Technician   : {technician}\r\n" + $"Services     : {combinedServices}\r\n" + "── FINANCIAL BREAKDOWN ──────────\r\n" + $"Labour Cost  : R {labor:N2}\r\n" + $"Material Sub : R {materials:N2}\r\n" + $"Transport    : R {transport:N2}\r\n" + $"VAT (15%)    : R {tax:N2}\r\n" + $"Grand Total  : R {total:N2}\r\n" + "─────────────────────────────────";
                if (targetTextBox != null)
                    targetTextBox.Text = invoice;
                textBox5.Text = $"R {total:N2}";
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to display job summary for job {jobID}.", ex);
                if (targetTextBox != null)
                    targetTextBox.Text = $"Job #{jobID}\r\n\r\n" + $"Grand Total: R {total:N2}";
                textBox5.Text = $"R {total:N2}";
            }
        }

        private DataRow GetSelectedJobRow()
        {
            if (dgvJobs.SelectedRows.Count > 0)
            {
                object item = dgvJobs.SelectedRows[0].DataBoundItem;
                if (item is DataRowView drv)
                    return drv.Row;
                if (item is DataRow dr)
                    return dr;
            }

            return null;
        }

        // ════════════════════════════════════════════════════════════════════
        //  UI HELPERS & CLEANUP
        // ════════════════════════════════════════════════════════════════════
        private void ResetAfterSave()
        {
            try
            {
                // ============================================================
                // RESET STATE FIRST
                // ============================================================
                selectedJobCustomerID = -1;
                currentEditingJobID = -1;
                // ============================================================
                // CLEAR CART
                // ============================================================
                if (cartTable != null)
                {
                    cartTable.Clear();
                }

                // ============================================================
                // RETURN TO CREATE MODE
                // ============================================================
                SetJobEditMode(false);
                // Customer/job fields
                ClearAddJobFields();
                txtSouthAfricanIDNumber.Clear();
                txtCompanyRegistrationNumber.Clear();
                cmbCustomerType.SelectedIndex = -1;
                if (cmbSevArea.Items.Count > 0)
                    cmbSevArea.SelectedIndex = 0;
                else
                    cmbSevArea.SelectedIndex = -1;
                textBox1.Clear();
                // ============================================================
                // RESET SERVICE CONTROLS
                // ============================================================
                comboBox2.SelectedIndex = -1;
                numericUpDown3.Minimum = 1;
                numericUpDown3.Maximum = 1000;
                numericUpDown3.Value = 1;
                button5.Enabled = false;
                button4.Enabled = false;
                // ============================================================
                // RESET TECHNICIAN
                // ============================================================
                comboBox4.SelectedIndex = comboBox4.Items.Count > 0 ? 0 : -1;
                // Reload technicians for normal CREATE mode.
                PopulateTechnicianCombo(DateTime.Today);
                // ============================================================
                // RESET TOTAL
                // ============================================================
                textBox5.Text = "R 0.00";
                // ============================================================
                // REFRESH JOBS
                // ============================================================
                LoadJobsGrid();
                UpdateStatButtons();
                // ============================================================
                // RETURN TO JOB TAB / NORMAL FORM
                // ============================================================
                tabControl1.SelectedTab = tabPage1;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to reset AdminForm after job operation.", ex);
            }
        }

        private void ClearAddJobFields()
        {
            txtCustName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtFAddress.Clear();
            _customerIdentity.Reset();
            textBox3.Clear();
            txtCustName.ReadOnly = false;
            txtPhone.ReadOnly = false;
            txtEmail.ReadOnly = false;
            txtFAddress.ReadOnly = false;
            cmbSevArea.Enabled = true;
            cmbCustomerType.Enabled = true;
            dateTimePicker3.Enabled = true;
            comboBox4.Enabled = true;
            comboBox2.Enabled = true;
            numericUpDown3.Enabled = true;
            button4.Enabled = true;
            dataGridViewCart.Enabled = true;
            textBox1.Enabled = true;
            cmbCustomerType.SelectedIndex = -1;
            if (cmbSevArea.Items.Count > 0)
                cmbSevArea.SelectedIndex = 0;
            else
                cmbSevArea.SelectedIndex = -1;
            comboBox4.SelectedIndex = comboBox4.Items.Count > 0 ? 0 : -1;
            dateTimePicker3.MinDate = DateTime.Today;
            dateTimePicker3.Value = DateTime.Today;
            textBox1.Clear();
            textBox5.Text = "R 0.00";
            btnSaveProfile.Enabled = true;
        }

        // ════════════════════════════════════════════════════════════════════
        //  EVENT HANDLERS
        // ════════════════════════════════════════════════════════════════════
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_jobService != null && tabControl1.SelectedTab == tabPage3)
                LoadJobsGrid();
            try
            {
                if (tabControl1.SelectedTab == tabPage1 && _jobService != null)
                    UpdateStatButtons();
                if (tabControl1.SelectedTab?.Name != "tabPage2")
                    return;
                // Do not overwrite the technician selection while
                // editing an existing job.
                if (isUpdating && currentEditingJobID > 0)
                    return;
                PopulateTechnicianCombo(dateTimePicker3.Value.Date);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load technicians when opening the job tab.", ex);
            }
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime selectedDate = dateTimePicker3.Value.Date;
                if (selectedDate < DateTime.Today)
                    return;
                if (isUpdating && currentEditingJobID > 0)
                {
                    DataTable technicians = _staffService.GetAvailableTechniciansForJob(selectedDate, currentEditingJobID);
                    DesktopRecordPresentation.BindTechnicians(comboBox4, technicians, lblTechnicianAvailability);
                    return;
                }

                // Normal NEW JOB mode
                PopulateTechnicianCombo(selectedDate);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to handle promise date change.", ex);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e) // Customer search
        {
            LoadCustomers(textBox3.Text.Trim());
        }

        private void dataGridView6_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            try
            {
                DataGridViewRow row = dataGridView6.Rows[e.RowIndex];
                if (row.Cells["CustomerID"].Value != DBNull.Value)
                {
                    int id = Convert.ToInt32(row.Cells["CustomerID"].Value);
                    DataTable dt = _customerService.GetAllCustomers();
                    DataRow[] found = dt.Select($"CustomerID = {id}");
                    if (found.Length > 0)
                    {
                        txtCustName.Text = found[0]["FullName"].ToString();
                        txtEmail.Text = found[0]["EmailAddress"].ToString();
                        txtPhone.Text = found[0]["PhoneNumber"].ToString();
                        txtFAddress.Text = found[0]["FullAddress"].ToString();
                        cmbCustomerType.Text = found[0]["CustomerType"].ToString();
                        cmbSevArea.Text = found[0]["Area"].ToString();
                        this.Tag = id;
                        btnUpdateProfile.Enabled = true;
                        txtCustName.ReadOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Customer selection error: " + ex.Message);
            }
        }

        private void dataGridView6_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SelectCustomerForJob(e.RowIndex);
        }

        private void SelectCustomerForJob(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dataGridView6.Rows.Count)
                return;
            try
            {
                DataGridViewRow row = dataGridView6.Rows[rowIndex];
                if (row.Cells["CustomerID"].Value != DBNull.Value)
                {
                    selectedJobCustomerID = Convert.ToInt32(row.Cells["CustomerID"].Value);
                    string name = row.Cells["FullName"].Value?.ToString() ?? "Selected Customer";
                    button5.Enabled = true;
                    UIHelper.ShowInfo($"Customer '{name}' selected for job.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to select customer. Error: " + ex.Message);
            }
        }

        private void txtCustName_TextChanged(object sender, EventArgs e)
        {
            string text = txtCustName.Text;
            if (!ValidationHelper.IsValidName(text))
            {
                txtCustName.Text = ValidationHelper.SanitizeAlphaOnly(text);
                txtCustName.SelectionStart = txtCustName.Text.Length;
            }
        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {
            string text = txtPhone.Text;
            if (!ValidationHelper.IsValidPhoneNumber(text))
            {
                txtPhone.Text = ValidationHelper.SanitizeDigitsOnly(text);
                txtPhone.SelectionStart = txtPhone.Text.Length;
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
        // Optional filter – leave empty if not needed
        }

        private void button10_Click(object sender, EventArgs e) // Clear all
        {
            try
            {
                selectedJobCustomerID = -1;
                currentEditingJobID = -1;
                if (cartTable != null)
                    cartTable.Clear();
                SetJobEditMode(false);
                ClearAddJobFields();
                comboBox2.SelectedIndex = -1;
                numericUpDown3.Minimum = 1;
                numericUpDown3.Maximum = 1000;
                numericUpDown3.Value = 1;
                button5.Enabled = false;
                button4.Enabled = false;
                comboBox4.SelectedIndex = comboBox4.Items.Count > 0 ? 0 : -1;
                textBox1.Clear();
                textBox5.Text = "R 0.00";
                PopulateTechnicianCombo(DateTime.Today);
                tabControl1.SelectedTab = tabPage1;
                UIHelper.ShowInfo("All inputs cleared.");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to clear AdminForm.", ex);
                UIHelper.ShowError("Failed to clear the form. Error: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e) => Logout();
        private void button7_Click(object sender, EventArgs e) => Logout();
        private void btnLogout_Click(object sender, EventArgs e) => Logout();
        // ── LOGOUT HANDLERS ──
        private void Logout()
        {
            try
            {
                UserSession.ClearSession();
                // Close the MDI container – this will trigger the LoginContext to show the login form again
                if (this.ParentForm != null)
                {
                    this.ParentForm.Close();
                }
                else
                {
                    // Fallback: if not in MDI, close this form and show login directly
                    this.Close();
                    new LoginForm().Show();
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Error during logout: " + ex.Message);
            }
        }

        private void OpenHelpGuide(string fileName)
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, "resources", fileName);
                if (File.Exists(path))
                    System.Diagnostics.Process.Start(path);
                else
                    UIHelper.ShowWarning("Help guide not found. Please ensure the file exists in the resources folder.");
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Unable to open help guide. Error: " + ex.Message);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e) => OpenHelpGuide("Admin_Help_Guide.pdf");
        // Unused event handlers (kept for designer compatibility)
        private void btnRefreshStaff_Click(object sender, EventArgs e)
        {
        }

        private void btnRefreshCustomers_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void btnRefreshJobs_Click(object sender, EventArgs e)
        {
            LoadJobsGrid();
        }

        private void btnRefreshInventory_Click(object sender, EventArgs e)
        {
            LoadInventoryGrid();
        }

        private void btnRefreshReports_Click(object sender, EventArgs e)
        {
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
        }

        private void cmbSevArea_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {
        }

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
        }

        private void dgvJobs_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
        }

        private void button8_Click(object sender, EventArgs e)
        {
        }

        private void txtFAddress_TextChanged(object sender, EventArgs e)
        {
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void dataGridViewCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
        }

        private void btnJobManager_Click(object sender, EventArgs e)
        {
        }

        private void btnJobManagement_Click(object sender, EventArgs e)
        {
        }

        public void btnHelpJobs_Click(object sender, EventArgs e)
        {
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox6_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void RefreshDashboardLists()
        {
            var jobs = _jobService.GetAllJobs();
            dataGridView2.AutoGenerateColumns = true;
            dataGridView2.Columns.Clear();
            var view = new DataView(jobs)
            {
                Sort = "JobID DESC",
                RowFilter = "JobStatus <> 'Archived'"
            };
            var recent = jobs.Clone();
            for (int i = 0; i < Math.Min(10, view.Count); i++)
                recent.ImportRow(view[i].Row);
            dataGridView2.DataSource = recent;
            dataGridView3.AutoGenerateColumns = true;
            dataGridView3.Columns.Clear();
            var low = GridPresentation.LowStock(_inventoryService.GetAllInventoryItems());
            dataGridView3.DataSource = low.DefaultView;
            lblLowStockEmpty.Visible = low.Rows.Count == 0;
        }

        private void DashboardRefresh_Click(object sender, EventArgs e)
        {
            if (!UserSession.Check())
                return;
            UpdateStatButtons();
        }

        private void OpenDashboardJobs(string status)
        {
            if (!UserSession.Check())
                return;
            tabControl1.SelectedTab = tabPage3;
            comboBox5.SelectedItem = status;
        }

        private void button1_Click(object sender, EventArgs e) => OpenDashboardJobs("Pending");
        private void button9_Click(object sender, EventArgs e) => OpenDashboardJobs("In Progress");
        private void button2_Click(object sender, EventArgs e) => OpenDashboardJobs("Completed");
    }
}
