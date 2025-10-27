using AhadiyyaMVC.Models;
using Microsoft.Data.SqlClient;

namespace AhadiyyaMVC.DataAccess
{
    public class UserRepository
    {
        private readonly DbConnectionHelper _db;
        private readonly BranchRepository _branch;
        private readonly DistrictRepository _district;

        public UserRepository(DbConnectionHelper db, DistrictRepository district, BranchRepository branch)
        {
            _db = db;
            _branch = branch;
            _district = district;
        }

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

        public List<User> GetUsers()
        {
            var users = new List<User>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id,Username FROM Users";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                            });
                        }
                    }
                }
            }
            return users;
        }

        public List<District> GetDistricts()
        {
            var districts = new List<District>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id,Name FROM Districts";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            districts.Add(new District
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                            });
                        }
                    }
                }
            }
            return districts;
        }

        public List<Role> GetRoles()
        {
            var roles = new List<Role>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT id,name FROM Roles";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Role
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                            });
                        }
                    }
                }
            }
            return roles;
        }

        public List<Branch> GetBranches()
        {
            var branches = new List<Branch>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id,Name FROM branches";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            branches.Add(new Branch
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                            });
                        }
                    }
                }
            }
            return branches;
        }

        public void AddBranch(User model)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO users (Username,PasswordHash,RoleId,DistrictId,BranchId,CreatedDate,IsActive) VALUES (@Username,@PasswordHash,@RoleId,@DistrictId,@BranchId,@CreatedDate,@IsActive)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
                    cmd.Parameters.AddWithValue("@Username", model.Username);
                    cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    cmd.Parameters.AddWithValue("@RoleId", model.RoleId);
                    cmd.Parameters.AddWithValue("@DistrictId", model.DistrictId);
                    cmd.Parameters.AddWithValue("@BranchId", model.BranchId);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@IsActive", 1);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
