using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class ChangeRequestEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label ReasonCharactersLabel;
        internal TextBox ReasonCharactersField;
        internal Label AllocatedMaterialsEditUsedquantityIncludingZeroLabel;
        internal DataGridView AllocatedMaterialsEditUsedquantityIncludingZeroField;

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
            this.ReasonCharactersLabel = new Label();
            this.ReasonCharactersLabel.Name = "ReasonCharactersLabel";
            this.ReasonCharactersLabel.Text = "Reason (10–1,000 characters)";
            this.ReasonCharactersLabel.AutoSize = true;
            this.Fields.Controls.Add(this.ReasonCharactersLabel);
            this.ReasonCharactersField = new TextBox();
            this.ReasonCharactersField.Name = "ReasonCharactersField";
            this.ReasonCharactersField.AccessibleName = "Reason (10–1,000 characters)";
            this.ReasonCharactersField.Width = 520;
            this.ReasonCharactersField.Margin = new Padding(3, 3, 3, 12);
            this.ReasonCharactersField.Multiline = true;
            this.ReasonCharactersField.Height = 90;
            this.Fields.Controls.Add(this.ReasonCharactersField);
            this.AllocatedMaterialsEditUsedquantityIncludingZeroLabel = new Label();
            this.AllocatedMaterialsEditUsedquantityIncludingZeroLabel.Name = "AllocatedMaterialsEditUsedquantityIncludingZeroLabel";
            this.AllocatedMaterialsEditUsedquantityIncludingZeroLabel.Text = "Allocated materials";
            this.AllocatedMaterialsEditUsedquantityIncludingZeroLabel.AutoSize = true;
            this.Fields.Controls.Add(this.AllocatedMaterialsEditUsedquantityIncludingZeroLabel);
            this.AllocatedMaterialsEditUsedquantityIncludingZeroField = new DataGridView();
            this.AllocatedMaterialsEditUsedquantityIncludingZeroField.Name = "AllocatedMaterialsEditUsedquantityIncludingZeroField";
            this.AllocatedMaterialsEditUsedquantityIncludingZeroField.AccessibleName = "Allocated materials";
            this.AllocatedMaterialsEditUsedquantityIncludingZeroField.Width = 520;
            this.AllocatedMaterialsEditUsedquantityIncludingZeroField.Margin = new Padding(3, 3, 3, 12);
            this.AllocatedMaterialsEditUsedquantityIncludingZeroField.Height = 180;
            this.Fields.Controls.Add(this.AllocatedMaterialsEditUsedquantityIncludingZeroField);
            this.Size = new System.Drawing.Size(760, 500);
            this.ResumeLayout(false);
        }
    }
}
