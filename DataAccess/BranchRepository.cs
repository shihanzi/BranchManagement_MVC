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
                string query = "SELECT Id,Name,DistrictId  FROM branches";
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
                                DistrictId = reader.GetInt32(2),
                            });
                        }
                    }
                }
            }
            return branches;
        }
    }
}
