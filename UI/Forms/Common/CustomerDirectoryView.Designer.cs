using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class CustomerDirectoryView
    {
        internal DataGridView Records;
        internal FlowLayoutPanel Toolbar;
        internal TextBox Query;
        internal Button SearchButton;
        internal Button EditCustomerButton;
        internal Button ActivateDeactivateButton;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Records = new DataGridView();
            this.Records.Name = "Records";
            this.Records.Dock = DockStyle.Fill;
            this.Records.ReadOnly = true;
            this.Records.AllowUserToAddRows = false;
            this.Records.AllowUserToDeleteRows = false;
            this.Records.RowHeadersVisible = false;
            this.Records.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.Records.MultiSelect = false;
            this.Records.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.Controls.Add(this.Records);
            this.Toolbar = new FlowLayoutPanel();
            this.Toolbar.Name = "Toolbar";
            this.Toolbar.Dock = DockStyle.Top;
            this.Toolbar.Height = 48;
            this.Toolbar.AutoScroll = true;
            this.Toolbar.WrapContents = false;
            this.Controls.Add(this.Toolbar);
            this.Query = new TextBox();
            this.Query.Name = "Query";
            this.Query.Width = 185;
            this.Query.MaxLength = 100;
            this.Query.AccessibleName = "Customer name";
            this.Toolbar.Controls.Add(this.Query);
            this.SearchButton = new Button();
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Text = "Search";
            this.SearchButton.AutoSize = true;
            this.SearchButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.SearchButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.SearchButton);
            this.EditCustomerButton = new Button();
            this.EditCustomerButton.Name = "EditCustomerButton";
            this.EditCustomerButton.Text = "Edit Customer";
            this.EditCustomerButton.AutoSize = true;
            this.EditCustomerButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.EditCustomerButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.EditCustomerButton);
            this.ActivateDeactivateButton = new Button();
            this.ActivateDeactivateButton.Name = "ActivateDeactivateButton";
            this.ActivateDeactivateButton.Text = "Activate / Deactivate";
            this.ActivateDeactivateButton.AutoSize = true;
            this.ActivateDeactivateButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.ActivateDeactivateButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.ActivateDeactivateButton);
            this.Size = new System.Drawing.Size(960, 560);
            this.ResumeLayout(false);
        }
    }
}
