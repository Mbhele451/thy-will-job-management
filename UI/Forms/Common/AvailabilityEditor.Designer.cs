using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class AvailabilityEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label AvailabilityLabel;
        internal ComboBox AvailabilityField;

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
            this.AvailabilityLabel = new Label();
            this.AvailabilityLabel.Name = "AvailabilityLabel";
            this.AvailabilityLabel.Text = "Availability";
            this.AvailabilityLabel.AutoSize = true;
            this.Fields.Controls.Add(this.AvailabilityLabel);
            this.AvailabilityField = new ComboBox();
            this.AvailabilityField.Name = "AvailabilityField";
            this.AvailabilityField.AccessibleName = "Availability";
            this.AvailabilityField.Width = 520;
            this.AvailabilityField.Margin = new Padding(3, 3, 3, 12);
            this.AvailabilityField.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Fields.Controls.Add(this.AvailabilityField);
            this.Size = new System.Drawing.Size(760, 500);
            this.ResumeLayout(false);
        }
    }
}
