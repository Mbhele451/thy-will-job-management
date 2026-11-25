
namespace Major_Project.UI.Forms.Technician
{
    partial class TechnicianForm
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
        private System.Windows.Forms.TabPage ProfileWorkspacePage;
        private Major_Project.UI.Forms.Common.StaffProfileView ProfileLayout;
        private System.Windows.Forms.TabPage JobChanges;
        private Major_Project.UI.Forms.Common.JobChangesView JobChangesLayout;
        private System.Windows.Forms.TabPage AssistantWorkspacePage;
        private Major_Project.UI.Forms.Common.AssistantView AssistantLayout;

        internal System.Windows.Forms.Label tabPage3FeedbackBanner;
        internal System.Windows.Forms.Label tabPage4FeedbackBanner;
        internal System.Windows.Forms.Label tabPage1FeedbackBanner;
        internal System.Windows.Forms.Label ProfileWorkspacePageFeedbackBanner;
        internal System.Windows.Forms.Label JobChangesFeedbackBanner;
        internal System.Windows.Forms.Label AssistantWorkspacePageFeedbackBanner;
        internal System.Windows.Forms.Label workflowFeedback;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TechnicianForm));
            this._jobStatus = new System.Windows.Forms.ComboBox();
            this._jobSearch = new System.Windows.Forms.TextBox();
            this.jobsToolbar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnJobsRefresh = new System.Windows.Forms.Button();
            this.btnJobDetails = new System.Windows.Forms.Button();
            this.dashboardLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblDashboardTitle = new System.Windows.Forms.Label();
            this.lblAdminGreeting = new System.Windows.Forms.Label();
            this.dashboardCards = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.gbxRecJobs = new System.Windows.Forms.GroupBox();
            this.dataGridView4 = new System.Windows.Forms.DataGridView();
            this.JobID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StaffID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JobStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PromiseDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.groupWst34DataSet = new Major_Project.GroupWst34DataSet();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn20 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.lblLowStockEmpty = new System.Windows.Forms.Label();
            this.dashboardActions = new System.Windows.Forms.FlowLayoutPanel();
            this.button3 = new System.Windows.Forms.Button();
            this.btnTechDash = new System.Windows.Forms.Button();
            this.btnDashboardRefresh = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.button11 = new System.Windows.Forms.Button();
            this.dgvJobs = new System.Windows.Forms.DataGridView();
            this.label24 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btnHelpTech = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.btnInvoice = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button12 = new System.Windows.Forms.Button();
            this.label26 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn29 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn30 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn31 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn32 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn33 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn34 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn35 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn36 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label27 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.gbxQuickAct = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label22 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new Major_Project.UI.Forms.Common.WorkspaceTabs();
            this.jobsTableAdapter = new Major_Project.GroupWst34DataSetTableAdapters.JobsTableAdapter();
            this.inventoryItemsTableAdapter = new Major_Project.GroupWst34DataSetTableAdapters.InventoryItemsTableAdapter();
            this.jobsToolbar.SuspendLayout();
            this.dashboardLayout.SuspendLayout();
            this.dashboardCards.SuspendLayout();
            this.gbxRecJobs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupWst34DataSet)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).BeginInit();
            this.dashboardActions.SuspendLayout();
            this.panel5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvJobs)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.gbxQuickAct.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // 
            // 
            // _jobStatus
            // 
            this._jobStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._jobStatus.Font = new System.Drawing.Font("Segoe UI", 11F);
            this._jobStatus.Location = new System.Drawing.Point(241, 11);
            this._jobStatus.Name = "_jobStatus";
            this._jobStatus.Size = new System.Drawing.Size(150, 25);
            this._jobStatus.TabIndex = 1;
            // 
            // _jobSearch
            // 
            this._jobSearch.AccessibleName = "Customer, service area or job number";
            this._jobSearch.Font = new System.Drawing.Font("Segoe UI", 11F);
            this._jobSearch.Location = new System.Drawing.Point(15, 11);
            this._jobSearch.Name = "_jobSearch";
            this._jobSearch.Size = new System.Drawing.Size(220, 25);
            this._jobSearch.TabIndex = 0;
            // 
            // jobsToolbar
            // 
            this.jobsToolbar.Controls.Add(this._jobSearch);
            this.jobsToolbar.Controls.Add(this._jobStatus);
            this.jobsToolbar.Controls.Add(this.btnJobsRefresh);
            this.jobsToolbar.Controls.Add(this.btnJobDetails);
            this.jobsToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.jobsToolbar.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.jobsToolbar.Location = new System.Drawing.Point(3, 3);
            this.jobsToolbar.Name = "jobsToolbar";
            this.jobsToolbar.Padding = new System.Windows.Forms.Padding(12, 8, 12, 0);
            this.jobsToolbar.Size = new System.Drawing.Size(1629, 48);
            this.jobsToolbar.TabIndex = 1;
            this.jobsToolbar.WrapContents = false;
            // 
            // btnJobsRefresh
            // 
            this.btnJobsRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnJobsRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJobsRefresh.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnJobsRefresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnJobsRefresh.Location = new System.Drawing.Point(397, 11);
            this.btnJobsRefresh.Name = "btnJobsRefresh";
            this.btnJobsRefresh.Size = new System.Drawing.Size(129, 36);
            this.btnJobsRefresh.TabIndex = 2;
            this.btnJobsRefresh.Text = "Refresh";
            this.btnJobsRefresh.UseVisualStyleBackColor = false;
            // 
            // btnJobDetails
            // 
            this.btnJobDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnJobDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJobDetails.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnJobDetails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnJobDetails.Location = new System.Drawing.Point(532, 11);
            this.btnJobDetails.Name = "btnJobDetails";
            this.btnJobDetails.Size = new System.Drawing.Size(129, 36);
            this.btnJobDetails.TabIndex = 3;
            this.btnJobDetails.Text = "Job Details";
            this.btnJobDetails.UseVisualStyleBackColor = false;
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
            this.dashboardLayout.Size = new System.Drawing.Size(1226, 848);
            this.dashboardLayout.TabIndex = 26;
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
            this.lblDashboardTitle.Text = "Technician dashboard";
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
            this.gbxRecJobs.Controls.Add(this.dataGridView4);
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
            // dataGridView4
            // 
            this.dataGridView4.AllowUserToAddRows = false;
            this.dataGridView4.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.dataGridView4.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView4.AutoGenerateColumns = false;
            this.dataGridView4.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView4.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.dataGridView4.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(237)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView4.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView4.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.JobID,
            this.CustomerID,
            this.StaffID,
            this.JobStatus,
            this.PromiseDate,
            this.Notes});
            this.dataGridView4.DataSource = this.bindingSource1;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView4.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView4.EnableHeadersVisualStyles = false;
            this.dataGridView4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dataGridView4.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.dataGridView4.Location = new System.Drawing.Point(12, 30);
            this.dataGridView4.MultiSelect = false;
            this.dataGridView4.Name = "dataGridView4";
            this.dataGridView4.ReadOnly = true;
            this.dataGridView4.RowHeadersVisible = false;
            this.dataGridView4.RowTemplate.Height = 28;
            this.dataGridView4.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView4.Size = new System.Drawing.Size(1154, 216);
            this.dataGridView4.TabIndex = 1;
            this.dataGridView4.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView4_CellContentClick);
            // 
            // JobID
            // 
            this.JobID.DataPropertyName = "JobID";
            this.JobID.HeaderText = "Job #";
            this.JobID.Name = "JobID";
            this.JobID.ReadOnly = true;
            // 
            // CustomerID
            // 
            this.CustomerID.DataPropertyName = "CustomerID";
            this.CustomerID.HeaderText = "Customer ID";
            this.CustomerID.Name = "CustomerID";
            this.CustomerID.ReadOnly = true;
            this.CustomerID.Visible = false;
            // 
            // StaffID
            // 
            this.StaffID.DataPropertyName = "StaffID";
            this.StaffID.HeaderText = "Staff ID";
            this.StaffID.Name = "StaffID";
            this.StaffID.ReadOnly = true;
            this.StaffID.Visible = false;
            // 
            // JobStatus
            // 
            this.JobStatus.DataPropertyName = "JobStatus";
            this.JobStatus.HeaderText = "Status";
            this.JobStatus.Name = "JobStatus";
            this.JobStatus.ReadOnly = true;
            // 
            // PromiseDate
            // 
            this.PromiseDate.DataPropertyName = "PromiseDate";
            this.PromiseDate.HeaderText = "Promise Date";
            this.PromiseDate.Name = "PromiseDate";
            this.PromiseDate.ReadOnly = true;
            // 
            // Notes
            // 
            this.Notes.DataPropertyName = "Notes";
            this.Notes.HeaderText = "Notes";
            this.Notes.Name = "Notes";
            this.Notes.ReadOnly = true;
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataMember = "Jobs";
            this.bindingSource1.DataSource = this.groupWst34DataSet;
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
            this.groupBox4.Enter += new System.EventHandler(this.groupBox4_Enter);
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToAddRows = false;
            this.dataGridView3.AllowUserToDeleteRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.dataGridView3.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView3.AutoGenerateColumns = false;
            this.dataGridView3.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView3.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.dataGridView3.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(237)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView3.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn13,
            this.dataGridViewTextBoxColumn14,
            this.dataGridViewTextBoxColumn15,
            this.dataGridViewTextBoxColumn16,
            this.dataGridViewTextBoxColumn18,
            this.dataGridViewTextBoxColumn20});
            this.dataGridView3.DataSource = this.bindingSource2;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView3.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView3.EnableHeadersVisualStyles = false;
            this.dataGridView3.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dataGridView3.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.dataGridView3.Location = new System.Drawing.Point(12, 30);
            this.dataGridView3.MultiSelect = false;
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.ReadOnly = true;
            this.dataGridView3.RowHeadersVisible = false;
            this.dataGridView3.RowTemplate.Height = 28;
            this.dataGridView3.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView3.Size = new System.Drawing.Size(1154, 148);
            this.dataGridView3.TabIndex = 0;
            this.dataGridView3.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView3_CellContentClick);
            // 
            // dataGridViewTextBoxColumn13
            // 
            this.dataGridViewTextBoxColumn13.DataPropertyName = "ItemID";
            this.dataGridViewTextBoxColumn13.HeaderText = "Item ID";
            this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            this.dataGridViewTextBoxColumn13.ReadOnly = true;
            this.dataGridViewTextBoxColumn13.Visible = false;
            // 
            // dataGridViewTextBoxColumn14
            // 
            this.dataGridViewTextBoxColumn14.DataPropertyName = "ItemName";
            this.dataGridViewTextBoxColumn14.HeaderText = "Item Name";
            this.dataGridViewTextBoxColumn14.Name = "dataGridViewTextBoxColumn14";
            this.dataGridViewTextBoxColumn14.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn15
            // 
            this.dataGridViewTextBoxColumn15.DataPropertyName = "Category";
            this.dataGridViewTextBoxColumn15.HeaderText = "Category";
            this.dataGridViewTextBoxColumn15.Name = "dataGridViewTextBoxColumn15";
            this.dataGridViewTextBoxColumn15.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn16
            // 
            this.dataGridViewTextBoxColumn16.DataPropertyName = "QuantityInStock";
            this.dataGridViewTextBoxColumn16.HeaderText = "In stock";
            this.dataGridViewTextBoxColumn16.Name = "dataGridViewTextBoxColumn16";
            this.dataGridViewTextBoxColumn16.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn18
            // 
            this.dataGridViewTextBoxColumn18.DataPropertyName = "ReorderLevel";
            this.dataGridViewTextBoxColumn18.HeaderText = "Reorder Level";
            this.dataGridViewTextBoxColumn18.Name = "dataGridViewTextBoxColumn18";
            this.dataGridViewTextBoxColumn18.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn20
            // 
            this.dataGridViewTextBoxColumn20.DataPropertyName = "UpdatedAt";
            this.dataGridViewTextBoxColumn20.HeaderText = "Updated At";
            this.dataGridViewTextBoxColumn20.Name = "dataGridViewTextBoxColumn20";
            this.dataGridViewTextBoxColumn20.ReadOnly = true;
            // 
            // bindingSource2
            // 
            this.bindingSource2.DataMember = "InventoryItems";
            this.bindingSource2.DataSource = this.groupWst34DataSet;
            this.bindingSource2.CurrentChanged += new System.EventHandler(this.bindingSource2_CurrentChanged);
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

            this.dashboardActions.Controls.Add(this.btnTechDash);
            this.dashboardActions.Controls.Add(this.btnDashboardRefresh);
            this.dashboardActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dashboardActions.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.dashboardActions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.dashboardActions.Location = new System.Drawing.Point(27, 723);
            this.dashboardActions.Name = "dashboardActions";
            this.dashboardActions.Size = new System.Drawing.Size(1172, 98);
            this.dashboardActions.TabIndex = 20;
            this.dashboardActions.WrapContents = false;
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
            this.button3.Location = new System.Drawing.Point(1060, 0);
            this.button3.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(112, 36);
            this.button3.TabIndex = 8;
            this.button3.Text = "Sign Out";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnTechDash
            // 
            this.btnTechDash.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnTechDash.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.btnTechDash.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTechDash.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnTechDash.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnTechDash.Location = new System.Drawing.Point(940, 0);
            this.btnTechDash.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btnTechDash.Name = "btnTechDash";
            this.btnTechDash.Size = new System.Drawing.Size(112, 36);
            this.btnTechDash.TabIndex = 43;
            this.btnTechDash.Text = "Help";
            this.btnTechDash.UseVisualStyleBackColor = false;
            // 
            // btnDashboardRefresh
            // 
            this.btnDashboardRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnDashboardRefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.btnDashboardRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboardRefresh.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnDashboardRefresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnDashboardRefresh.Location = new System.Drawing.Point(820, 0);
            this.btnDashboardRefresh.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.btnDashboardRefresh.Name = "btnDashboardRefresh";
            this.btnDashboardRefresh.Size = new System.Drawing.Size(112, 36);
            this.btnDashboardRefresh.TabIndex = 44;
            this.btnDashboardRefresh.Text = "Refresh";
            this.btnDashboardRefresh.UseVisualStyleBackColor = false;
            this.btnDashboardRefresh.Click += new System.EventHandler(this.DashboardRefresh_Click);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.panel5.Controls.Add(this.groupBox7);
            this.panel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.panel5.Location = new System.Drawing.Point(3, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(803, 736);
            this.panel5.TabIndex = 0;
            // 
            // groupBox7
            // 
            this.groupBox7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));

            this.groupBox7.Controls.Add(this.dgvJobs);
            this.groupBox7.Controls.Add(this.label24);
            this.groupBox7.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.groupBox7.Location = new System.Drawing.Point(3, 84);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(797, 692);
            this.groupBox7.TabIndex = 12;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Job List";
            // 
            // button11
            // 
            this.button11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.button11.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button11.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button11.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.button11.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.button11.Location = new System.Drawing.Point(11, 607);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(172, 77);
            this.button11.TabIndex = 16;
            this.button11.Text = "Sign Out";
            this.button11.UseVisualStyleBackColor = false;
            this.button11.Click += new System.EventHandler(this.button11_Click_1);
            // 
            // dgvJobs
            // 
            this.dgvJobs.AllowUserToAddRows = false;
            this.dgvJobs.AllowUserToDeleteRows = false;
            this.dgvJobs.AllowUserToOrderColumns = true;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.dgvJobs.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvJobs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvJobs.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.dgvJobs.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(237)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvJobs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvJobs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvJobs.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgvJobs.EnableHeadersVisualStyles = false;
            this.dgvJobs.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dgvJobs.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.dgvJobs.Location = new System.Drawing.Point(6, 29);
            this.dgvJobs.MultiSelect = false;
            this.dgvJobs.Name = "dgvJobs";
            this.dgvJobs.ReadOnly = true;
            this.dgvJobs.RowHeadersVisible = false;
            this.dgvJobs.RowTemplate.Height = 28;
            this.dgvJobs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvJobs.Size = new System.Drawing.Size(773, 572);
            this.dgvJobs.TabIndex = 15;
            this.dgvJobs.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvJobs_RowHeaderMouseClick);
            this.dgvJobs.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvJobs_RowHeaderMouseDoubleClick);
            this.dgvJobs.SelectionChanged += new System.EventHandler(this.dgvJobs_SelectionChanged_1);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.BackColor = System.Drawing.Color.Transparent;
            this.label24.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label24.Location = new System.Drawing.Point(7, 20);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(0, 25);
            this.label24.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panel5, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel6, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 51);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1629, 783);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.panel6.Controls.Add(this.btnHelpTech);
            this.panel6.Controls.Add(this.button4);
            this.panel6.Controls.Add(this.groupBox8);
            this.panel6.Controls.Add(this.btnInvoice);
            this.panel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.panel6.Location = new System.Drawing.Point(817, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(803, 736);
            this.panel6.TabIndex = 1;
            // 
            // btnHelpTech
            // 
            this.btnHelpTech.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.btnHelpTech.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.btnHelpTech.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelpTech.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.btnHelpTech.Location = new System.Drawing.Point(706, 17);
            this.btnHelpTech.Name = "btnHelpTech";
            this.btnHelpTech.Size = new System.Drawing.Size(79, 31);
            this.btnHelpTech.TabIndex = 42;
            this.btnHelpTech.Text = "Help";
            this.btnHelpTech.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.button4.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(82, 622);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(129, 53);
            this.button4.TabIndex = 16;
            this.button4.Text = "Start job";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.groupBox8.Controls.Add(this.textBox1);
            this.groupBox8.Controls.Add(this.label25);
            this.groupBox8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.groupBox8.Location = new System.Drawing.Point(6, 93);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(779, 378);
            this.groupBox8.TabIndex = 15;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Job Details";
            this.groupBox8.Enter += new System.EventHandler(this.groupBox8_Enter);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.textBox1.Location = new System.Drawing.Point(3, 16);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(773, 359);
            this.textBox1.TabIndex = 1;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.BackColor = System.Drawing.Color.Transparent;
            this.label25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label25.Location = new System.Drawing.Point(7, 20);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(0, 13);
            this.label25.TabIndex = 0;
            // 
            // btnInvoice
            // 
            this.btnInvoice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.btnInvoice.Enabled = false;
            this.btnInvoice.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.btnInvoice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInvoice.ForeColor = System.Drawing.Color.White;
            this.btnInvoice.Location = new System.Drawing.Point(610, 619);
            this.btnInvoice.Name = "btnInvoice";
            this.btnInvoice.Size = new System.Drawing.Size(155, 56);
            this.btnInvoice.TabIndex = 14;
            this.btnInvoice.Text = "Job Complete";
            this.btnInvoice.UseVisualStyleBackColor = false;
            this.btnInvoice.Click += new System.EventHandler(this.btnInvoice_Click_1);
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.AutoScrollMinSize = new System.Drawing.Size(1629, 790);
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.tabPage3.Controls.Add(this.tableLayoutPanel2);
            this.tabPage3.Controls.Add(this.jobsToolbar);
            this.tabPage3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1232, 854);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Job Management";
            // 
            // tabPage4
            // 
            this.tabPage4.AutoScroll = true;
            this.tabPage4.AutoScrollMinSize = new System.Drawing.Size(1629, 790);
            this.tabPage4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.tabPage4.Controls.Add(this.panel1);
            this.tabPage4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1232, 854);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Inventory";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));

            this.panel1.Controls.Add(this.label26);
            this.panel1.Controls.Add(this.textBox7);
            this.panel1.Controls.Add(this.groupBox9);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1629, 831);
            this.panel1.TabIndex = 1;
            // 
            // button12
            // 
            this.button12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.button12.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button12.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button12.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.button12.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.button12.Location = new System.Drawing.Point(19, 673);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(172, 77);
            this.button12.TabIndex = 15;
            this.button12.Text = "Sign Out";
            this.button12.UseVisualStyleBackColor = false;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.BackColor = System.Drawing.Color.Transparent;
            this.label26.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label26.Location = new System.Drawing.Point(235, 38);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(151, 20);
            this.label26.TabIndex = 14;
            this.label26.Text = "Search By Item Name";
            this.label26.Click += new System.EventHandler(this.label26_Click);
            // 
            // textBox7
            // 
            this.textBox7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.textBox7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.textBox7.Location = new System.Drawing.Point(448, 36);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(248, 20);
            this.textBox7.TabIndex = 13;
            this.textBox7.TextChanged += new System.EventHandler(this.textBox7_TextChanged);
            // 
            // groupBox9
            // 
            this.groupBox9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.groupBox9.Controls.Add(this.dataGridView1);
            this.groupBox9.Controls.Add(this.label27);
            this.groupBox9.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.groupBox9.Location = new System.Drawing.Point(307, 75);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(851, 643);
            this.groupBox9.TabIndex = 12;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Inventory";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(237)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn29,
            this.dataGridViewTextBoxColumn30,
            this.dataGridViewTextBoxColumn31,
            this.dataGridViewTextBoxColumn32,
            this.dataGridViewTextBoxColumn33,
            this.dataGridViewTextBoxColumn34,
            this.dataGridViewTextBoxColumn35,
            this.dataGridViewTextBoxColumn36});
            this.dataGridView1.DataSource = this.bindingSource2;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Segoe UI", 11F);
            dataGridViewCellStyle13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle13;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(214)))), ((int)(((byte)(232)))));
            this.dataGridView1.Location = new System.Drawing.Point(29, 28);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(792, 589);
            this.dataGridView1.TabIndex = 15;
            // 
            // dataGridViewTextBoxColumn29
            // 
            this.dataGridViewTextBoxColumn29.DataPropertyName = "ItemID";
            this.dataGridViewTextBoxColumn29.HeaderText = "Item ID";
            this.dataGridViewTextBoxColumn29.Name = "dataGridViewTextBoxColumn29";
            this.dataGridViewTextBoxColumn29.ReadOnly = true;
            this.dataGridViewTextBoxColumn29.Visible = false;
            // 
            // dataGridViewTextBoxColumn30
            // 
            this.dataGridViewTextBoxColumn30.DataPropertyName = "ItemName";
            this.dataGridViewTextBoxColumn30.HeaderText = "Item Name";
            this.dataGridViewTextBoxColumn30.Name = "dataGridViewTextBoxColumn30";
            this.dataGridViewTextBoxColumn30.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn31
            // 
            this.dataGridViewTextBoxColumn31.DataPropertyName = "Category";
            this.dataGridViewTextBoxColumn31.HeaderText = "Category";
            this.dataGridViewTextBoxColumn31.Name = "dataGridViewTextBoxColumn31";
            this.dataGridViewTextBoxColumn31.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn32
            // 
            this.dataGridViewTextBoxColumn32.DataPropertyName = "QuantityInStock";
            this.dataGridViewTextBoxColumn32.HeaderText = "In stock";
            this.dataGridViewTextBoxColumn32.Name = "dataGridViewTextBoxColumn32";
            this.dataGridViewTextBoxColumn32.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn33
            // 
            this.dataGridViewTextBoxColumn33.DataPropertyName = "UnitPrice";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle12.Format = "C2";
            this.dataGridViewTextBoxColumn33.DefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridViewTextBoxColumn33.HeaderText = "Unit Price";
            this.dataGridViewTextBoxColumn33.Name = "dataGridViewTextBoxColumn33";
            this.dataGridViewTextBoxColumn33.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn34
            // 
            this.dataGridViewTextBoxColumn34.DataPropertyName = "ReorderLevel";
            this.dataGridViewTextBoxColumn34.HeaderText = "Reorder Level";
            this.dataGridViewTextBoxColumn34.Name = "dataGridViewTextBoxColumn34";
            this.dataGridViewTextBoxColumn34.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn35
            // 
            this.dataGridViewTextBoxColumn35.DataPropertyName = "SupplierName";
            this.dataGridViewTextBoxColumn35.HeaderText = "Supplier Name";
            this.dataGridViewTextBoxColumn35.Name = "dataGridViewTextBoxColumn35";
            this.dataGridViewTextBoxColumn35.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn36
            // 
            this.dataGridViewTextBoxColumn36.DataPropertyName = "UpdatedAt";
            this.dataGridViewTextBoxColumn36.HeaderText = "Updated At";
            this.dataGridViewTextBoxColumn36.Name = "dataGridViewTextBoxColumn36";
            this.dataGridViewTextBoxColumn36.ReadOnly = true;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.BackColor = System.Drawing.Color.Transparent;
            this.label27.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label27.Location = new System.Drawing.Point(7, 20);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(0, 25);
            this.label27.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.pictureBox1.Image = global::Major_Project.Properties.Resources.THY_WII_electrical_and_construction_logo;
            this.pictureBox1.Location = new System.Drawing.Point(38, 217);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(622, 315);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // gbxQuickAct
            // 
            this.gbxQuickAct.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.gbxQuickAct.Controls.Add(this.pictureBox1);
            this.gbxQuickAct.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxQuickAct.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.gbxQuickAct.Location = new System.Drawing.Point(902, 111);
            this.gbxQuickAct.Name = "gbxQuickAct";
            this.gbxQuickAct.Size = new System.Drawing.Size(689, 565);
            this.gbxQuickAct.TabIndex = 4;
            this.gbxQuickAct.TabStop = false;
            this.gbxQuickAct.Text = "Quick Stats";
            this.gbxQuickAct.Visible = false;
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
            this.panel2.Size = new System.Drawing.Size(1226, 848);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.BackColor = System.Drawing.Color.Transparent;
            this.label22.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label22.Location = new System.Drawing.Point(1424, 698);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(84, 25);
            this.label22.TabIndex = 25;
            this.label22.Text = "Log Out";
            this.label22.Visible = false;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.BackColor = System.Drawing.Color.Transparent;
            this.label31.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.label31.Location = new System.Drawing.Point(928, 690);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(102, 17);
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
            this.pictureBox2.Location = new System.Drawing.Point(931, 717);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(109, 50);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 20;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1232, 854);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Dashboard";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.ProfileWorkspacePage = new System.Windows.Forms.TabPage();
            this.ProfileLayout = new Major_Project.UI.Forms.Common.StaffProfileView();
            this.ProfileWorkspacePage.Name = "ProfileWorkspacePage";
            this.ProfileWorkspacePage.Text = "My Profile";
            this.ProfileLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProfileWorkspacePage.Padding = new System.Windows.Forms.Padding(16);
            this.ProfileWorkspacePage.Controls.Add(this.ProfileLayout);
            this.tabControl1.Controls.Add(this.ProfileWorkspacePage);
            this.JobChanges = new System.Windows.Forms.TabPage();
            this.JobChangesLayout = new Major_Project.UI.Forms.Common.JobChangesView();
            this.JobChanges.Name = "JobChanges";
            this.JobChanges.Text = "Allocations & materials";
            this.JobChangesLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobChanges.Padding = new System.Windows.Forms.Padding(16);
            this.JobChanges.Controls.Add(this.JobChangesLayout);
            this.tabControl1.Controls.Add(this.JobChanges);
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
            this.tabControl1.TabIndex = 2;
            // 
            // jobsTableAdapter
            // 
            this.jobsTableAdapter.ClearBeforeFill = true;
            // 
            // inventoryItemsTableAdapter
            // 
            this.inventoryItemsTableAdapter.ClearBeforeFill = true;
            // 
            // TechnicianForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(1240, 880);
            this.ControlBox = false;
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(960, 640);
            this.Name = "TechnicianForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Technician";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Technician_Load);
            this.jobsToolbar.ResumeLayout(false);
            this.jobsToolbar.PerformLayout();
            this.dashboardLayout.ResumeLayout(false);
            this.dashboardCards.ResumeLayout(false);
            this.gbxRecJobs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupWst34DataSet)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).EndInit();
            this.dashboardActions.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvJobs)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.gbxQuickAct.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.tabPage3FeedbackBanner = new System.Windows.Forms.Label();
            this.tabPage3FeedbackBanner.Name = "WorkflowFeedback";
            this.tabPage3FeedbackBanner.Text = "Status message";
            this.tabPage3FeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabPage3FeedbackBanner.Height = 46;
            this.tabPage3FeedbackBanner.Visible = false;
            this.tabPage3.Controls.Add(this.tabPage3FeedbackBanner);
            this.tabPage3FeedbackBanner.SendToBack();
            this.tabPage4FeedbackBanner = new System.Windows.Forms.Label();
            this.tabPage4FeedbackBanner.Name = "WorkflowFeedback";
            this.tabPage4FeedbackBanner.Text = "Status message";
            this.tabPage4FeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabPage4FeedbackBanner.Height = 46;
            this.tabPage4FeedbackBanner.Visible = false;
            this.tabPage4.Controls.Add(this.tabPage4FeedbackBanner);
            this.tabPage4FeedbackBanner.SendToBack();
            this.tabPage1FeedbackBanner = new System.Windows.Forms.Label();
            this.tabPage1FeedbackBanner.Name = "WorkflowFeedback";
            this.tabPage1FeedbackBanner.Text = "Status message";
            this.tabPage1FeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabPage1FeedbackBanner.Height = 46;
            this.tabPage1FeedbackBanner.Visible = false;
            this.tabPage1.Controls.Add(this.tabPage1FeedbackBanner);
            this.tabPage1FeedbackBanner.SendToBack();
            this.ProfileWorkspacePageFeedbackBanner = new System.Windows.Forms.Label();
            this.ProfileWorkspacePageFeedbackBanner.Name = "WorkflowFeedback";
            this.ProfileWorkspacePageFeedbackBanner.Text = "Status message";
            this.ProfileWorkspacePageFeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.ProfileWorkspacePageFeedbackBanner.Height = 46;
            this.ProfileWorkspacePageFeedbackBanner.Visible = false;
            this.ProfileWorkspacePage.Controls.Add(this.ProfileWorkspacePageFeedbackBanner);
            this.ProfileWorkspacePageFeedbackBanner.SendToBack();
            this.JobChangesFeedbackBanner = new System.Windows.Forms.Label();
            this.JobChangesFeedbackBanner.Name = "WorkflowFeedback";
            this.JobChangesFeedbackBanner.Text = "Status message";
            this.JobChangesFeedbackBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.JobChangesFeedbackBanner.Height = 46;
            this.JobChangesFeedbackBanner.Visible = false;
            this.JobChanges.Controls.Add(this.JobChangesFeedbackBanner);
            this.JobChangesFeedbackBanner.SendToBack();
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

        #endregion
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.DataGridView dgvJobs;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button btnInvoice;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label lblAdminGreeting;

        private System.Windows.Forms.GroupBox gbxRecJobs;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox gbxQuickAct;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabPage tabPage1;
        private Major_Project.UI.Forms.Common.WorkspaceTabs tabControl1;
        private System.Windows.Forms.BindingSource inventoryItemsBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn categoryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn quantityInStockDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource jobsBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn jobIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn customerIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn staffIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn promiseDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn jobStatusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn notesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdAtDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn updatedAtDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn jobIDDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn customerIDDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn staffIDDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn promiseDateDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn jobStatusDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn notesDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdAtDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn updatedAtDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemIDDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemNameDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn categoryDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn quantityInStockDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridView dataGridView4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label27;
        // design-time dataset fields removed
        private System.Windows.Forms.DataGridViewTextBoxColumn itemIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn unitPriceDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn reorderLevelDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn supplierNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private GroupWst34DataSet groupWst34DataSet;
        private System.Windows.Forms.BindingSource bindingSource1;
        private GroupWst34DataSetTableAdapters.JobsTableAdapter jobsTableAdapter;
        private System.Windows.Forms.BindingSource bindingSource2;
        private GroupWst34DataSetTableAdapters.InventoryItemsTableAdapter inventoryItemsTableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn21;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn22;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn23;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn24;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn25;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalMaterialCostDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn labourCostDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn transportCostDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn taxAmountDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn grandTotalDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn26;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn27;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn28;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn29;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn30;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn31;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn32;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn33;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn34;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn35;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn36;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button btnHelpTech;
        private System.Windows.Forms.Button btnTechDash;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn14;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn15;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn16;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn18;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn20;
        private System.Windows.Forms.DataGridViewTextBoxColumn JobID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerID;
        private System.Windows.Forms.DataGridViewTextBoxColumn StaffID;
        private System.Windows.Forms.DataGridViewTextBoxColumn JobStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn PromiseDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Notes;
        private System.Windows.Forms.TableLayoutPanel dashboardLayout;
        private System.Windows.Forms.TableLayoutPanel dashboardCards;
        private System.Windows.Forms.FlowLayoutPanel dashboardActions;
        private System.Windows.Forms.Label lblDashboardTitle;
        private System.Windows.Forms.Button btnDashboardRefresh;
        private System.Windows.Forms.Label lblLowStockEmpty;
        private System.Windows.Forms.ComboBox _jobStatus;
        private System.Windows.Forms.TextBox _jobSearch;
        private System.Windows.Forms.FlowLayoutPanel jobsToolbar;
        private System.Windows.Forms.Button btnJobsRefresh;
        private System.Windows.Forms.Button btnJobDetails;
    }
}
