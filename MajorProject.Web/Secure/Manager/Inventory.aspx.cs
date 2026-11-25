using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;

namespace MajorProject.Web.Secure.Manager
{
    public partial class Inventory : ManagerBasePage
    {
        private InventoryService _inventoryService;
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _inventoryService = new InventoryService(ConnectionString);
            if (!IsPostBack)
            {
                LoadInventory();
                ClearSelectedItem();
            }
        }

        private void LoadInventory()
        {
            try
            {
                DataTable inventory;
                string search = (txtSearch.Text ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(search))
                {
                    inventory = _inventoryService.GetAllInventoryItems();
                }
                else
                {
                    inventory = _inventoryService.SearchInventoryItems(search);
                }

                if (inventory == null)
                {
                    gvInventory.DataSource = null;
                    gvInventory.DataBind();
                    return;
                }

                DataView view = inventory.DefaultView;
                ApplySorting(view);
                gvInventory.DataSource = view;
                gvInventory.DataBind();
            }
            catch (Exception ex)
            {
                gvInventory.DataSource = null;
                gvInventory.DataBind();
                HandleUnexpectedError("We could not load the inventory.", ex);
            }
        }

        private void ApplySorting(DataView view)
        {
            switch (ddlSort.SelectedValue)
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

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadInventory();
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadInventory();
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            ddlSort.SelectedValue = "ItemName";
            ClearMessage();
            LoadInventory();
        }

        protected void gvInventory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "ViewItem")
            {
                return;
            }

            int itemID;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out itemID) || itemID <= 0)
            {
                ShowError("Invalid inventory item.");
                return;
            }

            LoadSelectedItem(itemID);
        }

        private void LoadSelectedItem(int itemID)
        {
            try
            {
                DataTable item = _inventoryService.GetInventoryItemByID(itemID);
                if (item == null || item.Rows.Count == 0)
                {
                    ClearSelectedItem();
                    ShowError("The selected inventory item could not be found.");
                    return;
                }

                DataRow row = item.Rows[0];
                hfSelectedItemID.Value = GetValue(row, "ItemID");
                txtSelectedItemID.Text = GetValue(row, "ItemID");
                txtSelectedItemName.Text = GetValue(row, "ItemName");
                txtSelectedCategory.Text = GetValue(row, "Category");
                txtSelectedQuantity.Text = GetValue(row, "QuantityInStock");
                txtSelectedUnitOfMeasure.Text = GetValue(row, "UnitOfMeasure");
                txtSelectedUnitPrice.Text = FormatCurrency(row, "UnitPrice");
                txtSelectedReorderLevel.Text = GetValue(row, "ReorderLevel");
                txtSelectedSupplier.Text = GetValue(row, "SupplierName");
                txtSelectedUpdatedAt.Text = FormatDate(row, "UpdatedAt");
                ClearMessage();
            }
            catch (Exception ex)
            {
                ClearSelectedItem();
                HandleUnexpectedError("We could not load the selected inventory item.", ex);
            }
        }

        private void ClearSelectedItem()
        {
            hfSelectedItemID.Value = string.Empty;
            txtSelectedItemID.Text = string.Empty;
            txtSelectedItemName.Text = string.Empty;
            txtSelectedCategory.Text = string.Empty;
            txtSelectedQuantity.Text = string.Empty;
            txtSelectedUnitOfMeasure.Text = string.Empty;
            txtSelectedUnitPrice.Text = string.Empty;
            txtSelectedReorderLevel.Text = string.Empty;
            txtSelectedSupplier.Text = string.Empty;
            txtSelectedUpdatedAt.Text = string.Empty;
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            int itemID;
            if (!int.TryParse(hfSelectedItemID.Value, out itemID) || itemID <= 0)
            {
                ShowError("Select an inventory item before removing it.");
                return;
            }

            try
            {
                _inventoryService.DeleteInventoryItem(itemID);
                ClearSelectedItem();
                ShowSuccess("Inventory item permanently removed.");
                LoadInventory();
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("The inventory item could not be permanently removed.", ex);
            }
        }

        protected void btnUpdateQuantity_Click(object sender, EventArgs e)
        {
            int itemID;
            if (!int.TryParse(hfSelectedItemID.Value, out itemID) || itemID <= 0)
            {
                ShowError("Select an inventory item first.");
                return;
            }

            int quantity;
            if (!int.TryParse((txtStockQuantity.Text ?? string.Empty).Trim(), out quantity) || quantity <= 0)
            {
                ShowError("Enter a quantity greater than zero.");
                return;
            }

            string operation = rblStockAction.SelectedValue;
            int delta;
            if (operation == "Restock")
            {
                delta = quantity;
            }
            else if (operation == "Waste")
            {
                delta = -quantity;
            }
            else
            {
                ShowError("Select either Restock or Waste.");
                return;
            }

            try
            {
                _inventoryService.AdjustInventoryQuantity(itemID, delta);
                txtStockQuantity.Text = string.Empty;
                LoadSelectedItem(itemID);
                LoadInventory();
                if (operation == "Restock")
                {
                    ShowSuccess(string.Format("Restocked {0:N0} units of {1}.", quantity, Server.HtmlEncode(txtSelectedItemName.Text)));
                }
                else
                {
                    ShowSuccess("Waste recorded and stock updated.");
                }
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("The stock quantity could not be updated.", ex);
                ShowError("Stock update could not be confirmed. Refresh the item before trying again.");
            }
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            string itemName = (txtNewItemName.Text ?? string.Empty).Trim();
            string category = (txtNewCategory.Text ?? string.Empty).Trim();
            string supplier = (txtNewSupplier.Text ?? string.Empty).Trim();
            string unitOfMeasure = (ddlNewUnitOfMeasure.SelectedValue ?? string.Empty).Trim();
            int quantity;
            if (!int.TryParse((txtNewQuantity.Text ?? string.Empty).Trim(), out quantity) || quantity < 0)
            {
                ShowError("Initial quantity must be zero or greater.");
                return;
            }

            decimal unitPrice;
            if (!decimal.TryParse((txtNewUnitPrice.Text ?? string.Empty).Trim(), out unitPrice) || unitPrice <= 0)
            {
                ShowError("Unit price must be greater than zero.");
                return;
            }

            int reorderLevel;
            if (!int.TryParse((txtNewReorderLevel.Text ?? string.Empty).Trim(), out reorderLevel) || reorderLevel < 0)
            {
                ShowError("Reorder level must be zero or greater.");
                return;
            }

            if (string.IsNullOrWhiteSpace(unitOfMeasure))
            {
                ShowError("Select a unit of measure.");
                return;
            }

            try
            {
                _inventoryService.CreateInventoryItem(itemName, category, quantity, unitPrice, reorderLevel, supplier, unitOfMeasure);
                ClearNewItemForm();
                LoadInventory();
                ShowSuccess("New inventory item successfully added.");
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError("The inventory item could not be added.", ex);
            }
        }

        private void ClearNewItemForm()
        {
            txtNewItemName.Text = string.Empty;
            txtNewCategory.Text = string.Empty;
            txtNewQuantity.Text = string.Empty;
            txtNewUnitPrice.Text = string.Empty;
            txtNewReorderLevel.Text = string.Empty;
            txtNewSupplier.Text = string.Empty;
            ddlNewUnitOfMeasure.SelectedValue = "Each";
        }

        private string GetValue(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToString(row[column]);
        }

        private string FormatCurrency(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return string.Empty;
            }

            decimal value = Convert.ToDecimal(row[column]);
            return value.ToString("R #,##0.00");
        }

        private string FormatDate(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return string.Empty;
            }

            DateTime value = Convert.ToDateTime(row[column]);
            return value.ToString("dd MMM yyyy HH:mm");
        }

        private void ShowSuccess(string message)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.CssClass = "dashboard-alert dashboard-alert-success";
            lblMessage.Visible = true;
        }

        private void ShowError(string message)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.CssClass = "dashboard-alert dashboard-alert-error";
            lblMessage.Visible = true;
        }

        private void ClearMessage()
        {
            lblMessage.Text = string.Empty;
            lblMessage.Visible = false;
        }
    }
}
