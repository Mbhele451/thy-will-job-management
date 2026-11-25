using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class HandoverEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label PromiseDateLabel;
        internal DateTimePicker PromiseDateField;
        internal Label AvailableTechnicianLabel;
        internal ComboBox AvailableTechnicianField;
        internal Label HandoverReasonLabel;
        internal TextBox HandoverReasonField;

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
            this.PromiseDateLabel = new Label();
            this.PromiseDateLabel.Name = "PromiseDateLabel";
            this.PromiseDateLabel.Text = "Promise date";
            this.PromiseDateLabel.AutoSize = true;
            this.Fields.Controls.Add(this.PromiseDateLabel);
            this.PromiseDateField = new DateTimePicker();
            this.PromiseDateField.Name = "PromiseDateField";
            this.PromiseDateField.AccessibleName = "Promise date";
            this.PromiseDateField.Width = 520;
            this.PromiseDateField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.PromiseDateField);
            this.AvailableTechnicianLabel = new Label();
            this.AvailableTechnicianLabel.Name = "AvailableTechnicianLabel";
            this.AvailableTechnicianLabel.Text = "Available technician";
            this.AvailableTechnicianLabel.AutoSize = true;
            this.Fields.Controls.Add(this.AvailableTechnicianLabel);
            this.AvailableTechnicianField = new ComboBox();
            this.AvailableTechnicianField.Name = "AvailableTechnicianField";
            this.AvailableTechnicianField.AccessibleName = "Available technician";
            this.AvailableTechnicianField.Width = 520;
            this.AvailableTechnicianField.Margin = new Padding(3, 3, 3, 12);
            this.AvailableTechnicianField.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Fields.Controls.Add(this.AvailableTechnicianField);
            this.HandoverReasonLabel = new Label();
            this.HandoverReasonLabel.Name = "HandoverReasonLabel";
            this.HandoverReasonLabel.Text = "Handover reason";
            this.HandoverReasonLabel.AutoSize = true;
            this.Fields.Controls.Add(this.HandoverReasonLabel);
            this.HandoverReasonField = new TextBox();
            this.HandoverReasonField.Name = "HandoverReasonField";
            this.HandoverReasonField.AccessibleName = "Handover reason";
            this.HandoverReasonField.Width = 520;
            this.HandoverReasonField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.HandoverReasonField);
            this.Size = new System.Drawing.Size(760, 500);
            this.ResumeLayout(false);
        }
    }
}
