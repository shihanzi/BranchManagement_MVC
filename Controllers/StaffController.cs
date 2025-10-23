using AhadiyyaMVC.DataAccess;
using AhadiyyaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AhadiyyaMVC.Controllers
{
    public class StaffController : Controller
    {
        private readonly StaffRepository _staffRepo;
        private readonly DistrictRepository _districtRepo; // for dropdowns
        private readonly BranchRepository _branchRepo;

        public StaffController(StaffRepository staffRepo, DistrictRepository districtRepo, BranchRepository branchRepo)
        {
            _staffRepo = staffRepo;
            _districtRepo = districtRepo;
            _branchRepo = branchRepo;
        }

        public IActionResult Index()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? districtId = HttpContext.Session.GetInt32("DistrictId");
            int? branchId = HttpContext.Session.GetInt32("BranchId");

            var list = _staffRepo.GetAll(roleId, districtId, branchId);
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");
            int? sessionBranch = HttpContext.Session.GetInt32("BranchId");

            // DISTRICTS
            if (roleId == 2) // District Admin → only their district
            {
                ViewBag.Districts = _districtRepo
                    .GetDistricts()
                    .Where(d => d.Id == sessionDistrict.Value)
                    .ToList();
            }
            else
            {
                ViewBag.Districts = _districtRepo.GetDistricts(); // Super Admin & Branch Admin → full list
            }

            // BRANCHES
            if (roleId == 3) // Branch Admin → only their branch
            {
                ViewBag.Branches = _branchRepo
                    .GetBranches()
                    .Where(b => b.Id == sessionBranch.Value)
                    .ToList();
            }
            else if (roleId == 2) // District Admin → only branches in their district
            {
                ViewBag.Branches = _branchRepo.GetBranchesByDistrict(sessionDistrict.Value);
            }
            else
            {
                ViewBag.Branches = _branchRepo.GetBranches(); // Super Admin → full list
            }

            return View(new Staff());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Staff model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Districts = _districtRepo.GetDistricts();
                ViewBag.Branches = _branchRepo.GetBranches();
                return View(model);
            }

            // Optional: ensure current user can create staff in selected district/branch
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");
            int? sessionBranch = HttpContext.Session.GetInt32("BranchId");

            if (roleId == 2 && model.DistrictId != sessionDistrict) // District admin cannot add outside
            {
                ModelState.AddModelError("", "You cannot add staff to a different district.");
                ViewBag.Districts = _districtRepo.GetDistricts();
                ViewBag.Branches = _branchRepo.GetBranches();
                return View(model);
            }
            if (roleId == 3 && model.BranchId != sessionBranch)
            {
                ModelState.AddModelError("", "You cannot add staff to a different branch.");
                ViewBag.Districts = _districtRepo.GetDistricts();
                ViewBag.Branches = _branchRepo.GetBranches();
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                // refill dropdowns
                ViewBag.Districts = _districtRepo.GetDistricts();
                ViewBag.Branches = _branchRepo.GetBranches();
                return View(model);
            }

            _staffRepo.Add(model);
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = _staffRepo.GetById(id);
            if (model == null) return NotFound();

            // optional access check:
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");
            int? sessionBranch = HttpContext.Session.GetInt32("BranchId");
            if (roleId == 2 && model.DistrictId != sessionDistrict) return Forbid();
            if (roleId == 3 && model.BranchId != sessionBranch) return Forbid();

            ViewBag.Districts = _districtRepo.GetDistricts();
            ViewBag.Branches = (roleId == 2) ? _branchRepo.GetBranchesByDistrict(sessionDistrict.Value) : _branchRepo.GetBranches();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Staff model)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");
            int? sessionBranch = HttpContext.Session.GetInt32("BranchId");

            if (!ModelState.IsValid)
            {
                ViewBag.Districts = _districtRepo.GetDistricts();
                ViewBag.Branches = _branchRepo.GetBranchesByDistrict(sessionDistrict ?? 0);
                return View(model);
            }

            var staff = _staffRepo.GetById(model.Id);
            staff.Id = model.Id;
            staff.FirstName = model.FirstName;
            staff.LastName = model.LastName;
            staff.Email = model.Email;
            staff.Phone = model.Phone;
            staff.Address = model.Address;
            staff.EducationalQualifications = model.EducationalQualifications;
            staff.HandlingClasses = model.HandlingClasses;
            staff.DistrictId = model.DistrictId;
            staff.BranchId = model.BranchId;

            _staffRepo.Update(staff);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var model = _staffRepo.GetById(id);
            if (model == null) return NotFound();

            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");
            int? sessionBranch = HttpContext.Session.GetInt32("BranchId");

            if (roleId == 2 && model.DistrictId != sessionDistrict) return Forbid();
            if (roleId == 3 && model.BranchId != sessionBranch) return Forbid();
            // You can add checks similar to Edit/Delete access
            _staffRepo.Delete(id);
            return RedirectToAction("Index","Home");
        }
    }
}
