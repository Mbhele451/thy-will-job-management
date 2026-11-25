namespace Major_Project.UI.Forms.Common
{
    partial class ProfileDetailsView
    {
        private System.Windows.Forms.TextBox txtProfileDetails;
        private void InitializeComponent()
        {
            this.txtProfileDetails = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            this.txtProfileDetails.Name = "txtProfileDetails";
            this.txtProfileDetails.Multiline = true;
            this.txtProfileDetails.ReadOnly = true;
            this.txtProfileDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtProfileDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProfileDetails.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtProfileDetails.BackColor = System.Drawing.Color.FromArgb(249, 251, 255);
            this.txtProfileDetails.ForeColor = System.Drawing.Color.FromArgb(23, 41, 65);
            this.txtProfileDetails.Text = "Select My Profile to load your staff details.";
            this.Controls.Add(this.txtProfileDetails);
            this.Name = "ProfileDetailsView";
            this.Padding = new System.Windows.Forms.Padding(16);
            this.Size = new System.Drawing.Size(720, 480);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
