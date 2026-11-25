using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class SettingEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label SettingValueLabel;
        internal TextBox SettingValueField;

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
            this.SettingValueLabel = new Label();
            this.SettingValueLabel.Name = "SettingValueLabel";
            this.SettingValueLabel.Text = "Setting value";
            this.SettingValueLabel.AutoSize = true;
            this.Fields.Controls.Add(this.SettingValueLabel);
            this.SettingValueField = new TextBox();
            this.SettingValueField.Name = "SettingValueField";
            this.SettingValueField.AccessibleName = "Setting value";
            this.SettingValueField.Width = 520;
            this.SettingValueField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.SettingValueField);
            this.Size = new System.Drawing.Size(760, 500);
            this.ResumeLayout(false);
        }
    }
}
