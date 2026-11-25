using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    partial class ServiceMaterialsView
    {
        internal DataGridView Records;
        internal FlowLayoutPanel Toolbar;
        internal Button AddMaterialButton;
        internal Button UpdateQuantityButton;
        internal Button RemoveMaterialButton;

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
            this.AddMaterialButton = new Button();
            this.AddMaterialButton.Name = "AddMaterialButton";
            this.AddMaterialButton.Text = "Add Material";
            this.AddMaterialButton.AutoSize = true;
            this.AddMaterialButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.AddMaterialButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.AddMaterialButton);
            this.UpdateQuantityButton = new Button();
            this.UpdateQuantityButton.Name = "UpdateQuantityButton";
            this.UpdateQuantityButton.Text = "Update Quantity";
            this.UpdateQuantityButton.AutoSize = true;
            this.UpdateQuantityButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.UpdateQuantityButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.UpdateQuantityButton);
            this.RemoveMaterialButton = new Button();
            this.RemoveMaterialButton.Name = "RemoveMaterialButton";
            this.RemoveMaterialButton.Text = "Remove Material";
            this.RemoveMaterialButton.AutoSize = true;
            this.RemoveMaterialButton.MinimumSize = new System.Drawing.Size(0, 34);
            this.RemoveMaterialButton.Padding = new Padding(8, 0, 8, 0);
            this.Toolbar.Controls.Add(this.RemoveMaterialButton);
            this.Size = new System.Drawing.Size(960, 560);
            this.ResumeLayout(false);
        }
    }
}
