using System;
using System.Data;
using System.Data.SqlClient;

namespace MajorProject.Data.Data
{
    /// <summary>
    /// Provides database access for invoice information stored against Jobs.
    ///
    /// There is no separate Invoices table in the confirmed database schema.
    /// The Jobs table contains the financial information required for an
    /// invoice:
    ///
    /// LabourCost
    /// TransportCost
    /// TotalMaterialCost
    /// TaxAmount
    /// GrandTotal
    /// JobStatus
    /// PromiseDate
    /// CreatedAt
    ///
    /// The Jobs table also contains the generated invoice PDF and related
    /// email information:
    ///
    /// InvoiceFilePath
    /// InvoicePDFData
    /// InvoiceSentDate
    /// InvoiceSentToEmail
    ///
    /// Services are not stored directly on Jobs.
    /// The relationship is:
    ///
    /// Jobs
    ///     -> JobDetails
    ///         -> JobTypes
    ///
    /// A job can therefore contain multiple service types.
    /// </summary>
    public class InvoiceDataAccess : DataAccessBase
    {
        public InvoiceDataAccess(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Gets invoice-shaped financial data for every job.
        ///
        /// This method is intended for staff/admin use where customer
        /// ownership filtering is handled elsewhere.
        /// </summary>
        public DataTable GetAllJobInvoiceData()
        {
            const string query = @"
                SELECT
                    J.JobID,
                    J.CustomerID,
                    J.StaffID,
                    J.PromiseDate,
                    J.JobStatus,
                    J.LabourCost,
                    J.TransportCost,
                    J.TotalMaterialCost,
                    J.TaxAmount,
                    J.GrandTotal,
                    J.CreatedAt,
                    J.InvoiceFilePath,
                    J.InvoiceSentDate,
                    J.InvoiceSentToEmail,
                    STUFF(
                        (
                            SELECT
                                ', ' + JT2.JobTypeName
                                + ' x '
                                + CAST(JD2.Quantity AS VARCHAR(10))
                            FROM JobDetails JD2
                            INNER JOIN JobTypes JT2
                                ON JD2.JobTypeID = JT2.JobTypeID
                            WHERE JD2.JobID = J.JobID
                            ORDER BY JD2.JobDetailID
                            FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)'),
                        1,
                        2,
                        ''
                    ) AS JobTypeName
                FROM Jobs J
                ORDER BY J.JobID DESC";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// Gets invoice-shaped financial data for one job.
        ///
        /// This method is intended for staff/admin use.
        /// </summary>
        public DataTable GetJobInvoiceDataByJobID(int jobID)
        {
            const string query = @"
                SELECT
                    J.JobID,
                    J.CustomerID,
                    J.StaffID,
                    J.PromiseDate,
                    J.JobStatus,
                    J.LabourCost,
                    J.TransportCost,
                    J.TotalMaterialCost,
                    J.TaxAmount,
                    J.GrandTotal,
                    J.CreatedAt,
                    J.InvoiceFilePath,
                    J.InvoiceSentDate,
                    J.InvoiceSentToEmail,
                    STUFF(
                        (
                            SELECT
                                ', ' + JT2.JobTypeName
                                + ' x '
                                + CAST(JD2.Quantity AS VARCHAR(10))
                            FROM JobDetails JD2
                            INNER JOIN JobTypes JT2
                                ON JD2.JobTypeID = JT2.JobTypeID
                            WHERE JD2.JobID = J.JobID
                            ORDER BY JD2.JobDetailID
                            FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)'),
                        1,
                        2,
                        ''
                    ) AS JobTypeName
                FROM Jobs J
                WHERE J.JobID = @JobID";
            SqlParameter parameter = new SqlParameter("@JobID", SqlDbType.Int);
            parameter.Value = jobID;
            return ExecuteQuery(query, parameter);
        }

        /// <summary>
        /// Gets generated invoice data for all jobs belonging to a specific
        /// customer.
        ///
        /// Only jobs with a generated invoice file path are returned.
        ///
        /// Services are retrieved through:
        ///
        /// Jobs -> JobDetails -> JobTypes
        ///
        /// This prevents the incorrect assumption that Jobs contains
        /// JobTypeID.
        /// </summary>
        public DataTable GetJobInvoiceDataByCustomerID(int customerID)
        {
            const string query = @"
        SELECT
            J.JobID,
            J.CustomerID,
            J.PromiseDate,
            J.JobStatus,
            J.LabourCost,
            J.TransportCost,
            J.TotalMaterialCost,
            J.TaxAmount,
            J.GrandTotal,
            J.CreatedAt,
            J.InvoiceFilePath,
            J.InvoiceSentDate,
            J.InvoiceSentToEmail,

            -- Actual payment status
            CASE
                WHEN P.PaymentID IS NULL THEN 'Unpaid'
                WHEN P.PaymentStatus IS NULL
                     OR LTRIM(RTRIM(P.PaymentStatus)) = ''
                    THEN 'Unpaid'
                ELSE P.PaymentStatus
            END AS PaymentStatus,

            -- Payment completion date
            COALESCE(
                P.PaidAt,
                PA.CompletedAt
            ) AS PaymentDate,

            -- Successful payment reference
            PA.ProviderReference AS PaymentReference,

            -- Payment method
            COALESCE(
                PA.PaymentMethod,
                P.PaymentMethod
            ) AS PaymentMethod,

            -- Service descriptions
            STUFF(
                (
                    SELECT
                        ', ' + JT2.JobTypeName
                        + ' x '
                        + CAST(JD2.Quantity AS VARCHAR(10))
                    FROM JobDetails JD2
                    INNER JOIN JobTypes JT2
                        ON JD2.JobTypeID = JT2.JobTypeID
                    WHERE JD2.JobID = J.JobID
                    ORDER BY JD2.JobDetailID
                    FOR XML PATH(''), TYPE
                ).value('.', 'NVARCHAR(MAX)'),
                1,
                2,
                ''
            ) AS JobTypeName

        FROM Jobs J

        OUTER APPLY
        (
            SELECT TOP (1)
                P1.PaymentID,
                P1.PaymentStatus,
                P1.PaymentMethod,
                P1.PaidAt,
                P1.UpdatedAt,
                P1.CreatedAt
            FROM dbo.Payments P1
            WHERE P1.JobID = J.JobID
              AND P1.CustomerID = J.CustomerID
            ORDER BY
                CASE
                    WHEN UPPER(LTRIM(RTRIM(P1.PaymentStatus))) = 'PAID'
                        THEN 0
                    ELSE 1
                END,
                COALESCE(
                    P1.PaidAt,
                    P1.UpdatedAt,
                    P1.CreatedAt
                ) DESC,
                P1.PaymentID DESC
        ) P

        OUTER APPLY
        (
            SELECT TOP (1)
                PA1.ProviderReference,
                PA1.PaymentMethod,
                PA1.CompletedAt
            FROM dbo.PaymentAttempts PA1
            WHERE PA1.PaymentID = P.PaymentID
              AND UPPER(LTRIM(RTRIM(PA1.AttemptStatus))) = 'SUCCESSFUL'
            ORDER BY
                COALESCE(
                    PA1.CompletedAt,
                    PA1.CreatedAt
                ) DESC,
                PA1.PaymentAttemptID DESC
        ) PA

        WHERE J.CustomerID = @CustomerID
          AND J.InvoiceFilePath IS NOT NULL
          AND LTRIM(RTRIM(J.InvoiceFilePath)) <> ''
          AND (P.PaymentStatus = 'Paid' OR (J.JobStatus <> 'Cancelled' AND NOT EXISTS (
              SELECT 1 FROM dbo.JobChangeRequests r
              WHERE r.JobID = J.JobID AND r.RequestType = 'Cancellation' AND r.RequestStatus = 'Approved'
          )))

        ORDER BY J.JobID DESC";
            SqlParameter parameter = new SqlParameter("@CustomerID", SqlDbType.Int);
            parameter.Value = customerID;
            return ExecuteQuery(query, parameter);
        }

        /// <summary>
        /// Gets one generated invoice for a specific customer and job.
        ///
        /// CustomerID and JobID are both checked in the database query.
        /// This prevents a customer from retrieving another customer's
        /// invoice by changing the JobID.
        /// </summary>
        public DataTable GetCustomerInvoiceData(int customerID, int jobID)
        {
            const string query = @"
        SELECT
            J.JobID,
            J.CustomerID,
            C.FullName AS CustomerName,
            C.EmailAddress AS CustomerEmail,
            J.PromiseDate,
            J.JobStatus,
            J.LabourCost,
            J.TransportCost,
            J.TotalMaterialCost,
            J.TaxAmount,
            J.GrandTotal,
            J.CreatedAt,
            J.InvoiceFilePath,
            J.InvoiceSentDate,
            J.InvoiceSentToEmail,

            STUFF(
                (
                    SELECT
                        ', ' + JT2.JobTypeName
                        + ' x '
                        + CAST(JD2.Quantity AS VARCHAR(10))
                    FROM JobDetails JD2
                    INNER JOIN JobTypes JT2
                        ON JD2.JobTypeID = JT2.JobTypeID
                    WHERE JD2.JobID = J.JobID
                    ORDER BY JD2.JobDetailID
                    FOR XML PATH(''), TYPE
                ).value('.', 'NVARCHAR(MAX)'),
                1,
                2,
                ''
            ) AS JobTypeName

        FROM Jobs J

        INNER JOIN dbo.Customers C
            ON C.CustomerID = J.CustomerID

        WHERE J.CustomerID = @CustomerID
          AND J.JobID = @JobID
          AND J.InvoiceFilePath IS NOT NULL
          AND LTRIM(RTRIM(J.InvoiceFilePath)) <> ''";
            SqlParameter[] parameters =
            {
                new SqlParameter("@CustomerID", SqlDbType.Int)
                {
                    Value = customerID
                },
                new SqlParameter("@JobID", SqlDbType.Int)
                {
                    Value = jobID
                }
            };
            return ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Saves the generated invoice PDF and its file path.
        ///
        /// This method deliberately does NOT update InvoiceSentDate because
        /// storing a PDF does not mean that an email was successfully sent.
        /// </summary>
        public void SaveInvoiceData(int jobID, byte[] pdfData, string filePath)
        {
            const string query = @"
                UPDATE Jobs
                SET
                    InvoiceFilePath = @FilePath,
                    InvoicePDFData = @PDFData
                WHERE JobID = @JobID";
            SqlParameter[] parameters =
            {
                new SqlParameter("@JobID", SqlDbType.Int)
                {
                    Value = jobID
                },
                new SqlParameter("@FilePath", SqlDbType.NVarChar, 1000)
                {
                    Value = (object)filePath ?? DBNull.Value
                },
                new SqlParameter("@PDFData", SqlDbType.VarBinary, -1)
                {
                    Value = (object)pdfData ?? DBNull.Value
                }
            };
            ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Records that an invoice email was successfully sent.
        ///
        /// This should only be called after Mailjet confirms successful
        /// submission of the email.
        /// </summary>
        public void MarkInvoiceAsSent(int jobID, string customerEmail)
        {
            const string query = @"
                UPDATE Jobs
                SET
                    InvoiceSentDate = GETDATE(),
                    InvoiceSentToEmail = @Email
                WHERE JobID = @JobID";
            SqlParameter[] parameters =
            {
                new SqlParameter("@JobID", SqlDbType.Int)
                {
                    Value = jobID
                },
                new SqlParameter("@Email", SqlDbType.NVarChar, 400)
                {
                    Value = (object)customerEmail ?? DBNull.Value
                }
            };
            ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Retrieves the generated invoice PDF for a specific job.
        ///
        /// This method is intended for trusted staff/admin operations.
        /// Customer-facing pages should use GetCustomerInvoicePDFData()
        /// so that CustomerID is checked as well.
        /// </summary>
        public byte[] GetInvoicePDFData(int jobID)
        {
            const string query = @"
                SELECT InvoicePDFData
                FROM Jobs
                WHERE JobID = @JobID";
            SqlParameter parameter = new SqlParameter("@JobID", SqlDbType.Int);
            parameter.Value = jobID;
            object result = ExecuteScalar(query, parameter);
            if (result == null || result == DBNull.Value)
            {
                return null;
            }

            return result as byte[];
        }

        /// <summary>
        /// Retrieves a customer's invoice PDF only when the specified job
        /// belongs to that customer.
        ///
        /// This is the method that should be used by the customer invoice
        /// preview and download functionality.
        /// </summary>
        public byte[] GetCustomerInvoicePDFData(int customerID, int jobID)
        {
            const string query = @"
                SELECT InvoicePDFData
                FROM Jobs
                WHERE CustomerID = @CustomerID
                  AND JobID = @JobID
                  AND InvoiceFilePath IS NOT NULL
                  AND LTRIM(RTRIM(InvoiceFilePath)) <> ''";
            SqlParameter[] parameters =
            {
                new SqlParameter("@CustomerID", SqlDbType.Int)
                {
                    Value = customerID
                },
                new SqlParameter("@JobID", SqlDbType.Int)
                {
                    Value = jobID
                }
            };
            object result = ExecuteScalar(query, parameters);
            if (result == null || result == DBNull.Value)
            {
                return null;
            }

            return result as byte[];
        }
    }
}
