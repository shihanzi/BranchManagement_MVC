using AhadiyyaMVC.DataAccess;
using AhadiyyaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace AhadiyyaMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepository _userRepo;
        public UserController(UserRepository user)
        {
            _userRepo = user;
        }
        public IActionResult Index()
        {
            //int? districtId = HttpContext.Session.GetInt32("DistrictId");
            //int? branchId = HttpContext.Session.GetInt32("BranchId");

            var list = _userRepo.GetUsers();
            return View(list);
        }

        public IActionResult Create()
        {
            ViewBag.Roles = _userRepo.GetRoles();               // returns Id + Name
            ViewBag.Districts = _userRepo.GetDistricts();   // returns Id + Name
            ViewBag.Branches = _userRepo.GetBranches();       // returns Id + Name
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User model)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");

            // Branch Admins not allowed
            if (roleId == 3)
                return Forbid();

            // Enforce DistrictAdmin restriction: they can only create branches in their own district
            if (roleId == 2 && sessionDistrict.HasValue && model.DistrictId != sessionDistrict.Value)
            {
                ModelState.AddModelError("", "District Admin can only create branches in their own district.");
            }

            if (!ModelState.IsValid)
            {
                //ViewBag.Districts = (roleId == 2 && sessionDistrict.HasValue)
                //    ? _userRepo.GetDistricts().Where(d => d.Id == sessionDistrict.Value).ToList()
                //    : _userRepo.GetDistricts();

                return View(model);
            }

            _userRepo.AddBranch(model); // implement in repo
            return RedirectToAction("Index");
        }
    }
}
