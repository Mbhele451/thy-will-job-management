namespace Major_Project.UI.Forms.Common
{
    partial class PdfViewerForm
    {
        private System.ComponentModel.IContainer components;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        internal System.Windows.Forms.Label workflowFeedback;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pdfBrowser = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.viewerToolbar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSaveCopy = new System.Windows.Forms.Button();
            this.btnCloseViewer = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.savePdfDialog = new System.Windows.Forms.SaveFileDialog();
            this.components.Add(this.savePdfDialog);
            ((System.ComponentModel.ISupportInitialize)(this.pdfBrowser)).BeginInit();
            this.SuspendLayout();
            this.pdfBrowser.Name = "pdfBrowser";
            this.pdfBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewerToolbar.Name = "viewerToolbar";
            this.viewerToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.viewerToolbar.Height = 56;
            this.viewerToolbar.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.viewerToolbar.Controls.Add(this.btnSaveCopy);
            this.viewerToolbar.Controls.Add(this.btnCloseViewer);
            this.btnSaveCopy.Name = "btnSaveCopy";
            this.btnSaveCopy.Text = "Save a copy";
            this.btnSaveCopy.Size = new System.Drawing.Size(132, 36);
            this.btnSaveCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCopy.BackColor = System.Drawing.Color.FromArgb(40, 91, 156);
            this.btnSaveCopy.ForeColor = System.Drawing.Color.White;
            this.btnSaveCopy.Click += new System.EventHandler(this.SaveCopy_Click);
            this.btnCloseViewer.Name = "btnCloseViewer";
            this.btnCloseViewer.Text = "Close";
            this.btnCloseViewer.Size = new System.Drawing.Size(112, 36);
            this.btnCloseViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCloseViewer.Click += new System.EventHandler(this.CloseViewer_Click);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "Loading PDF…";
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Height = 48;
            this.lblStatus.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.savePdfDialog.Filter = "PDF documents (*.pdf)|*.pdf";
            this.savePdfDialog.DefaultExt = "pdf";
            this.savePdfDialog.AddExtension = true;
            this.savePdfDialog.OverwritePrompt = true;
            this.Controls.Add(this.pdfBrowser);
            this.Controls.Add(this.viewerToolbar);
            this.Controls.Add(this.lblStatus);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.BackColor = System.Drawing.Color.FromArgb(237, 243, 252);
            this.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.ClientSize = new System.Drawing.Size(1100, 800);
            this.MinimumSize = new System.Drawing.Size(720, 520);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Name = "PdfViewerForm";
            this.Text = "THY WILL | PDF viewer";
            this.Shown += new System.EventHandler(this.PdfViewerForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pdfBrowser)).EndInit();
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

        private Microsoft.Web.WebView2.WinForms.WebView2 pdfBrowser;
        private System.Windows.Forms.FlowLayoutPanel viewerToolbar;
        private System.Windows.Forms.Button btnSaveCopy;
        private System.Windows.Forms.Button btnCloseViewer;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.SaveFileDialog savePdfDialog;
    }
}
