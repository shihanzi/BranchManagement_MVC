using AhadiyyaMVC.DataAccess;
using Microsoft.AspNetCore.Mvc;
using AhadiyyaMVC.Models;

namespace AhadiyyaMVC.Controllers
{
    public class StaffController : Controller
    {
        private readonly UserRepository _userRepository;

        public StaffController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Create()
        {
            return View(new UserModel());
        }

        [HttpPost]
        public IActionResult Create(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Districts = _userRepository.GetAllDistricts();
                ViewBag.Branches = _userRepository.GetAllBranches();
                return View(model);
            }

            _userRepository.AddUser(model);
            return RedirectToAction("Index"); // Back to dashboard
        }

        public IActionResult Index()
        {
            string role = HttpContext.Session.GetString("Role");
            int? branchId = HttpContext.Session.GetInt32("BranchId");

            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "Account");

            if (role == "Admin")
                return View(_userRepository.GetAllStaffByBranch(0));
            if (role == "BranchManager" && branchId.HasValue)
                return View(_userRepository.GetAllStaffByBranch(branchId.Value));

            return RedirectToAction("AccessDenied", "Account");
        }
    }
}
