using AhadiyyaMVC.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace AhadiyyaMVC.DataAccess
{
    public class StaffRepository
    {
        private readonly DbConnectionHelper _db;
        public StaffRepository(DbConnectionHelper db) => _db = db;

        // Get list with role-based filter arguments (null = no-filter)
        public List<Staff> GetAll(int? roleId = null, int? sessionDistrictId = null, int? sessionBranchId = null)
        {
            var list = new List<Staff>();
            using var conn = _db.GetConnection();
            conn.Open();

            // base query
            string sql = @"SELECT Id, FirstName, LastName, Email, Phone, Address, EducationalQualifications, HandlingClasses, DistrictId, BranchId
                           FROM Staffs
                           WHERE 1=1";

            // apply filtering based on role
            if (roleId == 2 && sessionDistrictId.HasValue) // District Admin
            {
                sql += " AND DistrictId = @DistrictId";
            }
            else if ((roleId == 3 || roleId == 4) && sessionBranchId.HasValue) // Branch Admin / Staff (if used)
            {
                sql += " AND BranchId = @BranchId";
            }

            using var cmd = new SqlCommand(sql, conn);

            if (roleId == 2 && sessionDistrictId.HasValue)
                cmd.Parameters.AddWithValue("@DistrictId", sessionDistrictId.Value);

            if ((roleId == 3 || roleId == 4) && sessionBranchId.HasValue)
                cmd.Parameters.AddWithValue("@BranchId", sessionBranchId.Value);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Staff  
                {
                    Id = reader.GetInt32(0),
                    FirstName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    LastName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Phone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Address = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    EducationalQualifications = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    HandlingClasses = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    DistrictId = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                    BranchId = reader.IsDBNull(9) ? 0 : reader.GetInt32(9)
                });
            }
            return list;
        }

        public Staff GetById(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            string sql = @"SELECT Id, FirstName, LastName, Email, Phone, Address, EducationalQualifications, HandlingClasses, DistrictId, BranchId
                           FROM Staffs WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return new Staff
            {
                Id = reader.GetInt32(0),
                FirstName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                LastName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Phone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                Address = reader.IsDBNull(5) ? "" : reader.GetString(5),
                EducationalQualifications = reader.IsDBNull(6) ? "" : reader.GetString(6),
                HandlingClasses = reader.IsDBNull(7) ? "" : reader.GetString(7),
                DistrictId = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                BranchId = reader.IsDBNull(9) ? 0 : reader.GetInt32(9)
            };
        }

        public void Add(Staff model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            string sql = @"
                INSERT INTO Staffs (FirstName, LastName, Email, Phone, Address, EducationalQualifications, HandlingClasses, DistrictId, BranchId)
                VALUES (@FirstName, @LastName, @Email, @Phone, @Address, @Edu, @Handling, @DistrictId, @BranchId)";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FirstName", model.FirstName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastName", model.LastName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", model.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", model.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", model.Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Edu", model.EducationalQualifications ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Handling", model.HandlingClasses ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictId", model.DistrictId == 0 ? (object)DBNull.Value : model.DistrictId);
            cmd.Parameters.AddWithValue("@BranchId", model.BranchId == 0 ? (object)DBNull.Value : model.BranchId);
            cmd.ExecuteNonQuery();
        }

        public void Update(Staff model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            string sql = @"
                UPDATE Staffs SET
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    Phone = @Phone,
                    Address = @Address,
                    EducationalQualifications = @Edu,
                    HandlingClasses = @Handling,
                    DistrictId = @DistrictId,
                    BranchId = @BranchId
                WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FirstName", model.FirstName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastName", model.LastName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", model.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", model.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", model.Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Edu", model.EducationalQualifications ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Handling", model.HandlingClasses ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DistrictId", model.DistrictId == 0 ? (object)DBNull.Value : model.DistrictId);
            cmd.Parameters.AddWithValue("@BranchId", model.BranchId == 0 ? (object)DBNull.Value : model.BranchId);
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            string sql = "DELETE FROM Staffs WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
