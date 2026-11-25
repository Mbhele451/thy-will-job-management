using System.Data;
using MajorProject.Data.Data;

namespace MajorProject.Business.Services
{
    public class RoleService
    {
        private readonly RoleDataAccess _dataAccess;
        public RoleService(string connectionString)
        {
            _dataAccess = new RoleDataAccess(connectionString);
        }

        public DataTable GetAllRoles()
        {
            var table = _dataAccess.GetAllRoles();
            var view = new DataView(table);
            view.RowFilter = "RoleName IN ('Administrator','Manager','Technician')";
            return view.ToTable();
        }
    }
}
