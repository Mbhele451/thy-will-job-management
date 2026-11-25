using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class FilePathEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label FilePathLabel;
        internal TextBox FilePathField;

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
            this.FilePathLabel = new Label();
            this.FilePathLabel.Name = "FilePathLabel";
            this.FilePathLabel.Text = "File path";
            this.FilePathLabel.AutoSize = true;
            this.Fields.Controls.Add(this.FilePathLabel);
            this.FilePathField = new TextBox();
            this.FilePathField.Name = "FilePathField";
            this.FilePathField.AccessibleName = "File path";
            this.FilePathField.Width = 520;
            this.FilePathField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.FilePathField);
            this.Size = new System.Drawing.Size(760, 500);
            this.ResumeLayout(false);
        }
    }
}
