using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class PasswordEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label VerificationCodeLabel;
        internal TextBox VerificationCodeField;
        internal Label NewPasswordLabel;
        internal TextBox NewPasswordField;
        internal Label ConfirmNewPasswordLabel;
        internal TextBox ConfirmNewPasswordField;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Fields = new FlowLayoutPanel();
            this.Fields.Name = "Fields";
            this.Fields.Dock = DockStyle.Fill;
            this.Fields.FlowDirection = FlowDirection.TopDown;
            this.Fields.AutoScroll = true;
            this.Fields.WrapContents = false;
            this.Fields.Padding = new Padding(16);
            this.Controls.Add(this.Fields);
            this.VerificationCodeLabel = new Label();
            this.VerificationCodeLabel.Name = "VerificationCodeLabel";
            this.VerificationCodeLabel.Text = "Verification code";
            this.VerificationCodeLabel.AutoSize = true;
            this.Fields.Controls.Add(this.VerificationCodeLabel);
            this.VerificationCodeField = new TextBox();
            this.VerificationCodeField.Name = "VerificationCodeField";
            this.VerificationCodeField.AccessibleName = "Verification code";
            this.VerificationCodeField.Width = 520;
            this.VerificationCodeField.Margin = new Padding(3, 3, 3, 12);
            this.VerificationCodeField.MaxLength = 6;
            this.Fields.Controls.Add(this.VerificationCodeField);
            this.NewPasswordLabel = new Label();
            this.NewPasswordLabel.Name = "NewPasswordLabel";
            this.NewPasswordLabel.Text = "New password";
            this.NewPasswordLabel.AutoSize = true;
            this.Fields.Controls.Add(this.NewPasswordLabel);
            this.NewPasswordField = new TextBox();
            this.NewPasswordField.Name = "NewPasswordField";
            this.NewPasswordField.AccessibleName = "New password";
            this.NewPasswordField.Width = 520;
            this.NewPasswordField.Margin = new Padding(3, 3, 3, 12);
            this.NewPasswordField.UseSystemPasswordChar = true;
            this.Fields.Controls.Add(this.NewPasswordField);
            this.ConfirmNewPasswordLabel = new Label();
            this.ConfirmNewPasswordLabel.Name = "ConfirmNewPasswordLabel";
            this.ConfirmNewPasswordLabel.Text = "Confirm new password";
            this.ConfirmNewPasswordLabel.AutoSize = true;
            this.Fields.Controls.Add(this.ConfirmNewPasswordLabel);
            this.ConfirmNewPasswordField = new TextBox();
            this.ConfirmNewPasswordField.Name = "ConfirmNewPasswordField";
            this.ConfirmNewPasswordField.AccessibleName = "Confirm new password";
            this.ConfirmNewPasswordField.Width = 520;
            this.ConfirmNewPasswordField.Margin = new Padding(3, 3, 3, 12);
            this.ConfirmNewPasswordField.UseSystemPasswordChar = true;
            this.Fields.Controls.Add(this.ConfirmNewPasswordField);
            this.Size = new System.Drawing.Size(760, 500);
            this.ResumeLayout(false);
        }
    }
}
