using AhadiyyaMVC.Models;
using Microsoft.Data.SqlClient;

namespace AhadiyyaMVC.DataAccess
{
    public class DistrictRepository
    {
        private readonly DbConnectionHelper _db;
        public DistrictRepository(DbConnectionHelper db)
        {
            _db = db;
        }
        public List<District> GetDistricts()
        {
            var districts = new List<District>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id,Name  FROM districts";
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
    }
}
