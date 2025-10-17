using AhadiyyaMVC.Models;
using Microsoft.Data.SqlClient;

namespace AhadiyyaMVC.DataAccess
{
    public class UserRepository
    {
        private readonly DbConnectionHelper _db;
        public UserRepository(DbConnectionHelper db)
        {
            _db = db;
        }
        public List<UserModel> GetAllStaffByBranch(int branchId)
        {
            var staff = new List<UserModel>();
            using var conn = _db.GetConnection();
            conn.Open();

            var query = branchId == 0
                ? "SELECT * FROM Users WHERE Role = 'Staff'"
                : "SELECT * FROM Users WHERE Role = 'Staff' AND BranchId = @BranchId";

            using var cmd = new SqlCommand(query, conn);
            if (branchId != 0)
                cmd.Parameters.AddWithValue("@BranchId", branchId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                staff.Add(new UserModel
                {
                    Id = (int)reader["Id"],
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    Address = reader["Address"].ToString(),
                    EducationalQualifications = reader["EducationalQualifications"].ToString(),
                    HandlingClasses = reader["HandlingClasses"].ToString(),
                    BranchId = (int)reader["BranchId"],
                    DistrictId = (int)reader["DistrictId"]
                });
            }
            return staff;
        }

        public List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, FirstName, LastName, Phone, Email, Role, DistrictId, BranchId,HandlingClasses  FROM Users";

                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new UserModel
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                DistrictId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                BranchId = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                HandlingClasses = reader.IsDBNull(8) ? "" : reader.GetString(8)
                            });
                        }
                    }
                }
            }

            return users;
        }
        public List<DistrictModel> GetAllDistricts()
        {
            var districts = new List<DistrictModel>();

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
                            districts.Add(new DistrictModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return districts;
        }

        public List<BranchModel> GetAllBranches()
        {
            var branches = new List<BranchModel>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id,Name,DistrictId FROM Branches";

                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            branches.Add(new BranchModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return branches;
        }
        public void AddUser(UserModel user)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Users 
                (FirstName, LastName, Phone, Address, EducationalQualifications, HandlingClasses, Role, DistrictId, BranchId, Username, Password)
                VALUES (@FirstName, @LastName, @Phone, @Address, @Edu, @Classes, @Role, @DistrictId, @BranchId, @Username, @Password)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@Edu", user.EducationalQualifications);
                    cmd.Parameters.AddWithValue("@Classes", user.HandlingClasses);
                    cmd.Parameters.AddWithValue("@DistrictId", user.DistrictId);
                    cmd.Parameters.AddWithValue("@BranchId", user.BranchId);
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
