using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class ResetPasswordForm
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
        internal System.Windows.Forms.Label workflowFeedback;

        private void InitializeComponent()
        {
            this.lblAuthTitle = new System.Windows.Forms.Label();
            this.lblAuthSubtitle = new System.Windows.Forms.Label();
            this.authLayout = new System.Windows.Forms.TableLayoutPanel();
            this.checklistStatus1 = new System.Windows.Forms.Label();
            this.checklistStatus2 = new System.Windows.Forms.Label();
            this.checklistStatus3 = new System.Windows.Forms.Label();
            this.checklistStatus4 = new System.Windows.Forms.Label();
            this.checklistStatus5 = new System.Windows.Forms.Label();
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelPasswordChecklist = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.panelPasswordChecklist.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panelPasswordChecklist);
            this.panel1.Controls.Add(this.btnHelp);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textBox4);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // panelPasswordChecklist
            // 
            this.panelPasswordChecklist.Controls.Add(this.label9);
            this.panelPasswordChecklist.Controls.Add(this.label8);
            this.panelPasswordChecklist.Controls.Add(this.label7);
            this.panelPasswordChecklist.Controls.Add(this.label6);
            this.panelPasswordChecklist.Controls.Add(this.label5);
            this.panelPasswordChecklist.Location = new System.Drawing.Point(402, 262);
            this.panelPasswordChecklist.Name = "panelPasswordChecklist";
            this.panelPasswordChecklist.Size = new System.Drawing.Size(310, 156);
            this.panelPasswordChecklist.TabIndex = 12;
            this.panelPasswordChecklist.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(30, 112);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(270, 26);
            this.label9.TabIndex = 18;
            this.label9.Text = "At least one special character";
            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 84);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(270, 26);
            this.label8.TabIndex = 17;
            this.label8.Text = "At least one number";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(30, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(270, 26);
            this.label7.TabIndex = 16;
            this.label7.Text = "At least one uppercase letter";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(270, 26);
            this.label6.TabIndex = 15;
            this.label6.Text = "At least one lowercase letter";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(270, 26);
            this.label5.TabIndex = 14;
            this.label5.Text = "At least 8 characters";
            this.label5.Click += new System.EventHandler(this.label5_Click_1);
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.btnHelp.BackColor = System.Drawing.Color.FromArgb(249, 251, 255);
            this.btnHelp.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnHelp.Location = new System.Drawing.Point(216, 469);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(172, 39);
            this.btnHelp.TabIndex = 11;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = false;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.button3.BackColor = System.Drawing.Color.FromArgb(249, 251, 255);
            this.button3.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.button3.Location = new System.Drawing.Point(36, 469);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(172, 39);
            this.button3.TabIndex = 10;
            this.button3.Text = "Back to Sign In";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.button2.BackColor = System.Drawing.Color.FromArgb(40, 91, 156);
            this.button2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.button2.Location = new System.Drawing.Point(36, 415);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(352, 42);
            this.button2.TabIndex = 9;
            this.button2.Text = "Reset Password";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.button1.BackColor = System.Drawing.Color.FromArgb(40, 91, 156);
            this.button1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.button1.Location = new System.Drawing.Point(402, 140);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(130, 39);
            this.button1.TabIndex = 8;
            this.button1.Text = "Send Code";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label4.Location = new System.Drawing.Point(36, 332);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(300, 26);
            this.label4.TabIndex = 7;
            this.label4.Text = "Confirm password";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label3.Location = new System.Drawing.Point(36, 262);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(300, 26);
            this.label3.TabIndex = 6;
            this.label3.Text = "New password";
            // 
            // textBox4
            // 
            this.textBox4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.textBox4.Location = new System.Drawing.Point(36, 359);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(352, 28);
            this.textBox4.TabIndex = 5;
            this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // textBox3
            // 
            this.textBox3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.textBox3.Location = new System.Drawing.Point(36, 289);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(352, 28);
            this.textBox3.TabIndex = 4;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.textBox2.Location = new System.Drawing.Point(36, 219);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(352, 28);
            this.textBox2.TabIndex = 3;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.textBox1.Location = new System.Drawing.Point(36, 145);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(352, 28);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label2.Location = new System.Drawing.Point(36, 192);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(300, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "Verification code";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label1.Location = new System.Drawing.Point(36, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Email address";
            // 
            // ResetPasswordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.authLayout);
            this.Name = "ResetPasswordForm";
            this.Text = "ResetPasswordForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ResetPasswordForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelPasswordChecklist.ResumeLayout(false);
            this.panelPasswordChecklist.PerformLayout();

            this.lblAuthTitle.Name = "lblAuthTitle";
            this.lblAuthSubtitle.Name = "lblAuthSubtitle";
            this.authLayout.Name = "authLayout";
            this.checklistStatus1.Name = "checklistStatus1";
            this.checklistStatus2.Name = "checklistStatus2";
            this.checklistStatus3.Name = "checklistStatus3";
            this.checklistStatus4.Name = "checklistStatus4";
            this.checklistStatus5.Name = "checklistStatus5";
            this.authLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.authLayout.ColumnCount = 3;
            this.authLayout.RowCount = 3;
            this.authLayout.AutoScroll = true;
            this.authLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.authLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 740F));
            this.authLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.authLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.authLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 550F));
            this.authLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.authLayout.Controls.Add(this.panel1, 1, 1);
            this.panel1.BackgroundImage = null;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(249, 251, 255);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.lblAuthTitle.Text = "Reset password";
            this.lblAuthTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblAuthTitle.Location = new System.Drawing.Point(36, 24);
            this.lblAuthTitle.Size = new System.Drawing.Size(680, 40);
            this.lblAuthTitle.AutoSize = true;
            this.lblAuthSubtitle.Text = "Request a verification code to reset your password.";
            this.lblAuthSubtitle.Location = new System.Drawing.Point(36, 68);
            this.lblAuthSubtitle.Size = new System.Drawing.Size(680, 30);
            this.lblAuthSubtitle.AutoSize = true;
            this.panel1.Controls.Add(this.lblAuthTitle);
            this.panel1.Controls.Add(this.lblAuthSubtitle);
            this.checklistStatus1.Location = new System.Drawing.Point(0, 0);
            this.checklistStatus1.Size = new System.Drawing.Size(26, 26);
            this.checklistStatus1.Text = "✘";
            this.checklistStatus1.ForeColor = System.Drawing.Color.FromArgb(185, 50, 66);
            this.panelPasswordChecklist.Controls.Add(this.checklistStatus1);
            this.label5.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.checklistStatus2.Location = new System.Drawing.Point(0, 28);
            this.checklistStatus2.Size = new System.Drawing.Size(26, 26);
            this.checklistStatus2.Text = "✘";
            this.checklistStatus2.ForeColor = System.Drawing.Color.FromArgb(185, 50, 66);
            this.panelPasswordChecklist.Controls.Add(this.checklistStatus2);
            this.label6.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.checklistStatus3.Location = new System.Drawing.Point(0, 56);
            this.checklistStatus3.Size = new System.Drawing.Size(26, 26);
            this.checklistStatus3.Text = "✘";
            this.checklistStatus3.ForeColor = System.Drawing.Color.FromArgb(185, 50, 66);
            this.panelPasswordChecklist.Controls.Add(this.checklistStatus3);
            this.label7.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.checklistStatus4.Location = new System.Drawing.Point(0, 84);
            this.checklistStatus4.Size = new System.Drawing.Size(26, 26);
            this.checklistStatus4.Text = "✘";
            this.checklistStatus4.ForeColor = System.Drawing.Color.FromArgb(185, 50, 66);
            this.panelPasswordChecklist.Controls.Add(this.checklistStatus4);
            this.label8.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.checklistStatus5.Location = new System.Drawing.Point(0, 112);
            this.checklistStatus5.Size = new System.Drawing.Size(26, 26);
            this.checklistStatus5.Text = "✘";
            this.checklistStatus5.ForeColor = System.Drawing.Color.FromArgb(185, 50, 66);
            this.panelPasswordChecklist.Controls.Add(this.checklistStatus5);
            this.label9.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.textBox3.UseSystemPasswordChar = true;
            this.textBox4.UseSystemPasswordChar = true;
            this.panelPasswordChecklist.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.lblAuthTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblAuthTitle.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.lblAuthSubtitle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblAuthSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblAuthSubtitle.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.authLayout.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.authLayout.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.checklistStatus1.BackColor = System.Drawing.Color.Transparent;
            this.checklistStatus1.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.checklistStatus2.BackColor = System.Drawing.Color.Transparent;
            this.checklistStatus2.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.checklistStatus3.BackColor = System.Drawing.Color.Transparent;
            this.checklistStatus3.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.checklistStatus4.BackColor = System.Drawing.Color.Transparent;
            this.checklistStatus4.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.checklistStatus5.BackColor = System.Drawing.Color.Transparent;
            this.checklistStatus5.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.panel1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.panel1.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.panelPasswordChecklist.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.panelPasswordChecklist.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.label9.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.label8.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.label7.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.label6.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.label5.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.btnHelp.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.btnHelp.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelp.BackgroundImage = null;
            this.btnHelp.Image = null;
            this.button3.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.button3.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.BackgroundImage = null;
            this.button3.Image = null;
            this.button2.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.BackgroundImage = null;
            this.button2.Image = null;
            this.button1.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.BackgroundImage = null;
            this.button1.Image = null;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.textBox4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.textBox4.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.textBox3.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.textBox3.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.textBox2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.textBox2.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.textBox1.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.panel1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.BackColor = System.Drawing.Color.FromArgb(237, 243, 252);
            this.Text = "THY WILL | ResetPassword";
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
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

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnHelp;
        private Panel panelPasswordChecklist;
        private Label label5;
        private Label label7;
        private Label label6;
        private Label label8;
        private Label label9;
        private System.Windows.Forms.Label lblAuthTitle;
        private System.Windows.Forms.Label lblAuthSubtitle;
        private System.Windows.Forms.TableLayoutPanel authLayout;
        private System.Windows.Forms.Label checklistStatus1;
        private System.Windows.Forms.Label checklistStatus2;
        private System.Windows.Forms.Label checklistStatus3;
        private System.Windows.Forms.Label checklistStatus4;
        private System.Windows.Forms.Label checklistStatus5;
    }
}
