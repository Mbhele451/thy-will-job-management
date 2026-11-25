using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class ProfileEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label FullNameLabel;
        internal TextBox FullNameField;
        internal Label PhoneLabel;
        internal TextBox PhoneField;
        internal Label EmailLabel;
        internal TextBox EmailField;

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
            this.FullNameLabel = new Label();
            this.FullNameLabel.Name = "FullNameLabel";
            this.FullNameLabel.Text = "Full name";
            this.FullNameLabel.AutoSize = true;
            this.Fields.Controls.Add(this.FullNameLabel);
            this.FullNameField = new TextBox();
            this.FullNameField.Name = "FullNameField";
            this.FullNameField.AccessibleName = "Full name";
            this.FullNameField.Width = 520;
            this.FullNameField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.FullNameField);
            this.PhoneLabel = new Label();
            this.PhoneLabel.Name = "PhoneLabel";
            this.PhoneLabel.Text = "Phone";
            this.PhoneLabel.AutoSize = true;
            this.Fields.Controls.Add(this.PhoneLabel);
            this.PhoneField = new TextBox();
            this.PhoneField.Name = "PhoneField";
            this.PhoneField.AccessibleName = "Phone";
            this.PhoneField.Width = 520;
            this.PhoneField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.PhoneField);
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
            this.Size = new System.Drawing.Size(760, 500);
            this.ResumeLayout(false);
        }
    }
}
