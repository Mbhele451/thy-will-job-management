using System.Data;
using System.Data.SqlClient;

namespace MajorProject.Data.Data
{
    public class RoleDataAccess : DataAccessBase
    {
        public RoleDataAccess(string connectionString) : base(connectionString)
        {
        }

        public DataTable GetAllRoles()
        {
            const string query = "SELECT RoleID, RoleName FROM Roles";
            return ExecuteQuery(query);
        }
    }
}
