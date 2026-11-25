using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class ServicePriceEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label ServiceNameLabel;
        internal TextBox ServiceNameField;
        internal Label LabourCostLabel;
        internal NumericUpDown LabourCostField;
        internal Label TransportCostLabel;
        internal NumericUpDown TransportCostField;
        internal Label VatPercentageLabel;
        internal NumericUpDown VatPercentageField;

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
            this.ServiceNameLabel = new Label();
            this.ServiceNameLabel.Name = "ServiceNameLabel";
            this.ServiceNameLabel.Text = "Service name";
            this.ServiceNameLabel.AutoSize = true;
            this.Fields.Controls.Add(this.ServiceNameLabel);
            this.ServiceNameField = new TextBox();
            this.ServiceNameField.Name = "ServiceNameField";
            this.ServiceNameField.AccessibleName = "Service name";
            this.ServiceNameField.Width = 520;
            this.ServiceNameField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.ServiceNameField);
            this.LabourCostLabel = new Label();
            this.LabourCostLabel.Name = "LabourCostLabel";
            this.LabourCostLabel.Text = "Labour cost";
            this.LabourCostLabel.AutoSize = true;
            this.Fields.Controls.Add(this.LabourCostLabel);
            this.LabourCostField = new NumericUpDown();
            this.LabourCostField.Name = "LabourCostField";
            this.LabourCostField.AccessibleName = "Labour cost";
            this.LabourCostField.Width = 520;
            this.LabourCostField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.LabourCostField);
            this.TransportCostLabel = new Label();
            this.TransportCostLabel.Name = "TransportCostLabel";
            this.TransportCostLabel.Text = "Transport cost";
            this.TransportCostLabel.AutoSize = true;
            this.Fields.Controls.Add(this.TransportCostLabel);
            this.TransportCostField = new NumericUpDown();
            this.TransportCostField.Name = "TransportCostField";
            this.TransportCostField.AccessibleName = "Transport cost";
            this.TransportCostField.Width = 520;
            this.TransportCostField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.TransportCostField);
            this.VatPercentageLabel = new Label();
            this.VatPercentageLabel.Name = "VatPercentageLabel";
            this.VatPercentageLabel.Text = "VAT percentage";
            this.VatPercentageLabel.AutoSize = true;
            this.Fields.Controls.Add(this.VatPercentageLabel);
            this.VatPercentageField = new NumericUpDown();
            this.VatPercentageField.Name = "VatPercentageField";
            this.VatPercentageField.AccessibleName = "VAT percentage";
            this.VatPercentageField.Width = 520;
            this.VatPercentageField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.VatPercentageField);
            this.Size = new System.Drawing.Size(760, 500);
            this.ResumeLayout(false);
        }
    }
}
