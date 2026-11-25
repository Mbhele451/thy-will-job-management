using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    // Data-dependent formatting only. Layout and colours belong in the Designer.
    internal static class GridPresentation
    {
        private static readonly HashSet<DataGridView> Formatting = new HashSet<DataGridView>();
        internal static readonly CultureInfo Rand = CultureInfo.GetCultureInfo("en-ZA");
        internal static void BindJobs(DataGridView grid, BindingSource source, DataTable rows)
        {
            object selectedJob = (grid.CurrentRow?.DataBoundItem as DataRowView)?["JobID"];
            string filter = source.Filter;
            grid.SuspendLayout();
            try
            {
                // Detach before clearing. Assigning the same BindingSource again does
                // not rebuild columns cleared while that source was still attached.
                grid.DataSource = null;
                grid.AutoGenerateColumns = true;
                grid.Columns.Clear();
                source.DataSource = rows;
                source.Filter = filter;
                grid.DataSource = source;
                FormatColumns(grid);
                DataGridViewRow selected = null;
                foreach (DataGridViewRow row in grid.Rows)
                    if (selectedJob != null && row.DataBoundItem is DataRowView view && Equals(view["JobID"], selectedJob))
                        selected = row;
                if (selected == null && grid.Rows.Count > 0)
                    selected = grid.Rows[0];
                grid.ClearSelection();
                if (selected != null)
                {
                    foreach (DataGridViewColumn column in grid.Columns)
                        if (column.Visible)
                        {
                            grid.CurrentCell = selected.Cells[column.Index];
                            break;
                        }

                    selected.Selected = true;
                }
                else
                    grid.CurrentCell = null;
            }
            finally
            {
                grid.ResumeLayout(true);
            }
        }

        internal static void Attach(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is DataGridView grid)
                {
                    grid.DataBindingComplete -= Bound;
                    grid.DataBindingComplete += Bound;
                    grid.ColumnAdded -= ColumnAdded;
                    grid.ColumnAdded += ColumnAdded;
                    grid.CellFormatting -= PaymentFormatting;
                    grid.CellFormatting += PaymentFormatting;
                    FormatColumns(grid);
                }

                Attach(child);
            }
        }

        internal static void StyleGrid(DataGridView grid)
        {
            grid.Font = new Font("Segoe UI", 11F);
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.EnableHeadersVisualStyles = false;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);
            grid.RowTemplate.Height = 28;
            grid.DataBindingComplete -= Bound;
            grid.DataBindingComplete += Bound;
            grid.ColumnAdded -= ColumnAdded;
            grid.ColumnAdded += ColumnAdded;
            grid.CellFormatting -= PaymentFormatting;
            grid.CellFormatting += PaymentFormatting;
            FormatColumns(grid);
        }

        private static void Bound(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            FormatColumns((DataGridView)sender);
        }

        private static void ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            HideDocumentColumn(e.Column);
        }

        private static bool HideDocumentColumn(DataGridViewColumn column)
        {
            string key = string.IsNullOrEmpty(column.DataPropertyName) ? column.Name : column.DataPropertyName;
            // WinForms auto-generates image cells for byte[] columns. PDF bytes cannot
            // be decoded as images; keep the payload bound for exports, but never paint it.
            bool document = column.ValueType == typeof(byte[]) || key.Equals("InvoicePDFData", StringComparison.OrdinalIgnoreCase) || key.Equals("InvoiceFilePath", StringComparison.OrdinalIgnoreCase) || key == "HasInvoicePdf";
            if (document)
                column.Visible = false;
            return document;
        }

        internal static void FormatColumns(DataGridView grid)
        {
            if (grid == null || grid.IsDisposed || !Formatting.Add(grid))
                return;
            try
            {
                foreach (DataGridViewColumn column in grid.Columns.Cast<DataGridViewColumn>().ToArray())
                {
                    if (column.DataGridView != grid)
                        continue;
                    if (HideDocumentColumn(column))
                        continue;
                    string key = string.IsNullOrEmpty(column.DataPropertyName) ? column.Name : column.DataPropertyName;
                    // Keep the job reference visible; hide foreign keys without removing bound data.
                    if (key == "CustomerID" || key == "StaffID" || key == "JobTypeID" || key == "ItemID" || key == "RoleID" || key == "CardToken" || key.IndexOf("Password", StringComparison.OrdinalIgnoreCase) >= 0 || key == "SouthAfricanIDNumber" || key == "PassportNumber")
                        column.Visible = false;
                    switch (key)
                    {
                        case "JobID":
                            column.HeaderText = "Job #";
                            break;
                        case "BaseLabourCost":
                        case "LabourCost":
                            column.HeaderText = "Labour";
                            break;
                        case "BaseTransportCost":
                        case "TransportCost":
                            column.HeaderText = "Transport";
                            break;
                        case "TotalMaterialCost":
                        case "MaterialCost":
                            column.HeaderText = "Materials";
                            break;
                        case "QuantityInStock":
                            column.HeaderText = "In stock";
                            break;
                        case "JobTypeName":
                            column.HeaderText = "Service";
                            break;
                        case "JobStatus":
                            column.HeaderText = "Status";
                            break;
                        case "RoleName":
                            column.HeaderText = "Role";
                            break;
                        case "IsAvailable":
                            column.HeaderText = "Availability";
                            break;
                        case "GrandTotal":
                            column.HeaderText = "Grand total";
                            break;
                        default:
                            column.HeaderText = Regex.Replace(key, "(?<=[a-z0-9])(?=[A-Z])", " ");
                            break;
                    }

                    if (Regex.IsMatch(key, "(?:Cost|Price|Amount|Total|Salary|Balance|Paid|Due|Fee|Charge)$") && key != "QuantityTotal")
                    {
                        column.DefaultCellStyle.Format = "C2";
                        column.DefaultCellStyle.FormatProvider = Rand;
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }

                    if (column.ValueType == typeof(DateTime))
                        column.DefaultCellStyle.Format = "dd MMM yyyy";
                }

                if (grid.Columns.Cast<DataGridViewColumn>().Any(column => column.DataPropertyName == "ItemName") && grid.Columns.Cast<DataGridViewColumn>().Any(column => column.DataPropertyName == "QuantityInStock"))
                {
                    grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    grid.MultiSelect = false;
                    grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    foreach (DataGridViewColumn column in grid.Columns)
                    {
                        if (column.DataPropertyName == "Category")
                            column.Visible = false;
                        if (column.DataPropertyName != "ItemName" && column.Visible)
                            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        if (column.DataPropertyName == "ItemName")
                        {
                            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            column.FillWeight = 300;
                            column.MinimumWidth = 260;
                            column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        }
                    }
                }

                if (grid.Name == "dataGridViewCart")
                {
                    string[] cart =
                    {
                        "JobTypeName",
                        "Quantity",
                        "GrandTotal"
                    };
                    foreach (var column in grid.Columns.Cast<DataGridViewColumn>().ToArray())
                        if (column.DataGridView == grid)
                            column.Visible = Array.IndexOf(cart, string.IsNullOrEmpty(column.DataPropertyName) ? column.Name : column.DataPropertyName) >= 0;
                }

                if (grid.Name == "dgvJobs")
                {
                    string[] summary =
                    {
                        "JobID",
                        "CustomerName",
                        "TechnicianName",
                        "JobStatus",
                        "PaymentStatus",
                        "PromiseDate",
                        "GrandTotal"
                    };
                    foreach (var column in grid.Columns.Cast<DataGridViewColumn>().ToArray())
                    {
                        if (column.DataGridView != grid)
                            continue;
                        string key = string.IsNullOrEmpty(column.DataPropertyName) ? column.Name : column.DataPropertyName;
                        bool visible = Array.IndexOf(summary, key) >= 0;
                        if (column.Visible != visible)
                            column.Visible = visible;
                    }
                }
            }
            finally
            {
                Formatting.Remove(grid);
            }
        }

        private static void PaymentFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (e.ColumnIndex < 0 || e.ColumnIndex >= grid.Columns.Count)
                return;
            var column = grid.Columns[e.ColumnIndex];
            if (column.DataPropertyName != "PaymentStatus" && column.Name != "PaymentStatus")
                return;
            string status = Convert.ToString(e.Value).Trim();
            if (status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
            {
                e.CellStyle.BackColor = Color.FromArgb(226, 243, 233);
                e.CellStyle.ForeColor = Color.FromArgb(24, 113, 69);
            }
            else
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 242, 219);
                e.CellStyle.ForeColor = Color.FromArgb(146, 85, 9);
            }
        }

        internal static DataTable LowStock(DataTable inventory)
        {
            var low = inventory.Clone();
            if (!inventory.Columns.Contains("QuantityInStock") || !inventory.Columns.Contains("ReorderLevel"))
                throw new InvalidOperationException("Inventory must include stock quantities and reorder levels.");
            foreach (DataRow row in inventory.Rows)
                if (row["QuantityInStock"] != DBNull.Value && row["ReorderLevel"] != DBNull.Value && Convert.ToDecimal(row["QuantityInStock"]) <= Convert.ToDecimal(row["ReorderLevel"]))
                    low.ImportRow(row);
            low.DefaultView.Sort = "QuantityInStock ASC";
            return low;
        }

        internal static void JobActions(DataGridView grid, Button edit, Button archive)
        {
            string status = grid.CurrentRow?.DataBoundItem is DataRowView row && row.Row.Table.Columns.Contains("JobStatus") ? Convert.ToString(row["JobStatus"]).Trim() : "";
            edit.Enabled = status == "Pending" || status == "In Progress";
            archive.Enabled = status == "Completed" || status == "Cancelled";
        }
    }
}
