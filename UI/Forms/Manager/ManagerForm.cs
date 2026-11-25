using System.Linq;
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

namespace Major_Project.UI.Forms.Manager
{
    public partial class ManagerForm : Form
    {
        // ---- Services ----
        private readonly JobService _jobService;
        private readonly InventoryService _inventoryService;
        private readonly CustomerService _customerService;
        private readonly StaffService _staffService;
        private readonly JobTypeService _jobTypeService;
        private readonly RoleService _roleService;
        private DataTable _inventoryDataTable;
        private DataTable _dashboardJobs;
        // ---- Form state ----
        private readonly int _staffID;
        private readonly string _fullName;
        private bool isUpdating = false;
        private bool _savingJob;
        private int currentEditingJobID = -1;
        private int selectedJobCustomerID = -1;
        private DataTable cartTable = new DataTable();
        private BindingSource jobsViewBindingSource = new BindingSource();
        private HelpManager _helpManager;
        private readonly CustomerIdentityFields _customerIdentity;
        internal WorkspaceTabs NavigationTabs => tabControl1;

        internal event Action<int> LowStockChanged;
        public ManagerForm()
        {
            InitializeComponent();
        }

        public ManagerForm(int staffID, string fullName)
        {
            InitializeComponent();
            GridPresentation.Attach(this);
            btnUpdate.Enabled = false;
            btnDelJob.Enabled = false;
            _customerIdentity = new CustomerIdentityFields(cmbCustomerType, txtSouthAfricanIDNumber, lblSA_ID, txtCompanyRegistrationNumber, lblCompanyRegNo, cmbIdentityDocument, txtPassportNumber, txtPassportCountry, lblIdentityDocument, lblPassportNumber, lblPassportCountry);
            StaffWorkspacePages.AddCustomerDirectory(tabControl1, () => LoadCustomers());
            StaffWorkspacePages.AddServices(tabControl1, LoadJobTypes);
            StaffWorkspacePages.AddProfile(tabControl1);
            JobChangeWorkspace.Add(tabControl1, true);
            AddStaffManagementActions();
            dataGridView4.SelectionChanged += (sender, args) =>
            {
                UpdateStaffActionState();
                if (_staffService != null && dataGridView4.CurrentRow?.DataBoundItem is DataRowView row)
                    LoadStaffProfile(Convert.ToInt32(row["StaffID"]));
            };
            dataGridView1.SelectionChanged += (sender, args) =>
            {
                var row = (dataGridView1.CurrentRow?.DataBoundItem as DataRowView)?.Row;
                btnRmvItem.Enabled = row != null;
                if (row != null)
                    LoadInventoryItemDetails(row);
                else
                {
                    textBox18.Clear();
                    textBox17.Clear();
                }
            };
            dataGridView4.CellClick += (sender, args) =>
            {
                if (args.RowIndex >= 0)
                    WorkflowUi.Run(this, () => LoadStaffProfile(Convert.ToInt32(WorkflowUi.Selected(dataGridView4)["StaffID"])));
            };
            StaffJobActions.Bind(this, dgvJobs, LoadJobsGrid, btnCancelSelectedJob, btnCollectCash, btnSaveInvoicePdf, btnSaveReceiptPdf, btnViewInvoicePdf, btnViewReceiptPdf);
            dataGridView6.CellDoubleClick += (sender, e) =>
            {
                if (e.ColumnIndex >= 0)
                    SelectCustomerForJob(e.RowIndex);
            };
            _helpManager = new HelpManager();
            RegisterManagerHelp();
            // Attach help buttons
            _helpManager.AttachHelpButton(btnHelpDashboard, defaultControl: dataGridView2);
            _helpManager.AttachHelpButton(btnHelpAddJob, defaultControl: comboBox2);
            _helpManager.AttachHelpButton(btnHelpJobManagement, defaultControl: dgvJobs);
            _helpManager.AttachHelpButton(btnUpdateStockHelp, numUpdateQuantity);
            _helpManager.AttachHelpButton(btnAddStockHelp, txtNewItemName);
            _helpManager.AttachHelpButton(btnHelpInventory, defaultControl: dataGridView1);
            _helpManager.AttachHelpButton(btnHelpProfiles, defaultControl: dataGridView4);
            _staffID = staffID;
            _fullName = fullName;
            string conn = Properties.Settings.Default.GroupWst34ConnectionString;
            _jobService = new JobService(conn);
            _inventoryService = new InventoryService(conn);
            _customerService = new CustomerService(conn);
            _staffService = new StaffService(conn);
            _jobTypeService = new JobTypeService(conn);
            _roleService = new RoleService(conn);
        }

        // ════════════════════════════════════════════════════════════════════
        //  FORM LOAD
        // ════════════════════════════════════════════════════════════════════
        private void Manager_Load(object sender, EventArgs e)
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
                // ── Populate Inventory Sort/Filter ComboBox ──
                comboBox6.Items.Clear();
                comboBox6.Items.Add("All Items");
                comboBox6.Items.Add("Low Stock (Qty <= Reorder)");
                comboBox6.Items.Add("Name (A-Z)");
                comboBox6.Items.Add("Name (Z-A)");
                comboBox6.Items.Add("Price (Low to High)");
                comboBox6.Items.Add("Price (High to Low)");
                comboBox6.Items.Add("Stock (Low to High)");
                comboBox6.Items.Add("Stock (High to Low)");
                comboBox6.SelectedIndex = 0; // Default: All Items
                // Load data
                LoadJobTypes();
                LoadCustomers();
                LoadStaff();
                LoadRoles();
                LoadInventoryGrid();
                LoadDashboardData();
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

                // ── Salary NumericUpDown configuration ──
                numSalary.Minimum = 15000;
                numSalary.Maximum = 999999;
                numSalary.DecimalPlaces = 2;
                numSalary.Increment = 100;
                // For new item price and reorder level (if they exist)
                numNewPrice.Minimum = 10;
                numNewPrice.Maximum = 99999;
                numNewPrice.DecimalPlaces = 2;
                numNewPrice.Increment = 10;
                numNewReorderLevel.Minimum = 10;
                numNewReorderLevel.Maximum = 99999;
                numNewReorderLevel.Increment = 10;
                numNewQuantity.Minimum = 10;
                numNewQuantity.Maximum = 99999;
                numNewQuantity.Increment = 10;
                comboBox6.SelectedIndexChanged += comboBox6_SelectedIndexChanged;
                // Staff management: bind staff grid
                LoadStaffGrid();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load the manager dashboard. Please check your database connection. Error: " + ex.Message);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  DATA LOADING
        // ════════════════════════════════════════════════════════════════════
        private void LoadDashboardData()
        {
            try
            {
                // ── Recent Jobs ──
                _dashboardJobs = _jobService.GetAllJobs();
                // Optionally limit to recent (e.g., 10 rows) but keep full table for filtering
                // For now, we'll show all; filtering will be applied via buttons.
                dataGridView2.AutoGenerateColumns = true;
                dataGridView2.Columns.Clear();
                dataGridView3.AutoGenerateColumns = true;
                dataGridView3.Columns.Clear();
                ShowRecentDashboardJobs();
                // Set friendly column headers
                if (dataGridView2.Columns.Contains("JobID"))
                    dataGridView2.Columns["JobID"].HeaderText = "Job ID";
                if (dataGridView2.Columns.Contains("CustomerID"))
                    dataGridView2.Columns["CustomerID"].HeaderText = "Customer";
                if (dataGridView2.Columns.Contains("StaffID"))
                    dataGridView2.Columns["StaffID"].HeaderText = "Staff ID";
                if (dataGridView2.Columns.Contains("PromiseDate"))
                    dataGridView2.Columns["PromiseDate"].HeaderText = "Promise Date";
                if (dataGridView2.Columns.Contains("JobStatus"))
                    dataGridView2.Columns["JobStatus"].HeaderText = "Status";
                if (dataGridView2.Columns.Contains("TotalMaterialCost"))
                    dataGridView2.Columns["TotalMaterialCost"].HeaderText = "Materials";
                if (dataGridView2.Columns.Contains("LabourCost"))
                    dataGridView2.Columns["LabourCost"].HeaderText = "Labour";
                if (dataGridView2.Columns.Contains("TransportCost"))
                    dataGridView2.Columns["TransportCost"].HeaderText = "Transport";
                // ── Inventory Summary ──
                DataTable inventory = _inventoryService.GetAllInventoryItems();
                var low = GridPresentation.LowStock(inventory);
                dataGridView3.DataSource = low.DefaultView;
                lblLowStockEmpty.Visible = low.Rows.Count == 0;
                // Set friendly headers for inventory
                if (dataGridView3.Columns.Contains("ItemID"))
                    dataGridView3.Columns["ItemID"].HeaderText = "ID";
                if (dataGridView3.Columns.Contains("ItemName"))
                    dataGridView3.Columns["ItemName"].HeaderText = "Item Name";
                if (dataGridView3.Columns.Contains("Category"))
                    dataGridView3.Columns["Category"].HeaderText = "Category";
                if (dataGridView3.Columns.Contains("QuantityInStock"))
                    dataGridView3.Columns["QuantityInStock"].HeaderText = "In Stock";
                if (dataGridView3.Columns.Contains("UnitPrice"))
                    dataGridView3.Columns["UnitPrice"].HeaderText = "Unit Price";
                if (dataGridView3.Columns.Contains("ReorderLevel"))
                    dataGridView3.Columns["ReorderLevel"].HeaderText = "Reorder Level";
                if (dataGridView3.Columns.Contains("SupplierName"))
                    dataGridView3.Columns["SupplierName"].HeaderText = "Supplier";
                if (dataGridView3.Columns.Contains("UpdatedAt"))
                    dataGridView3.Columns["UpdatedAt"].HeaderText = "Last Updated";
                GridPresentation.FormatColumns(dataGridView2);
                GridPresentation.FormatColumns(dataGridView3);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load dashboard data: " + ex.Message);
            }
        }

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
                UIHelper.ShowError("Unable to load job types. Error: " + ex.Message);
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
        // Staff data loaded via LoadStaffGrid()
        }

        private void LoadStaffGrid()
        {
            try
            {
                DataTable dt = _staffService.GetAllStaff();
                staffBindingSource.DataSource = dt;
                staffBindingSource.Filter = null;
                dataGridView4.DataSource = staffBindingSource;
                // Optional: Set friendly column headers
                if (dataGridView4.Columns.Contains("StaffID"))
                    dataGridView4.Columns["StaffID"].HeaderText = "ID";
                if (dataGridView4.Columns.Contains("FullName"))
                    dataGridView4.Columns["FullName"].HeaderText = "Full Name";
                if (dataGridView4.Columns.Contains("PhoneNumber"))
                    dataGridView4.Columns["PhoneNumber"].HeaderText = "Phone";
                if (dataGridView4.Columns.Contains("EmailAddress"))
                    dataGridView4.Columns["EmailAddress"].HeaderText = "Email";
                if (dataGridView4.Columns.Contains("EmploymentStatus"))
                    dataGridView4.Columns["EmploymentStatus"].HeaderText = "Status";
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load staff list. Error: " + ex.Message);
            }
        }

        private void LoadRoles()
        {
            try
            {
                DataTable dt = _roleService.GetAllRoles();
                cmbRole.DataSource = dt;
                cmbRole.DisplayMember = "RoleName";
                cmbRole.ValueMember = "RoleID";
                cmbRole.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load roles. Error: " + ex.Message);
            }
        }

        private void LoadInventoryGrid()
        {
            try
            {
                _inventoryDataTable = _inventoryService.GetAllInventoryItems();
                dataGridView1.DataSource = _inventoryDataTable;
                // Set friendly column headers
                if (dataGridView1.Columns.Contains("ItemID"))
                    dataGridView1.Columns["ItemID"].HeaderText = "ID";
                if (dataGridView1.Columns.Contains("ItemName"))
                    dataGridView1.Columns["ItemName"].HeaderText = "Item Name";
                if (dataGridView1.Columns.Contains("QuantityInStock"))
                    dataGridView1.Columns["QuantityInStock"].HeaderText = "Qty";
                if (dataGridView1.Columns.Contains("UnitPrice"))
                    dataGridView1.Columns["UnitPrice"].HeaderText = "Unit Price";
                if (dataGridView1.Columns.Contains("ReorderLevel"))
                    dataGridView1.Columns["ReorderLevel"].HeaderText = "Reorder";
                if (dataGridView1.Columns.Contains("SupplierName"))
                    dataGridView1.Columns["SupplierName"].HeaderText = "Supplier";
                if (dataGridView1.Columns.Contains("UpdatedAt"))
                    dataGridView1.Columns["UpdatedAt"].HeaderText = "Last Updated";
                LoadCategories();
                ApplyInventorySortAndFilter(); // Apply default sort
                LowStockChanged?.Invoke(GridPresentation.LowStock(_inventoryDataTable).Rows.Count);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load inventory. Error: " + ex.Message);
            }
        }

        private void LoadCategories()
        {
            try
            {
                DataTable dt = _inventoryService.GetAllInventoryItems();
                DataTable categoryTable = dt.DefaultView.ToTable(true, "Category");
                cmbCategory.DataSource = categoryTable;
                cmbCategory.DisplayMember = "Category";
                cmbCategory.ValueMember = "Category";
                cmbCategory.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load categories. Error: " + ex.Message);
            }
        }

        private void LoadJobsGrid()
        {
            try
            {
                DataTable jobs = _jobService.GetJobViews();
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
                button1.Text = $"Pending\n{_jobService.GetTotalJobCountByStatus("Pending")}";
                button9.Text = $"In progress\n{_jobService.GetTotalJobCountByStatus("In Progress")}";
                button2.Text = $"Completed\n{_jobService.GetTotalJobCountByStatus("Completed")}";
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to update dashboard statistics. Error: " + ex.Message);
            }
        }

        private void RegisterManagerHelp()
        {
            // Dashboard tab
            _helpManager.RegisterControlSection("Dashboard", (dataGridView2, "Recent Jobs", "Shows the most recent jobs. Use the status buttons to filter."), (dataGridView3, "Inventory Summary", "Quick view of current inventory levels."), (button1, "Pending Jobs", "Filter the job list to show only pending jobs."), (button9, "In Progress", "Filter the job list to show only in-progress jobs."), (button2, "Completed Jobs", "Filter the job list to show only completed jobs."));
            // Add New Job tab
            _helpManager.RegisterControlSection("Add Job", (txtCustName, "Customer Name", "Enter the customer's full name."), (txtPhone, "Phone", "Enter a valid 10-digit phone number."), (txtEmail, "Email", "Enter the customer's email address."), (txtFAddress, "Address", "Enter the customer's physical address."), (cmbSevArea, "Service Area", "Select the service area."), (cmbCustomerType, "Customer Type", "Select residential, commercial, etc."), (comboBox2, "Job Type", "Select the type of job/service."), (numericUpDown3, "Quantity", "Specify the quantity for the selected service."), (dateTimePicker3, "Promise Date", "Select the target completion date."), (comboBox4, "Technician", "Assign an available technician."), (textBox1, "Job Notes", "Add any special instructions or notes."), (dataGridViewCart, "Service Cart", "Review all services added to the job."));
            // Job Management tab
            _helpManager.RegisterControlSection("Job Management", (dgvJobs, "Jobs List", "All jobs in the system. Click a row to view details."), (comboBox5, "Status Filter", "Filter jobs by status (All, Pending, In Progress, Completed)."), (txtMgmtDetails, "Job Details", "Displays the full details of the selected job."), (btnDelJob, "Archive Job", "Archives a completed or cancelled job."), (btnUpdate, "Edit Job", "Edits a pending job's details."));
            // Inventory tab
            _helpManager.RegisterControlSection("Inventory", (dataGridView1, "Inventory List", "All inventory items. Double-click a row to edit stock."), (textBox7, "Search", "Search for an item by name."), (comboBox6, "Sort/Filter", "Sort the inventory list or filter by low stock."), (textBox17, "Selected Item", "Shows the name of the selected item."), (numUpdateQuantity, "Adjustment", "Enter the quantity to add or subtract."), (radioButton1, "Restock", "Adds the specified quantity to stock."), (radioButton2, "Waste", "Subtracts the specified quantity from stock."), (button7, "Update Stock", "Applies the stock adjustment."), (txtNewItemName, "New Item Name", "Enter the name of the new inventory item."), (cmbCategory, "Category", "Select the category for the new item."), (numNewQuantity, "Initial Quantity", "Enter the starting quantity."), (numNewPrice, "Unit Price", "Enter the price per unit."), (numNewReorderLevel, "Reorder Level", "Set the minimum stock level."), (txtNewSupplier, "Supplier", "Enter the supplier's name."), (btnAddNewItem, "Add Item", "Adds the new item to inventory."));
            // Profiles tab
            _helpManager.RegisterControlSection("Profiles", (dataGridView4, "Staff List", "All staff members. Double-click to view profile."), (txtMemberID, "Staff ID", "The unique identifier of the selected staff member."), (cmbRole, "Role", "Assign or view the user's role (Admin, Manager, Technician)."), (txtFullName, "Full Name", "The full name of the staff member."), (txtPhone2, "Phone", "The staff member's phone number."), (txtEmail2, "Email", "The staff member's email address."), (txtUserName, "Username", "The login username."), (txtpswd1, "Password", "Enter a new password."), (txtpswd2, "Confirm Password", "Re-enter the password to confirm."), (numSalary, "Salary", "The staff member's monthly salary."), (btnSaveStaff, "Save Staff", "Saves a new staff member."), (btnUpdateProfile, "Update Staff", "Updates an existing staff member's details."), (button19, "Archive/Delete", "Archives or permanently deletes a staff member."));
            // Also register the container so any control not explicitly registered gets a generic help.
            _helpManager.RegisterContainer(this);
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
        private void button5_Click_1(object sender, EventArgs e) // Add to cart
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

        private void button4_Click_1(object sender, EventArgs e) // Remove from cart
        {
            try
            {
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

        private async void button6_Click_1(object sender, EventArgs e) // Save/Update Job
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
                        DisplayJobSummaryInvoice(newJobID, materialCost, labourCost, transportCost, taxAmount, grandTotal, txtMgmtDetails);
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
                UIHelper.ShowWarning("Please select a job.");
                return;
            }

            string currentStatus = row["JobStatus"].ToString();
            int jobID = Convert.ToInt32(row["JobID"]);
            if (!await WorkflowUi.ConfirmAsync(this, $"Archive this {currentStatus} job?"))
                return;
            try
            {
                if (!UserSession.Check())
                    return;
                _jobService.ArchiveJob(currentStatus, jobID);
                UIHelper.ShowSuccess("Job archived.");
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

        private void comboBox5_SelectedIndexChanged_1(object sender, EventArgs e) // Status filter
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
            txtMgmtDetails.Text = DesktopRecordPresentation.JobDetails(row);
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

        private void btnUpdate_Click(object sender, EventArgs e) // Edit job
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
            targetTextBox = txtMgmtDetails; // Job summaries never belong to the staff profile panel.
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

        internal void ShowLowStock()
        {
            tabControl1.SelectedTab = tabPage4;
            inventoryTabs.SelectedTab = tabStock;
            comboBox6.SelectedItem = "Low Stock (Qty <= Reorder)";
            ApplyInventorySortAndFilter();
        }

        private void ApplyInventorySortAndFilter()
        {
            if (_inventoryDataTable == null)
                return;
            string selected = comboBox6.SelectedItem?.ToString() ?? "All Items";
            string searchText = textBox7.Text.Trim();
            DataView dv = _inventoryDataTable.DefaultView;
            // Build filter
            List<string> filters = new List<string>();
            // 1. Low stock filter
            if (selected == "Low Stock (Qty <= Reorder)")
            {
                filters.Add("QuantityInStock <= ReorderLevel");
            }

            // 2. Search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string safeSearch = searchText.Replace("'", "''");
                filters.Add($"ItemName LIKE '%{safeSearch}%'");
            }

            dv.RowFilter = filters.Count > 0 ? string.Join(" AND ", filters) : "";
            // Build sort
            switch (selected)
            {
                case "Name (A-Z)":
                    dv.Sort = "ItemName ASC";
                    break;
                case "Name (Z-A)":
                    dv.Sort = "ItemName DESC";
                    break;
                case "Category (A-Z)":
                    dv.Sort = "Category ASC, ItemName ASC";
                    break;
                case "Price (Low to High)":
                    dv.Sort = "UnitPrice ASC";
                    break;
                case "Price (High to Low)":
                    dv.Sort = "UnitPrice DESC";
                    break;
                case "Stock (Low to High)":
                    dv.Sort = "QuantityInStock ASC";
                    break;
                case "Stock (High to Low)":
                    dv.Sort = "QuantityInStock DESC";
                    break;
                case "All Items":
                default:
                    dv.Sort = "ItemName ASC";
                    break;
            }

            // If Low Stock filter is selected but no sort specified, default to Qty ASC
            if (selected == "Low Stock (Qty <= Reorder)" && string.IsNullOrEmpty(dv.Sort))
            {
                dv.Sort = "QuantityInStock ASC";
            }

            // Refresh DataGridView
            dataGridView1.DataSource = dv;
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

        private void ResetFormState()
        {
            ClearAddJobFields();
            selectedJobCustomerID = -1;
            isUpdating = false;
            currentEditingJobID = -1;
            cartTable.Clear();
            button5.Enabled = false;
            btnSaveProfile.Enabled = true;
            txtCustName.ReadOnly = false;
            txtPhone.ReadOnly = false;
            txtEmail.ReadOnly = false;
            txtFAddress.ReadOnly = false;
            cmbSevArea.Enabled = true;
            cmbCustomerType.Enabled = true;
            dateTimePicker3.Enabled = true;
            comboBox4.Enabled = true;
            textBox1.Clear();
            LoadJobsGrid();
            tabControl1.SelectedTab = tabPage1;
        }

        // ════════════════════════════════════════════════════════════════════
        //  EVENT HANDLERS
        // ════════════════════════════════════════════════════════════════════
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_jobService != null && tabControl1.SelectedTab == tabPage3)
                LoadJobsGrid();
            if (tabControl1.SelectedTab?.Name == "tabPage2")
            {
                if (isUpdating && currentEditingJobID > 0)
                    DesktopRecordPresentation.BindTechnicians(comboBox4, _staffService.GetAvailableTechniciansForJob(dateTimePicker3.Value.Date, currentEditingJobID), lblTechnicianAvailability);
                else
                    PopulateTechnicianCombo(dateTimePicker3.Value.Date);
            }

            if (tabControl1.SelectedTab == tabPage4)
                LoadInventoryGrid();
            if (tabControl1.SelectedTab == tabPage1) // Dashboard tab
            {
                // Reset the dashboard job filter to show all jobs
                LoadDashboardData();
                UpdateStatButtons();
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

        private void textBox3_TextChanged_1(object sender, EventArgs e) // Customer search
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
                        btnSaveProfile.Enabled = false;
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
        // Optional filter
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
        private void button11_Click_1(object sender, EventArgs e) => Logout();
        private void button12_Click_1(object sender, EventArgs e) => Logout();
        private void button16_Click(object sender, EventArgs e) => Logout();
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

        // ════════════════════════════════════════════════════════════════════
        //  INVENTORY MANAGEMENT
        // ════════════════════════════════════════════════════════════════════
        private void InventoryRowDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !(dataGridView1.Rows[e.RowIndex].DataBoundItem is DataRowView item))
                return;
            LoadInventoryItemDetails(item.Row);
            inventoryTabs.SelectedTab = tabUpdateStock;
            numUpdateQuantity.Focus();
            numUpdateQuantity.Select(0, numUpdateQuantity.Text.Length);
        }

        private void LoadInventoryItemDetails(DataRow row)
        {
            try
            {
                // Populate the fields
                textBox17.Text = row["ItemName"]?.ToString() ?? "";
                textBox17.Enabled = false;
                numUpdateQuantity.Value = numUpdateQuantity.Minimum;
                // Build summary
                textBox18.Text = $"ITEM SUMMARY\r\n----------------------------\r\n" + $"Name: {row["ItemName"]}\r\n" + $"Current Stock: {row["QuantityInStock"]}\r\n" + $"Unit Price: {Convert.ToDecimal(row["UnitPrice"]).ToString("C2", GridPresentation.Rand)}\r\n" + $"Reorder Level: {row["ReorderLevel"]}\r\n" + $"Supplier: {row["SupplierName"]}\r\n" + $"Last Updated: {row["UpdatedAt"]}";
                btnRmvItem.Enabled = true;
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load inventory details: " + ex.Message);
            }
        }

        private void button7_Click_1(object sender, EventArgs e) // Update stock
        {
            if (!(dataGridView1.CurrentRow?.DataBoundItem is DataRowView selected))
            {
                WorkflowUi.Warn(tabUpdateStock, "Stock was not updated. Select an inventory item first.");
                return;
            }

            int quantity = (int)numUpdateQuantity.Value;
            bool restock = radioButton1.Checked;
            if (!restock && !radioButton2.Checked)
            {
                WorkflowUi.Warn(tabUpdateStock, "Stock was not updated. Choose Restock or Waste.");
                return;
            }

            if (!UserSession.Check())
                return;
            int itemId = Convert.ToInt32(selected["ItemID"]);
            string name = Convert.ToString(selected["ItemName"]);
            try
            {
                _inventoryService.AdjustInventoryQuantity(itemId, restock ? quantity : -quantity);
            }
            catch (Exception ex)
            {
                WorkflowUi.ShowError(tabUpdateStock, "Stock update could not be confirmed. " + ex.Message);
                return;
            }

            LoadInventoryGrid();
            var updatedRow = dataGridView1.Rows.Cast<DataGridViewRow>().FirstOrDefault(row => row.DataBoundItem is DataRowView item && Convert.ToInt32(item["ItemID"]) == itemId);
            if (updatedRow != null)
                dataGridView1.CurrentCell = updatedRow.Cells.Cast<DataGridViewCell>().First(cell => cell.Visible);
            else
                inventoryTabs.SelectedTab = tabStock;
            WorkflowUi.Notify(inventoryTabs.SelectedTab, restock ? $"Restocked {name} by {quantity:N0} units." : $"Recorded {quantity:N0} units of waste for {name}.");
        }

        /* private void button13_Click(object sender, EventArgs e)  // Add new item
        {
            if (string.IsNullOrWhiteSpace(txtNewItemName.Text))
            {
                UIHelper.ShowWarning("Item name is required.");
                return;
            }

            try
            {
                if (!UserSession.Check()) return;
                _inventoryService.CreateInventoryItem(
                    txtNewItemName.Text,
                    cmbCategory.Text,
                    (int)numNewQuantity.Value,
                    (decimal)numNewPrice.Value,
                    (int)numNewReorderLevel.Value,
                    txtNewSupplier.Text
                );
                LoadInventoryGrid();
                UIHelper.ShowSuccess("Item added successfully.");
                txtNewItemName.Clear();
                numNewQuantity.Value = 100;
                numNewPrice.Value = 1;
                numNewReorderLevel.Value = 1;
                txtNewSupplier.Clear();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to add item. Error: " + ex.Message);
            }
        }*/
        private void AddInventoryItem_Click(object sender, EventArgs e)
        {
            WorkflowUi.Run(this, () =>
            {
                string name = txtNewItemName.Text.Trim();
                if (_inventoryService.GetAllInventoryItems().AsEnumerable().Any(row => string.Equals(Convert.ToString(row["ItemName"]).Trim(), name, StringComparison.OrdinalIgnoreCase)))
                    throw new ArgumentException("This item already exists. Select it in Stock and use Update stock.");
                _inventoryService.CreateInventoryItem(name, cmbCategory.Text, (int)numNewQuantity.Value, numNewPrice.Value, (int)numNewReorderLevel.Value, txtNewSupplier.Text.Trim(), null);
                LoadInventoryGrid();
                txtNewItemName.Clear();
                txtNewSupplier.Clear();
                UIHelper.ShowSuccess("Inventory item created.");
            });
        }

        private async void btnRmvItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                UIHelper.ShowWarning("Select an item to delete.");
                return;
            }

            try
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int itemId;
                string itemName;
                if (row.DataBoundItem is DataRowView drv)
                {
                    DataRow dataRow = drv.Row;
                    itemId = Convert.ToInt32(dataRow["ItemID"]);
                    itemName = dataRow["ItemName"].ToString();
                }
                else
                {
                    // Fallback using column index
                    itemId = Convert.ToInt32(row.Cells[0].Value);
                    itemName = row.Cells[1].Value.ToString();
                }

                if (!await WorkflowUi.ConfirmAsync(this, $"Delete '{itemName}' permanently?"))
                    return;
                if (!UserSession.Check())
                    return;
                _inventoryService.DeleteInventoryItem(itemId);
                LoadInventoryGrid();
                UIHelper.ShowSuccess("Item deleted.");
                // Clear the details fields
                textBox17.Clear();
                textBox18.Clear();
                btnRmvItem.Enabled = false;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to delete item. Error: " + ex.Message);
            }
        }

        private void textBox7_TextChanged_1(object sender, EventArgs e) // Inventory search
        {
            ApplyInventorySortAndFilter();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyInventorySortAndFilter();
        }

        private void comboBox6_SelectedIndexChanged_1(object sender, EventArgs e) // Sort
        {
            LoadInventoryGrid(); // Re-fetch and sort can be implemented here if needed
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "QuantityInStock" && e.Value != null)
            {
                int qty = Convert.ToInt32(e.Value);
                if (qty <= 5)
                {
                    e.CellStyle.BackColor = Color.LightCoral;
                    e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  STAFF MANAGEMENT
        // ════════════════════════════════════════════════════════════════════
        private void LoadStaffProfile(int staffID)
        {
            try
            {
                DataTable dt = _staffService.GetStaffByID(staffID);
                if (dt.Rows.Count == 0)
                {
                    UIHelper.ShowWarning("Staff member not found.");
                    return;
                }

                DataRow r = dt.Rows[0];
                txtMemberID.Text = r["StaffID"].ToString();
                txtMemberID.Enabled = false;
                cmbRole.SelectedValue = r["RoleID"];
                txtFullName.Text = r["FullName"].ToString();
                txtPhone2.Text = r["PhoneNumber"].ToString();
                txtEmail2.Text = r["EmailAddress"].ToString();
                txtUserName.Text = r["Username"].ToString();
                txtUserName.ReadOnly = true;
                txtpswd1.Clear();
                txtpswd2.Clear();
                txtpswd1.Enabled = true;
                txtpswd2.Enabled = true;
                btnSaveStaff.Enabled = false;
                decimal salary = Convert.ToDecimal(r["Salary"]);
                numSalary.Minimum = Math.Min(numSalary.Minimum, salary);
                numSalary.Maximum = Math.Max(numSalary.Maximum, salary);
                numSalary.Value = salary;
                textBox2.Text = DesktopRecordPresentation.StaffSummary(r);
                groupBox11.Text = "Edit staff profile";
                lblStaffSelection.Text = "Selected: " + Convert.ToString(r["FullName"]);
                cmbEmployment.SelectedItem = Convert.ToString(r["EmploymentStatus"]);
                cmbAvailability.SelectedIndex = Convert.ToBoolean(r["IsAvailable"] == DBNull.Value ? false : r["IsAvailable"]) ? 0 : 1;
                btnAvailability.Enabled = Convert.ToString(r["RoleName"]) == "Technician" && Convert.ToString(r["EmploymentStatus"]) == "Active";
                button17.Enabled = true;
                button19.Enabled = true;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load profile details: " + ex.Message);
            }
        }

        private void dataGridView4_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            try
            {
                DataGridViewRow row = dataGridView4.Rows[e.RowIndex];
                // Get StaffID from the DataBoundItem (most reliable)
                if (row.DataBoundItem is DataRowView drv)
                {
                    DataRow dataRow = drv.Row;
                    int staffID = Convert.ToInt32(dataRow["StaffID"]);
                    LoadStaffProfile(staffID);
                }
                else
                {
                    // Fallback: use column index (0 is usually StaffID)
                    if (row.Cells.Count > 0 && row.Cells[0].Value != DBNull.Value)
                    {
                        int staffID = Convert.ToInt32(row.Cells[0].Value);
                        LoadStaffProfile(staffID);
                    }
                    else
                    {
                        UIHelper.ShowWarning("Unable to identify the selected staff member.");
                    }
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to load staff profile. Error: " + ex.Message);
            }
        }

        private void btnSaveStaff_Click(object sender, EventArgs e)
        {
            // Validate
            if (cmbRole.SelectedValue == null)
            {
                UIHelper.ShowWarning("Select a role.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                UIHelper.ShowWarning("Full Name is required.");
                return;
            }

            if (!ValidationHelper.IsValidPhoneNumber(txtPhone2.Text))
            {
                UIHelper.ShowWarning("Please enter a valid 10-digit phone number.");
                return;
            }

            if (!ValidationHelper.IsValidEmail(txtEmail2.Text))
            {
                UIHelper.ShowWarning("Please enter a valid email address.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                UIHelper.ShowWarning("Username is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtpswd1.Text))
            {
                UIHelper.ShowWarning("Password is required.");
                return;
            }

            if (txtpswd1.Text != txtpswd2.Text)
            {
                UIHelper.ShowWarning("Passwords do not match.");
                return;
            }

            if (numSalary.Value <= 0)
            {
                UIHelper.ShowWarning("Salary must be positive.");
                return;
            }

            try
            {
                if (!UserSession.Check())
                    return;
                _staffService.InsertStaff((int)cmbRole.SelectedValue, txtFullName.Text.Trim(), txtPhone2.Text.Trim(), txtEmail2.Text.Trim(), numSalary.Value, txtUserName.Text.Trim(), txtpswd1.Text);
                LoadStaffGrid();
                UIHelper.ShowSuccess("Staff member saved.");
                ClearStaffFields();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Failed to save staff member. Error: " + ex.Message);
            }
        }

        private void AddStaffManagementActions()
        {
            tabPage5.Text = "Staff Management";
            button17.Enabled = false;
            DataRow SelectedStaff()
            {
                if (!int.TryParse(txtMemberID.Text, out int id))
                    throw new ArgumentException("Select a staff member from the directory first.");
                var rows = _staffService.GetStaffByID(id);
                if (rows.Rows.Count != 1 || Convert.ToString(rows.Rows[0]["RoleName"]) == "SysAdmin")
                    throw new InvalidOperationException("Select a staff member managed by this workspace.");
                return rows.Rows[0];
            }

            btnNewStaff.Click += (sender, args) => ClearStaffFields();
            btnEmployment.Click += (sender, args) => WorkflowUi.Run(this, () =>
            {
                var row = SelectedStaff();
                int id = Convert.ToInt32(row["StaffID"]);
                if (cmbEmployment.SelectedIndex < 0)
                    throw new ArgumentException("Select an employment status.");
                if (id == UserSession.StaffID && cmbEmployment.Text == "Inactive")
                    throw new ArgumentException("You cannot deactivate your own account.");
                _staffService.ChangeEmploymentStatus(id, cmbEmployment.Text);
                if (cmbEmployment.Text == "Inactive")
                    _staffService.ChangeAvailability(id, false);
                LoadStaffGrid();
                LoadStaffProfile(id);
                WorkflowUi.Notify(this, "Employment status updated.");
            });
            btnAvailability.Click += (sender, args) => WorkflowUi.Run(this, () =>
            {
                var row = SelectedStaff();
                if (Convert.ToString(row["RoleName"]) != "Technician" || Convert.ToString(row["EmploymentStatus"]) != "Active")
                    throw new ArgumentException("Availability is for active technicians.");
                if (cmbAvailability.SelectedIndex < 0)
                    throw new ArgumentException("Select availability.");
                int id = Convert.ToInt32(row["StaffID"]);
                _staffService.ChangeAvailability(id, cmbAvailability.SelectedIndex == 0);
                LoadStaffGrid();
                LoadStaffProfile(id);
                WorkflowUi.Notify(this, "Availability updated.");
            });
            btnResetStaffPassword.Click += (sender, args) => WorkflowUi.Run(this, () =>
            {
                int id = Convert.ToInt32(SelectedStaff()["StaffID"]);
                if (txtpswd1.Text != txtpswd2.Text || !ValidationHelper.IsValidPassword(txtpswd1.Text))
                    throw new ArgumentException("Enter and confirm a password that meets the password requirements.");
                _staffService.ChangePassword(id, txtpswd1.Text);
                txtpswd1.Clear();
                txtpswd2.Clear();
                WorkflowUi.Notify(this, "Password updated.");
            });
        }

        private async void button19_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtMemberID.Text, out int id))
            {
                UIHelper.ShowWarning("Select a staff member first.");
                return;
            }

            await WorkflowUi.RunAsync(this, async () =>
            {
                if (id == UserSession.StaffID)
                    throw new ArgumentException("You cannot archive your own staff account.");
                if (!await WorkflowUi.ConfirmAsync(this, "Archive the selected staff member?") || !UserSession.Check())
                    return;
                _staffService.ArchiveStaff(id);
                LoadStaffGrid();
                ClearStaffFields();
                UIHelper.ShowSuccess("Staff archived.");
            });
        }

        private void button17_Click(object sender, EventArgs e)
        {
            WorkflowUi.Run(this, () =>
            {
                if (!int.TryParse(txtMemberID.Text, out int id) || !(cmbRole.SelectedValue is int roleId))
                    throw new ArgumentException("Select a staff member and role first.");
                _staffService.UpdateStaffDetails(id, roleId, txtFullName.Text.Trim(), txtPhone2.Text.Trim(), txtEmail2.Text.Trim(), numSalary.Value);
                LoadStaffGrid();
                ClearStaffFields();
                UIHelper.ShowSuccess("Staff details updated.");
            });
        }

        private void ClearStaffFields()
        {
            txtMemberID.Clear();
            txtMemberID.Enabled = true;
            txtFullName.Clear();
            txtPhone2.Clear();
            txtEmail2.Clear();
            txtUserName.Clear();
            txtUserName.ReadOnly = false;
            txtpswd1.Clear();
            txtpswd2.Clear();
            txtpswd1.Enabled = true;
            txtpswd2.Enabled = true;
            btnSaveStaff.Enabled = true;
            numSalary.Minimum = 15000;
            numSalary.Value = numSalary.Minimum;
            cmbRole.SelectedIndex = -1;
            textBox2.Text = DesktopRecordPresentation.StaffSummary((dataGridView4.CurrentRow?.DataBoundItem as DataRowView)?.Row);
            groupBox11.Text = "Add staff profile";
            lblStaffSelection.Text = "Enter the new staff member's details.";
            cmbEmployment.SelectedIndex = -1;
            cmbAvailability.SelectedIndex = -1;
            button17.Enabled = false;
            button19.Enabled = false;
        }

        private void textBox15_TextChanged(object sender, EventArgs e) // Staff search by name
        {
            if (_staffService == null)
                return;
            if (string.IsNullOrWhiteSpace(textBox15.Text))
            {
                LoadStaffGrid();
                return;
            }

            WorkflowUi.Run(this, () => dataGridView4.DataSource = _staffService.SearchStaffByName(textBox15.Text.Trim()));
        }

        private void txtPhone2_TextChanged(object sender, EventArgs e)
        {
        // Contact editor only. Staff filtering belongs to textBox15.
        }

        // ════════════════════════════════════════════════════════════════════
        //  REPORTS & HELP
        // ════════════════════════════════════════════════════════════════════
        private void button8_Click_1(object sender, EventArgs e)
        {
        }

        private void button8_Click_2(object sender, EventArgs e) // opens reports tab
        {
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenHelpGuide("Manager_Help_Guide.pdf");
        }

        private void OpenHelpGuide(string fileName)
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, "resources", fileName);
                if (File.Exists(path))
                    System.Diagnostics.Process.Start(path);
                else
                    UIHelper.ShowWarning("Help guide not found.");
            }
            catch (Exception ex)
            {
                UIHelper.ShowError("Unable to open help guide. Error: " + ex.Message);
            }
        }

        private void FilterDashboardJobs(string status)
        {
            if (_dashboardJobs == null)
                return;
            DataView dv = _dashboardJobs.DefaultView;
            if (string.IsNullOrEmpty(status))
            {
                dv.RowFilter = "JobStatus <> 'Archived'";
            }
            else
            {
                // Escape single quotes in status
                string safeStatus = status.Replace("'", "''");
                dv.RowFilter = $"JobStatus = '{safeStatus}'";
            }

            dv.Sort = "JobID DESC";
            var recent = _dashboardJobs.Clone();
            for (int i = 0; i < Math.Min(10, dv.Count); i++)
                recent.ImportRow(dv[i].Row);
            dataGridView2.DataSource = recent;
        }

        // ════════════════════════════════════════════════════════════════════
        //  DESIGNER STUBS (keep to avoid designer errors)
        // ════════════════════════════════════════════════════════════════════
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void label7_Click(object sender, EventArgs e)
        {
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void gbxQuickAct_Enter(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void gbxRecJobs_Enter(object sender, EventArgs e)
        {
        }

        private void lblAdminGreeting_Click(object sender, EventArgs e)
        {
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {
        }

        private void label16_Click(object sender, EventArgs e)
        {
        }

        private void label11_Click(object sender, EventArgs e)
        {
        }

        private void label12_Click(object sender, EventArgs e)
        {
        }

        private void label13_Click(object sender, EventArgs e)
        {
        }

        private void label14_Click(object sender, EventArgs e)
        {
        }

        private void label15_Click(object sender, EventArgs e)
        {
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {
        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {
        }

        private void label21_Click(object sender, EventArgs e)
        {
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
        }

        private void label17_Click(object sender, EventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {
        }

        private void label18_Click(object sender, EventArgs e)
        {
        }

        private void button8_Click(object sender, EventArgs e)
        {
        }

        private void label19_Click(object sender, EventArgs e)
        {
        }

        private void label20_Click(object sender, EventArgs e)
        {
        }

        private async void button6_Click(object sender, EventArgs e)
        {
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void label10_Click(object sender, EventArgs e)
        {
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
        }

        private void txtJobDestn_TextChanged(object sender, EventArgs e)
        {
        }

        private void btnSaveJob_Click(object sender, EventArgs e)
        {
        }

        private void label6_Click(object sender, EventArgs e)
        {
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnCancJob_Click(object sender, EventArgs e)
        {
            ClearAddJobFields();
        }

        private void label8_Click(object sender, EventArgs e)
        {
        }

        private void label9_Click(object sender, EventArgs e)
        {
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label22_Click(object sender, EventArgs e)
        {
        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {
        }

        private void dgvJobs_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void label24_Click(object sender, EventArgs e)
        {
        }

        private void label23_Click(object sender, EventArgs e)
        {
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
        }

        private void groupBox8_Enter(object sender, EventArgs e)
        {
        }

        private void label25_Click(object sender, EventArgs e)
        {
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label26_Click(object sender, EventArgs e)
        {
        }

        private void groupBox9_Enter(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void label27_Click(object sender, EventArgs e)
        {
        }

        private void label28_Click(object sender, EventArgs e)
        {
        }

        private void button14_Click(object sender, EventArgs e)
        {
        }

        private void button6_Click_2(object sender, EventArgs e)
        {
        }

        private void checkBox8_CheckedChanged_2(object sender, EventArgs e)
        {
        }

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void groupBox7_Enter_1(object sender, EventArgs e)
        {
        }

        private void label23_Click_1(object sender, EventArgs e)
        {
        }

        private void dgvJobs_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label22_Click_1(object sender, EventArgs e)
        {
        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
        }

        private void textBox5_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void label28_Click_1(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateStatButtons();
            FilterDashboardJobs("Pending");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateStatButtons();
            FilterDashboardJobs("Completed");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            UpdateStatButtons();
            FilterDashboardJobs("In Progress");
        }

        private void textBox12_Leave(object sender, EventArgs e)
        {
        }

        private void textBox12_ControlRemoved(object sender, ControlEventArgs e)
        {
        }

        private void ShowRecentDashboardJobs()
        {
            if (_dashboardJobs == null)
                return;
            var view = new DataView(_dashboardJobs)
            {
                Sort = "JobID DESC",
                RowFilter = "JobStatus <> 'Archived'"
            };
            var recent = _dashboardJobs.Clone();
            for (int i = 0; i < Math.Min(10, view.Count); i++)
                recent.ImportRow(view[i].Row);
            dataGridView2.DataSource = recent;
        }

        private void DashboardRefresh_Click(object sender, EventArgs e)
        {
            if (!UserSession.Check())
                return;
            LoadDashboardData();
            UpdateStatButtons();
        }

        private void UpdateStaffActionState()
        {
            var record = dataGridView4.CurrentRow?.DataBoundItem as DataRowView;
            textBox2.Text = DesktopRecordPresentation.StaffSummary(record?.Row);
            bool selected = record != null;
            bool protectedRole = selected && record.Row.Table.Columns.Contains("RoleName") && string.Equals(Convert.ToString(record["RoleName"]), "SysAdmin", StringComparison.OrdinalIgnoreCase);
            bool ownAccount = selected && record.Row.Table.Columns.Contains("StaffID") && Convert.ToInt32(record["StaffID"]) == UserSession.StaffID;
            button17.Enabled = selected && !protectedRole;
            button19.Enabled = selected && !protectedRole && !ownAccount;
        }

        private void StaffAvailability_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex >= dataGridView4.Columns.Count)
                return;
            if (dataGridView4.Columns[e.ColumnIndex].DataPropertyName != "IsAvailable")
                return;
            var row = (dataGridView4.Rows[e.RowIndex].DataBoundItem as DataRowView)?.Row;
            if (row == null)
                return;
            e.Value = DesktopRecordPresentation.Availability(row);
            e.FormattingApplied = true;
        }
    }
}
