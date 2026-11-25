using System;
using System.Data;
using System.Data.SqlClient;

namespace MajorProject.Data.Data
{
    /// <summary>
    /// Base class for database access.
    ///
    /// The Data layer is responsible only for database operations.
    /// It must not contain UI logic.
    /// </summary>
    public abstract class DataAccessBase
    {
        protected string ConnectionString { get; }

        protected DataAccessBase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));
            }

            var settings = new SqlConnectionStringBuilder(connectionString);
            settings.ConnectTimeout = Math.Min(settings.ConnectTimeout == 0 ? 8 : settings.ConnectTimeout, 8);
            ConnectionString = settings.ConnectionString;
        }

        protected DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        adapter.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        throw new InvalidOperationException("The database request could not be completed. Check your connection and try again. No results have been loaded.", ex);
                    }

                    return dt;
                }
            }
        }

        protected int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        protected object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Creates an open SQL connection.
        /// Used by multi-step transactional operations.
        /// </summary>
        protected SqlConnection CreateOpenConnection()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
