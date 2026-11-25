using System;
using System.Data;
using System.Data.SqlClient;

namespace MajorProject.Data.Data
{
    public class InventoryDataAccess : DataAccessBase
    {
        public InventoryDataAccess(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Gets all inventory items.
        /// </summary>
        public DataTable GetAllInventoryItems()
        {
            const string query = @"
                SELECT
                    ItemID,
                    ItemName,
                    Category,
                    QuantityInStock,
                    UnitPrice,
                    ReorderLevel,
                    SupplierName,
                    UnitOfMeasure,
                    UpdatedAt
                FROM InventoryItems
                ORDER BY ItemName";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// Gets an inventory item by ID.
        /// </summary>
        public DataTable GetInventoryItemByID(int itemID)
        {
            const string query = @"
                SELECT
                    ItemID,
                    ItemName,
                    Category,
                    QuantityInStock,
                    UnitPrice,
                    ReorderLevel,
                    SupplierName,
                    UnitOfMeasure,
                    UpdatedAt
                FROM InventoryItems
                WHERE ItemID = @ItemID";
            return ExecuteQuery(query, new SqlParameter("@ItemID", itemID));
        }

        /// <summary>
        /// Searches inventory items by name.
        /// </summary>
        public DataTable SearchInventoryItems(string itemName)
        {
            const string query = @"
                SELECT
                    ItemID,
                    ItemName,
                    Category,
                    QuantityInStock,
                    UnitPrice,
                    ReorderLevel,
                    SupplierName,
                    UnitOfMeasure,
                    UpdatedAt
                FROM InventoryItems
                WHERE ItemName LIKE @ItemName
                ORDER BY ItemName";
            return ExecuteQuery(query, new SqlParameter("@ItemName", "%" + itemName + "%"));
        }

        /// <summary>
        /// Updates the exact inventory quantity.
        /// </summary>
        public int UpdateInventoryQuantity(int itemID, int quantity)
        {
            const string query = @"
                UPDATE InventoryItems
                SET
                    QuantityInStock = @Quantity,
                    UpdatedAt = @UpdatedAt
                WHERE ItemID = @ItemID";
            return ExecuteNonQuery(query, new SqlParameter("@Quantity", quantity), new SqlParameter("@UpdatedAt", DateTime.Now), new SqlParameter("@ItemID", itemID));
        }

        /// <summary>
        /// Adjusts inventory quantity by item name.
        /// Retained for existing ManagerForm functionality.
        /// </summary>
        public int AdjustInventoryQuantityByName(string itemName, int delta)
        {
            const string query = @"
                UPDATE InventoryItems
                SET
                    QuantityInStock = QuantityInStock + @Delta,
                    UpdatedAt = @UpdatedAt
                WHERE ItemName = @ItemName
                  AND QuantityInStock + @Delta >= 0";
            return ExecuteNonQuery(query, new SqlParameter("@Delta", delta), new SqlParameter("@UpdatedAt", DateTime.Now), new SqlParameter("@ItemName", itemName));
        }

        /// <summary>
        /// Creates an inventory item.
        /// </summary>
        public int CreateInventoryItem(string itemName, string category, int quantityInStock, decimal unitPrice, int reorderLevel, string supplierName, string unitOfMeasure)
        {
            const string query = @"
                INSERT INTO InventoryItems
                (
                    ItemName,
                    Category,
                    QuantityInStock,
                    UnitPrice,
                    ReorderLevel,
                    SupplierName,
                    UnitOfMeasure,
                    UpdatedAt
                )
                VALUES
                (
                    @ItemName,
                    @Category,
                    @QuantityInStock,
                    @UnitPrice,
                    @ReorderLevel,
                    @SupplierName,
                    @UnitOfMeasure,
                    @UpdatedAt
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            object result = ExecuteScalar(query, new SqlParameter("@ItemName", itemName), new SqlParameter("@Category", (object)category ?? DBNull.Value), new SqlParameter("@QuantityInStock", quantityInStock), new SqlParameter("@UnitPrice", unitPrice), new SqlParameter("@ReorderLevel", reorderLevel), new SqlParameter("@SupplierName", (object)supplierName ?? DBNull.Value), new SqlParameter("@UnitOfMeasure", (object)unitOfMeasure ?? DBNull.Value), new SqlParameter("@UpdatedAt", DateTime.Now));
            if (result == null || result == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Permanently deletes an inventory item.
        ///
        /// Deletion is blocked when the item is currently used
        /// by a service through JobTypeMaterials.
        ///
        /// Historical JobMaterials and InventoryTransactions
        /// retain their records while their ItemID is cleared.
        /// </summary>
        public int DeleteInventoryItem(int itemID)
        {
            using (SqlConnection connection = CreateOpenConnection())
            using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    const string checkItemQuery = @"
                        SELECT COUNT(*)
                        FROM InventoryItems
                        WHERE ItemID = @ItemID";
                    using (SqlCommand checkItemCommand = new SqlCommand(checkItemQuery, connection, transaction))
                    {
                        checkItemCommand.Parameters.AddWithValue("@ItemID", itemID);
                        int itemExists = Convert.ToInt32(checkItemCommand.ExecuteScalar());
                        if (itemExists == 0)
                        {
                            transaction.Rollback();
                            return 0;
                        }
                    }

                    const string checkServiceQuery = @"
                        SELECT COUNT(*)
                        FROM JobTypeMaterials
                        WHERE ItemID = @ItemID";
                    using (SqlCommand checkServiceCommand = new SqlCommand(checkServiceQuery, connection, transaction))
                    {
                        checkServiceCommand.Parameters.AddWithValue("@ItemID", itemID);
                        int serviceReferences = Convert.ToInt32(checkServiceCommand.ExecuteScalar());
                        if (serviceReferences > 0)
                        {
                            transaction.Rollback();
                            throw new InvalidOperationException("This inventory item is currently used by one or more services. Remove it from those services before permanently deleting it.");
                        }
                    }

                    const string clearJobMaterialsQuery = @"
                        UPDATE JobMaterials
                        SET ItemID = NULL
                        WHERE ItemID = @ItemID";
                    using (SqlCommand clearJobMaterialsCommand = new SqlCommand(clearJobMaterialsQuery, connection, transaction))
                    {
                        clearJobMaterialsCommand.Parameters.AddWithValue("@ItemID", itemID);
                        clearJobMaterialsCommand.ExecuteNonQuery();
                    }

                    const string clearInventoryTransactionsQuery = @"
                        UPDATE InventoryTransactions
                        SET ItemID = NULL
                        WHERE ItemID = @ItemID";
                    using (SqlCommand clearInventoryTransactionsCommand = new SqlCommand(clearInventoryTransactionsQuery, connection, transaction))
                    {
                        clearInventoryTransactionsCommand.Parameters.AddWithValue("@ItemID", itemID);
                        clearInventoryTransactionsCommand.ExecuteNonQuery();
                    }

                    const string deleteItemQuery = @"
                        DELETE FROM InventoryItems
                        WHERE ItemID = @ItemID";
                    int affectedRows;
                    using (SqlCommand deleteItemCommand = new SqlCommand(deleteItemQuery, connection, transaction))
                    {
                        deleteItemCommand.Parameters.AddWithValue("@ItemID", itemID);
                        affectedRows = deleteItemCommand.ExecuteNonQuery();
                    }

                    if (affectedRows != 1)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    transaction.Commit();
                    return affectedRows;
                }
                catch
                {
                    if (transaction.Connection != null)
                    {
                        transaction.Rollback();
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Adjusts inventory quantity while preventing
        /// the quantity from becoming negative.
        /// </summary>
        public int AdjustInventoryQuantity(int itemID, int delta)
        {
            const string query = @"
                UPDATE InventoryItems
                SET
                    QuantityInStock = QuantityInStock + @Delta,
                    UpdatedAt = @UpdatedAt
                WHERE ItemID = @ItemID
                  AND QuantityInStock + @Delta >= 0";
            return ExecuteNonQuery(query, new SqlParameter("@Delta", delta), new SqlParameter("@UpdatedAt", DateTime.Now), new SqlParameter("@ItemID", itemID));
        }
    }
}
