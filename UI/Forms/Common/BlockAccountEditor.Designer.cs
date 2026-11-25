using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class BlockAccountEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label ReasonLabel;
        internal TextBox ReasonField;

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
            this.ReasonLabel = new Label();
            this.ReasonLabel.Name = "ReasonLabel";
            this.ReasonLabel.Text = "Reason";
            this.ReasonLabel.AutoSize = true;
            this.Fields.Controls.Add(this.ReasonLabel);
            this.ReasonField = new TextBox();
            this.ReasonField.Name = "ReasonField";
            this.ReasonField.AccessibleName = "Reason";
            this.ReasonField.Width = 520;
            this.ReasonField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.ReasonField);
            this.Size = new System.Drawing.Size(760, 500);
            this.ResumeLayout(false);
        }
    }
}
