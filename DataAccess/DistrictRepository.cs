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

        public District GetById(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, Name FROM districts WHERE Id = @id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new District
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void AddDistrict(District model)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO districts (Id,Name) VALUES (@Id,@Name)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", model.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<District> GetDistricts()
        {
            var districts = new List<District>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id,Name FROM districts";
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

        public District GetDistrictsbyId(int id)
        {
            var district = new List<District>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id,Name  FROM Districts where @id=id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new District
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void DeleteDistricts(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM Districts WHERE Id = @Id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
