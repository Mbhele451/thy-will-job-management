using MajorProject.Business.Helpers;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;

namespace MajorProject.Web.Secure.Technician
{
    public partial class Inventory : TechnicianBasePage
    {
        private string ConnectionString
        {
            get
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"];
                if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    throw new InvalidOperationException("The GroupWst34ConnectionString connection string is not configured.");
                }

                return settings.ConnectionString;
            }
        }

        private InventoryService InventoryService
        {
            get
            {
                return new InventoryService(ConnectionString);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadInventory();
            }
        }

        private void LoadInventory()
        {
            try
            {
                DataTable inventory = InventoryService.GetAllInventoryItems();
                BindInventory(inventory);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Failed to load inventory.", ex);
            }
        }

        private void BindInventory(DataTable inventory)
        {
            if (inventory == null)
            {
                dgvInventory.DataSource = null;
                dgvInventory.DataBind();
                return;
            }

            DataView view = inventory.DefaultView;
            ApplySorting(view);
            dgvInventory.DataSource = view;
            dgvInventory.DataBind();
        }

        private void ApplySorting(DataView view)
        {
            switch (ddlInventorySort.SelectedValue)
            {
                case "QuantityAsc":
                    view.Sort = "QuantityInStock ASC";
                    break;
                case "QuantityDesc":
                    view.Sort = "QuantityInStock DESC";
                    break;
                case "Category":
                    view.Sort = "Category ASC, ItemName ASC";
                    break;
                case "PriceAsc":
                    view.Sort = "UnitPrice ASC";
                    break;
                case "PriceDesc":
                    view.Sort = "UnitPrice DESC";
                    break;
                case "UpdatedDesc":
                    view.Sort = "UpdatedAt DESC";
                    break;
                case "ItemName":
                default:
                    view.Sort = "ItemName ASC";
                    break;
            }
        }

        protected void txtInventorySearch_TextChanged(object sender, EventArgs e)
        {
            LoadFilteredInventory();
        }

        protected void ddlInventorySort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFilteredInventory();
        }

        private void LoadFilteredInventory()
        {
            try
            {
                string search = (txtInventorySearch.Text ?? string.Empty).Trim();
                DataTable inventory;
                if (string.IsNullOrWhiteSpace(search))
                {
                    inventory = InventoryService.GetAllInventoryItems();
                }
                else
                {
                    inventory = InventoryService.SearchInventoryItems(search);
                }

                BindInventory(inventory);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("Failed to filter inventory.", ex);
            }
        }

        protected void dgvInventory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedIndex < 0)
            {
                return;
            }

            try
            {
                DataKey key = dgvInventory.DataKeys[dgvInventory.SelectedIndex];
                if (key == null)
                {
                    return;
                }

                int itemID = Convert.ToInt32(key.Value);
                DataTable item = InventoryService.GetInventoryItemByID(itemID);
                if (item == null || item.Rows.Count == 0)
                {
                    txtStockDetails.Text = "The selected inventory item could not be found.";
                    return;
                }

                DataRow row = item.Rows[0];
                txtStockDetails.Text = "ITEM DETAILS" + Environment.NewLine + "------------------------------" + Environment.NewLine + "Item ID       : " + GetValue(row, "ItemID") + Environment.NewLine + "Item Name     : " + GetValue(row, "ItemName") + Environment.NewLine + "Category      : " + GetValue(row, "Category") + Environment.NewLine + "Quantity      : " + GetValue(row, "QuantityInStock") + Environment.NewLine + "Unit          : " + GetValue(row, "UnitOfMeasure") + Environment.NewLine + "Unit Price    : R " + GetValue(row, "UnitPrice") + Environment.NewLine + "Reorder Level : " + GetValue(row, "ReorderLevel") + Environment.NewLine + "Supplier      : " + GetValue(row, "SupplierName") + Environment.NewLine + "Last Updated  : " + GetValue(row, "UpdatedAt");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to display inventory item.", ex);
            }
        }

        private string GetValue(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToString(row[column]);
        }
    }
}
