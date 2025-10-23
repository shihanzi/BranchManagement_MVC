using AhadiyyaMVC.Models;
using Microsoft.Data.SqlClient;

namespace AhadiyyaMVC.DataAccess
{
    public class BranchRepository
    {
        private readonly DbConnectionHelper _db;
        public BranchRepository(DbConnectionHelper db)
        {
            _db = db;
        }
        public List<Branch> GetBranches()
        {
            var branches = new List<Branch>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT branches.Id,branches.Name,Districts.Name  FROM branches inner join Districts on DistrictId=Districts.Id";
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
                                DistrictName = reader.GetString(2),
                            });
                        }
                    }
                }
            }
            return branches;
        }

        public List<Branch> GetBranchesByDistrict(int districtId)
        {
            var branches = new List<Branch>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id,Name,DistrictId FROM branches WHERE DistrictId = @districtId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@districtId", districtId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            branches.Add(new Branch
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                DistrictId = reader.GetInt32(2),
                            });
                        }
                    }
                }
            }
            return branches;
        }
        public Branch GetBranchesbyId(int id)
        {
            var branches = new List<Branch>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id,Name,DistrictId  FROM branches where @id=id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Branch
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                DistrictId = reader.GetInt32(2),
                            };
                        }
                    }
                }
            }
            return null;
        }
        public void AddBranch(Branch model)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO branches (Name, DistrictId) VALUES (@Name, @DistrictId)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", model.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DistrictId", model.DistrictId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateBranch(Branch model)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE branches SET Name = @Name, DistrictId = @DistrictId WHERE Id = @Id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", model.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DistrictId", model.DistrictId);
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteBranch(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM branches WHERE Id = @Id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
