using System.Data;
using System.Data.SqlClient;

namespace MajorProject.Data.Data
{
    public class JobTypeDataAccess : DataAccessBase
    {
        public JobTypeDataAccess(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Gets all services.
        /// </summary>
        public DataTable GetAllJobTypes()
        {
            const string query = @"
                SELECT
                    JobTypeID,
                    JobTypeName,
                    ISNULL(BaseLabourCost, 0) AS BaseLabourCost,
                    ISNULL(BaseTransportCost, 0) AS BaseTransportCost,
                    ISNULL(TaxPercentage, 0) AS TaxPercentage
                FROM JobTypes
                ORDER BY JobTypeName";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// Gets a single service together with its
        /// current material cost.
        ///
        /// Material requirements come from JobTypeMaterials.
        /// Material prices come from InventoryItems.
        /// </summary>
        public DataTable GetJobCostsAndMaterials(int jobTypeID)
        {
            const string query = @"
                SELECT
                    jt.JobTypeID,
                    jt.JobTypeName,
                    ISNULL(jt.BaseLabourCost, 0) AS BaseLabourCost,
                    ISNULL(jt.BaseTransportCost, 0) AS BaseTransportCost,
                    ISNULL(jt.TaxPercentage, 0) AS TaxPercentage,
                    ISNULL(
                        (
                            SELECT SUM(
                                jtm.QuantityRequired
                                * ISNULL(ii.UnitPrice, 0)
                            )
                            FROM JobTypeMaterials jtm
                            INNER JOIN InventoryItems ii
                                ON jtm.ItemID = ii.ItemID
                            WHERE jtm.JobTypeID = jt.JobTypeID
                        ),
                        0
                    ) AS MaterialCost
                FROM JobTypes jt
                WHERE jt.JobTypeID = @JobTypeID";
            return ExecuteQuery(query, new SqlParameter("@JobTypeID", jobTypeID));
        }

        /// <summary>
        /// Gets all materials configured for a service.
        /// </summary>
        public DataTable GetMaterialsByJobType(int jobTypeID)
        {
            const string query = @"
                SELECT
                    jtm.JobTypeMaterialID,
                    jtm.JobTypeID,
                    jtm.ItemID,
                    ii.ItemName,
                    ii.Category,
                    jtm.QuantityRequired,
                    ii.UnitOfMeasure,
                    ISNULL(ii.UnitPrice, 0) AS UnitPrice,
                    (
                        jtm.QuantityRequired
                        * ISNULL(ii.UnitPrice, 0)
                    ) AS MaterialCost
                FROM JobTypeMaterials jtm
                INNER JOIN InventoryItems ii
                    ON jtm.ItemID = ii.ItemID
                WHERE jtm.JobTypeID = @JobTypeID
                ORDER BY ii.ItemName";
            return ExecuteQuery(query, new SqlParameter("@JobTypeID", jobTypeID));
        }

        /// <summary>
        /// Gets all inventory items that can be assigned
        /// to a service.
        /// </summary>
        public DataTable GetAvailableInventoryItems()
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
                    UnitOfMeasure
                FROM InventoryItems
                ORDER BY ItemName";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// Gets a single service by ID.
        /// </summary>
        public DataTable GetJobTypeByID(int jobTypeID)
        {
            const string query = @"
                SELECT
                    JobTypeID,
                    JobTypeName,
                    ISNULL(BaseLabourCost, 0) AS BaseLabourCost,
                    ISNULL(BaseTransportCost, 0) AS BaseTransportCost,
                    ISNULL(TaxPercentage, 0) AS TaxPercentage
                FROM JobTypes
                WHERE JobTypeID = @JobTypeID";
            return ExecuteQuery(query, new SqlParameter("@JobTypeID", jobTypeID));
        }

        /// <summary>
        /// Updates the name and pricing configuration
        /// of an existing service.
        /// </summary>
        public void UpdateJobType(int jobTypeID, string jobTypeName, decimal labourCost, decimal transportCost, decimal taxPercentage)
        {
            const string query = @"
                UPDATE JobTypes
                SET
                    JobTypeName = @JobTypeName,
                    BaseLabourCost = @BaseLabourCost,
                    BaseTransportCost = @BaseTransportCost,
                    TaxPercentage = @TaxPercentage
                WHERE JobTypeID = @JobTypeID";
            ExecuteNonQuery(query, new SqlParameter("@JobTypeID", jobTypeID), new SqlParameter("@JobTypeName", jobTypeName), new SqlParameter("@BaseLabourCost", labourCost), new SqlParameter("@BaseTransportCost", transportCost), new SqlParameter("@TaxPercentage", taxPercentage));
        }

        /// <summary>
        /// Creates a new service and returns its ID.
        /// </summary>
        public int CreateJobType(string jobTypeName, decimal labourCost, decimal transportCost, decimal taxPercentage)
        {
            const string query = @"
                INSERT INTO JobTypes
                (
                    JobTypeName,
                    BaseLabourCost,
                    BaseTransportCost,
                    TaxPercentage
                )
                VALUES
                (
                    @JobTypeName,
                    @BaseLabourCost,
                    @BaseTransportCost,
                    @TaxPercentage
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            object result = ExecuteScalar(query, new SqlParameter("@JobTypeName", jobTypeName), new SqlParameter("@BaseLabourCost", labourCost), new SqlParameter("@BaseTransportCost", transportCost), new SqlParameter("@TaxPercentage", taxPercentage));
            if (result == null || result == System.DBNull.Value)
            {
                return 0;
            }

            return System.Convert.ToInt32(result);
        }

        /// <summary>
        /// Adds an inventory item to a service.
        /// </summary>
        public void AddMaterialToJobType(int jobTypeID, int itemID, int quantityRequired)
        {
            const string query = @"
                INSERT INTO JobTypeMaterials
                (
                    JobTypeID,
                    ItemID,
                    QuantityRequired
                )
                VALUES
                (
                    @JobTypeID,
                    @ItemID,
                    @QuantityRequired
                )";
            ExecuteNonQuery(query, new SqlParameter("@JobTypeID", jobTypeID), new SqlParameter("@ItemID", itemID), new SqlParameter("@QuantityRequired", quantityRequired));
        }

        /// <summary>
        /// Updates the required quantity of a material
        /// assigned to a service.
        /// </summary>
        public void UpdateMaterialQuantity(int jobTypeMaterialID, int quantityRequired)
        {
            const string query = @"
                UPDATE JobTypeMaterials
                SET QuantityRequired = @QuantityRequired
                WHERE JobTypeMaterialID = @JobTypeMaterialID";
            ExecuteNonQuery(query, new SqlParameter("@JobTypeMaterialID", jobTypeMaterialID), new SqlParameter("@QuantityRequired", quantityRequired));
        }

        /// <summary>
        /// Removes an inventory item from a service.
        /// </summary>
        public void RemoveMaterialFromJobType(int jobTypeMaterialID)
        {
            const string query = @"
                DELETE FROM JobTypeMaterials
                WHERE JobTypeMaterialID = @JobTypeMaterialID";
            ExecuteNonQuery(query, new SqlParameter("@JobTypeMaterialID", jobTypeMaterialID));
        }

        /// <summary>
        /// Gets all jobs with their service type.
        /// </summary>
        public DataTable GetJobDetailsWithType()
        {
            const string query = @"
                SELECT
                    j.JobID,
                    c.FullName AS CustomerName,
                    s.FullName AS TechnicianName,
                    jt.JobTypeName,
                    j.PromiseDate,
                    j.JobStatus
                FROM Jobs j
                INNER JOIN Customers c
                    ON j.CustomerID = c.CustomerID
                LEFT JOIN Staff s
                    ON j.StaffID = s.StaffID
                LEFT JOIN JobDetails jd
                    ON j.JobID = jd.JobID
                LEFT JOIN JobTypes jt
                    ON jd.JobTypeID = jt.JobTypeID
                WHERE j.JobStatus <> 'Archived'
                ORDER BY j.PromiseDate, j.JobID";
            return ExecuteQuery(query);
        }
    }
}
