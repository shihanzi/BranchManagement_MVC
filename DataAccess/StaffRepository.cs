using AhadiyyaMVC.Models;
using Microsoft.Data.SqlClient;

namespace AhadiyyaMVC.DataAccess
{
    public class StaffRepository
    {
        private readonly DbConnectionHelper _db;
        public StaffRepository(DbConnectionHelper db)
        {
            _db = db;
        }
        public List<Staff> GetAllStaffByBranch(int branchId)
        {
            var staff = new List<Staff>();
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
                staff.Add(new Staff
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

        public List<Staff> GetAllUsers()
        {
            var users = new List<Staff>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, FirstName, LastName,Phone, Address, DistrictId, BranchId,HandlingClasses,Email FROM Staffs";

                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new Staff
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Address = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                DistrictId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                BranchId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                HandlingClasses = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                Email = reader.GetString(8),
                            });
                        }
                    }
                }
            }

            return users;
        }
        public List<District> GetAllDistricts()
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
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return districts;
        }

        public List<Branch> GetAllBranches()
        {
            var branches = new List<Branch>();

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
                            branches.Add(new Branch
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
        public void AddUser(Staff user)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Staffs 
                (FirstName, LastName, Phone, Address, EducationalQualifications, HandlingClasses, DistrictId, BranchId,Email)
                VALUES (@FirstName, @LastName, @Phone, @Address, @Edu, @Classes, @DistrictId, @BranchId,@Email)";

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
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
