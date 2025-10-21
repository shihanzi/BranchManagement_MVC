using AhadiyyaMVC.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AhadiyyaMVC.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly DbConnectionHelper _db;
        private readonly UserRepository _userRepo;

        public AccountController(DbConnectionHelper db, UserRepository userRepo)
        {
            _db = db;
            _userRepo = userRepo;
        }

        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(string username, string password)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, PasswordHash, RoleId, DistrictId, BranchId FROM Users WHERE Username = @username AND IsActive = 1";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var reader = cmd.ExecuteReader())
                    {

                        if (reader.Read())
                        {
                            string passwordHash = reader.GetString(1);

                            // ✅ Compare hashed password
                            bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, passwordHash);

                            if (!isValidPassword)
                            {
                                ViewBag.Error = "Invalid username or password";
                                return View();
                            }
                            int role = reader.GetInt32(2);

                            HttpContext.Session.SetInt32("UserId", reader.GetInt32(0));
                            HttpContext.Session.SetInt32("RoleId", role);
                            HttpContext.Session.SetString("Username", username);

                            // ✅ SUCCESS → store session
                            if (role == 2 && !reader.IsDBNull(3))
                                HttpContext.Session.SetInt32("DistrictId", reader.GetInt32(3));

                            if ((role == 3 || role == 4) && !reader.IsDBNull(4))
                                HttpContext.Session.SetInt32("BranchId", reader.GetInt32(4));
                            
                            if (role == 1) return RedirectToAction("Index", "Home");      // Super Admin
                            if (role == 2) return RedirectToAction("Index", "Branch");        // District Admin
                            if (role == 3) return RedirectToAction("Index", "Staff");         // Branch Admin or Staff

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            //string hash = BCrypt.Net.BCrypt.HashPassword("123");
                            //return Content(hash);
                            ViewBag.Error = "Invalid username or password";
                            return View();
                        }
                    }
                }
            }
        }
        // Logout
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
