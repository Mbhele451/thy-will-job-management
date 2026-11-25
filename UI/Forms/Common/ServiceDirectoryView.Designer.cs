using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class ServiceDirectoryView
    {
        internal DataGridView Records;
        internal FlowLayoutPanel Toolbar;
        internal Button RefreshButton;
        internal Button EditPricesButton;
        internal Button ManageMaterialsButton;

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
            this.RefreshButton = new Button();
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.AutoSize = true;
            this.RefreshButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.RefreshButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.RefreshButton);
            this.EditPricesButton = new Button();
            this.EditPricesButton.Name = "EditPricesButton";
            this.EditPricesButton.Text = "Edit Prices";
            this.EditPricesButton.AutoSize = true;
            this.EditPricesButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.EditPricesButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.EditPricesButton);
            this.ManageMaterialsButton = new Button();
            this.ManageMaterialsButton.Name = "ManageMaterialsButton";
            this.ManageMaterialsButton.Text = "Manage Materials";
            this.ManageMaterialsButton.AutoSize = true;
            this.ManageMaterialsButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.ManageMaterialsButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.ManageMaterialsButton);
            this.Size = new System.Drawing.Size(960, 560);
            this.ResumeLayout(false);
        }
    }
}
