using AhadiyyaMVC.Models;
using Microsoft.Data.SqlClient;

namespace AhadiyyaMVC.DataAccess
{
    public class RoleRepository
    {
        private readonly DbConnectionHelper _db;
        public RoleRepository(DbConnectionHelper db) => _db = db;

        public List<Role> GetAllRoles()
        {
            var roles = new List<Role>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT Id, Name FROM Roles", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    roles.Add(new Role { Id = reader.GetInt32(0), Name = reader.GetString(1) });
            }
            return roles;
        }
    }
}
