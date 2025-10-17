namespace AhadiyyaMVC.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string Username { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string HandlingClasses { get; set; }
        public string EducationalQualifications { get; set; }
        public int DistrictId { get; set; }  
        public int BranchId { get; set; }
    }
}
