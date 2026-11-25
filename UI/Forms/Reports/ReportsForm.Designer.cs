namespace Major_Project.UI.Forms.Reports
{
    partial class ReportsForm
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
            this.webViewReport = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webViewReport)).BeginInit();
            this.SuspendLayout();
            // 
            // webViewReport
            // 
            this.webViewReport.AllowExternalDrop = true;
            this.webViewReport.CreationProperties = null;
            this.webViewReport.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webViewReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webViewReport.Location = new System.Drawing.Point(0, 0);
            this.webViewReport.Name = "webViewReport";
            this.webViewReport.Size = new System.Drawing.Size(800, 450);
            this.webViewReport.TabIndex = 0;
            this.webViewReport.ZoomFactor = 1D;
            // 
            // Reports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.webViewReport);
            this.Name = "Reports";
            this.Text = "Reports";
            this.Load += new System.EventHandler(this.Reports_Load);
            ((System.ComponentModel.ISupportInitialize)(this.webViewReport)).EndInit();
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

        private Microsoft.Web.WebView2.WinForms.WebView2 webViewReport;
    }
}