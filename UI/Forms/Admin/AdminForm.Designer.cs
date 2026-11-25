using System;
using System.Windows.Forms;

namespace Major_Project.UI.Forms.Admin
{
    partial class AdminForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private System.Windows.Forms.TabPage CustomersWorkspacePage;
        private Major_Project.UI.Forms.Common.CustomerDirectoryView CustomersLayout;
        private System.Windows.Forms.TabPage StockWorkspacePage;
        private Major_Project.UI.Forms.Common.StockDirectoryView StockLayout;
        private System.Windows.Forms.TabPage JobChanges;
        private Major_Project.UI.Forms.Common.JobChangesView JobChangesLayout;
        private System.Windows.Forms.TabPage ProfileWorkspacePage;
        private Major_Project.UI.Forms.Common.StaffProfileView ProfileLayout;
        private System.Windows.Forms.TabPage AssistantWorkspacePage;
        private Major_Project.UI.Forms.Common.AssistantView AssistantLayout;

        internal System.Windows.Forms.Label tabJobDetailsFeedbackBanner;
        internal System.Windows.Forms.Label tabPage1FeedbackBanner;
        internal System.Windows.Forms.Label tabPage2FeedbackBanner;
        internal System.Windows.Forms.Label tabPage3FeedbackBanner;
        internal System.Windows.Forms.Label CustomersWorkspacePageFeedbackBanner;
        internal System.Windows.Forms.Label StockWorkspacePageFeedbackBanner;
        internal System.Windows.Forms.Label JobChangesFeedbackBanner;
        internal System.Windows.Forms.Label ProfileWorkspacePageFeedbackBanner;
        internal System.Windows.Forms.Label AssistantWorkspacePageFeedbackBanner;
        internal System.Windows.Forms.Label workflowFeedback;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabJobDetails = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.jobDetailsToolbar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnBackToJobs = new System.Windows.Forms.Button();
            this.btnOpenJobDetails = new System.Windows.Forms.Button();
            this.lblTechnicianAvailability = new System.Windows.Forms.Label();
            this.cartGrandTotalColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cartServiceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cartQuantityColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jobsFilterBar = new System.Windows.Forms.Panel();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.lblJobStatusFilter = new System.Windows.Forms.Label();
            this.jobDetailActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelJob = new System.Windows.Forms.Button();
            this.btnJobManagement = new System.Windows.Forms.Button();
            this.btnViewInvoicePdf = new System.Windows.Forms.Button();
            this.btnViewReceiptPdf = new System.Windows.Forms.Button();
            this.jobActionsToolbar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancelSelectedJob = new System.Windows.Forms.Button();
            this.btnCollectCash = new System.Windows.Forms.Button();
            this.btnSaveInvoicePdf = new System.Windows.Forms.Button();
            this.btnSaveReceiptPdf = new System.Windows.Forms.Button();
            this.cmbIdentityDocument = new System.Windows.Forms.ComboBox();
            this.txtPassportNumber = new System.Windows.Forms.TextBox();
            this.txtPassportCountry = new System.Windows.Forms.TextBox();
            this.lblIdentityDocument = new System.Windows.Forms.Label();
            this.lblPassportNumber = new System.Windows.Forms.Label();
            this.lblPassportCountry = new System.Windows.Forms.Label();
            this.dashboardLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblDashboardTitle = new System.Windows.Forms.Label();
            this.lblAdminGreeting = new System.Windows.Forms.Label();
            this.dashboardCards = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.gbxRecJobs = new System.Windows.Forms.GroupBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jobsBindingSource3 = new System.Windows.Forms.BindingSource(this.components);
            this.groupWst34DataSet = new Major_Project.GroupWst34DataSet();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn20 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn21 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inventoryItemsBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.lblLowStockEmpty = new System.Windows.Forms.Label();
            this.dashboardActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnDashboardHelp = new System.Windows.Forms.Button();
            this.btnDashboardRefresh = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new Major_Project.UI.Forms.Common.WorkspaceTabs();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label31 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.gbxQuickAct = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnHelpJobs = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.jobTypesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.dateTimePicker3 = new System.Windows.Forms.DateTimePicker();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.dataGridViewCart = new System.Windows.Forms.DataGridView();
            this.label21 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.button10 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblCompanyRegNo = new System.Windows.Forms.Label();
            this.lblSA_ID = new System.Windows.Forms.Label();
            this.txtSouthAfricanIDNumber = new System.Windows.Forms.TextBox();
            this.txtCompanyRegistrationNumber = new System.Windows.Forms.TextBox();
            this.btnUpdateProfile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbCustomerType = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.dataGridView6 = new System.Windows.Forms.DataGridView();
            this.CustomerID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FullName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PhoneNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EmailAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FullAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.customersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnSaveProfile = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.cmbSevArea = new System.Windows.Forms.ComboBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.txtFAddress = new System.Windows.Forms.TextBox();
            this.txtCustName = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.dgvJobs = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn22 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn23 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn24 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn25 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn26 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn27 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn28 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn29 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn30 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn31 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn32 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn33 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn34 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.jobsBindingSource4 = new System.Windows.Forms.BindingSource(this.components);
            this.inventoryItemsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.jobsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.jobsTableAdapter = new Major_Project.GroupWst34DataSetTableAdapters.JobsTableAdapter();
            this.inventoryItemsTableAdapter = new Major_Project.GroupWst34DataSetTableAdapters.InventoryItemsTableAdapter();
            this.tableAdapterManager = new Major_Project.GroupWst34DataSetTableAdapters.TableAdapterManager();
            this.jobsTableAdapter1 = new Major_Project.GroupWst34DataSetTableAdapters.JobsTableAdapter();
            this.customersTableAdapter = new Major_Project.GroupWst34DataSetTableAdapters.CustomersTableAdapter();
            this.jobsViewTableAdapter1 = new Major_Project.GroupWst34DataSetTableAdapters.JobsViewTableAdapter();
            this.jobTypesTableAdapter1 = new Major_Project.GroupWst34DataSetTableAdapters.JobTypesTableAdapter();
            this.staffTableAdapter1 = new Major_Project.GroupWst34DataSetTableAdapters.StaffTableAdapter();
            this.jobDetailsTableAdapter1 = new Major_Project.GroupWst34DataSetTableAdapters.JobDetailsTableAdapter();
            this.jobStatusHistoryTableAdapter1 = new Major_Project.GroupWst34DataSetTableAdapters.JobStatusHistoryTableAdapter();
            this.jobMaterialsTableAdapter1 = new Major_Project.GroupWst34DataSetTableAdapters.JobMaterialsTableAdapter();
            this.JobTypesTableAdapteer = new Major_Project.GroupWst34DataSetTableAdapters.JobTypesTableAdapter();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabJobDetails.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.jobDetailsToolbar.SuspendLayout();
            this.jobsFilterBar.SuspendLayout();
            this.jobDetailActions.SuspendLayout();
            this.jobActionsToolbar.SuspendLayout();
            this.dashboardLayout.SuspendLayout();
            this.dashboardCards.SuspendLayout();
            this.gbxRecJobs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobsBindingSource3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupWst34DataSet)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inventoryItemsBindingSource1)).BeginInit();
            this.dashboardActions.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.gbxQuickAct.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.jobTypesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCart)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customersBindingSource)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvJobs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobsBindingSource4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inventoryItemsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tabJobDetails
            // 
            this.tabJobDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.tabJobDetails.Controls.Add(this.groupBox8);
            this.tabJobDetails.Controls.Add(this.jobDetailsToolbar);
            this.tabJobDetails.Location = new System.Drawing.Point(4, 29);
            this.tabJobDetails.Name = "tabJobDetails";
            this.tabJobDetails.Padding = new System.Windows.Forms.Padding(16);
            this.tabJobDetails.Size = new System.Drawing.Size(1642, 996);
            this.tabJobDetails.TabIndex = 3;
            this.tabJobDetails.Text = "Job Details";
            // 
            // groupBox8
            // 
            this.groupBox8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.groupBox8.Controls.Add(this.textBox2);
            this.groupBox8.Controls.Add(this.label25);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.groupBox8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.groupBox8.Location = new System.Drawing.Point(16, 68);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(12);
            this.groupBox8.Size = new System.Drawing.Size(1610, 912);
            this.groupBox8.TabIndex = 15;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Job Details";
            this.groupBox8.Enter += new System.EventHandler(this.groupBox8_Enter);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.textBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.textBox2.Location = new System.Drawing.Point(12, 32);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(1586, 868);
            this.textBox2.TabIndex = 40;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.BackColor = System.Drawing.Color.Transparent;
            this.label25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label25.Location = new System.Drawing.Point(16, 29);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(0, 20);
            this.label25.TabIndex = 0;
            this.label25.Visible = false;
            // 
            // jobDetailsToolbar
            // 
            this.jobDetailsToolbar.Controls.Add(this.btnBackToJobs);
            this.jobDetailsToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.jobDetailsToolbar.Location = new System.Drawing.Point(16, 16);
            this.jobDetailsToolbar.Name = "jobDetailsToolbar";
            this.jobDetailsToolbar.Padding = new System.Windows.Forms.Padding(0, 4, 0, 8);
            this.jobDetailsToolbar.Size = new System.Drawing.Size(1610, 52);
            this.jobDetailsToolbar.TabIndex = 16;
            // 
            // btnBackToJobs
            // 
            this.btnBackToJobs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnBackToJobs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackToJobs.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnBackToJobs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnBackToJobs.Location = new System.Drawing.Point(3, 7);
            this.btnBackToJobs.Name = "btnBackToJobs";
            this.btnBackToJobs.Size = new System.Drawing.Size(152, 36);
            this.btnBackToJobs.TabIndex = 0;
            this.btnBackToJobs.Text = "Back to Jobs";
            this.btnBackToJobs.UseVisualStyleBackColor = false;
            this.btnBackToJobs.Click += new System.EventHandler(this.BackToJobs_Click);
            // 
            // btnOpenJobDetails
            // 
            this.btnOpenJobDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnOpenJobDetails.Enabled = false;
            this.btnOpenJobDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenJobDetails.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnOpenJobDetails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnOpenJobDetails.Location = new System.Drawing.Point(1011, 11);
            this.btnOpenJobDetails.Name = "btnOpenJobDetails";
            this.btnOpenJobDetails.Size = new System.Drawing.Size(152, 36);
            this.btnOpenJobDetails.TabIndex = 6;
            this.btnOpenJobDetails.Text = "Job Details";
            this.btnOpenJobDetails.UseVisualStyleBackColor = false;
            this.btnOpenJobDetails.Click += new System.EventHandler(this.OpenJobDetails_Click);
            // 
            // lblTechnicianAvailability
            // 
            this.lblTechnicianAvailability.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTechnicianAvailability.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(94)))), ((int)(((byte)(121)))));
            this.lblTechnicianAvailability.Location = new System.Drawing.Point(386, 730);
            this.lblTechnicianAvailability.Name = "lblTechnicianAvailability";
            this.lblTechnicianAvailability.Size = new System.Drawing.Size(340, 38);
            this.lblTechnicianAvailability.TabIndex = 80;
            this.lblTechnicianAvailability.Text = "Choose a promise date to check technician availability.";
            // 
            // cartGrandTotalColumn
            // 
            this.cartGrandTotalColumn.DataPropertyName = "GrandTotal";
            dataGridViewCellStyle1.Format = "C2";
            this.cartGrandTotalColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.cartGrandTotalColumn.HeaderText = "Grand total";
            this.cartGrandTotalColumn.Name = "cartGrandTotalColumn";
            this.cartGrandTotalColumn.ReadOnly = true;
            // 
            // cartServiceColumn
            // 
            this.cartServiceColumn.DataPropertyName = "JobTypeName";
            this.cartServiceColumn.HeaderText = "Service";
            this.cartServiceColumn.Name = "cartServiceColumn";
            this.cartServiceColumn.ReadOnly = true;
            // 
            // cartQuantityColumn
            // 
            this.cartQuantityColumn.DataPropertyName = "Quantity";
            this.cartQuantityColumn.HeaderText = "Quantity";
            this.cartQuantityColumn.Name = "cartQuantityColumn";
            this.cartQuantityColumn.ReadOnly = true;
            // 
            // jobsFilterBar
            // 
            this.jobsFilterBar.Controls.Add(this.comboBox5);
            this.jobsFilterBar.Controls.Add(this.lblJobStatusFilter);
            this.jobsFilterBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.jobsFilterBar.Location = new System.Drawing.Point(0, 0);
            this.jobsFilterBar.Name = "jobsFilterBar";
            this.jobsFilterBar.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.jobsFilterBar.Size = new System.Drawing.Size(1630, 48);
            this.jobsFilterBar.TabIndex = 13;
            // 
            // comboBox5
            // 
            this.comboBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.comboBox5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox5.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.comboBox5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.comboBox5.FormattingEnabled = true;
            this.comboBox5.Items.AddRange(new object[] {
            "Pending",
            "Completed"});
            this.comboBox5.Location = new System.Drawing.Point(130, 10);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(190, 28);
            this.comboBox5.TabIndex = 11;
            this.comboBox5.SelectedIndexChanged += new System.EventHandler(this.comboBox5_SelectedIndexChanged);
            // 
            // lblJobStatusFilter
            // 
            this.lblJobStatusFilter.AutoSize = true;
            this.lblJobStatusFilter.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblJobStatusFilter.Location = new System.Drawing.Point(12, 12);
            this.lblJobStatusFilter.Name = "lblJobStatusFilter";
            this.lblJobStatusFilter.Size = new System.Drawing.Size(74, 20);
            this.lblJobStatusFilter.TabIndex = 12;
            this.lblJobStatusFilter.Text = "Job status";
            // 
            // jobDetailActions
            // 
            this.jobDetailActions.Controls.Add(this.btnUpdate);
            this.jobDetailActions.Controls.Add(this.btnDelJob);
            this.jobDetailActions.Controls.Add(this.btnJobManagement);
            this.jobDetailActions.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.jobDetailActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jobDetailActions.Location = new System.Drawing.Point(3, 881);
            this.jobDetailActions.Name = "jobDetailActions";
            this.jobDetailActions.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.jobDetailActions.Size = new System.Drawing.Size(1630, 54);
            this.jobDetailActions.TabIndex = 1;
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.btnUpdate.Enabled = false;
            this.btnUpdate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnUpdate.ForeColor = System.Drawing.Color.White;
            this.btnUpdate.Location = new System.Drawing.Point(15, 11);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(132, 36);
            this.btnUpdate.TabIndex = 14;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnInvoice_Click);
            // 
            // btnDelJob
            // 
            this.btnDelJob.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(50)))), ((int)(((byte)(66)))));
            this.btnDelJob.Enabled = false;
            this.btnDelJob.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(50)))), ((int)(((byte)(66)))));
            this.btnDelJob.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelJob.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnDelJob.ForeColor = System.Drawing.Color.White;
            this.btnDelJob.Location = new System.Drawing.Point(153, 11);
            this.btnDelJob.Name = "btnDelJob";
            this.btnDelJob.Size = new System.Drawing.Size(132, 36);
            this.btnDelJob.TabIndex = 13;
            this.btnDelJob.Text = "Archive";
            this.btnDelJob.UseVisualStyleBackColor = false;
            this.btnDelJob.Click += new System.EventHandler(this.btnDelJob_Click);
            // 
            // btnJobManagement
            // 
            this.btnJobManagement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnJobManagement.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.btnJobManagement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJobManagement.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnJobManagement.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnJobManagement.Location = new System.Drawing.Point(291, 11);
            this.btnJobManagement.Name = "btnJobManagement";
            this.btnJobManagement.Size = new System.Drawing.Size(132, 36);
            this.btnJobManagement.TabIndex = 80;
            this.btnJobManagement.Text = "Help";
            this.btnJobManagement.UseVisualStyleBackColor = false;
            this.btnJobManagement.Click += new System.EventHandler(this.btnJobManagement_Click);
            // 
            // btnViewInvoicePdf
            // 
            this.btnViewInvoicePdf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnViewInvoicePdf.Enabled = false;
            this.btnViewInvoicePdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewInvoicePdf.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnViewInvoicePdf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnViewInvoicePdf.Location = new System.Drawing.Point(679, 11);
            this.btnViewInvoicePdf.Name = "btnViewInvoicePdf";
            this.btnViewInvoicePdf.Size = new System.Drawing.Size(160, 36);
            this.btnViewInvoicePdf.TabIndex = 4;
            this.btnViewInvoicePdf.Text = "View Invoice PDF";
            this.btnViewInvoicePdf.UseVisualStyleBackColor = false;
            // 
            // btnViewReceiptPdf
            // 
            this.btnViewReceiptPdf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnViewReceiptPdf.Enabled = false;
            this.btnViewReceiptPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewReceiptPdf.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnViewReceiptPdf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnViewReceiptPdf.Location = new System.Drawing.Point(845, 11);
            this.btnViewReceiptPdf.Name = "btnViewReceiptPdf";
            this.btnViewReceiptPdf.Size = new System.Drawing.Size(160, 36);
            this.btnViewReceiptPdf.TabIndex = 5;
            this.btnViewReceiptPdf.Text = "View Receipt PDF";
            this.btnViewReceiptPdf.UseVisualStyleBackColor = false;
            // 
            // jobActionsToolbar
            // 
            this.jobActionsToolbar.AutoScroll = true;
            this.jobActionsToolbar.Controls.Add(this.btnCancelSelectedJob);
            this.jobActionsToolbar.Controls.Add(this.btnCollectCash);
            this.jobActionsToolbar.Controls.Add(this.btnSaveInvoicePdf);
            this.jobActionsToolbar.Controls.Add(this.btnSaveReceiptPdf);
            this.jobActionsToolbar.Controls.Add(this.btnViewInvoicePdf);
            this.jobActionsToolbar.Controls.Add(this.btnViewReceiptPdf);
            this.jobActionsToolbar.Controls.Add(this.btnOpenJobDetails);
            this.jobActionsToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.jobActionsToolbar.Location = new System.Drawing.Point(3, 3);
            this.jobActionsToolbar.Name = "jobActionsToolbar";
            this.jobActionsToolbar.Padding = new System.Windows.Forms.Padding(12, 8, 12, 0);
            this.jobActionsToolbar.Size = new System.Drawing.Size(1636, 52);
            this.jobActionsToolbar.TabIndex = 1;
            this.jobActionsToolbar.WrapContents = false;
            // 
            // btnCancelSelectedJob
            // 
            this.btnCancelSelectedJob.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(50)))), ((int)(((byte)(66)))));
            this.btnCancelSelectedJob.Enabled = false;
            this.btnCancelSelectedJob.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelSelectedJob.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnCancelSelectedJob.ForeColor = System.Drawing.Color.White;
            this.btnCancelSelectedJob.Location = new System.Drawing.Point(15, 11);
            this.btnCancelSelectedJob.Name = "btnCancelSelectedJob";
            this.btnCancelSelectedJob.Size = new System.Drawing.Size(160, 36);
            this.btnCancelSelectedJob.TabIndex = 0;
            this.btnCancelSelectedJob.Text = "Cancel Job";
            this.btnCancelSelectedJob.UseVisualStyleBackColor = false;
            // 
            // btnCollectCash
            // 
            this.btnCollectCash.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnCollectCash.Enabled = false;
            this.btnCollectCash.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollectCash.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnCollectCash.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnCollectCash.Location = new System.Drawing.Point(181, 11);
            this.btnCollectCash.Name = "btnCollectCash";
            this.btnCollectCash.Size = new System.Drawing.Size(160, 36);
            this.btnCollectCash.TabIndex = 1;
            this.btnCollectCash.Text = "Collect Cash";
            this.btnCollectCash.UseVisualStyleBackColor = false;
            // 
            // btnSaveInvoicePdf
            // 
            this.btnSaveInvoicePdf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnSaveInvoicePdf.Enabled = false;
            this.btnSaveInvoicePdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveInvoicePdf.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnSaveInvoicePdf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnSaveInvoicePdf.Location = new System.Drawing.Point(347, 11);
            this.btnSaveInvoicePdf.Name = "btnSaveInvoicePdf";
            this.btnSaveInvoicePdf.Size = new System.Drawing.Size(160, 36);
            this.btnSaveInvoicePdf.TabIndex = 2;
            this.btnSaveInvoicePdf.Text = "Save Invoice PDF";
            this.btnSaveInvoicePdf.UseVisualStyleBackColor = false;
            // 
            // btnSaveReceiptPdf
            // 
            this.btnSaveReceiptPdf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnSaveReceiptPdf.Enabled = false;
            this.btnSaveReceiptPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveReceiptPdf.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnSaveReceiptPdf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnSaveReceiptPdf.Location = new System.Drawing.Point(513, 11);
            this.btnSaveReceiptPdf.Name = "btnSaveReceiptPdf";
            this.btnSaveReceiptPdf.Size = new System.Drawing.Size(160, 36);
            this.btnSaveReceiptPdf.TabIndex = 3;
            this.btnSaveReceiptPdf.Text = "Save Receipt PDF";
            this.btnSaveReceiptPdf.UseVisualStyleBackColor = false;
            // 
            // cmbIdentityDocument
            // 
            this.cmbIdentityDocument.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.cmbIdentityDocument.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIdentityDocument.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbIdentityDocument.Items.AddRange(new object[] {
            "South African ID",
            "Passport"});
            this.cmbIdentityDocument.Location = new System.Drawing.Point(28, 710);
            this.cmbIdentityDocument.Name = "cmbIdentityDocument";
            this.cmbIdentityDocument.Size = new System.Drawing.Size(200, 28);
            this.cmbIdentityDocument.TabIndex = 48;
            // 
            // txtPassportNumber
            // 
            this.txtPassportNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.txtPassportNumber.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPassportNumber.Location = new System.Drawing.Point(264, 710);
            this.txtPassportNumber.MaxLength = 20;
            this.txtPassportNumber.Name = "txtPassportNumber";
            this.txtPassportNumber.Size = new System.Drawing.Size(200, 27);
            this.txtPassportNumber.TabIndex = 49;
            this.txtPassportNumber.Visible = false;
            // 
            // txtPassportCountry
            // 
            this.txtPassportCountry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.txtPassportCountry.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPassportCountry.Location = new System.Drawing.Point(500, 710);
            this.txtPassportCountry.MaxLength = 50;
            this.txtPassportCountry.Name = "txtPassportCountry";
            this.txtPassportCountry.Size = new System.Drawing.Size(200, 27);
            this.txtPassportCountry.TabIndex = 50;
            this.txtPassportCountry.Visible = false;
            // 
            // lblIdentityDocument
            // 
            this.lblIdentityDocument.BackColor = System.Drawing.Color.Transparent;
            this.lblIdentityDocument.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblIdentityDocument.Location = new System.Drawing.Point(28, 680);
            this.lblIdentityDocument.Name = "lblIdentityDocument";
            this.lblIdentityDocument.Size = new System.Drawing.Size(200, 26);
            this.lblIdentityDocument.TabIndex = 51;
            this.lblIdentityDocument.Text = "Identity document";
            // 
            // lblPassportNumber
            // 
            this.lblPassportNumber.BackColor = System.Drawing.Color.Transparent;
            this.lblPassportNumber.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPassportNumber.Location = new System.Drawing.Point(264, 680);
            this.lblPassportNumber.Name = "lblPassportNumber";
            this.lblPassportNumber.Size = new System.Drawing.Size(200, 26);
            this.lblPassportNumber.TabIndex = 52;
            this.lblPassportNumber.Text = "Passport number";
            this.lblPassportNumber.Visible = false;
            // 
            // lblPassportCountry
            // 
            this.lblPassportCountry.BackColor = System.Drawing.Color.Transparent;
            this.lblPassportCountry.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPassportCountry.Location = new System.Drawing.Point(500, 680);
            this.lblPassportCountry.Name = "lblPassportCountry";
            this.lblPassportCountry.Size = new System.Drawing.Size(200, 26);
            this.lblPassportCountry.TabIndex = 53;
            this.lblPassportCountry.Text = "Issuing country";
            this.lblPassportCountry.Visible = false;
            // 
            // dashboardLayout
            // 
            this.dashboardLayout.AutoScroll = true;
            this.dashboardLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.dashboardLayout.ColumnCount = 1;
            this.dashboardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dashboardLayout.Controls.Add(this.lblDashboardTitle, 0, 0);
            this.dashboardLayout.Controls.Add(this.lblAdminGreeting, 0, 1);
            this.dashboardLayout.Controls.Add(this.dashboardCards, 0, 2);
            this.dashboardLayout.Controls.Add(this.gbxRecJobs, 0, 3);
            this.dashboardLayout.Controls.Add(this.groupBox4, 0, 4);
            this.dashboardLayout.Controls.Add(this.dashboardActions, 0, 5);
            this.dashboardLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dashboardLayout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.dashboardLayout.Location = new System.Drawing.Point(0, 0);
            this.dashboardLayout.Name = "dashboardLayout";
            this.dashboardLayout.Padding = new System.Windows.Forms.Padding(24);
            this.dashboardLayout.RowCount = 6;
            this.dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 270F));
            this.dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.dashboardLayout.Size = new System.Drawing.Size(1226, 841);
            this.dashboardLayout.TabIndex = 25;
            // 
            // lblDashboardTitle
            // 
            this.lblDashboardTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblDashboardTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDashboardTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblDashboardTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.lblDashboardTitle.Location = new System.Drawing.Point(27, 24);
            this.lblDashboardTitle.Name = "lblDashboardTitle";
            this.lblDashboardTitle.Size = new System.Drawing.Size(1172, 48);
            this.lblDashboardTitle.TabIndex = 0;
            this.lblDashboardTitle.Text = "Administrator dashboard";
            // 
            // lblAdminGreeting
            // 
            this.lblAdminGreeting.BackColor = System.Drawing.Color.Transparent;
            this.lblAdminGreeting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAdminGreeting.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblAdminGreeting.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.lblAdminGreeting.Location = new System.Drawing.Point(24, 72);
            this.lblAdminGreeting.Margin = new System.Windows.Forms.Padding(0);
            this.lblAdminGreeting.Name = "lblAdminGreeting";
            this.lblAdminGreeting.Size = new System.Drawing.Size(1178, 36);
            this.lblAdminGreeting.TabIndex = 2;
            this.lblAdminGreeting.Text = "Your workspace overview";
            this.lblAdminGreeting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dashboardCards
            // 
            this.dashboardCards.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.dashboardCards.ColumnCount = 3;
            this.dashboardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.dashboardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.dashboardCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.dashboardCards.Controls.Add(this.button1, 0, 0);
            this.dashboardCards.Controls.Add(this.button9, 1, 0);
            this.dashboardCards.Controls.Add(this.button2, 2, 0);
            this.dashboardCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dashboardCards.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.dashboardCards.Location = new System.Drawing.Point(24, 108);
            this.dashboardCards.Margin = new System.Windows.Forms.Padding(0);
            this.dashboardCards.Name = "dashboardCards";
            this.dashboardCards.RowCount = 1;
            this.dashboardCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.dashboardCards.Size = new System.Drawing.Size(1178, 112);
            this.dashboardCards.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(380, 100);
            this.button1.TabIndex = 6;
            this.button1.Text = "Pending\r\n—";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button9
            // 
            this.button9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.button9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button9.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button9.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.button9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.button9.Location = new System.Drawing.Point(392, 0);
            this.button9.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(380, 100);
            this.button9.TabIndex = 5;
            this.button9.Text = "In progress\r\n—";
            this.button9.UseVisualStyleBackColor = false;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.button2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.button2.Location = new System.Drawing.Point(784, 0);
            this.button2.Margin = new System.Windows.Forms.Padding(0, 0, 12, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(382, 100);
            this.button2.TabIndex = 7;
            this.button2.Text = "Completed\r\n—";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // gbxRecJobs
            // 
            this.gbxRecJobs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gbxRecJobs.Controls.Add(this.dataGridView2);
            this.gbxRecJobs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxRecJobs.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.gbxRecJobs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.gbxRecJobs.Location = new System.Drawing.Point(24, 220);
            this.gbxRecJobs.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.gbxRecJobs.Name = "gbxRecJobs";
            this.gbxRecJobs.Padding = new System.Windows.Forms.Padding(12);
            this.gbxRecJobs.Size = new System.Drawing.Size(1178, 258);
            this.gbxRecJobs.TabIndex = 3;
            this.gbxRecJobs.TabStop = false;
            this.gbxRecJobs.Text = "Recent jobs";
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.dataGridView2.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView2.AutoGenerateColumns = false;
            this.dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView2.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.dataGridView2.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(237)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn9,
            this.dataGridViewTextBoxColumn10,
            this.dataGridViewTextBoxColumn11,
            this.dataGridViewTextBoxColumn12,
            this.dataGridViewTextBoxColumn13});
            this.dataGridView2.DataSource = this.jobsBindingSource3;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.EnableHeadersVisualStyles = false;
            this.dataGridView2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dataGridView2.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.dataGridView2.Location = new System.Drawing.Point(12, 32);
            this.dataGridView2.MultiSelect = false;
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.RowTemplate.Height = 28;
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.Size = new System.Drawing.Size(1154, 214);
            this.dataGridView2.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "JobID";
            this.dataGridViewTextBoxColumn1.HeaderText = "Job #";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "CustomerID";
            this.dataGridViewTextBoxColumn2.HeaderText = "Customer ID";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Visible = false;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "StaffID";
            this.dataGridViewTextBoxColumn3.HeaderText = "Staff ID";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Visible = false;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "PromiseDate";
            this.dataGridViewTextBoxColumn4.HeaderText = "Promise Date";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "JobStatus";
            this.dataGridViewTextBoxColumn5.HeaderText = "Status";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "TotalMaterialCost";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "C2";
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn6.HeaderText = "Materials";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.DataPropertyName = "LabourCost";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "C2";
            this.dataGridViewTextBoxColumn7.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn7.HeaderText = "Labour";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.DataPropertyName = "TransportCost";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "C2";
            this.dataGridViewTextBoxColumn8.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewTextBoxColumn8.HeaderText = "Transport";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.DataPropertyName = "TaxAmount";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "C2";
            this.dataGridViewTextBoxColumn9.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewTextBoxColumn9.HeaderText = "Tax Amount";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.DataPropertyName = "GrandTotal";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "C2";
            this.dataGridViewTextBoxColumn10.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewTextBoxColumn10.HeaderText = "Grand Total";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.DataPropertyName = "Notes";
            this.dataGridViewTextBoxColumn11.HeaderText = "Notes";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.DataPropertyName = "CreatedAt";
            this.dataGridViewTextBoxColumn12.HeaderText = "Created At";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            this.dataGridViewTextBoxColumn12.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn13
            // 
            this.dataGridViewTextBoxColumn13.DataPropertyName = "UpdatedAt";
            this.dataGridViewTextBoxColumn13.HeaderText = "Updated At";
            this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            this.dataGridViewTextBoxColumn13.ReadOnly = true;
            // 
            // jobsBindingSource3
            // 
            this.jobsBindingSource3.DataMember = "Jobs";
            this.jobsBindingSource3.DataSource = this.groupWst34DataSet;
            // 
            // groupWst34DataSet
            // 
            this.groupWst34DataSet.DataSetName = "GroupWst34DataSet";
            this.groupWst34DataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.groupBox4.Controls.Add(this.dataGridView3);
            this.groupBox4.Controls.Add(this.lblLowStockEmpty);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.groupBox4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.groupBox4.Location = new System.Drawing.Point(24, 490);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(12);
            this.groupBox4.Size = new System.Drawing.Size(1178, 218);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Low stock - at or below reorder level";
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToAddRows = false;
            this.dataGridView3.AllowUserToDeleteRows = false;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.dataGridView3.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridView3.AutoGenerateColumns = false;
            this.dataGridView3.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView3.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.dataGridView3.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(237)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView3.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn14,
            this.dataGridViewTextBoxColumn15,
            this.dataGridViewTextBoxColumn16,
            this.dataGridViewTextBoxColumn17,
            this.dataGridViewTextBoxColumn18,
            this.dataGridViewTextBoxColumn19,
            this.dataGridViewTextBoxColumn20,
            this.dataGridViewTextBoxColumn21});
            this.dataGridView3.DataSource = this.inventoryItemsBindingSource1;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView3.DefaultCellStyle = dataGridViewCellStyle13;
            this.dataGridView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView3.EnableHeadersVisualStyles = false;
            this.dataGridView3.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dataGridView3.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.dataGridView3.Location = new System.Drawing.Point(12, 32);
            this.dataGridView3.MultiSelect = false;
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.ReadOnly = true;
            this.dataGridView3.RowHeadersVisible = false;
            this.dataGridView3.RowTemplate.Height = 28;
            this.dataGridView3.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView3.Size = new System.Drawing.Size(1154, 146);
            this.dataGridView3.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn14
            // 
            this.dataGridViewTextBoxColumn14.DataPropertyName = "ItemID";
            this.dataGridViewTextBoxColumn14.HeaderText = "Item ID";
            this.dataGridViewTextBoxColumn14.Name = "dataGridViewTextBoxColumn14";
            this.dataGridViewTextBoxColumn14.ReadOnly = true;
            this.dataGridViewTextBoxColumn14.Visible = false;
            // 
            // dataGridViewTextBoxColumn15
            // 
            this.dataGridViewTextBoxColumn15.DataPropertyName = "ItemName";
            this.dataGridViewTextBoxColumn15.HeaderText = "Item Name";
            this.dataGridViewTextBoxColumn15.Name = "dataGridViewTextBoxColumn15";
            this.dataGridViewTextBoxColumn15.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn16
            // 
            this.dataGridViewTextBoxColumn16.DataPropertyName = "Category";
            this.dataGridViewTextBoxColumn16.HeaderText = "Category";
            this.dataGridViewTextBoxColumn16.Name = "dataGridViewTextBoxColumn16";
            this.dataGridViewTextBoxColumn16.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn17
            // 
            this.dataGridViewTextBoxColumn17.DataPropertyName = "QuantityInStock";
            this.dataGridViewTextBoxColumn17.HeaderText = "In stock";
            this.dataGridViewTextBoxColumn17.Name = "dataGridViewTextBoxColumn17";
            this.dataGridViewTextBoxColumn17.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn18
            // 
            this.dataGridViewTextBoxColumn18.DataPropertyName = "UnitPrice";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle12.Format = "C2";
            this.dataGridViewTextBoxColumn18.DefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridViewTextBoxColumn18.HeaderText = "Unit Price";
            this.dataGridViewTextBoxColumn18.Name = "dataGridViewTextBoxColumn18";
            this.dataGridViewTextBoxColumn18.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn19
            // 
            this.dataGridViewTextBoxColumn19.DataPropertyName = "ReorderLevel";
            this.dataGridViewTextBoxColumn19.HeaderText = "Reorder Level";
            this.dataGridViewTextBoxColumn19.Name = "dataGridViewTextBoxColumn19";
            this.dataGridViewTextBoxColumn19.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn20
            // 
            this.dataGridViewTextBoxColumn20.DataPropertyName = "SupplierName";
            this.dataGridViewTextBoxColumn20.HeaderText = "Supplier Name";
            this.dataGridViewTextBoxColumn20.Name = "dataGridViewTextBoxColumn20";
            this.dataGridViewTextBoxColumn20.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn21
            // 
            this.dataGridViewTextBoxColumn21.DataPropertyName = "UpdatedAt";
            this.dataGridViewTextBoxColumn21.HeaderText = "Updated At";
            this.dataGridViewTextBoxColumn21.Name = "dataGridViewTextBoxColumn21";
            this.dataGridViewTextBoxColumn21.ReadOnly = true;
            // 
            // inventoryItemsBindingSource1
            // 
            this.inventoryItemsBindingSource1.DataMember = "InventoryItems";
            this.inventoryItemsBindingSource1.DataSource = this.groupWst34DataSet;
            // 
            // lblLowStockEmpty
            // 
            this.lblLowStockEmpty.BackColor = System.Drawing.Color.Transparent;
            this.lblLowStockEmpty.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblLowStockEmpty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.lblLowStockEmpty.Location = new System.Drawing.Point(12, 178);
            this.lblLowStockEmpty.Name = "lblLowStockEmpty";
            this.lblLowStockEmpty.Size = new System.Drawing.Size(1154, 28);
            this.lblLowStockEmpty.TabIndex = 1;
            this.lblLowStockEmpty.Text = "No items need reordering.";
            this.lblLowStockEmpty.Visible = false;
            // 
            // dashboardActions
            // 
            this.dashboardActions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.dashboardActions.Controls.Add(this.btnDashboardHelp);
            this.dashboardActions.Controls.Add(this.btnDashboardRefresh);
            this.dashboardActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dashboardActions.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.dashboardActions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.dashboardActions.Location = new System.Drawing.Point(27, 723);
            this.dashboardActions.Name = "dashboardActions";
            this.dashboardActions.Size = new System.Drawing.Size(1172, 91);
            this.dashboardActions.TabIndex = 20;
            this.dashboardActions.WrapContents = false;
            // 
            // btnDashboardHelp
            // 
            this.btnDashboardHelp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnDashboardHelp.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.btnDashboardHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboardHelp.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnDashboardHelp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnDashboardHelp.Location = new System.Drawing.Point(1060, 0);
            this.btnDashboardHelp.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btnDashboardHelp.Name = "btnDashboardHelp";
            this.btnDashboardHelp.Size = new System.Drawing.Size(112, 36);
            this.btnDashboardHelp.TabIndex = 0;
            this.btnDashboardHelp.Text = "Help";
            this.btnDashboardHelp.UseVisualStyleBackColor = false;
            this.btnDashboardHelp.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // btnDashboardRefresh
            // 
            this.btnDashboardRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnDashboardRefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.btnDashboardRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboardRefresh.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnDashboardRefresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnDashboardRefresh.Location = new System.Drawing.Point(940, 0);
            this.btnDashboardRefresh.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btnDashboardRefresh.Name = "btnDashboardRefresh";
            this.btnDashboardRefresh.Size = new System.Drawing.Size(112, 36);
            this.btnDashboardRefresh.TabIndex = 1;
            this.btnDashboardRefresh.Text = "Refresh";
            this.btnDashboardRefresh.UseVisualStyleBackColor = false;
            this.btnDashboardRefresh.Click += new System.EventHandler(this.DashboardRefresh_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1240, 880);
            this.panel1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabJobDetails);
            this.CustomersWorkspacePage = new System.Windows.Forms.TabPage();
            this.CustomersLayout = new Major_Project.UI.Forms.Common.CustomerDirectoryView();
            this.CustomersWorkspacePage.Name = "CustomersWorkspacePage";
            this.CustomersWorkspacePage.Text = "Customers";
            this.CustomersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CustomersWorkspacePage.Padding = new System.Windows.Forms.Padding(16);
            this.CustomersWorkspacePage.Controls.Add(this.CustomersLayout);
            this.tabControl1.Controls.Add(this.CustomersWorkspacePage);
            this.StockWorkspacePage = new System.Windows.Forms.TabPage();
            this.StockLayout = new Major_Project.UI.Forms.Common.StockDirectoryView();
            this.StockWorkspacePage.Name = "StockWorkspacePage";
            this.StockWorkspacePage.Text = "Inventory";
            this.StockLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StockWorkspacePage.Padding = new System.Windows.Forms.Padding(16);
            this.StockWorkspacePage.Controls.Add(this.StockLayout);
            this.tabControl1.Controls.Add(this.StockWorkspacePage);
            this.JobChanges = new System.Windows.Forms.TabPage();
            this.JobChangesLayout = new Major_Project.UI.Forms.Common.JobChangesView();
            this.JobChanges.Name = "JobChanges";
            this.JobChanges.Text = "Rescheduling requests";
            this.JobChangesLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobChanges.Padding = new System.Windows.Forms.Padding(16);
            this.JobChanges.Controls.Add(this.JobChangesLayout);
            this.tabControl1.Controls.Add(this.JobChanges);
            this.ProfileWorkspacePage = new System.Windows.Forms.TabPage();
            this.ProfileLayout = new Major_Project.UI.Forms.Common.StaffProfileView();
            this.ProfileWorkspacePage.Name = "ProfileWorkspacePage";
            this.ProfileWorkspacePage.Text = "My Profile";
            this.ProfileLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProfileWorkspacePage.Padding = new System.Windows.Forms.Padding(16);
            this.ProfileWorkspacePage.Controls.Add(this.ProfileLayout);
            this.tabControl1.Controls.Add(this.ProfileWorkspacePage);
            this.AssistantWorkspacePage = new System.Windows.Forms.TabPage();
            this.AssistantLayout = new Major_Project.UI.Forms.Common.AssistantView();
            this.AssistantWorkspacePage.Name = "AssistantWorkspacePage";
            this.AssistantWorkspacePage.Text = "Assistant";
            this.AssistantLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AssistantWorkspacePage.Padding = new System.Windows.Forms.Padding(16);
            this.AssistantWorkspacePage.Controls.Add(this.AssistantLayout);
            this.tabControl1.Controls.Add(this.AssistantWorkspacePage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1240, 880);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1232, 847);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Dashboard";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.panel2.Controls.Add(this.label31);
            this.panel2.Controls.Add(this.pictureBox2);
            this.panel2.Controls.Add(this.gbxQuickAct);
            this.panel2.Controls.Add(this.dashboardLayout);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1226, 841);
            this.panel2.TabIndex = 1;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.BackColor = System.Drawing.Color.Transparent;
            this.label31.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label31.Location = new System.Drawing.Point(1000, 755);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(116, 20);
            this.label31.TabIndex = 24;
            this.label31.Text = "System Manual";
            this.label31.Visible = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(1003, 774);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(109, 50);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 21;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // gbxQuickAct
            // 
            this.gbxQuickAct.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gbxQuickAct.Controls.Add(this.pictureBox1);
            this.gbxQuickAct.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxQuickAct.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.gbxQuickAct.Location = new System.Drawing.Point(977, 80);
            this.gbxQuickAct.Name = "gbxQuickAct";
            this.gbxQuickAct.Size = new System.Drawing.Size(624, 654);
            this.gbxQuickAct.TabIndex = 4;
            this.gbxQuickAct.TabStop = false;
            this.gbxQuickAct.Text = "Quick Stats";
            this.gbxQuickAct.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.pictureBox1.Image = global::Major_Project.Properties.Resources.THY_WII_electrical_and_construction_logo;
            this.pictureBox1.Location = new System.Drawing.Point(88, 268);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(482, 320);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.AutoScrollMinSize = new System.Drawing.Size(1642, 1003);
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1642, 996);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Add New Job";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.83554F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.16446F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox5, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1003F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1642, 1003);
            this.tableLayoutPanel1.TabIndex = 22;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.groupBox3.Controls.Add(this.btnHelpJobs);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Controls.Add(this.button5);
            this.groupBox3.Controls.Add(this.comboBox2);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.numericUpDown3);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.dateTimePicker3);
            this.groupBox3.Controls.Add(this.comboBox4);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Controls.Add(this.dataGridViewCart);
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.textBox5);
            this.groupBox3.Controls.Add(this.button10);
            this.groupBox3.Controls.Add(this.button6);
            this.groupBox3.Controls.Add(this.lblTechnicianAvailability);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.groupBox3.Location = new System.Drawing.Point(821, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(818, 997);
            this.groupBox3.TabIndex = 26;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Book a job";
            // 
            // btnHelpJobs
            // 
            this.btnHelpJobs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnHelpJobs.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.btnHelpJobs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelpJobs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnHelpJobs.Location = new System.Drawing.Point(727, 25);
            this.btnHelpJobs.Name = "btnHelpJobs";
            this.btnHelpJobs.Size = new System.Drawing.Size(75, 38);
            this.btnHelpJobs.TabIndex = 79;
            this.btnHelpJobs.Text = "Help";
            this.btnHelpJobs.UseVisualStyleBackColor = false;
            this.btnHelpJobs.Click += new System.EventHandler(this.btnHelpJobs_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label3.Location = new System.Drawing.Point(28, 532);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 21);
            this.label3.TabIndex = 78;
            this.label3.Text = "Describe the work";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.textBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.textBox1.Location = new System.Drawing.Point(28, 562);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(700, 70);
            this.textBox1.TabIndex = 77;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.button5.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Location = new System.Drawing.Point(28, 124);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(200, 44);
            this.button5.TabIndex = 76;
            this.button5.Text = "Add Service";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.comboBox2.DataSource = this.jobTypesBindingSource;
            this.comboBox2.DisplayMember = "JobTypeName";
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(28, 72);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(480, 29);
            this.comboBox2.TabIndex = 74;
            this.comboBox2.ValueMember = "JobTypeID";
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // jobTypesBindingSource
            // 
            this.jobTypesBindingSource.DataMember = "JobTypes";
            this.jobTypesBindingSource.DataSource = this.groupWst34DataSet;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label4.Location = new System.Drawing.Point(28, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 20);
            this.label4.TabIndex = 75;
            this.label4.Text = "Select a service";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label9.Location = new System.Drawing.Point(540, 42);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 21);
            this.label9.TabIndex = 73;
            this.label9.Text = "Quantity";
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.numericUpDown3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.numericUpDown3.Location = new System.Drawing.Point(540, 72);
            this.numericUpDown3.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(160, 29);
            this.numericUpDown3.TabIndex = 72;
            this.numericUpDown3.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown3.ValueChanged += new System.EventHandler(this.numericUpDown3_ValueChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.Transparent;
            this.label17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label17.Location = new System.Drawing.Point(28, 666);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(112, 21);
            this.label17.TabIndex = 71;
            this.label17.Text = "Promise Date";
            // 
            // dateTimePicker3
            // 
            this.dateTimePicker3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.dateTimePicker3.Location = new System.Drawing.Point(28, 696);
            this.dateTimePicker3.Name = "dateTimePicker3";
            this.dateTimePicker3.Size = new System.Drawing.Size(320, 29);
            this.dateTimePicker3.TabIndex = 70;
            this.dateTimePicker3.ValueChanged += new System.EventHandler(this.dateTimePicker3_ValueChanged);
            // 
            // comboBox4
            // 
            this.comboBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.comboBox4.DisplayMember = "FullName";
            this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Location = new System.Drawing.Point(386, 696);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(320, 29);
            this.comboBox4.TabIndex = 68;
            this.comboBox4.ValueMember = "StaffID";
            this.comboBox4.SelectedIndexChanged += new System.EventHandler(this.comboBox4_SelectedIndexChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.BackColor = System.Drawing.Color.Transparent;
            this.label19.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label19.Location = new System.Drawing.Point(386, 666);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(125, 20);
            this.label19.TabIndex = 69;
            this.label19.Text = "Assign Technician";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label5.Location = new System.Drawing.Point(28, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 21);
            this.label5.TabIndex = 64;
            this.label5.Text = "Your booking";
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(50)))), ((int)(((byte)(66)))));
            this.button4.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(50)))), ((int)(((byte)(66)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(28, 460);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(200, 40);
            this.button4.TabIndex = 63;
            this.button4.Text = "Remove service";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // dataGridViewCart
            // 
            this.dataGridViewCart.AllowUserToAddRows = false;
            this.dataGridViewCart.AllowUserToDeleteRows = false;
            this.dataGridViewCart.AllowUserToOrderColumns = true;
            dataGridViewCellStyle14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.dataGridViewCart.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle14;
            this.dataGridViewCart.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewCart.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.dataGridViewCart.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(237)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewCart.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.dataGridViewCart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCart.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cartServiceColumn,
            this.cartQuantityColumn,
            this.cartGrandTotalColumn});
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewCart.DefaultCellStyle = dataGridViewCellStyle16;
            this.dataGridViewCart.EnableHeadersVisualStyles = false;
            this.dataGridViewCart.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dataGridViewCart.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.dataGridViewCart.Location = new System.Drawing.Point(28, 230);
            this.dataGridViewCart.MultiSelect = false;
            this.dataGridViewCart.Name = "dataGridViewCart";
            this.dataGridViewCart.ReadOnly = true;
            this.dataGridViewCart.RowHeadersVisible = false;
            this.dataGridViewCart.RowHeadersWidth = 51;
            this.dataGridViewCart.RowTemplate.Height = 28;
            this.dataGridViewCart.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewCart.Size = new System.Drawing.Size(700, 210);
            this.dataGridViewCart.TabIndex = 62;
            this.dataGridViewCart.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCart_CellContentClick);
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.Transparent;
            this.label21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label21.Location = new System.Drawing.Point(310, 462);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(150, 28);
            this.label21.TabIndex = 40;
            this.label21.Text = "Grand total";
            // 
            // textBox5
            // 
            this.textBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.textBox5.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.textBox5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.textBox5.Location = new System.Drawing.Point(470, 460);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(258, 27);
            this.textBox5.TabIndex = 39;
            this.textBox5.Text = "R 0.00";
            this.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox5.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.button10.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.button10.Location = new System.Drawing.Point(248, 790);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(140, 44);
            this.button10.TabIndex = 26;
            this.button10.Text = "Cancel";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.button6.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.ForeColor = System.Drawing.Color.White;
            this.button6.Location = new System.Drawing.Point(28, 790);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(200, 44);
            this.button6.TabIndex = 22;
            this.button6.Text = "Save booking";
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.groupBox5.Controls.Add(this.lblCompanyRegNo);
            this.groupBox5.Controls.Add(this.lblSA_ID);
            this.groupBox5.Controls.Add(this.txtSouthAfricanIDNumber);
            this.groupBox5.Controls.Add(this.txtCompanyRegistrationNumber);
            this.groupBox5.Controls.Add(this.btnUpdateProfile);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.cmbCustomerType);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.textBox3);
            this.groupBox5.Controls.Add(this.dataGridView6);
            this.groupBox5.Controls.Add(this.btnSaveProfile);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.cmbSevArea);
            this.groupBox5.Controls.Add(this.txtEmail);
            this.groupBox5.Controls.Add(this.txtPhone);
            this.groupBox5.Controls.Add(this.txtFAddress);
            this.groupBox5.Controls.Add(this.txtCustName);
            this.groupBox5.Controls.Add(this.cmbIdentityDocument);
            this.groupBox5.Controls.Add(this.txtPassportNumber);
            this.groupBox5.Controls.Add(this.txtPassportCountry);
            this.groupBox5.Controls.Add(this.lblIdentityDocument);
            this.groupBox5.Controls.Add(this.lblPassportNumber);
            this.groupBox5.Controls.Add(this.lblPassportCountry);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.groupBox5.Location = new System.Drawing.Point(3, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(812, 997);
            this.groupBox5.TabIndex = 25;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Customer registration";
            this.groupBox5.Enter += new System.EventHandler(this.groupBox5_Enter);
            // 
            // lblCompanyRegNo
            // 
            this.lblCompanyRegNo.AutoSize = true;
            this.lblCompanyRegNo.BackColor = System.Drawing.Color.Transparent;
            this.lblCompanyRegNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.lblCompanyRegNo.Location = new System.Drawing.Point(500, 680);
            this.lblCompanyRegNo.Name = "lblCompanyRegNo";
            this.lblCompanyRegNo.Size = new System.Drawing.Size(211, 21);
            this.lblCompanyRegNo.TabIndex = 47;
            this.lblCompanyRegNo.Text = "Company Registration No.";
            // 
            // lblSA_ID
            // 
            this.lblSA_ID.AutoSize = true;
            this.lblSA_ID.BackColor = System.Drawing.Color.Transparent;
            this.lblSA_ID.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblSA_ID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.lblSA_ID.Location = new System.Drawing.Point(264, 680);
            this.lblSA_ID.Name = "lblSA_ID";
            this.lblSA_ID.Size = new System.Drawing.Size(117, 20);
            this.lblSA_ID.TabIndex = 46;
            this.lblSA_ID.Text = "South African ID";
            // 
            // txtSouthAfricanIDNumber
            // 
            this.txtSouthAfricanIDNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.txtSouthAfricanIDNumber.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSouthAfricanIDNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.txtSouthAfricanIDNumber.Location = new System.Drawing.Point(264, 710);
            this.txtSouthAfricanIDNumber.MaxLength = 13;
            this.txtSouthAfricanIDNumber.Name = "txtSouthAfricanIDNumber";
            this.txtSouthAfricanIDNumber.Size = new System.Drawing.Size(200, 27);
            this.txtSouthAfricanIDNumber.TabIndex = 45;
            this.txtSouthAfricanIDNumber.TextChanged += new System.EventHandler(this.textBox6_TextChanged_1);
            // 
            // txtCompanyRegistrationNumber
            // 
            this.txtCompanyRegistrationNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.txtCompanyRegistrationNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.txtCompanyRegistrationNumber.Location = new System.Drawing.Point(500, 710);
            this.txtCompanyRegistrationNumber.Name = "txtCompanyRegistrationNumber";
            this.txtCompanyRegistrationNumber.Size = new System.Drawing.Size(228, 29);
            this.txtCompanyRegistrationNumber.TabIndex = 44;
            this.txtCompanyRegistrationNumber.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // btnUpdateProfile
            // 
            this.btnUpdateProfile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.btnUpdateProfile.Enabled = false;
            this.btnUpdateProfile.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.btnUpdateProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateProfile.ForeColor = System.Drawing.Color.White;
            this.btnUpdateProfile.Location = new System.Drawing.Point(248, 790);
            this.btnUpdateProfile.Name = "btnUpdateProfile";
            this.btnUpdateProfile.Size = new System.Drawing.Size(200, 44);
            this.btnUpdateProfile.TabIndex = 43;
            this.btnUpdateProfile.Text = "Update Profile";
            this.btnUpdateProfile.UseVisualStyleBackColor = false;
            this.btnUpdateProfile.Click += new System.EventHandler(this.btnUpdateProfile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label1.Location = new System.Drawing.Point(28, 594);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 21);
            this.label1.TabIndex = 42;
            this.label1.Text = "Customer Type";
            // 
            // cmbCustomerType
            // 
            this.cmbCustomerType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.cmbCustomerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomerType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.cmbCustomerType.FormattingEnabled = true;
            this.cmbCustomerType.Items.AddRange(new object[] {
            "Residential",
            "Business"});
            this.cmbCustomerType.Location = new System.Drawing.Point(28, 624);
            this.cmbCustomerType.Name = "cmbCustomerType";
            this.cmbCustomerType.Size = new System.Drawing.Size(320, 29);
            this.cmbCustomerType.TabIndex = 41;
            this.cmbCustomerType.SelectedIndexChanged += new System.EventHandler(this.cmbCustomerType_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.Transparent;
            this.label16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label16.Location = new System.Drawing.Point(28, 42);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(219, 21);
            this.label16.TabIndex = 40;
            this.label16.Text = "Search By Customer  Name:";
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.textBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.textBox3.Location = new System.Drawing.Point(28, 72);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(700, 29);
            this.textBox3.TabIndex = 39;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // dataGridView6
            // 
            this.dataGridView6.AllowUserToAddRows = false;
            this.dataGridView6.AllowUserToDeleteRows = false;
            dataGridViewCellStyle17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.dataGridView6.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle17;
            this.dataGridView6.AutoGenerateColumns = false;
            this.dataGridView6.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView6.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView6.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.dataGridView6.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(237)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView6.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle18;
            this.dataGridView6.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView6.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CustomerID,
            this.FullName,
            this.PhoneNumber,
            this.EmailAddress,
            this.FullAddress,
            this.Area});
            this.dataGridView6.DataSource = this.customersBindingSource;
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle19.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle19.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle19.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            dataGridViewCellStyle19.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView6.DefaultCellStyle = dataGridViewCellStyle19;
            this.dataGridView6.EnableHeadersVisualStyles = false;
            this.dataGridView6.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dataGridView6.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.dataGridView6.Location = new System.Drawing.Point(28, 122);
            this.dataGridView6.MultiSelect = false;
            this.dataGridView6.Name = "dataGridView6";
            this.dataGridView6.ReadOnly = true;
            this.dataGridView6.RowHeadersVisible = false;
            this.dataGridView6.RowTemplate.Height = 28;
            this.dataGridView6.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView6.Size = new System.Drawing.Size(700, 180);
            this.dataGridView6.TabIndex = 38;
            this.dataGridView6.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView6_CellClick);
            this.dataGridView6.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView6_CellContentClick);
            this.dataGridView6.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView6_RowHeaderMouseDoubleClick);
            // 
            // CustomerID
            // 
            this.CustomerID.DataPropertyName = "CustomerID";
            this.CustomerID.HeaderText = "Customer ID";
            this.CustomerID.Name = "CustomerID";
            this.CustomerID.ReadOnly = true;
            this.CustomerID.Visible = false;
            // 
            // FullName
            // 
            this.FullName.DataPropertyName = "FullName";
            this.FullName.HeaderText = "Full Name";
            this.FullName.Name = "FullName";
            this.FullName.ReadOnly = true;
            // 
            // PhoneNumber
            // 
            this.PhoneNumber.DataPropertyName = "PhoneNumber";
            this.PhoneNumber.HeaderText = "Phone Number";
            this.PhoneNumber.Name = "PhoneNumber";
            this.PhoneNumber.ReadOnly = true;
            // 
            // EmailAddress
            // 
            this.EmailAddress.DataPropertyName = "EmailAddress";
            this.EmailAddress.HeaderText = "Email Address";
            this.EmailAddress.Name = "EmailAddress";
            this.EmailAddress.ReadOnly = true;
            // 
            // FullAddress
            // 
            this.FullAddress.DataPropertyName = "FullAddress";
            this.FullAddress.HeaderText = "Full Address";
            this.FullAddress.Name = "FullAddress";
            this.FullAddress.ReadOnly = true;
            // 
            // Area
            // 
            this.Area.DataPropertyName = "Area";
            this.Area.HeaderText = "Area";
            this.Area.Name = "Area";
            this.Area.ReadOnly = true;
            // 
            // customersBindingSource
            // 
            this.customersBindingSource.DataMember = "Customers";
            this.customersBindingSource.DataSource = this.groupWst34DataSet;
            // 
            // btnSaveProfile
            // 
            this.btnSaveProfile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.btnSaveProfile.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.btnSaveProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveProfile.ForeColor = System.Drawing.Color.White;
            this.btnSaveProfile.Location = new System.Drawing.Point(28, 790);
            this.btnSaveProfile.Name = "btnSaveProfile";
            this.btnSaveProfile.Size = new System.Drawing.Size(200, 44);
            this.btnSaveProfile.TabIndex = 37;
            this.btnSaveProfile.Text = "Save Profile";
            this.btnSaveProfile.UseVisualStyleBackColor = false;
            this.btnSaveProfile.Click += new System.EventHandler(this.btnSaveProfile_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label11.Location = new System.Drawing.Point(28, 494);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(106, 21);
            this.label11.TabIndex = 22;
            this.label11.Text = "Full Address:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label12.Location = new System.Drawing.Point(386, 326);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(126, 21);
            this.label12.TabIndex = 21;
            this.label12.Text = "Phone Number";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label13.Location = new System.Drawing.Point(28, 410);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(194, 21);
            this.label13.TabIndex = 20;
            this.label13.Text = "Customer Email Address";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label14.Location = new System.Drawing.Point(386, 410);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(105, 21);
            this.label14.TabIndex = 19;
            this.label14.Text = "Service Area";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.Transparent;
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label15.Location = new System.Drawing.Point(28, 326);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(136, 21);
            this.label15.TabIndex = 18;
            this.label15.Text = "Enter Full Name:";
            // 
            // cmbSevArea
            // 
            this.cmbSevArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.cmbSevArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSevArea.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.cmbSevArea.FormattingEnabled = true;
            this.cmbSevArea.Items.AddRange(new object[] {
            "Umhlanga",
            "Bearea",
            "Morningside",
            "KwaMashu",
            "Durban North",
            "PineTown"});
            this.cmbSevArea.Location = new System.Drawing.Point(386, 440);
            this.cmbSevArea.Name = "cmbSevArea";
            this.cmbSevArea.Size = new System.Drawing.Size(320, 29);
            this.cmbSevArea.TabIndex = 17;
            this.cmbSevArea.SelectedIndexChanged += new System.EventHandler(this.cmbSevArea_SelectedIndexChanged);
            // 
            // txtEmail
            // 
            this.txtEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.txtEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.txtEmail.Location = new System.Drawing.Point(28, 440);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(320, 29);
            this.txtEmail.TabIndex = 16;
            this.txtEmail.TextChanged += new System.EventHandler(this.txtEmail_TextChanged);
            // 
            // txtPhone
            // 
            this.txtPhone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.txtPhone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.txtPhone.Location = new System.Drawing.Point(386, 356);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(320, 29);
            this.txtPhone.TabIndex = 15;
            this.txtPhone.TextChanged += new System.EventHandler(this.txtPhone_TextChanged);
            // 
            // txtFAddress
            // 
            this.txtFAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.txtFAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.txtFAddress.Location = new System.Drawing.Point(28, 524);
            this.txtFAddress.Multiline = true;
            this.txtFAddress.Name = "txtFAddress";
            this.txtFAddress.Size = new System.Drawing.Size(700, 46);
            this.txtFAddress.TabIndex = 14;
            this.txtFAddress.TextChanged += new System.EventHandler(this.txtFAddress_TextChanged);
            // 
            // txtCustName
            // 
            this.txtCustName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.txtCustName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.txtCustName.Location = new System.Drawing.Point(28, 356);
            this.txtCustName.Name = "txtCustName";
            this.txtCustName.Size = new System.Drawing.Size(320, 29);
            this.txtCustName.TabIndex = 13;
            this.txtCustName.TextChanged += new System.EventHandler(this.txtCustName_TextChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.tabPage3.Controls.Add(this.tableLayoutPanel2);
            this.tabPage3.Controls.Add(this.jobActionsToolbar);
            this.tabPage3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1642, 996);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Job Management";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.panel5, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.jobDetailActions, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 55);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1636, 938);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.panel5.Controls.Add(this.groupBox7);
            this.panel5.Controls.Add(this.label23);
            this.panel5.Controls.Add(this.jobsFilterBar);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.panel5.Location = new System.Drawing.Point(3, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1630, 872);
            this.panel5.TabIndex = 0;
            this.panel5.Paint += new System.Windows.Forms.PaintEventHandler(this.panel5_Paint);
            // 
            // groupBox7
            // 
            this.groupBox7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.groupBox7.Controls.Add(this.dgvJobs);
            this.groupBox7.Controls.Add(this.label24);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.groupBox7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.groupBox7.Location = new System.Drawing.Point(0, 48);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(12);
            this.groupBox7.Size = new System.Drawing.Size(1630, 824);
            this.groupBox7.TabIndex = 12;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Job List";
            // 
            // dgvJobs
            // 
            this.dgvJobs.AllowUserToAddRows = false;
            this.dgvJobs.AllowUserToDeleteRows = false;
            this.dgvJobs.AllowUserToOrderColumns = true;
            dataGridViewCellStyle20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.dgvJobs.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle20;
            this.dgvJobs.AutoGenerateColumns = false;
            this.dgvJobs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvJobs.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.dgvJobs.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(237)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle21.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle21.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvJobs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle21;
            this.dgvJobs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.jobsPaymentStatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jobsPaymentStatusColumn.Name = "jobsPaymentStatusColumn";
            this.jobsPaymentStatusColumn.DataPropertyName = "PaymentStatus";
            this.jobsPaymentStatusColumn.HeaderText = "Payment Status";
            this.jobsPaymentStatusColumn.ReadOnly = true;
            this.dgvJobs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.jobsPaymentStatusColumn,
            this.dataGridViewTextBoxColumn22,
            this.dataGridViewTextBoxColumn23,
            this.dataGridViewTextBoxColumn24,
            this.dataGridViewTextBoxColumn25,
            this.dataGridViewTextBoxColumn26,
            this.dataGridViewTextBoxColumn27,
            this.dataGridViewTextBoxColumn28,
            this.dataGridViewTextBoxColumn29,
            this.dataGridViewTextBoxColumn30,
            this.dataGridViewTextBoxColumn31,
            this.dataGridViewTextBoxColumn32,
            this.dataGridViewTextBoxColumn33,
            this.dataGridViewTextBoxColumn34});
            this.dgvJobs.DataSource = this.jobsBindingSource3;
            dataGridViewCellStyle27.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle27.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle27.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle27.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle27.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            dataGridViewCellStyle27.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle27.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvJobs.DefaultCellStyle = dataGridViewCellStyle27;
            this.dgvJobs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvJobs.EnableHeadersVisualStyles = false;
            this.dgvJobs.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dgvJobs.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.dgvJobs.Location = new System.Drawing.Point(12, 32);
            this.dgvJobs.MultiSelect = false;
            this.dgvJobs.Name = "dgvJobs";
            this.dgvJobs.ReadOnly = true;
            this.dgvJobs.RowHeadersVisible = false;
            this.dgvJobs.RowTemplate.Height = 28;
            this.dgvJobs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvJobs.Size = new System.Drawing.Size(1606, 780);
            this.dgvJobs.TabIndex = 15;
            this.dgvJobs.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvJobs_CellContentClick);
            this.dgvJobs.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Jobs_DoubleClick);
            this.dgvJobs.SelectionChanged += new System.EventHandler(this.dgvJobs_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn22
            // 
            this.dataGridViewTextBoxColumn22.DataPropertyName = "JobID";
            this.dataGridViewTextBoxColumn22.HeaderText = "Job #";
            this.dataGridViewTextBoxColumn22.Name = "dataGridViewTextBoxColumn22";
            this.dataGridViewTextBoxColumn22.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn23
            // 
            this.dataGridViewTextBoxColumn23.DataPropertyName = "CustomerID";
            this.dataGridViewTextBoxColumn23.HeaderText = "Customer ID";
            this.dataGridViewTextBoxColumn23.Name = "dataGridViewTextBoxColumn23";
            this.dataGridViewTextBoxColumn23.ReadOnly = true;
            this.dataGridViewTextBoxColumn23.Visible = false;
            // 
            // dataGridViewTextBoxColumn24
            // 
            this.dataGridViewTextBoxColumn24.DataPropertyName = "StaffID";
            this.dataGridViewTextBoxColumn24.HeaderText = "Staff ID";
            this.dataGridViewTextBoxColumn24.Name = "dataGridViewTextBoxColumn24";
            this.dataGridViewTextBoxColumn24.ReadOnly = true;
            this.dataGridViewTextBoxColumn24.Visible = false;
            // 
            // dataGridViewTextBoxColumn25
            // 
            this.dataGridViewTextBoxColumn25.DataPropertyName = "PromiseDate";
            this.dataGridViewTextBoxColumn25.HeaderText = "Promise Date";
            this.dataGridViewTextBoxColumn25.Name = "dataGridViewTextBoxColumn25";
            this.dataGridViewTextBoxColumn25.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn26
            // 
            this.dataGridViewTextBoxColumn26.DataPropertyName = "JobStatus";
            this.dataGridViewTextBoxColumn26.HeaderText = "Status";
            this.dataGridViewTextBoxColumn26.Name = "dataGridViewTextBoxColumn26";
            this.dataGridViewTextBoxColumn26.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn27
            // 
            this.dataGridViewTextBoxColumn27.DataPropertyName = "TotalMaterialCost";
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle22.Format = "C2";
            this.dataGridViewTextBoxColumn27.DefaultCellStyle = dataGridViewCellStyle22;
            this.dataGridViewTextBoxColumn27.HeaderText = "Materials";
            this.dataGridViewTextBoxColumn27.Name = "dataGridViewTextBoxColumn27";
            this.dataGridViewTextBoxColumn27.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn28
            // 
            this.dataGridViewTextBoxColumn28.DataPropertyName = "LabourCost";
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle23.Format = "C2";
            this.dataGridViewTextBoxColumn28.DefaultCellStyle = dataGridViewCellStyle23;
            this.dataGridViewTextBoxColumn28.HeaderText = "Labour";
            this.dataGridViewTextBoxColumn28.Name = "dataGridViewTextBoxColumn28";
            this.dataGridViewTextBoxColumn28.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn29
            // 
            this.dataGridViewTextBoxColumn29.DataPropertyName = "TransportCost";
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle24.Format = "C2";
            this.dataGridViewTextBoxColumn29.DefaultCellStyle = dataGridViewCellStyle24;
            this.dataGridViewTextBoxColumn29.HeaderText = "Transport";
            this.dataGridViewTextBoxColumn29.Name = "dataGridViewTextBoxColumn29";
            this.dataGridViewTextBoxColumn29.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn30
            // 
            this.dataGridViewTextBoxColumn30.DataPropertyName = "TaxAmount";
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle25.Format = "C2";
            this.dataGridViewTextBoxColumn30.DefaultCellStyle = dataGridViewCellStyle25;
            this.dataGridViewTextBoxColumn30.HeaderText = "Tax Amount";
            this.dataGridViewTextBoxColumn30.Name = "dataGridViewTextBoxColumn30";
            this.dataGridViewTextBoxColumn30.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn31
            // 
            this.dataGridViewTextBoxColumn31.DataPropertyName = "GrandTotal";
            dataGridViewCellStyle26.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle26.Format = "C2";
            this.dataGridViewTextBoxColumn31.DefaultCellStyle = dataGridViewCellStyle26;
            this.dataGridViewTextBoxColumn31.HeaderText = "Grand Total";
            this.dataGridViewTextBoxColumn31.Name = "dataGridViewTextBoxColumn31";
            this.dataGridViewTextBoxColumn31.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn32
            // 
            this.dataGridViewTextBoxColumn32.DataPropertyName = "Notes";
            this.dataGridViewTextBoxColumn32.HeaderText = "Notes";
            this.dataGridViewTextBoxColumn32.Name = "dataGridViewTextBoxColumn32";
            this.dataGridViewTextBoxColumn32.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn33
            // 
            this.dataGridViewTextBoxColumn33.DataPropertyName = "CreatedAt";
            this.dataGridViewTextBoxColumn33.HeaderText = "Created At";
            this.dataGridViewTextBoxColumn33.Name = "dataGridViewTextBoxColumn33";
            this.dataGridViewTextBoxColumn33.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn34
            // 
            this.dataGridViewTextBoxColumn34.DataPropertyName = "UpdatedAt";
            this.dataGridViewTextBoxColumn34.HeaderText = "Updated At";
            this.dataGridViewTextBoxColumn34.Name = "dataGridViewTextBoxColumn34";
            this.dataGridViewTextBoxColumn34.ReadOnly = true;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.BackColor = System.Drawing.Color.Transparent;
            this.label24.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label24.Location = new System.Drawing.Point(16, 29);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(0, 20);
            this.label24.TabIndex = 0;
            this.label24.Visible = false;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.BackColor = System.Drawing.Color.Transparent;
            this.label23.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label23.Location = new System.Drawing.Point(15, 36);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(131, 20);
            this.label23.TabIndex = 12;
            this.label23.Text = "Filter By Job status";
            this.label23.Visible = false;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.BackColor = System.Drawing.Color.Transparent;
            this.label22.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label22.Location = new System.Drawing.Point(1400, 747);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(84, 24);
            this.label22.TabIndex = 25;
            this.label22.Text = "Log Out";
            this.label22.Visible = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.button3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.button3.Location = new System.Drawing.Point(1395, 774);
            this.button3.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(112, 36);
            this.button3.TabIndex = 8;
            this.button3.Text = "Sign Out";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.button7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button7.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.button7.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.button7.Location = new System.Drawing.Point(16, 699);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(172, 77);
            this.button7.TabIndex = 15;
            this.button7.Text = "Sign Out";
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Visible = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.panel6.Location = new System.Drawing.Point(821, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(812, 789);
            this.panel6.TabIndex = 1;
            // 
            // jobsBindingSource4
            // 
            this.jobsBindingSource4.DataMember = "Jobs";
            this.jobsBindingSource4.DataSource = this.groupWst34DataSet;
            // 
            // inventoryItemsBindingSource
            // 
            this.inventoryItemsBindingSource.DataMember = "InventoryItems";
            // 
            // jobsBindingSource
            // 
            this.jobsBindingSource.DataMember = "Jobs";
            // 
            // jobsTableAdapter
            // 
            this.jobsTableAdapter.ClearBeforeFill = true;
            // 
            // inventoryItemsTableAdapter
            // 
            this.inventoryItemsTableAdapter.ClearBeforeFill = true;
            // 
            // tableAdapterManager
            // 
            this.tableAdapterManager.BackupDataSetBeforeUpdate = false;
            this.tableAdapterManager.CustomersTableAdapter = null;
            this.tableAdapterManager.InventoryItems1TableAdapter = null;
            this.tableAdapterManager.InventoryItemsTableAdapter = this.inventoryItemsTableAdapter;
            this.tableAdapterManager.InventoryTransactionsTableAdapter = null;
            this.tableAdapterManager.JobDetailsTableAdapter = null;
            this.tableAdapterManager.JobMaterialsTableAdapter = null;
            this.tableAdapterManager.JobsTableAdapter = null;
            this.tableAdapterManager.JobStatusHistoryTableAdapter = null;
            this.tableAdapterManager.JobTypesTableAdapter = null;
            this.tableAdapterManager.RolesTableAdapter = null;
            this.tableAdapterManager.StaffRolesTableAdapter = null;
            this.tableAdapterManager.StaffTableAdapter = null;
            this.tableAdapterManager.UpdateOrder = Major_Project.GroupWst34DataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // jobsTableAdapter1
            // 
            this.jobsTableAdapter1.ClearBeforeFill = true;
            // 
            // customersTableAdapter
            // 
            this.customersTableAdapter.ClearBeforeFill = true;
            // 
            // jobsViewTableAdapter1
            // 
            this.jobsViewTableAdapter1.ClearBeforeFill = true;
            // 
            // jobTypesTableAdapter1
            // 
            this.jobTypesTableAdapter1.ClearBeforeFill = true;
            // 
            // staffTableAdapter1
            // 
            this.staffTableAdapter1.ClearBeforeFill = true;
            // 
            // jobDetailsTableAdapter1
            // 
            this.jobDetailsTableAdapter1.ClearBeforeFill = true;
            // 
            // jobStatusHistoryTableAdapter1
            // 
            this.jobStatusHistoryTableAdapter1.ClearBeforeFill = true;
            // 
            // jobMaterialsTableAdapter1
            // 
            this.jobMaterialsTableAdapter1.ClearBeforeFill = true;
            // 
            // JobTypesTableAdapteer
            // 
            this.JobTypesTableAdapteer.ClearBeforeFill = true;
            // 
            // toolTip1
            // 
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(1240, 880);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.HelpButton = true;
            this.MinimumSize = new System.Drawing.Size(960, 640);
            this.Name = "AdminForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Admin";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Admin_Load);
            this.tabJobDetails.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.jobDetailsToolbar.ResumeLayout(false);
            this.jobsFilterBar.ResumeLayout(false);
            this.jobsFilterBar.PerformLayout();
            this.jobDetailActions.ResumeLayout(false);
            this.jobActionsToolbar.ResumeLayout(false);
            this.dashboardLayout.ResumeLayout(false);
            this.dashboardCards.ResumeLayout(false);
            this.gbxRecJobs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobsBindingSource3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupWst34DataSet)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inventoryItemsBindingSource1)).EndInit();
            this.dashboardActions.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.gbxQuickAct.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.jobTypesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCart)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customersBindingSource)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvJobs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobsBindingSource4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inventoryItemsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobsBindingSource)).EndInit();
            this.tabJobDetailsFeedbackBanner = new System.Windows.Forms.Label();
            this.tabJobDetailsFeedbackBanner.Name = "WorkflowFeedback";
            this.tabJobDetailsFeedbackBanner.Text = "Status message";
            this.tabJobDetailsFeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabJobDetailsFeedbackBanner.Height = 46;
            this.tabJobDetailsFeedbackBanner.Visible = false;
            this.tabJobDetails.Controls.Add(this.tabJobDetailsFeedbackBanner);
            this.tabJobDetailsFeedbackBanner.SendToBack();
            this.tabPage1FeedbackBanner = new System.Windows.Forms.Label();
            this.tabPage1FeedbackBanner.Name = "WorkflowFeedback";
            this.tabPage1FeedbackBanner.Text = "Status message";
            this.tabPage1FeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabPage1FeedbackBanner.Height = 46;
            this.tabPage1FeedbackBanner.Visible = false;
            this.tabPage1.Controls.Add(this.tabPage1FeedbackBanner);
            this.tabPage1FeedbackBanner.SendToBack();
            this.tabPage2FeedbackBanner = new System.Windows.Forms.Label();
            this.tabPage2FeedbackBanner.Name = "WorkflowFeedback";
            this.tabPage2FeedbackBanner.Text = "Status message";
            this.tabPage2FeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabPage2FeedbackBanner.Height = 46;
            this.tabPage2FeedbackBanner.Visible = false;
            this.tabPage2.Controls.Add(this.tabPage2FeedbackBanner);
            this.tabPage2FeedbackBanner.SendToBack();
            this.tabPage3FeedbackBanner = new System.Windows.Forms.Label();
            this.tabPage3FeedbackBanner.Name = "WorkflowFeedback";
            this.tabPage3FeedbackBanner.Text = "Status message";
            this.tabPage3FeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabPage3FeedbackBanner.Height = 46;
            this.tabPage3FeedbackBanner.Visible = false;
            this.tabPage3.Controls.Add(this.tabPage3FeedbackBanner);
            this.tabPage3FeedbackBanner.SendToBack();
            this.CustomersWorkspacePageFeedbackBanner = new System.Windows.Forms.Label();
            this.CustomersWorkspacePageFeedbackBanner.Name = "WorkflowFeedback";
            this.CustomersWorkspacePageFeedbackBanner.Text = "Status message";
            this.CustomersWorkspacePageFeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.CustomersWorkspacePageFeedbackBanner.Height = 46;
            this.CustomersWorkspacePageFeedbackBanner.Visible = false;
            this.CustomersWorkspacePage.Controls.Add(this.CustomersWorkspacePageFeedbackBanner);
            this.CustomersWorkspacePageFeedbackBanner.SendToBack();
            this.StockWorkspacePageFeedbackBanner = new System.Windows.Forms.Label();
            this.StockWorkspacePageFeedbackBanner.Name = "WorkflowFeedback";
            this.StockWorkspacePageFeedbackBanner.Text = "Status message";
            this.StockWorkspacePageFeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.StockWorkspacePageFeedbackBanner.Height = 46;
            this.StockWorkspacePageFeedbackBanner.Visible = false;
            this.StockWorkspacePage.Controls.Add(this.StockWorkspacePageFeedbackBanner);
            this.StockWorkspacePageFeedbackBanner.SendToBack();
            this.JobChangesFeedbackBanner = new System.Windows.Forms.Label();
            this.JobChangesFeedbackBanner.Name = "WorkflowFeedback";
            this.JobChangesFeedbackBanner.Text = "Status message";
            this.JobChangesFeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.JobChangesFeedbackBanner.Height = 46;
            this.JobChangesFeedbackBanner.Visible = false;
            this.JobChanges.Controls.Add(this.JobChangesFeedbackBanner);
            this.JobChangesFeedbackBanner.SendToBack();
            this.ProfileWorkspacePageFeedbackBanner = new System.Windows.Forms.Label();
            this.ProfileWorkspacePageFeedbackBanner.Name = "WorkflowFeedback";
            this.ProfileWorkspacePageFeedbackBanner.Text = "Status message";
            this.ProfileWorkspacePageFeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.ProfileWorkspacePageFeedbackBanner.Height = 46;
            this.ProfileWorkspacePageFeedbackBanner.Visible = false;
            this.ProfileWorkspacePage.Controls.Add(this.ProfileWorkspacePageFeedbackBanner);
            this.ProfileWorkspacePageFeedbackBanner.SendToBack();
            this.AssistantWorkspacePageFeedbackBanner = new System.Windows.Forms.Label();
            this.AssistantWorkspacePageFeedbackBanner.Name = "WorkflowFeedback";
            this.AssistantWorkspacePageFeedbackBanner.Text = "Status message";
            this.AssistantWorkspacePageFeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.AssistantWorkspacePageFeedbackBanner.Height = 46;
            this.AssistantWorkspacePageFeedbackBanner.Visible = false;
            this.AssistantWorkspacePage.Controls.Add(this.AssistantWorkspacePageFeedbackBanner);
            this.AssistantWorkspacePageFeedbackBanner.SendToBack();
            this.workflowFeedback = new System.Windows.Forms.Label();
            this.workflowFeedback.Name = "WorkflowFeedback";
            this.workflowFeedback.Text = "Status message";
            this.workflowFeedback.Dock = System.Windows.Forms.DockStyle.Top;
            this.workflowFeedback.Height = 46;
            this.workflowFeedback.Visible = false;
            this.Controls.Add(this.workflowFeedback);
            this.workflowFeedback.SendToBack();
            this.ResumeLayout(false);

        }



        private void groupBox8_Enter(object sender, EventArgs e) { }
        #endregion

        private System.Windows.Forms.Panel panel1;
        private Major_Project.UI.Forms.Common.WorkspaceTabs tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox gbxRecJobs;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Label lblAdminGreeting;
        private System.Windows.Forms.GroupBox gbxQuickAct;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.DataGridView dgvJobs;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.ComboBox comboBox5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelJob;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button button7;

        private System.Windows.Forms.BindingSource jobsBindingSource;

        private System.Windows.Forms.DataGridViewTextBoxColumn jobIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn customerIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn staffIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn promiseDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn jobStatusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalMaterialCostDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn labourCostDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn transportCostDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn taxAmountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn grandTotalDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn notesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdAtDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn updatedAtDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource inventoryItemsBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn categoryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn quantityInStockDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn unitPriceDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn reorderLevelDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn supplierNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn updatedAtDataGridViewTextBoxColumn1;
        private GroupWst34DataSetTableAdapters.JobsTableAdapter jobsTableAdapter;
        private GroupWst34DataSet groupWst34DataSet;
        private System.Windows.Forms.BindingSource inventoryItemsBindingSource1;
        private GroupWst34DataSetTableAdapters.InventoryItemsTableAdapter inventoryItemsTableAdapter;
        private GroupWst34DataSetTableAdapters.TableAdapterManager tableAdapterManager;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn14;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn15;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn16;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn17;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn18;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn19;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn20;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn21;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.BindingSource jobsBindingSource3;
        private GroupWst34DataSetTableAdapters.JobsTableAdapter jobsTableAdapter1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnUpdateProfile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbCustomerType;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.DataGridView dataGridView6;
        private System.Windows.Forms.Button btnSaveProfile;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cmbSevArea;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtFAddress;
        private System.Windows.Forms.TextBox txtCustName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.DateTimePicker dateTimePicker3;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.DataGridView dataGridViewCart;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.BindingSource customersBindingSource;
        private GroupWst34DataSetTableAdapters.CustomersTableAdapter customersTableAdapter;
        private GroupWst34DataSetTableAdapters.JobsViewTableAdapter jobsViewTableAdapter1;
        private GroupWst34DataSetTableAdapters.JobTypesTableAdapter jobTypesTableAdapter1;
        private GroupWst34DataSetTableAdapters.StaffTableAdapter staffTableAdapter1;
        private GroupWst34DataSetTableAdapters.JobDetailsTableAdapter jobDetailsTableAdapter1;
        private GroupWst34DataSetTableAdapters.JobStatusHistoryTableAdapter jobStatusHistoryTableAdapter1;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerID;
        private System.Windows.Forms.DataGridViewTextBoxColumn FullName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PhoneNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn EmailAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn FullAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn Area;
        private System.Windows.Forms.BindingSource jobTypesBindingSource;
        private System.Windows.Forms.BindingSource jobsBindingSource4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn22;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn23;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn24;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn25;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn26;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn27;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn28;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn29;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn30;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn31;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn32;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn33;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn34;
        private GroupWst34DataSetTableAdapters.JobMaterialsTableAdapter jobMaterialsTableAdapter1;
        private GroupWst34DataSetTableAdapters.JobTypesTableAdapter JobTypesTableAdapteer;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button btnHelpJobs;
        private System.Windows.Forms.Button btnJobManagement;
        private TextBox txtSouthAfricanIDNumber;
        private TextBox txtCompanyRegistrationNumber;
        private Label lblCompanyRegNo;
        private Label lblSA_ID;
        private System.Windows.Forms.TableLayoutPanel dashboardLayout;
        private System.Windows.Forms.TableLayoutPanel dashboardCards;
        private System.Windows.Forms.FlowLayoutPanel dashboardActions;
        private System.Windows.Forms.Label lblDashboardTitle;
        private System.Windows.Forms.Button btnDashboardRefresh;
        private System.Windows.Forms.Label lblLowStockEmpty;
        private System.Windows.Forms.Button btnDashboardHelp;
        private System.Windows.Forms.ComboBox cmbIdentityDocument;
        private System.Windows.Forms.TextBox txtPassportNumber;
        private System.Windows.Forms.TextBox txtPassportCountry;
        private System.Windows.Forms.Label lblIdentityDocument;
        private System.Windows.Forms.Label lblPassportNumber;
        private System.Windows.Forms.Label lblPassportCountry;
        private System.Windows.Forms.FlowLayoutPanel jobActionsToolbar;
        private System.Windows.Forms.Button btnCancelSelectedJob;
        private System.Windows.Forms.Button btnCollectCash;
        private System.Windows.Forms.Button btnSaveInvoicePdf;
        private System.Windows.Forms.Button btnSaveReceiptPdf;
        private System.Windows.Forms.Panel jobsFilterBar;
        private System.Windows.Forms.Label lblJobStatusFilter;
        private System.Windows.Forms.FlowLayoutPanel jobDetailActions;
        private System.Windows.Forms.Button btnViewInvoicePdf;
        private System.Windows.Forms.Button btnViewReceiptPdf;
        private System.Windows.Forms.DataGridViewTextBoxColumn jobsPaymentStatusColumn;
        private System.Windows.Forms.TabPage tabJobDetails;
        private System.Windows.Forms.FlowLayoutPanel jobDetailsToolbar;
        private System.Windows.Forms.Button btnBackToJobs;
        private System.Windows.Forms.Button btnOpenJobDetails;
        private System.Windows.Forms.Label lblTechnicianAvailability;
        private System.Windows.Forms.DataGridViewTextBoxColumn cartGrandTotalColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cartServiceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cartQuantityColumn;
    }
}
