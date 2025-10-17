using AhadiyyaMVC.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AhadiyyaMVC.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly DbConnectionHelper _db;

        public AccountController(DbConnectionHelper db)
        {
            _db = db;
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
                string query = "SELECT Id, Role, DistrictId, BranchId FROM Users WHERE Username=@username AND Password=@password";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = cmd.ExecuteReader())
                    { 
                        if (reader.Read())
                        {
                            // Store session info
                            HttpContext.Session.SetInt32("UserId", reader.GetInt32(0));
                            HttpContext.Session.SetString("Role", reader.GetString(1));
                            HttpContext.Session.SetString("Username", username);

                            if (!reader.IsDBNull(2))
                                HttpContext.Session.SetInt32("DistrictId", reader.GetInt32(2));

                            if (!reader.IsDBNull(3))
                                HttpContext.Session.SetInt32("BranchId", reader.GetInt32(3));

                            // Redirect based on role (optional)
                            string role = reader.GetString(1);
                            if (role == "Admin") return RedirectToAction("Index", "Home");
                            if (role == "DistrictManager") return RedirectToAction("Index", "Branch");
                            if (role == "BranchManager" || role == "Staff") return RedirectToAction("Index", "Staff");

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
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
