using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class StaffProfileView
    {
        internal ProfileDetailsView Details;
        internal FlowLayoutPanel Toolbar;
        internal Button EditMyProfileButton;
        internal Button ChangePasswordButton;
        internal Button SetMyAvailabilityButton;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Details = new ProfileDetailsView();
            this.Details.Name = "Details";
            this.Details.Dock = DockStyle.Fill;
            this.Controls.Add(this.Details);
            this.Toolbar = new FlowLayoutPanel();
            this.Toolbar.Name = "Toolbar";
            this.Toolbar.Dock = DockStyle.Top;
            this.Toolbar.Height = 48;
            this.Toolbar.AutoScroll = true;
            this.Toolbar.WrapContents = false;
            this.Controls.Add(this.Toolbar);
            this.EditMyProfileButton = new Button();
            this.EditMyProfileButton.Name = "EditMyProfileButton";
            this.EditMyProfileButton.Text = "Edit My Profile";
            this.EditMyProfileButton.AutoSize = true;
            this.EditMyProfileButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.EditMyProfileButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.EditMyProfileButton);
            this.ChangePasswordButton = new Button();
            this.ChangePasswordButton.Name = "ChangePasswordButton";
            this.ChangePasswordButton.Text = "Change Password";
            this.ChangePasswordButton.AutoSize = true;
            this.ChangePasswordButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.ChangePasswordButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.ChangePasswordButton);
            this.SetMyAvailabilityButton = new Button();
            this.SetMyAvailabilityButton.Name = "SetMyAvailabilityButton";
            this.SetMyAvailabilityButton.Text = "Set My Availability";
            this.SetMyAvailabilityButton.AutoSize = true;
            this.SetMyAvailabilityButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.SetMyAvailabilityButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.SetMyAvailabilityButton);
            this.Size = new System.Drawing.Size(960, 560);
            this.ResumeLayout(false);
        }
    }
}
