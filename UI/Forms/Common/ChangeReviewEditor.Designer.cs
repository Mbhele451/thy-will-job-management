using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class ChangeReviewEditor
    {
        internal FlowLayoutPanel Fields;
        internal Label MaterialsUsedReturnedLabel;
        internal DataGridView MaterialsUsedReturnedField;
        internal Label DecisionLabel;
        internal ComboBox DecisionField;
        internal Label DecisionNoteLabel;
        internal TextBox DecisionNoteField;
        internal Label ApprovedDateRescheduleOnlyLabel;
        internal DateTimePicker ApprovedDateRescheduleOnlyField;
        internal Label AvailableTechnicianLabel;
        internal ComboBox AvailableTechnicianField;

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
            this.MaterialsUsedReturnedLabel = new Label();
            this.MaterialsUsedReturnedLabel.Name = "MaterialsUsedReturnedLabel";
            this.MaterialsUsedReturnedLabel.Text = "Materials used / returned";
            this.MaterialsUsedReturnedLabel.AutoSize = true;
            this.Fields.Controls.Add(this.MaterialsUsedReturnedLabel);
            this.MaterialsUsedReturnedField = new DataGridView();
            this.MaterialsUsedReturnedField.Name = "MaterialsUsedReturnedField";
            this.MaterialsUsedReturnedField.AccessibleName = "Materials used / returned";
            this.MaterialsUsedReturnedField.Width = 520;
            this.MaterialsUsedReturnedField.Margin = new Padding(3, 3, 3, 12);
            this.MaterialsUsedReturnedField.Height = 180;
            this.Fields.Controls.Add(this.MaterialsUsedReturnedField);
            this.DecisionLabel = new Label();
            this.DecisionLabel.Name = "DecisionLabel";
            this.DecisionLabel.Text = "Decision";
            this.DecisionLabel.AutoSize = true;
            this.Fields.Controls.Add(this.DecisionLabel);
            this.DecisionField = new ComboBox();
            this.DecisionField.Name = "DecisionField";
            this.DecisionField.AccessibleName = "Decision";
            this.DecisionField.Width = 520;
            this.DecisionField.Margin = new Padding(3, 3, 3, 12);
            this.DecisionField.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Fields.Controls.Add(this.DecisionField);
            this.DecisionNoteLabel = new Label();
            this.DecisionNoteLabel.Name = "DecisionNoteLabel";
            this.DecisionNoteLabel.Text = "Decision note";
            this.DecisionNoteLabel.AutoSize = true;
            this.Fields.Controls.Add(this.DecisionNoteLabel);
            this.DecisionNoteField = new TextBox();
            this.DecisionNoteField.Name = "DecisionNoteField";
            this.DecisionNoteField.AccessibleName = "Decision note";
            this.DecisionNoteField.Width = 520;
            this.DecisionNoteField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.DecisionNoteField);
            this.ApprovedDateRescheduleOnlyLabel = new Label();
            this.ApprovedDateRescheduleOnlyLabel.Name = "ApprovedDateRescheduleOnlyLabel";
            this.ApprovedDateRescheduleOnlyLabel.Text = "Approved date (reschedule only)";
            this.ApprovedDateRescheduleOnlyLabel.AutoSize = true;
            this.Fields.Controls.Add(this.ApprovedDateRescheduleOnlyLabel);
            this.ApprovedDateRescheduleOnlyField = new DateTimePicker();
            this.ApprovedDateRescheduleOnlyField.Name = "ApprovedDateRescheduleOnlyField";
            this.ApprovedDateRescheduleOnlyField.AccessibleName = "Approved date (reschedule only)";
            this.ApprovedDateRescheduleOnlyField.Width = 520;
            this.ApprovedDateRescheduleOnlyField.Margin = new Padding(3, 3, 3, 12);
            this.Fields.Controls.Add(this.ApprovedDateRescheduleOnlyField);
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
            this.Size = new System.Drawing.Size(760, 500);
            this.ResumeLayout(false);
        }
    }
}
