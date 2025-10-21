using AhadiyyaMVC.DataAccess;
using Microsoft.AspNetCore.Mvc;
using AhadiyyaMVC.Models;

namespace AhadiyyaMVC.Controllers
{
    public class StaffController : Controller
    {
        private readonly StaffRepository _userRepository;
        private readonly DistrictRepository _districtRepository;
        private readonly BranchRepository _branchRepository;

        public StaffController(StaffRepository userRepository, DistrictRepository districtRepo, BranchRepository branchRepo)
        {
            _userRepository = userRepository;
            _districtRepository = districtRepo;
            _branchRepository = branchRepo;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Districts = _districtRepository.GetDistricts();
            ViewBag.Branches = _branchRepository.GetBranches();
            return View(new Staff());
        }

        [HttpPost]
        public IActionResult Create(Staff model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Districts = _userRepository.GetAllDistricts();
                ViewBag.Branches = _userRepository.GetAllBranches();
                return View(model);
            }

            _userRepository.AddUser(model);
            return RedirectToAction("Index", "Home");
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
