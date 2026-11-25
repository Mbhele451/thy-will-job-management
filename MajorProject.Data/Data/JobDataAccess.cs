using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace MajorProject.Data.Data
{
    public class JobDataAccess : DataAccessBase
    {
        private const string PaymentStatusApply = @"
                OUTER APPLY (
                    SELECT TOP (1) p.PaymentStatus
                    FROM dbo.Payments p
                    WHERE p.JobID = j.JobID AND p.CustomerID = j.CustomerID
                    ORDER BY CASE WHEN UPPER(LTRIM(RTRIM(p.PaymentStatus))) = 'PAID' THEN 0 ELSE 1 END,
                        COALESCE(p.PaidAt, p.UpdatedAt, p.CreatedAt) DESC, p.PaymentID DESC
                ) payment";
        private const string PaymentStatusSelect = @"
                    COALESCE(NULLIF(LTRIM(RTRIM(payment.PaymentStatus)), ''), 'Unpaid') AS PaymentStatus,
                    CAST(CASE WHEN DATALENGTH(j.InvoicePDFData) > 0 THEN 1 ELSE 0 END AS bit) AS HasInvoicePdf";
        private const string TechnicianChangeState = @"
                    CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.JobChangeRequests r
                        WHERE r.JobID=j.JobID AND r.RequestStatus='Pending') THEN 1 ELSE 0 END AS bit) AS HasPendingChange,
                    CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.JobChangeRequests r
                        WHERE r.JobID=j.JobID AND r.RequestStatus='Pending'
                        AND r.RequestType='Cancellation' AND j.JobStatus='In Progress'
                        AND NOT EXISTS (SELECT 1 FROM dbo.AppSecurityAudit a
                            WHERE a.ActionName='CancellationMaterials'
                            AND a.Detail='Submitted used quantities for request #' + CONVERT(varchar(20),r.RequestID))
                    ) THEN 1 ELSE 0 END AS bit) AS MaterialReportRequired";
        // ============================================================
        // VALIDATION LIMITS
        // ============================================================
        private const int MaxQuantityPerLine = 1000;
        private const int MaxCartLines = 50;
        private const int MaxJobNoteLength = 255;
        // ============================================================
        // CONSTRUCTOR
        // ============================================================
        public JobDataAccess(string connectionString) : base(connectionString)
        {
        }

        // ============================================================
        // JOB RETRIEVAL
        // ============================================================
        public DataTable GetAllJobs()
        {
            const string query = @"
                SELECT j.*, " + PaymentStatusSelect + @"
                FROM Jobs j " + PaymentStatusApply + @"
                ORDER BY j.JobID DESC";
            return ExecuteQuery(query);
        }

        public DataTable GetJobByID(int jobID)
        {
            const string query = @"
                SELECT *
                FROM Jobs
                WHERE JobID = @JobID";
            return ExecuteQuery(query, new SqlParameter("@JobID", jobID));
        }

        public DataTable GetJobsByStaffID(int staffID)
        {
            const string query = @"
                SELECT
                    JobID,
                    CustomerID,
                    StaffID,
                    JobStatus,
                    PromiseDate,
                    Notes
                FROM Jobs
                WHERE StaffID = @StaffID
                  AND JobStatus <> 'Archived'
                ORDER BY PromiseDate, JobID";
            return ExecuteQuery(query, new SqlParameter("@StaffID", staffID));
        }

        public DataTable GetJobsByStatus(string status)
        {
            const string query = @"
                SELECT *
                FROM Jobs
                WHERE JobStatus = @Status
                ORDER BY PromiseDate, JobID";
            return ExecuteQuery(query, new SqlParameter("@Status", status));
        }

        public int GetJobCountByStatus(int staffID, string status)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM Jobs
                WHERE StaffID = @StaffID
                  AND JobStatus = @Status";
            object result = ExecuteScalar(query, new SqlParameter("@StaffID", staffID), new SqlParameter("@Status", status));
            if (result == null || result == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(result);
        }

        public int GetTotalJobCountByStatus(string status)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM Jobs
                WHERE JobStatus = @Status";
            object result = ExecuteScalar(query, new SqlParameter("@Status", status));
            if (result == null || result == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(result);
        }

        // ============================================================
        // AUTHORITATIVE JOB CREATION
        // ============================================================
        //
        // The cart supplies ONLY:
        // - JobTypeID
        // - Quantity
        //
        // Everything else is calculated here from the database.
        //
        // JobTypes:
        // - Labour
        // - Transport
        // - Tax percentage
        //
        // JobTypeMaterials:
        // - Required material quantities
        //
        // InventoryItems:
        // - Current stock
        // - Current unit prices
        //
        // JobMaterials:
        // - Actual material allocation for this job
        //
        // InventoryTransactions:
        // - Inventory audit history
        //
        // Job creation, material allocation, inventory deduction and
        // inventory transactions all happen inside one transaction.
        // ============================================================
        // ============================================================
        // AUTHORITATIVE JOB CREATION
        // ============================================================
        public int CreateJobWithDetailsAndMaterials(int customerID, DateTime promiseDate, string notes, DataTable cart)
        {
            return CreateJobWithDetailsAndMaterialsInternal(customerID, null, promiseDate, notes, cart);
        }

        public int CreateJobWithDetailsAndMaterials(int customerID, int staffID, DateTime promiseDate, string notes, DataTable cart)
        {
            return CreateJobWithDetailsAndMaterialsInternal(customerID, staffID, promiseDate, notes, cart);
        }

        private int CreateJobWithDetailsAndMaterialsInternal(int customerID, int? requestedStaffID, DateTime promiseDate, string notes, DataTable cart)
        {
            if (customerID <= 0)
            {
                throw new ArgumentException("A valid customer is required.");
            }

            if (requestedStaffID.HasValue && requestedStaffID.Value <= 0)
            {
                throw new ArgumentException("A valid technician is required.");
            }

            if (promiseDate.Date < DateTime.Today)
            {
                throw new ArgumentException("Promise date cannot be in the past.");
            }

            if (cart == null || cart.Rows.Count == 0)
            {
                throw new ArgumentException("At least one service is required.");
            }

            if (cart.Rows.Count > MaxCartLines)
            {
                throw new ArgumentException("A job cannot contain more than " + MaxCartLines + " service lines.");
            }

            notes = (notes ?? string.Empty).Trim();
            if (notes.Length > MaxJobNoteLength)
            {
                throw new ArgumentException("The job note cannot exceed " + MaxJobNoteLength + " characters.");
            }

            using (SqlConnection connection = CreateOpenConnection())
            using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // =================================================
                    // 0. LOCK TECHNICIAN ALLOCATION FOR THIS DATE
                    // =================================================
                    AcquireTechnicianDateLock(connection, transaction, promiseDate.Date);
                    // =================================================
                    // 1. VALIDATE CUSTOMER
                    // =================================================
                    const string customerQuery = @"
                SELECT CustomerID, AccountStatus
                FROM Customers WITH (UPDLOCK, HOLDLOCK)
                WHERE CustomerID = @CustomerID;";
                    using (SqlCommand command = new SqlCommand(customerQuery, connection, transaction))
                    {
                        command.Parameters.Add(new SqlParameter("@CustomerID", customerID));
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                throw new InvalidOperationException("The selected customer does not exist.");
                            }

                            string accountStatus = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            if (!accountStatus.Equals("Active", StringComparison.OrdinalIgnoreCase))
                            {
                                throw new InvalidOperationException("The selected customer is no longer active.");
                            }
                        }
                    }

                    // =================================================
                    // 2. SELECT OR VALIDATE TECHNICIAN
                    // =================================================
                    int staffID;
                    if (!requestedStaffID.HasValue)
                    {
                        // ---------------------------------------------
                        // AUTOMATIC ALLOCATION
                        // ---------------------------------------------
                        const string selectionQuery = @"
                    SELECT TOP (1)
                        s.StaffID,
                        (
                            SELECT COUNT(*)
                            FROM Jobs AS workloadJobs
                            WHERE workloadJobs.StaffID = s.StaffID
                              AND workloadJobs.CreatedAt >=
                                  DATEADD(DAY, -30, GETDATE())
                              AND workloadJobs.CreatedAt < GETDATE()
                              AND workloadJobs.JobStatus NOT IN
                                  ('Cancelled', 'Archived')
                        ) AS RecentWorkload
                    FROM Staff AS s WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN Roles AS r
                        ON s.RoleID = r.RoleID
                    WHERE r.RoleName = 'Technician'
                      AND s.EmploymentStatus = 'Active'
                      AND s.IsAvailable = 1
                      AND NOT EXISTS (SELECT 1 FROM dbo.AppAccountSecurity security WITH (UPDLOCK, HOLDLOCK) WHERE security.StaffID=s.StaffID AND security.IsBlocked=1)
                      AND NOT EXISTS
                      (
                          SELECT 1
                          FROM Jobs AS activeJobs WITH (UPDLOCK, HOLDLOCK)
                          WHERE activeJobs.StaffID = s.StaffID
                            AND activeJobs.PromiseDate >= @PromiseDate
                            AND activeJobs.PromiseDate <
                                DATEADD(DAY, 1, @PromiseDate)
                            AND activeJobs.JobStatus NOT IN
                                ('Completed', 'Cancelled', 'Archived')
                      )
                    ORDER BY
                        RecentWorkload ASC,
                        NEWID();";
                        using (SqlCommand command = new SqlCommand(selectionQuery, connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@PromiseDate", SqlDbType.Date) { Value = promiseDate.Date });
                            object result = command.ExecuteScalar();
                            if (result == null || result == DBNull.Value)
                            {
                                throw new InvalidOperationException("No available technician could be found for the selected date. Please choose another date.");
                            }

                            staffID = Convert.ToInt32(result);
                        }

                        if (staffID <= 0)
                        {
                            throw new InvalidOperationException("The system could not allocate a technician.");
                        }
                    }
                    else
                    {
                        // ---------------------------------------------
                        // EXPLICIT TECHNICIAN ASSIGNMENT
                        // ---------------------------------------------
                        staffID = requestedStaffID.Value;
                        const string technicianQuery = @"
                    SELECT
                        s.StaffID,
                        s.EmploymentStatus,
                        s.IsAvailable,
                        r.RoleName
                    FROM Staff AS s WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN Roles AS r
                        ON s.RoleID = r.RoleID
                    WHERE s.StaffID = @StaffID
                      AND NOT EXISTS (SELECT 1 FROM dbo.AppAccountSecurity security WITH (UPDLOCK, HOLDLOCK) WHERE security.StaffID=s.StaffID AND security.IsBlocked=1);";
                        using (SqlCommand command = new SqlCommand(technicianQuery, connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@StaffID", staffID));
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    throw new InvalidOperationException("The selected technician does not exist.");
                                }

                                string roleName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                string employmentStatus = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                bool isAvailable = !reader.IsDBNull(2) && Convert.ToBoolean(reader.GetValue(2));
                                if (!roleName.Equals("Technician", StringComparison.OrdinalIgnoreCase))
                                {
                                    throw new InvalidOperationException("The selected staff member is not a valid technician.");
                                }

                                if (!employmentStatus.Equals("Active", StringComparison.OrdinalIgnoreCase))
                                {
                                    throw new InvalidOperationException("The selected technician is no longer active.");
                                }

                                if (!isAvailable)
                                {
                                    throw new InvalidOperationException("The selected technician is currently unavailable.");
                                }
                            }
                        }
                    }

                    // =================================================
                    // 3. FINAL TECHNICIAN DATE CONFLICT CHECK
                    // =================================================
                    const string conflictQuery = @"
                SELECT COUNT(*)
                FROM Jobs WITH (UPDLOCK, HOLDLOCK)
                WHERE StaffID = @StaffID
                  AND PromiseDate >= @PromiseDate
                  AND PromiseDate < DATEADD(DAY, 1, @PromiseDate)
                  AND JobStatus NOT IN
                      ('Completed', 'Cancelled', 'Archived');";
                    object conflictResult = ExecuteTransactionScalar(connection, transaction, conflictQuery, new SqlParameter("@StaffID", staffID), new SqlParameter("@PromiseDate", promiseDate.Date));
                    int conflicts = conflictResult == null || conflictResult == DBNull.Value ? 0 : Convert.ToInt32(conflictResult);
                    if (conflicts > 0)
                    {
                        throw new InvalidOperationException("The selected technician is already assigned to another active job on the selected promise date.");
                    }

                    // =================================================
                    // 4. CALCULATE AUTHORITATIVE JOB TOTALS
                    // =================================================
                    decimal authoritativeLabour = 0m;
                    decimal authoritativeTransport = 0m;
                    decimal authoritativeMaterials = 0m;
                    decimal authoritativeTax = 0m;
                    Dictionary<int, int> stockRequirements = new Dictionary<int, int>();
                    Dictionary<int, decimal> unitPrices = new Dictionary<int, decimal>();
                    int validCartLines = 0;
                    foreach (DataRow cartRow in cart.Rows)
                    {
                        if (cartRow.RowState == DataRowState.Deleted)
                        {
                            continue;
                        }

                        if (!cartRow.Table.Columns.Contains("JobTypeID") || !cartRow.Table.Columns.Contains("Quantity"))
                        {
                            throw new ArgumentException("The cart must contain JobTypeID and Quantity columns.");
                        }

                        int jobTypeID;
                        int quantity;
                        if (!int.TryParse(Convert.ToString(cartRow["JobTypeID"]), out jobTypeID) || jobTypeID <= 0)
                        {
                            throw new ArgumentException("The job contains an invalid service type.");
                        }

                        if (!int.TryParse(Convert.ToString(cartRow["Quantity"]), out quantity) || quantity <= 0)
                        {
                            throw new ArgumentException("Every service quantity must be greater than zero.");
                        }

                        if (quantity > MaxQuantityPerLine)
                        {
                            throw new ArgumentException("A service quantity cannot exceed " + MaxQuantityPerLine + ".");
                        }

                        validCartLines++;
                        // ---------------------------------------------
                        // 4A. LOAD SERVICE PRICING
                        // ---------------------------------------------
                        const string serviceQuery = @"
                    SELECT
                        JobTypeName,
                        BaseLabourCost,
                        BaseTransportCost,
                        TaxPercentage
                    FROM JobTypes WITH (UPDLOCK, HOLDLOCK)
                    WHERE JobTypeID = @JobTypeID;";
                        string jobTypeName;
                        decimal baseLabour;
                        decimal baseTransport;
                        decimal taxPercentage;
                        using (SqlCommand command = new SqlCommand(serviceQuery, connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@JobTypeID", jobTypeID));
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    throw new InvalidOperationException("One of the selected service types no longer exists.");
                                }

                                jobTypeName = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                                baseLabour = reader.IsDBNull(1) ? -1m : reader.GetDecimal(1);
                                baseTransport = reader.IsDBNull(2) ? -1m : reader.GetDecimal(2);
                                taxPercentage = reader.IsDBNull(3) ? -1m : reader.GetDecimal(3);
                            }
                        }

                        if (baseLabour < 0m)
                        {
                            throw new InvalidOperationException("The service '" + jobTypeName + "' has an invalid labour cost configuration.");
                        }

                        if (baseTransport < 0m)
                        {
                            throw new InvalidOperationException("The service '" + jobTypeName + "' has an invalid transport cost configuration.");
                        }

                        if (taxPercentage < 0m || taxPercentage > 100m)
                        {
                            throw new InvalidOperationException("The service '" + jobTypeName + "' has an invalid tax configuration.");
                        }

                        // ---------------------------------------------
                        // 4B. LOAD SERVICE MATERIAL TEMPLATE
                        // ---------------------------------------------
                        const string materialsQuery = @"
                    SELECT
                        jtm.ItemID,
                        jtm.QuantityRequired,
                        i.ItemName,
                        i.UnitPrice
                    FROM JobTypeMaterials AS jtm WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN InventoryItems AS i WITH (UPDLOCK, HOLDLOCK)
                        ON jtm.ItemID = i.ItemID
                    WHERE jtm.JobTypeID = @JobTypeID
                      AND jtm.QuantityRequired > 0
                    ORDER BY jtm.JobTypeMaterialID;";
                        decimal materialCostPerUnit = 0m;
                        using (SqlCommand command = new SqlCommand(materialsQuery, connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@JobTypeID", jobTypeID));
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int itemID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                    int quantityRequired = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                    string itemName = reader.IsDBNull(2) ? "Unknown item" : reader.GetString(2);
                                    decimal unitPrice = reader.IsDBNull(3) ? -1m : reader.GetDecimal(3);
                                    if (itemID <= 0 || quantityRequired <= 0)
                                    {
                                        throw new InvalidOperationException("The service '" + jobTypeName + "' contains an invalid material configuration.");
                                    }

                                    if (unitPrice < 0m)
                                    {
                                        throw new InvalidOperationException("Inventory item '" + itemName + "' has an invalid unit price.");
                                    }

                                    materialCostPerUnit += quantityRequired * unitPrice;
                                    long totalRequired = (long)quantityRequired * quantity;
                                    if (totalRequired > int.MaxValue)
                                    {
                                        throw new InvalidOperationException("The calculated inventory requirement is too large.");
                                    }

                                    int requiredQuantity = (int)totalRequired;
                                    if (stockRequirements.ContainsKey(itemID))
                                    {
                                        long combinedQuantity = (long)stockRequirements[itemID] + requiredQuantity;
                                        if (combinedQuantity > int.MaxValue)
                                        {
                                            throw new InvalidOperationException("The calculated inventory requirement is too large.");
                                        }

                                        stockRequirements[itemID] = (int)combinedQuantity;
                                    }
                                    else
                                    {
                                        stockRequirements.Add(itemID, requiredQuantity);
                                    }

                                    if (!unitPrices.ContainsKey(itemID))
                                    {
                                        unitPrices.Add(itemID, unitPrice);
                                    }
                                }
                            }
                        }

                        // ---------------------------------------------
                        // 4C. CALCULATE SERVICE LINE TOTALS
                        // ---------------------------------------------
                        decimal lineLabour = baseLabour * quantity;
                        decimal lineTransport = baseTransport * quantity;
                        decimal lineMaterials = materialCostPerUnit * quantity;
                        decimal lineSubtotal = lineLabour + lineTransport + lineMaterials;
                        decimal lineTax = lineSubtotal * (taxPercentage / 100m);
                        authoritativeLabour += lineLabour;
                        authoritativeTransport += lineTransport;
                        authoritativeMaterials += lineMaterials;
                        authoritativeTax += lineTax;
                    }

                    if (validCartLines == 0)
                    {
                        throw new ArgumentException("At least one valid service is required.");
                    }

                    // =================================================
                    // 5. CALCULATE GRAND TOTAL
                    // =================================================
                    decimal authoritativeSubtotal = authoritativeLabour + authoritativeTransport + authoritativeMaterials;
                    decimal authoritativeGrandTotal = authoritativeSubtotal + authoritativeTax;
                    if (authoritativeLabour < 0m || authoritativeTransport < 0m || authoritativeMaterials < 0m || authoritativeTax < 0m || authoritativeGrandTotal < 0m)
                    {
                        throw new InvalidOperationException("The job contains invalid financial values.");
                    }

                    // =================================================
                    // 6. VALIDATE CURRENT INVENTORY
                    // =================================================
                    foreach (KeyValuePair<int, int> requirement in stockRequirements)
                    {
                        if (requirement.Key <= 0 || requirement.Value <= 0)
                        {
                            throw new InvalidOperationException("The job contains an invalid inventory requirement.");
                        }

                        const string stockQuery = @"
                    SELECT
                        ItemName,
                        QuantityInStock,
                        UnitPrice
                    FROM InventoryItems WITH (UPDLOCK, HOLDLOCK)
                    WHERE ItemID = @ItemID;";
                        using (SqlCommand command = new SqlCommand(stockQuery, connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@ItemID", requirement.Key));
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    throw new InvalidOperationException("Inventory item #" + requirement.Key + " could not be found.");
                                }

                                string itemName = reader.IsDBNull(0) ? "Unknown item" : reader.GetString(0);
                                int available = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                decimal unitPrice = reader.IsDBNull(2) ? -1m : reader.GetDecimal(2);
                                if (unitPrice < 0m)
                                {
                                    throw new InvalidOperationException("Inventory item '" + itemName + "' has an invalid unit price.");
                                }

                                if (available < requirement.Value)
                                {
                                    throw new InvalidOperationException("Insufficient stock for " + itemName + ". Available: " + available + ", Needed: " + requirement.Value);
                                }

                                unitPrices[requirement.Key] = unitPrice;
                            }
                        }
                    }

                    // =================================================
                    // 7. CREATE JOB HEADER
                    // =================================================
                    const string jobQuery = @"
                INSERT INTO Jobs
                (
                    CustomerID,
                    StaffID,
                    PromiseDate,
                    JobStatus,
                    TotalMaterialCost,
                    LabourCost,
                    TransportCost,
                    TaxAmount,
                    GrandTotal,
                    Notes,
                    CreatedAt,
                    UpdatedAt
                )
                VALUES
                (
                    @CustomerID,
                    @StaffID,
                    @PromiseDate,
                    'Pending',
                    @TotalMaterialCost,
                    @LabourCost,
                    @TransportCost,
                    @TaxAmount,
                    @GrandTotal,
                    @Notes,
                    @CreatedAt,
                    @UpdatedAt
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    int jobID;
                    using (SqlCommand command = new SqlCommand(jobQuery, connection, transaction))
                    {
                        DateTime now = DateTime.Now;
                        command.Parameters.Add(new SqlParameter("@CustomerID", customerID));
                        command.Parameters.Add(new SqlParameter("@StaffID", staffID));
                        command.Parameters.Add(new SqlParameter("@PromiseDate", promiseDate.Date));
                        command.Parameters.Add(new SqlParameter("@TotalMaterialCost", authoritativeMaterials));
                        command.Parameters.Add(new SqlParameter("@LabourCost", authoritativeLabour));
                        command.Parameters.Add(new SqlParameter("@TransportCost", authoritativeTransport));
                        command.Parameters.Add(new SqlParameter("@TaxAmount", authoritativeTax));
                        command.Parameters.Add(new SqlParameter("@GrandTotal", authoritativeGrandTotal));
                        command.Parameters.Add(new SqlParameter("@Notes", string.IsNullOrWhiteSpace(notes) ? (object)DBNull.Value : notes));
                        command.Parameters.Add(new SqlParameter("@CreatedAt", now));
                        command.Parameters.Add(new SqlParameter("@UpdatedAt", now));
                        object result = command.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            throw new InvalidOperationException("The job could not be created.");
                        }

                        jobID = Convert.ToInt32(result);
                    }

                    if (jobID <= 0)
                    {
                        throw new InvalidOperationException("The database did not return a valid JobID.");
                    }

                    // =================================================
                    // 8. CREATE JOB DETAILS
                    // =================================================
                    const string detailQuery = @"
                INSERT INTO JobDetails
                (
                    JobID,
                    JobTypeID,
                    Quantity
                )
                VALUES
                (
                    @JobID,
                    @JobTypeID,
                    @Quantity
                );";
                    foreach (DataRow row in cart.Rows)
                    {
                        if (row.RowState == DataRowState.Deleted)
                        {
                            continue;
                        }

                        int jobTypeID = Convert.ToInt32(row["JobTypeID"]);
                        int quantity = Convert.ToInt32(row["Quantity"]);
                        using (SqlCommand command = new SqlCommand(detailQuery, connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@JobID", jobID));
                            command.Parameters.Add(new SqlParameter("@JobTypeID", jobTypeID));
                            command.Parameters.Add(new SqlParameter("@Quantity", quantity));
                            if (command.ExecuteNonQuery() != 1)
                            {
                                throw new InvalidOperationException("Unable to create the job service detail.");
                            }
                        }
                    }

                    // =================================================
                    // 9. CREATE JOB MATERIALS
                    // =================================================
                    const string materialQuery = @"
                INSERT INTO JobMaterials
                (
                    JobID,
                    ItemID,
                    QuantityUsed,
                    TotalCost
                )
                VALUES
                (
                    @JobID,
                    @ItemID,
                    @QuantityUsed,
                    @TotalCost
                );";
                    foreach (KeyValuePair<int, int> requirement in stockRequirements)
                    {
                        if (requirement.Value <= 0)
                        {
                            continue;
                        }

                        if (!unitPrices.ContainsKey(requirement.Key))
                        {
                            throw new InvalidOperationException("The unit price for inventory item #" + requirement.Key + " could not be determined.");
                        }

                        decimal unitPrice = unitPrices[requirement.Key];
                        decimal totalCost = requirement.Value * unitPrice;
                        using (SqlCommand command = new SqlCommand(materialQuery, connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@JobID", jobID));
                            command.Parameters.Add(new SqlParameter("@ItemID", requirement.Key));
                            command.Parameters.Add(new SqlParameter("@QuantityUsed", requirement.Value));
                            command.Parameters.Add(new SqlParameter("@TotalCost", totalCost));
                            if (command.ExecuteNonQuery() != 1)
                            {
                                throw new InvalidOperationException("Unable to create material record for inventory item #" + requirement.Key + ".");
                            }
                        }
                    }

                    // =================================================
                    // 10. DEDUCT INVENTORY
                    // =================================================
                    const string inventoryQuery = @"
                UPDATE InventoryItems
                SET
                    QuantityInStock = QuantityInStock - @Quantity,
                    UpdatedAt = @UpdatedAt
                WHERE ItemID = @ItemID
                  AND QuantityInStock >= @Quantity;";
                    foreach (KeyValuePair<int, int> requirement in stockRequirements)
                    {
                        if (requirement.Value <= 0)
                        {
                            continue;
                        }

                        using (SqlCommand command = new SqlCommand(inventoryQuery, connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@ItemID", requirement.Key));
                            command.Parameters.Add(new SqlParameter("@Quantity", requirement.Value));
                            command.Parameters.Add(new SqlParameter("@UpdatedAt", DateTime.Now));
                            if (command.ExecuteNonQuery() != 1)
                            {
                                throw new InvalidOperationException("Inventory could not be updated for item #" + requirement.Key + ".");
                            }
                        }
                    }

                    // =================================================
                    // 11. RECORD INVENTORY OUT TRANSACTIONS
                    // =================================================
                    const string transactionQuery = @"
                INSERT INTO InventoryTransactions
                (
                    ItemID,
                    JobID,
                    TransactionType,
                    Quantity,
                    TransactionDate
                )
                VALUES
                (
                    @ItemID,
                    @JobID,
                    'OUT',
                    @Quantity,
                    @TransactionDate
                );";
                    foreach (KeyValuePair<int, int> requirement in stockRequirements)
                    {
                        if (requirement.Value <= 0)
                        {
                            continue;
                        }

                        using (SqlCommand command = new SqlCommand(transactionQuery, connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@ItemID", requirement.Key));
                            command.Parameters.Add(new SqlParameter("@JobID", jobID));
                            command.Parameters.Add(new SqlParameter("@Quantity", requirement.Value));
                            command.Parameters.Add(new SqlParameter("@TransactionDate", DateTime.Now));
                            if (command.ExecuteNonQuery() != 1)
                            {
                                throw new InvalidOperationException("The inventory transaction could not be recorded for item #" + requirement.Key + ".");
                            }
                        }
                    }

                    // =================================================
                    // 12. COMMIT
                    // =================================================
                    transaction.Commit();
                    return jobID;
                }
                catch
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                        // Preserve the original exception.
                    }

                    throw;
                }
            }
        }

        // ============================================================
        // TECHNICIAN ALLOCATION LOCK
        // ============================================================
        internal static void AcquireTechnicianDateLock(SqlConnection connection, SqlTransaction transaction, DateTime promiseDate)
        {
            string resource = "THYWILL:TECHNICIAN-ALLOCATION:" + promiseDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            const string query = @"
        DECLARE @LockResult INT;

        EXEC @LockResult = sys.sp_getapplock
            @Resource = @Resource,
            @LockMode = 'Exclusive',
            @LockOwner = 'Transaction',
            @LockTimeout = 15000,
            @DbPrincipal = 'public';

        SELECT @LockResult;";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@Resource", SqlDbType.NVarChar, 255) { Value = resource });
                object result = command.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    throw new InvalidOperationException("The technician allocation lock could not be acquired.");
                }

                int lockResult = Convert.ToInt32(result);
                if (lockResult < 0)
                {
                    throw new InvalidOperationException("Technician allocation is busy. Please try again.");
                }
            }
        }

        // ============================================================
        // JOB STATUS
        // ============================================================
        public int UpdateJobStatus(int jobID, string status, string notes = null)
        {
            if (string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Submit a cancellation request for manager review.");
            string query = "UPDATE Jobs SET JobStatus = @Status, UpdatedAt = @UpdatedAt";
            if (!string.IsNullOrWhiteSpace(notes))
            {
                query += ", Notes = @Notes";
            }

            query += @" WHERE JobID = @JobID AND JobStatus NOT IN ('Cancelled','Archived')
                AND NOT EXISTS(SELECT 1 FROM dbo.JobChangeRequests r WHERE r.JobID=@JobID AND r.RequestStatus='Pending')";
            if (!string.IsNullOrWhiteSpace(notes))
            {
                return ExecuteNonQuery(query, new SqlParameter("@Status", status), new SqlParameter("@UpdatedAt", DateTime.Now), new SqlParameter("@Notes", notes), new SqlParameter("@JobID", jobID));
            }

            return ExecuteNonQuery(query, new SqlParameter("@Status", status), new SqlParameter("@UpdatedAt", DateTime.Now), new SqlParameter("@JobID", jobID));
        }

        // ============================================================
        // TRANSACTION HELPERS
        // ============================================================
        private DataTable ExecuteTransactionQuery(SqlConnection connection, SqlTransaction transaction, string query, params SqlParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        private object ExecuteTransactionScalar(SqlConnection connection, SqlTransaction transaction, string query, params SqlParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                return command.ExecuteScalar();
            }
        }

        // ============================================================
        // RESCHEDULE JOB
        // ============================================================
        //
        // This is the authoritative method for changing a job's:
        // - PromiseDate
        // - Technician
        //
        // The availability check is performed inside the transaction.
        // ============================================================
        public void RescheduleJob(int jobID, DateTime newPromiseDate, int newStaffID)
        {
            if (jobID <= 0)
            {
                throw new ArgumentException("Invalid job.");
            }

            if (newStaffID <= 0)
            {
                throw new ArgumentException("Invalid technician.");
            }

            if (newPromiseDate.Date < DateTime.Today)
            {
                throw new ArgumentException("Promise date cannot be earlier than today.");
            }

            using (SqlConnection connection = CreateOpenConnection())
            using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // Acquire the same date lock used by new bookings.
                    AcquireTechnicianDateLock(connection, transaction, newPromiseDate.Date);
                    const string jobQuery = @"
                SELECT
                    JobID,
                    StaffID,
                    PromiseDate,
                    JobStatus
                FROM Jobs WITH (UPDLOCK, HOLDLOCK)
                WHERE JobID = @JobID;";
                    DataTable jobTable = ExecuteTransactionQuery(connection, transaction, jobQuery, new SqlParameter("@JobID", jobID));
                    if (jobTable.Rows.Count == 0)
                    {
                        throw new InvalidOperationException("The selected job could not be found.");
                    }

                    DataRow jobRow = jobTable.Rows[0];
                    string status = Convert.ToString(jobRow["JobStatus"]).Trim();
                    if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase) || status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) || status.Equals("Archived", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Completed, cancelled or archived jobs cannot be rescheduled.");
                    }

                    if (ExecuteTransactionQuery(connection, transaction, "SELECT RequestID FROM dbo.JobChangeRequests WITH(UPDLOCK,HOLDLOCK) WHERE JobID=@j AND RequestStatus='Pending'", new SqlParameter("@j", jobID)).Rows.Count > 0)
                        throw new InvalidOperationException("Review the pending job request before changing this assignment.");
                    // Validate technician.
                    const string technicianQuery = @"
                SELECT
                    s.StaffID,
                    s.EmploymentStatus,
                    s.IsAvailable,
                    r.RoleName
                FROM Staff AS s WITH (UPDLOCK, HOLDLOCK)
                INNER JOIN Roles AS r
                    ON s.RoleID = r.RoleID
                WHERE s.StaffID = @StaffID
                  AND NOT EXISTS (SELECT 1 FROM dbo.AppAccountSecurity security WITH (UPDLOCK, HOLDLOCK) WHERE security.StaffID=s.StaffID AND security.IsBlocked=1);";
                    DataTable technicianTable = ExecuteTransactionQuery(connection, transaction, technicianQuery, new SqlParameter("@StaffID", newStaffID));
                    if (technicianTable.Rows.Count == 0)
                    {
                        throw new InvalidOperationException("The selected technician could not be found.");
                    }

                    DataRow technicianRow = technicianTable.Rows[0];
                    string roleName = Convert.ToString(technicianRow["RoleName"]).Trim();
                    string employmentStatus = Convert.ToString(technicianRow["EmploymentStatus"]).Trim();
                    bool isAvailable = technicianRow["IsAvailable"] != DBNull.Value && Convert.ToBoolean(technicianRow["IsAvailable"]);
                    if (!roleName.Equals("Technician", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("The selected staff member is not a technician.");
                    }

                    if (!employmentStatus.Equals("Active", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("The selected technician is not active.");
                    }

                    if (!isAvailable)
                    {
                        throw new InvalidOperationException("The selected technician is currently unavailable.");
                    }

                    // Check for another active job on the new date.
                    const string conflictQuery = @"
                SELECT COUNT(*)
                FROM Jobs WITH (UPDLOCK, HOLDLOCK)
                WHERE StaffID = @StaffID
                  AND PromiseDate >= @PromiseDate
                  AND PromiseDate < DATEADD(DAY, 1, @PromiseDate)
                  AND JobID <> @JobID
                  AND JobStatus NOT IN
                      ('Completed', 'Cancelled', 'Archived');";
                    object conflictResult = ExecuteTransactionScalar(connection, transaction, conflictQuery, new SqlParameter("@StaffID", newStaffID), new SqlParameter("@PromiseDate", newPromiseDate.Date), new SqlParameter("@JobID", jobID));
                    int conflictCount = conflictResult == null || conflictResult == DBNull.Value ? 0 : Convert.ToInt32(conflictResult);
                    if (conflictCount > 0)
                    {
                        throw new InvalidOperationException("The selected technician is already assigned to another active job on the new promise date.");
                    }

                    // Update assignment and promise date.
                    const string updateQuery = @"
                UPDATE Jobs
                SET
                    PromiseDate = @PromiseDate,
                    StaffID = @StaffID,
                    UpdatedAt = @UpdatedAt
                WHERE JobID = @JobID
                  AND JobStatus NOT IN
                      ('Completed', 'Cancelled', 'Archived');";
                    using (SqlCommand command = new SqlCommand(updateQuery, connection, transaction))
                    {
                        command.Parameters.Add(new SqlParameter("@PromiseDate", newPromiseDate.Date));
                        command.Parameters.Add(new SqlParameter("@StaffID", newStaffID));
                        command.Parameters.Add(new SqlParameter("@UpdatedAt", DateTime.Now));
                        command.Parameters.Add(new SqlParameter("@JobID", jobID));
                        if (command.ExecuteNonQuery() != 1)
                        {
                            throw new InvalidOperationException("The job could not be rescheduled.");
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                        // Preserve the original exception.
                    }

                    throw;
                }
            }
        }

        // ============================================================
        // CANCEL JOB AND RESTORE INVENTORY
        // ============================================================
        //
        // This is the manager/admin cancellation path.
        //
        // The exact JobMaterials allocation is restored.
        //
        // JobTypeMaterials is NOT used for cancellation because the
        // service template may have changed since the job was created.
        // ============================================================
        public bool CancelJobAndRestoreInventory(int jobID, out bool isLateCancellation)
        {
            isLateCancellation = false;
            throw new InvalidOperationException("Record the customer's cancellation request for manager review.");
        }

        // ============================================================
        // TECHNICIAN CANCEL JOB
        // ============================================================
        //
        // This path additionally verifies that the job belongs to the
        // technician performing the cancellation.
        // ============================================================
        public bool CancelTechnicianJob(int jobID, int staffID, out bool isLateCancellation)
        {
            isLateCancellation = false;
            throw new InvalidOperationException("Only customers or office staff can request cancellation. Report material usage when notified.");
        }

        // ============================================================
        // CANCEL JOB INTERNAL TRANSACTION
        // ============================================================
        private bool CancelJobAndRestoreInventoryInternal(int jobID, int staffID, bool requireTechnicianOwnership, out bool isLateCancellation)
        {
            isLateCancellation = false;
            if (jobID <= 0)
            {
                throw new ArgumentException("Invalid job.");
            }

            using (SqlConnection connection = CreateOpenConnection())
            using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // =================================================
                    // 1. LOCK JOB
                    // =================================================
                    const string jobQuery = @"
                        SELECT
                            JobID,
                            StaffID,
                            PromiseDate,
                            JobStatus
                        FROM Jobs WITH (UPDLOCK, HOLDLOCK)
                        WHERE JobID = @JobID;";
                    DataTable jobTable = ExecuteTransactionQuery(connection, transaction, jobQuery, new SqlParameter("@JobID", jobID));
                    if (jobTable.Rows.Count == 0)
                    {
                        throw new InvalidOperationException("The selected job could not be found.");
                    }

                    DataRow jobRow = jobTable.Rows[0];
                    int assignedStaffID = jobRow["StaffID"] == DBNull.Value ? 0 : Convert.ToInt32(jobRow["StaffID"]);
                    if (requireTechnicianOwnership && assignedStaffID != staffID)
                    {
                        throw new InvalidOperationException("The selected job does not belong to this technician.");
                    }

                    string status = Convert.ToString(jobRow["JobStatus"]).Trim();
                    if (!status.Equals("Pending", StringComparison.OrdinalIgnoreCase) && !status.Equals("In Progress", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Only Pending or In Progress jobs can be cancelled.");
                    }

                    if (jobRow["PromiseDate"] == DBNull.Value)
                    {
                        throw new InvalidOperationException("The selected job does not have a valid promise date.");
                    }

                    DateTime promiseDate = Convert.ToDateTime(jobRow["PromiseDate"]).Date;
                    int daysUntilJob = (promiseDate - DateTime.Today).Days;
                    isLateCancellation = daysUntilJob < 3;
                    // =================================================
                    // 2. LOAD EXACT JOB MATERIALS
                    // =================================================
                    const string materialsQuery = @"
                        SELECT
                            JobMaterialID,
                            ItemID,
                            QuantityUsed
                        FROM JobMaterials WITH (UPDLOCK, HOLDLOCK)
                        WHERE JobID = @JobID
                        ORDER BY JobMaterialID;";
                    DataTable materialsTable = ExecuteTransactionQuery(connection, transaction, materialsQuery, new SqlParameter("@JobID", jobID));
                    // =================================================
                    // 3. RESTORE INVENTORY
                    // =================================================
                    const string inventoryQuery = @"
                        UPDATE InventoryItems
                        SET
                            QuantityInStock = ISNULL(QuantityInStock, 0) + @QuantityUsed,
                            UpdatedAt = @UpdatedAt
                        WHERE ItemID = @ItemID;";
                    foreach (DataRow materialRow in materialsTable.Rows)
                    {
                        if (materialRow["ItemID"] == DBNull.Value)
                        {
                            throw new InvalidOperationException("A job material contains an invalid inventory item.");
                        }

                        if (materialRow["QuantityUsed"] == DBNull.Value)
                        {
                            throw new InvalidOperationException("A job material contains an invalid quantity.");
                        }

                        int itemID = Convert.ToInt32(materialRow["ItemID"]);
                        int quantityUsed = Convert.ToInt32(materialRow["QuantityUsed"]);
                        if (itemID <= 0)
                        {
                            throw new InvalidOperationException("A job material contains an invalid inventory item.");
                        }

                        if (quantityUsed <= 0)
                        {
                            throw new InvalidOperationException("A job material contains an invalid quantity.");
                        }

                        using (SqlCommand command = new SqlCommand(inventoryQuery, connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@QuantityUsed", quantityUsed));
                            command.Parameters.Add(new SqlParameter("@UpdatedAt", DateTime.Now));
                            command.Parameters.Add(new SqlParameter("@ItemID", itemID));
                            int affectedRows = command.ExecuteNonQuery();
                            if (affectedRows != 1)
                            {
                                throw new InvalidOperationException("Inventory item " + itemID + " could not be restored.");
                            }
                        }

                        // =================================================
                        // 4. RECORD INVENTORY IN TRANSACTION
                        // =================================================
                        const string transactionQuery = @"
                            INSERT INTO InventoryTransactions
                            (
                                ItemID,
                                JobID,
                                TransactionType,
                                Quantity,
                                TransactionDate
                            )
                            VALUES
                            (
                                @ItemID,
                                @JobID,
                                'IN',
                                @Quantity,
                                @TransactionDate
                            );";
                        using (SqlCommand command = new SqlCommand(transactionQuery, connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@ItemID", itemID));
                            command.Parameters.Add(new SqlParameter("@JobID", jobID));
                            command.Parameters.Add(new SqlParameter("@Quantity", quantityUsed));
                            command.Parameters.Add(new SqlParameter("@TransactionDate", DateTime.Now));
                            int affectedRows = command.ExecuteNonQuery();
                            if (affectedRows != 1)
                            {
                                throw new InvalidOperationException("The inventory restoration transaction could not be recorded for item #" + itemID + ".");
                            }
                        }
                    }

                    // =================================================
                    // 5. CANCEL JOB
                    // =================================================
                    const string cancelQuery = @"
                        UPDATE Jobs
                        SET
                            JobStatus = 'Cancelled',
                            UpdatedAt = @UpdatedAt
                        WHERE JobID = @JobID
                          AND JobStatus IN ('Pending', 'In Progress')";
                    if (requireTechnicianOwnership)
                    {
                        using (SqlCommand command = new SqlCommand(cancelQuery + " AND StaffID = @StaffID;", connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@UpdatedAt", DateTime.Now));
                            command.Parameters.Add(new SqlParameter("@JobID", jobID));
                            command.Parameters.Add(new SqlParameter("@StaffID", staffID));
                            int affectedRows = command.ExecuteNonQuery();
                            if (affectedRows != 1)
                            {
                                throw new InvalidOperationException("The job could not be cancelled.");
                            }
                        }
                    }
                    else
                    {
                        using (SqlCommand command = new SqlCommand(cancelQuery, connection, transaction))
                        {
                            command.Parameters.Add(new SqlParameter("@UpdatedAt", DateTime.Now));
                            command.Parameters.Add(new SqlParameter("@JobID", jobID));
                            int affectedRows = command.ExecuteNonQuery();
                            if (affectedRows != 1)
                            {
                                throw new InvalidOperationException("The job could not be cancelled.");
                            }
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                    }

                    throw;
                }
            }
        }

        // ============================================================
        // ARCHIVE JOB
        // ============================================================
        public int ArchiveJob(int jobID)
        {
            const string query = @"
                UPDATE Jobs
                SET
                    JobStatus = 'Archived',
                    UpdatedAt = GETDATE()
                WHERE JobID = @JobID
                  AND JobStatus IN ('Completed', 'Cancelled');";
            return ExecuteNonQuery(query, new SqlParameter("@JobID", jobID));
        }

        // ============================================================
        // JOB VIEWS
        // ============================================================
        public DataTable GetJobViews()
        {
            const string query = @"
                SELECT DISTINCT
                    j.JobID,
                    j.CustomerID,
                    c.FullName AS CustomerName,
                    c.PhoneNumber,
                    c.EmailAddress,
                    c.FullAddress,
                    c.Area AS ServiceArea,
                    j.StaffID,
                    s.FullName AS TechnicianName,
                    j.JobStatus,
                    j.PromiseDate,
                    j.TotalMaterialCost,
                    j.LabourCost,
                    j.TransportCost,
                    j.TaxAmount,
                    j.GrandTotal,
                    j.Notes,
                    j.CreatedAt,
                    STUFF((SELECT ', ' + jt.JobTypeName + ' x ' + CAST(jd.Quantity AS varchar(10))
                        FROM JobDetails jd INNER JOIN JobTypes jt ON jt.JobTypeID = jd.JobTypeID
                        WHERE jd.JobID = j.JobID ORDER BY jd.JobDetailID
                        FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '') AS Services,
                    " + PaymentStatusSelect + @"
                FROM Jobs AS j
                INNER JOIN Customers AS c
                    ON j.CustomerID = c.CustomerID
                LEFT JOIN Staff AS s
                    ON j.StaffID = s.StaffID
                " + PaymentStatusApply + @"
                WHERE j.JobStatus <> 'Archived'
                ORDER BY j.PromiseDate, j.JobID";
            return ExecuteQuery(query);
        }

        // ============================================================
        // JOB DETAILS WITH SERVICE TYPE
        // ============================================================
        public DataTable GetJobDetailsWithType()
        {
            const string query = @"
                SELECT
                    j.JobID,
                    c.FullName AS CustomerName,
                    s.FullName AS TechnicianName,
                    STUFF(
                        (
                            SELECT
                                ', ' + jt2.JobTypeName
                            FROM JobDetails AS jd2
                            INNER JOIN JobTypes AS jt2
                                ON jd2.JobTypeID = jt2.JobTypeID
                            WHERE jd2.JobID = j.JobID
                            ORDER BY jd2.JobDetailID
                            FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)'),
                        1,
                        2,
                        ''
                    ) AS JobTypeName,
                    j.PromiseDate,
                    j.JobStatus,
                    ISNULL(j.LabourCost, 0) AS LabourCost,
                    ISNULL(j.TotalMaterialCost, 0) AS MaterialCost,
                    ISNULL(j.TransportCost, 0) AS TransportCost,
                    ISNULL(j.TaxAmount, 0) AS TaxAmount,
                    ISNULL(j.GrandTotal, 0) AS GrandTotal,
                    j.CreatedAt
                FROM Jobs AS j
                INNER JOIN Customers AS c
                    ON j.CustomerID = c.CustomerID
                LEFT JOIN Staff AS s
                    ON j.StaffID = s.StaffID
                WHERE j.JobStatus <> 'Archived'
                ORDER BY j.PromiseDate, j.JobID;";
            return ExecuteQuery(query);
        }

        // ============================================================
        // TECHNICIAN JOB LIST
        // ============================================================
        public DataTable GetTechnicianJobs(int staffID)
        {
            const string query = @"
                SELECT
                    j.JobID,
                    j.CustomerID,
                    c.FullName AS CustomerName,
                    c.Area,
                    j.PromiseDate,
                    j.JobStatus, " + TechnicianChangeState + @"
                FROM Jobs AS j
                INNER JOIN Customers AS c
                    ON j.CustomerID = c.CustomerID
                WHERE j.StaffID = @StaffID
                  AND j.JobStatus <> 'Archived'
                ORDER BY j.PromiseDate, j.JobID;";
            return ExecuteQuery(query, new SqlParameter("@StaffID", staffID));
        }

        // ============================================================
        // TECHNICIAN JOB DETAILS
        // ============================================================
        public DataTable GetTechnicianJobByID(int jobID, int staffID)
        {
            const string query = @"
                SELECT
                    j.JobID,
                    j.CustomerID,
                    c.FullName AS CustomerFullName,
                    c.FullAddress,
                    c.Area,
                    c.PhoneNumber,
                    j.StaffID,
                    j.JobStatus,
                    j.PromiseDate,
                    j.Notes,
                    j.CreatedAt, " + TechnicianChangeState + @"
                FROM Jobs AS j
                INNER JOIN Customers AS c
                    ON j.CustomerID = c.CustomerID
                WHERE j.JobID = @JobID
                  AND j.StaffID = @StaffID
                  AND j.JobStatus <> 'Archived';";
            return ExecuteQuery(query, new SqlParameter("@JobID", jobID), new SqlParameter("@StaffID", staffID));
        }

        // ============================================================
        // TECHNICIAN START JOB
        // ============================================================
        public int StartTechnicianJob(int jobID, int staffID)
        {
            const string query = @"
                UPDATE Jobs
                SET
                    JobStatus = 'In Progress',
                    UpdatedAt = GETDATE()
                WHERE JobID = @JobID
                  AND StaffID = @StaffID
                                    AND EXISTS(SELECT 1 FROM dbo.Staff s JOIN dbo.Roles r ON r.RoleID=s.RoleID LEFT JOIN dbo.AppAccountSecurity x ON x.StaffID=s.StaffID WHERE s.StaffID=@StaffID AND s.EmploymentStatus='Active' AND r.RoleName='Technician' AND COALESCE(x.IsBlocked,0)=0)
                  AND NOT EXISTS(SELECT 1 FROM dbo.JobChangeRequests cr WHERE cr.JobID=@JobID AND cr.RequestStatus='Pending')                  AND JobStatus = 'Pending'
                  AND PromiseDate IS NOT NULL
                  AND CAST(PromiseDate AS DATE) <= CAST(GETDATE() AS DATE);";
            return ExecuteNonQuery(query, new SqlParameter("@JobID", jobID), new SqlParameter("@StaffID", staffID));
        }

        // ============================================================
        // TECHNICIAN COMPLETE JOB
        // ============================================================
        public int CompleteTechnicianJob(int jobID, int staffID)
        {
            const string query = @"
                UPDATE Jobs
                SET
                    JobStatus = 'Completed',
                    UpdatedAt = GETDATE()
                WHERE JobID = @JobID
                  AND StaffID = @StaffID
                                    AND EXISTS(SELECT 1 FROM dbo.Staff s JOIN dbo.Roles r ON r.RoleID=s.RoleID LEFT JOIN dbo.AppAccountSecurity x ON x.StaffID=s.StaffID WHERE s.StaffID=@StaffID AND s.EmploymentStatus='Active' AND r.RoleName='Technician' AND COALESCE(x.IsBlocked,0)=0)
                  AND NOT EXISTS(SELECT 1 FROM dbo.JobChangeRequests cr WHERE cr.JobID=@JobID AND cr.RequestStatus='Pending')                  AND JobStatus = 'In Progress';";
            return ExecuteNonQuery(query, new SqlParameter("@JobID", jobID), new SqlParameter("@StaffID", staffID));
        }

        // ============================================================
        // JOB DETAILS BY JOB ID
        // ============================================================
        public DataTable GetJobDetailsByJobID(int jobID)
        {
            const string query = @"
                SELECT
                    JD.JobDetailID,
                    JD.JobID,
                    JD.JobTypeID,
                    JT.JobTypeName,
                    JD.Quantity
                FROM JobDetails AS JD
                INNER JOIN JobTypes AS JT
                    ON JD.JobTypeID = JT.JobTypeID
                WHERE JD.JobID = @JobID
                ORDER BY JD.JobDetailID";
            SqlParameter parameter = new SqlParameter("@JobID", SqlDbType.Int);
            parameter.Value = jobID;
            return ExecuteQuery(query, parameter);
        }

        // ============================================================
        // SEARCH JOBS
        // ============================================================
        public DataTable SearchJobs(string searchText, string status)
        {
            const string query = @"
                SELECT
                    j.JobID,
                    c.FullName AS CustomerName,
                    s.FullName AS TechnicianName,
                    STUFF(
                        (
                            SELECT
                                ', ' + jt2.JobTypeName
                            FROM JobDetails AS jd2
                            INNER JOIN JobTypes AS jt2
                                ON jd2.JobTypeID = jt2.JobTypeID
                            WHERE jd2.JobID = j.JobID
                            ORDER BY jd2.JobDetailID
                            FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)'),
                        1,
                        2,
                        ''
                    ) AS JobTypeName,
                    j.PromiseDate,
                    j.JobStatus,
                    ISNULL(j.LabourCost, 0) AS LabourCost,
                    ISNULL(j.TotalMaterialCost, 0) AS MaterialCost,
                    ISNULL(j.TransportCost, 0) AS TransportCost,
                    ISNULL(j.TaxAmount, 0) AS TaxAmount,
                    ISNULL(j.GrandTotal, 0) AS GrandTotal,
                    j.CreatedAt
                FROM Jobs AS j
                INNER JOIN Customers AS c
                    ON j.CustomerID = c.CustomerID
                LEFT JOIN Staff AS s
                    ON j.StaffID = s.StaffID
                WHERE j.JobStatus <> 'Archived'
                  AND
                  (
                      @SearchText = ''
                      OR CONVERT(VARCHAR(20), j.JobID) LIKE '%' + @SearchText + '%'
                      OR c.FullName LIKE '%' + @SearchText + '%'
                      OR ISNULL(s.FullName, '') LIKE '%' + @SearchText + '%'
                  )
                  AND
                  (
                      @Status = 'All'
                      OR j.JobStatus = @Status
                  )
                ORDER BY j.PromiseDate, j.JobID;";
            return ExecuteQuery(query, new SqlParameter("@SearchText", (object)(searchText ?? string.Empty).Trim()), new SqlParameter("@Status", string.IsNullOrWhiteSpace(status) ? "All" : status.Trim()));
        }

        // ============================================================
        // CUSTOMER JOB LIST
        // ============================================================
        //
        // CUSTOMER-SAFE DATA ONLY.
        //
        // Excluded:
        // - LabourCost
        // - TotalMaterialCost
        // - TransportCost
        // - TaxAmount
        // - TechnicianName
        // - StaffID
        // - Internal Notes
        // - Employee information
        //
        // GrandTotal is retained because it represents the customer's
        // billed amount.
        // ============================================================
        public DataTable GetJobsByCustomerID(int customerID)
        {
            const string query = @"
                SELECT
                    j.JobID,
                    j.CustomerID,
                    STUFF(
                        (
                            SELECT
                                ', ' + jt2.JobTypeName
                                + ' x '
                                + CAST(jd2.Quantity AS VARCHAR(10))
                            FROM JobDetails AS jd2
                            INNER JOIN JobTypes AS jt2
                                ON jd2.JobTypeID = jt2.JobTypeID
                            WHERE jd2.JobID = j.JobID
                            ORDER BY jd2.JobDetailID
                            FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)'),
                        1,
                        2,
                        ''
                    ) AS JobTypeName,
                    c.Area,
                    c.FullAddress,
                    j.PromiseDate,
                    j.JobStatus,
                    ISNULL(j.GrandTotal, 0) AS GrandTotal,
                    j.CreatedAt,
                    j.UpdatedAt, " + PaymentStatusSelect + @"
                FROM Jobs AS j
                INNER JOIN Customers AS c
                    ON j.CustomerID = c.CustomerID
                " + PaymentStatusApply + @"
                WHERE j.CustomerID = @CustomerID
                  AND j.JobStatus <> 'Archived'
                ORDER BY j.CreatedAt DESC, j.JobID DESC;";
            SqlParameter parameter = new SqlParameter("@CustomerID", SqlDbType.Int);
            parameter.Value = customerID;
            return ExecuteQuery(query, parameter);
        }

        // ============================================================
        // JOB MANAGEMENT DETAILS
        // ============================================================
        public DataTable GetJobManagementDetails(int jobID)
        {
            const string query = @"
        SELECT
            j.JobID,
            j.CustomerID,
            j.StaffID,
            c.FullName AS CustomerName,
            s.FullName AS TechnicianName,

            STUFF(
                (
                    SELECT
                        ', ' + jt2.JobTypeName
                        + ' x '
                        + CAST(jd2.Quantity AS VARCHAR(10))
                    FROM JobDetails AS jd2
                    INNER JOIN JobTypes AS jt2
                        ON jd2.JobTypeID = jt2.JobTypeID
                    WHERE jd2.JobID = j.JobID
                    ORDER BY jd2.JobDetailID
                    FOR XML PATH(''), TYPE
                ).value('.', 'NVARCHAR(MAX)'),
                1,
                2,
                ''
            ) AS Services,

            j.PromiseDate,
            j.JobStatus,
            j.LabourCost,
            j.TotalMaterialCost,
            j.TransportCost,
            j.TaxAmount,
            j.GrandTotal,
            j.InvoiceFilePath,
            j.Notes,
            j.CreatedAt

        FROM Jobs AS j

        INNER JOIN Customers AS c
            ON j.CustomerID = c.CustomerID

        LEFT JOIN Staff AS s
            ON j.StaffID = s.StaffID

        WHERE j.JobID = @JobID;";
            return ExecuteQuery(query, new SqlParameter("@JobID", jobID));
        }

        // ============================================================
        // CUSTOMER JOB DETAILS
        // ============================================================
        //
        // CUSTOMER-SAFE DATA ONLY.
        //
        // The CustomerID and JobID are both required so that a customer
        // can only retrieve a job that belongs to their own account.
        //
        // Excluded:
        // - Technician information
        // - StaffID
        // - Internal notes
        // - Internal cost breakdown
        //
        // GrandTotal is retained because it represents the customer's
        // billed amount.
        // ============================================================
        public DataTable GetJobByCustomerID(int customerID, int jobID)
        {
            const string query = @"
        SELECT
            j.JobID,
            j.CustomerID,
            STUFF(
                (
                    SELECT
                        ', ' + jt2.JobTypeName
                        + ' x '
                        + CAST(jd2.Quantity AS VARCHAR(10))
                    FROM JobDetails AS jd2
                    INNER JOIN JobTypes AS jt2
                        ON jd2.JobTypeID = jt2.JobTypeID
                    WHERE jd2.JobID = j.JobID
                    ORDER BY jd2.JobDetailID
                    FOR XML PATH(''), TYPE
                ).value('.', 'NVARCHAR(MAX)'),
                1,
                2,
                ''
            ) AS JobTypeName,
            c.Area,
            c.FullAddress,
            j.PromiseDate,
            j.JobStatus,
            ISNULL(j.GrandTotal, 0) AS GrandTotal,
            j.CreatedAt,
            j.UpdatedAt
        FROM Jobs AS j
        INNER JOIN Customers AS c
            ON j.CustomerID = c.CustomerID
        WHERE j.CustomerID = @CustomerID
          AND j.JobID = @JobID
          AND j.JobStatus <> 'Archived';";
            SqlParameter customerParameter = new SqlParameter("@CustomerID", SqlDbType.Int);
            customerParameter.Value = customerID;
            SqlParameter jobParameter = new SqlParameter("@JobID", SqlDbType.Int);
            jobParameter.Value = jobID;
            return ExecuteQuery(query, customerParameter, jobParameter);
        }
    }
}
