using System;
using System.Data;
using MajorProject.Data.Data;

namespace MajorProject.Business.Services
{
    public class InventoryService
    {
        private readonly InventoryDataAccess _dataAccess;
        public InventoryService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));
            }

            _dataAccess = new InventoryDataAccess(connectionString);
        }

        /// <summary>
        /// Gets all inventory items.
        /// </summary>
        public DataTable GetAllInventoryItems()
        {
            return _dataAccess.GetAllInventoryItems();
        }

        /// <summary>
        /// Gets an inventory item by ID.
        /// </summary>
        public DataTable GetInventoryItemByID(int itemID)
        {
            ValidateItemID(itemID);
            return _dataAccess.GetInventoryItemByID(itemID);
        }

        /// <summary>
        /// Searches inventory items by name.
        /// </summary>
        public DataTable SearchInventoryItems(string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                return GetAllInventoryItems();
            }

            return _dataAccess.SearchInventoryItems(itemName.Trim());
        }

        /// <summary>
        /// Updates the exact inventory quantity.
        /// </summary>
        public void UpdateInventoryQuantity(int itemID, int quantity)
        {
            ValidateItemID(itemID);
            if (quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
            }

            int affected = _dataAccess.UpdateInventoryQuantity(itemID, quantity);
            if (affected != 1)
            {
                throw new InvalidOperationException("The inventory quantity could not be updated.");
            }
        }

        private static void ValidateAdjustment(int delta)
        {
            if (delta == 0 || delta > MaximumRestockQuantity)
                throw new ArgumentException("Enter a restock quantity between 1 and 20,000.");
        }

        public const int MaximumRestockQuantity = 20000;
        /// <summary>
        /// Adjusts inventory quantity by a positive or negative amount.
        /// </summary>
        public void AdjustInventoryQuantity(int itemID, int delta)
        {
            ValidateItemID(itemID);
            ValidateAdjustment(delta);
            int affected = _dataAccess.AdjustInventoryQuantity(itemID, delta);
            if (affected != 1)
            {
                if (delta < 0)
                {
                    throw new InvalidOperationException("The quantity to remove is greater than the current stock.");
                }

                throw new InvalidOperationException("The inventory quantity could not be updated.");
            }
        }

        /// <summary>
        /// Creates a new inventory item.
        /// </summary>
        public void CreateInventoryItem(string itemName, string category, int quantityInStock, decimal unitPrice, int reorderLevel, string supplierName, string unitOfMeasure)
        {
            ValidateItemName(itemName);
            if (quantityInStock < 0)
            {
                throw new ArgumentException("Initial quantity cannot be negative.", nameof(quantityInStock));
            }

            if (unitPrice <= 0)
            {
                throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));
            }

            if (reorderLevel < 0)
            {
                throw new ArgumentException("Reorder level cannot be negative.", nameof(reorderLevel));
            }

            ValidateOptionalText(category, 100, "Category");
            ValidateOptionalText(supplierName, 100, "Supplier name");
            ValidateOptionalText(unitOfMeasure, 20, "Unit of measure");
            int itemID = _dataAccess.CreateInventoryItem(itemName.Trim(), CleanOptionalText(category), quantityInStock, unitPrice, reorderLevel, CleanOptionalText(supplierName), CleanOptionalText(unitOfMeasure));
            if (itemID <= 0)
            {
                throw new InvalidOperationException("The inventory item could not be created.");
            }
        }

        /// <summary>
        /// Permanently deletes an inventory item.
        ///
        /// The item cannot be deleted while it is assigned
        /// to a service through JobTypeMaterials.
        /// </summary>
        public void DeleteInventoryItem(int itemID)
        {
            ValidateItemID(itemID);
            int affected = _dataAccess.DeleteInventoryItem(itemID);
            if (affected == 0)
            {
                throw new InvalidOperationException("The inventory item could not be found.");
            }

            if (affected != 1)
            {
                throw new InvalidOperationException("The inventory item could not be permanently removed.");
            }
        }

        /// <summary>
        /// Determines whether an inventory item's
        /// quantity is at or below its reorder level.
        /// </summary>
        public bool IsQuantityLow(int itemID)
        {
            ValidateItemID(itemID);
            DataTable inventory = _dataAccess.GetInventoryItemByID(itemID);
            if (inventory == null || inventory.Rows.Count == 0)
            {
                return false;
            }

            int quantity = Convert.ToInt32(inventory.Rows[0]["QuantityInStock"]);
            int reorderLevel = 5;
            if (inventory.Rows[0]["ReorderLevel"] != DBNull.Value)
            {
                reorderLevel = Convert.ToInt32(inventory.Rows[0]["ReorderLevel"]);
            }

            return quantity <= reorderLevel;
        }

        /// <summary>
        /// Adjusts inventory quantity by item name.
        /// Retained for existing ManagerForm functionality.
        /// </summary>
        public void AdjustInventoryQuantityByName(string itemName, int delta)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                throw new ArgumentException("Item name is required.", nameof(itemName));
            }

            ValidateAdjustment(delta);
            int affected = _dataAccess.AdjustInventoryQuantityByName(itemName.Trim(), delta);
            if (affected != 1)
            {
                if (delta < 0)
                {
                    throw new InvalidOperationException("The quantity to remove is greater than the current stock.");
                }

                throw new InvalidOperationException("The inventory quantity could not be updated.");
            }
        }

        private void ValidateItemID(int itemID)
        {
            if (itemID <= 0)
            {
                throw new ArgumentException("Invalid inventory item.", nameof(itemID));
            }
        }

        private void ValidateItemName(string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                throw new ArgumentException("Item name is required.", nameof(itemName));
            }

            if (itemName.Trim().Length > 100)
            {
                throw new ArgumentException("Item name cannot exceed 100 characters.", nameof(itemName));
            }
        }

        private void ValidateOptionalText(string value, int maximumLength, string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Trim().Length > maximumLength)
            {
                throw new ArgumentException(fieldName + " cannot exceed " + maximumLength + " characters.");
            }
        }

        private string CleanOptionalText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }
    }
}
