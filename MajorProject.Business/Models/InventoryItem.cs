using System;

namespace MajorProject.Business.Models
{
    /// <summary>
    /// Represents an inventory item, matching the real InventoryItems table:
    /// InventoryItems(ItemID, ItemName, Category, QuantityInStock, UnitPrice,
    ///                ReorderLevel, SupplierName, UpdatedAt).
    ///
    /// Fix vs. previous version: removed Description/CreatedDate/LastRestockedDate,
    /// none of which are real columns. Renamed Quantity -> QuantityInStock and
    /// UnitCost -> UnitPrice to match the schema exactly.
    /// </summary>
    public class InventoryItem
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public int QuantityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public int ReorderLevel { get; set; }
        public string SupplierName { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal TotalValue => QuantityInStock * UnitPrice;
        public bool IsLowStock => QuantityInStock <= ReorderLevel;

        public override string ToString()
        {
            return $"{ItemName} (Qty: {QuantityInStock})";
        }
    }
}
