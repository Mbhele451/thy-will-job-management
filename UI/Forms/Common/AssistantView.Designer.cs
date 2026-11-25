using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class AssistantView
    {
        internal RichTextBox Transcript;
        internal FlowLayoutPanel Toolbar;
        internal TextBox Query;
        internal Button AskButton;
        internal Button OpenButton;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Transcript = new RichTextBox();
            this.Transcript.Name = "Transcript";
            this.Transcript.Dock = DockStyle.Fill;
            this.Transcript.ReadOnly = true;
            this.Transcript.DetectUrls = false;
            this.Transcript.Text = "Ask about your jobs or profile.";
            this.Controls.Add(this.Transcript);
            this.Toolbar = new FlowLayoutPanel();
            this.Toolbar.Name = "Toolbar";
            this.Toolbar.Dock = DockStyle.Top;
            this.Toolbar.Height = 48;
            this.Toolbar.AutoScroll = true;
            this.Toolbar.WrapContents = false;
            this.Controls.Add(this.Toolbar);
            this.Query = new TextBox();
            this.Query.Name = "Query";
            this.Query.Width = 340;
            this.Query.MaxLength = 400;
            this.Query.AccessibleName = "Ask THY WILL";
            this.Toolbar.Controls.Add(this.Query);
            this.AskButton = new Button();
            this.AskButton.Name = "AskButton";
            this.AskButton.Text = "Ask";
            this.AskButton.AutoSize = true;
            this.AskButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.AskButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.AskButton);
            this.OpenButton = new Button();
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Text = "Open";
            this.OpenButton.AutoSize = true;
            this.OpenButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.OpenButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.OpenButton);
            this.Size = new System.Drawing.Size(960, 560);
            this.ResumeLayout(false);
        }
    }
}
