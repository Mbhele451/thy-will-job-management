using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class WorkflowFrame
    {
        internal Panel Body;
        internal FlowLayoutPanel Header;
        internal Button Back;
        internal Label Heading;
        internal Label Notes;
        internal FlowLayoutPanel Actions;
        internal Button HelpAction;
        internal Button SubmitAction;

        internal System.Windows.Forms.Label workflowFeedback;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Body = new Panel();
            this.Body.Name = "Body";
            this.Body.Dock = DockStyle.Fill;
            this.Controls.Add(this.Body);
            this.Header = new FlowLayoutPanel();
            this.Header.Name = "Header";
            this.Header.Dock = DockStyle.Top;
            this.Header.Height = 48;
            this.Controls.Add(this.Header);
            this.Back = new Button();
            this.Back.Name = "Back";
            this.Back.Text = "Back";
            this.Back.AutoSize = true;
            this.Back.AccessibleName = "Back to previous view";
            this.Header.Controls.Add(this.Back);
            this.Heading = new Label();
            this.Heading.Name = "Heading";
            this.Heading.Text = "Workflow";
            this.Heading.AutoSize = true;
            this.Header.Controls.Add(this.Heading);
            this.Notes = new Label();
            this.Notes.Name = "Notes";
            this.Notes.Dock = DockStyle.Top;
            this.Notes.Text = "Instructions";
            this.Notes.AutoSize = true;
            this.Notes.MaximumSize = new System.Drawing.Size(720, 0);
            this.Controls.Add(this.Notes);
            this.Actions = new FlowLayoutPanel();
            this.Actions.Name = "Actions";
            this.Actions.Dock = DockStyle.Bottom;
            this.Actions.Height = 46;
            this.Controls.Add(this.Actions);
            this.HelpAction = new Button();
            this.HelpAction.Name = "HelpAction";
            this.HelpAction.Text = "Help";
            this.HelpAction.AutoSize = true;
            this.Actions.Controls.Add(this.HelpAction);
            this.SubmitAction = new Button();
            this.SubmitAction.Name = "SubmitAction";
            this.SubmitAction.Text = "Save";
            this.SubmitAction.AutoSize = true;
            this.Actions.Controls.Add(this.SubmitAction);
            this.Size = new System.Drawing.Size(760, 500);
            this.workflowFeedback = new System.Windows.Forms.Label();
            this.workflowFeedback.Name = "WorkflowFeedback";
            this.workflowFeedback.Text = "Status message";
            this.workflowFeedback.Dock = System.Windows.Forms.DockStyle.Top;
            this.workflowFeedback.Height = 46;
            this.workflowFeedback.Visible = false;
            this.Controls.Add(this.workflowFeedback);
            this.workflowFeedback.SendToBack();
            this.ResumeLayout(false);
        }
    }
}
