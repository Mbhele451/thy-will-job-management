using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class MaterialQuantityEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label RequiredQuantityLabel;
        internal NumericUpDown RequiredQuantityField;

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
            this.RequiredQuantityLabel = new Label();
            this.RequiredQuantityLabel.Name = "RequiredQuantityLabel";
            this.RequiredQuantityLabel.Text = "Required quantity";
            this.RequiredQuantityLabel.AutoSize = true;
            this.Fields.Controls.Add(this.RequiredQuantityLabel);
            this.RequiredQuantityField = new NumericUpDown();
            this.RequiredQuantityField.Name = "RequiredQuantityField";
            this.RequiredQuantityField.AccessibleName = "Required quantity";
            this.RequiredQuantityField.Width = 520;
            this.RequiredQuantityField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.RequiredQuantityField);
            this.Size = new System.Drawing.Size(760, 500);
            this.ResumeLayout(false);
        }
    }
}
