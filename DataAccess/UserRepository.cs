using AhadiyyaMVC.Models;
using Microsoft.Data.SqlClient;

namespace AhadiyyaMVC.DataAccess
{
    public class UserRepository
    {
        private readonly DbConnectionHelper _db;
        public UserRepository(DbConnectionHelper db) => _db = db;

        public User? GetUser(string username, string password)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            string sql = @"SELECT U.Id, U.Username, U.PasswordHash, U.RoleId, R.Name AS RoleName,
                                  U.DistrictId, U.BranchId
                           FROM Users U
                           JOIN Roles R ON U.RoleId = R.Id
                           WHERE U.Username = @username AND U.PasswordHash = @password AND U.IsActive = 1";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                RoleId = reader.GetInt32(3),
                RoleName = reader.GetString(4),
                DistrictId = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                BranchId = reader.IsDBNull(6) ? null : reader.GetInt32(6)
            };
        }
    }
}
