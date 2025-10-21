namespace AhadiyyaMVC.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }  // hash later
        public int RoleId { get; set; }
        public int? DistrictId { get; set; }
        public int? BranchId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string? RoleName { get; set; }
    }
}
