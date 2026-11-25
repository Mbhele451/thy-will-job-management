using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class CustomerContactEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label PhoneLabel;
        internal TextBox PhoneField;
        internal Label EmailLabel;
        internal TextBox EmailField;
        internal Label FullAddressLabel;
        internal TextBox FullAddressField;
        internal Label ServiceAreaLabel;
        internal TextBox ServiceAreaField;

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
            this.FullAddressLabel = new Label();
            this.FullAddressLabel.Name = "FullAddressLabel";
            this.FullAddressLabel.Text = "Full address";
            this.FullAddressLabel.AutoSize = true;
            this.Fields.Controls.Add(this.FullAddressLabel);
            this.FullAddressField = new TextBox();
            this.FullAddressField.Name = "FullAddressField";
            this.FullAddressField.AccessibleName = "Full address";
            this.FullAddressField.Width = 520;
            this.FullAddressField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.FullAddressField);
            this.ServiceAreaLabel = new Label();
            this.ServiceAreaLabel.Name = "ServiceAreaLabel";
            this.ServiceAreaLabel.Text = "Service area";
            this.ServiceAreaLabel.AutoSize = true;
            this.Fields.Controls.Add(this.ServiceAreaLabel);
            this.ServiceAreaField = new TextBox();
            this.ServiceAreaField.Name = "ServiceAreaField";
            this.ServiceAreaField.AccessibleName = "Service area";
            this.ServiceAreaField.Width = 520;
            this.ServiceAreaField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.ServiceAreaField);
            this.Size = new System.Drawing.Size(760, 500);
            this.ResumeLayout(false);
        }
    }
}
