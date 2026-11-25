using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class CredentialEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label UsernameLabel;
        internal TextBox UsernameField;
        internal Label RoleLabel;
        internal ComboBox RoleField;
        internal Label EmailLabel;
        internal TextBox EmailField;
        internal Label NewPasswordOptionalLabel;
        internal TextBox NewPasswordOptionalField;
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
            this.UsernameLabel = new Label();
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Text = "Username";
            this.UsernameLabel.AutoSize = true;
            this.Fields.Controls.Add(this.UsernameLabel);
            this.UsernameField = new TextBox();
            this.UsernameField.Name = "UsernameField";
            this.UsernameField.AccessibleName = "Username";
            this.UsernameField.Width = 520;
            this.UsernameField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.UsernameField);
            this.RoleLabel = new Label();
            this.RoleLabel.Name = "RoleLabel";
            this.RoleLabel.Text = "Role";
            this.RoleLabel.AutoSize = true;
            this.Fields.Controls.Add(this.RoleLabel);
            this.RoleField = new ComboBox();
            this.RoleField.Name = "RoleField";
            this.RoleField.AccessibleName = "Role";
            this.RoleField.Width = 520;
            this.RoleField.Margin = new Padding(3, 3, 3, 12);
            this.RoleField.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Fields.Controls.Add(this.RoleField);
            this.EmailLabel = new Label();
            this.EmailLabel.Name = "EmailLabel";
            this.EmailLabel.Text = "Email";
            this.EmailLabel.AutoSize = true;
            this.Fields.Controls.Add(this.EmailLabel);
            this.EmailField = new TextBox();
            this.EmailField.Name = "EmailField";
            this.EmailField.AccessibleName = "Email";
            this.EmailField.Width = 520;
            this.EmailField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.EmailField);
            this.NewPasswordOptionalLabel = new Label();
            this.NewPasswordOptionalLabel.Name = "NewPasswordOptionalLabel";
            this.NewPasswordOptionalLabel.Text = "New password (optional)";
            this.NewPasswordOptionalLabel.AutoSize = true;
            this.Fields.Controls.Add(this.NewPasswordOptionalLabel);
            this.NewPasswordOptionalField = new TextBox();
            this.NewPasswordOptionalField.Name = "NewPasswordOptionalField";
            this.NewPasswordOptionalField.AccessibleName = "New password (optional)";
            this.NewPasswordOptionalField.Width = 520;
            this.NewPasswordOptionalField.Margin = new Padding(3, 3, 3, 12);
            this.NewPasswordOptionalField.UseSystemPasswordChar = true;
            this.Fields.Controls.Add(this.NewPasswordOptionalField);
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
