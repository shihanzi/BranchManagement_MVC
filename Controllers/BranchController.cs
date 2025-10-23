using AhadiyyaMVC.DataAccess;
using AhadiyyaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace AhadiyyaMVC.Controllers
{
    public class BranchController : Controller
    {
        private readonly BranchRepository _branchRepo;
        private readonly DistrictRepository _districtRepo;
        public BranchController(BranchRepository branchRepo,DistrictRepository districtRepo)
            => (_branchRepo, _districtRepo) = (branchRepo,districtRepo);
        public IActionResult Index()
        {
            var model = _branchRepo.GetBranches();
            return View(model);
        }

        [HttpGet]
        
        public IActionResult Create()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");
            int? sessionBranch = HttpContext.Session.GetInt32("BranchId");

            // Branch Admins shouldn't be here at all
            if (roleId == 3)
                return RedirectToAction("Index", "Staff");

            // Fill District dropdown according to role:
            if (roleId == 2 && sessionDistrict.HasValue) // DistrictAdmin -> only their district
            {
                ViewBag.Districts = _districtRepo.GetDistricts().Where(d => d.Id == sessionDistrict.Value).ToList();
            }
            else // SuperAdmin or others -> all districts
            {
                ViewBag.Districts = _districtRepo.GetDistricts();
            }

            return View(new Branch());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Branch model)
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
                ViewBag.Districts = (roleId == 2 && sessionDistrict.HasValue)
                    ? _districtRepo.GetDistricts().Where(d => d.Id == sessionDistrict.Value).ToList()
                    : _districtRepo.GetDistricts();

                return View(model);
            }

            _branchRepo.AddBranch(model); // implement in repo
            return RedirectToAction("Index");
        }

        // GET: Branch/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");

            // Branch Admins not allowed
            if (roleId == 3)
                return RedirectToAction("Index", "Staff");

            var model = _branchRepo.GetBranchesbyId(id);
            if (model == null) return NotFound();

            // District Admin may only edit branches in their district
            if (roleId == 2 && sessionDistrict.HasValue && model.DistrictId != sessionDistrict.Value)
                return Forbid();

            // prepare districts dropdown
            if (roleId == 2 && sessionDistrict.HasValue)
                ViewBag.Districts = _districtRepo.GetDistricts().Where(d => d.Id == sessionDistrict.Value).ToList();
            else
                ViewBag.Districts = _districtRepo.GetDistricts();

            return View(model);
        }

        // POST: Branch/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Branch model)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");

            if (roleId == 3)
                return Forbid();

            // District admin cannot change branch to a different district
            if (roleId == 2 && sessionDistrict.HasValue && model.DistrictId != sessionDistrict.Value)
            {
                ModelState.AddModelError("", "District Admin can only assign branches to their own district.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Districts = (roleId == 2 && sessionDistrict.HasValue)
                    ? _districtRepo.GetDistricts().Where(d => d.Id == sessionDistrict.Value).ToList()
                    : _districtRepo.GetDistricts();

                return View(model);
            }

            // Additional server-side permission check to ensure editing allowed
            var existing = _branchRepo.GetBranchesbyId(model.Id);
            if (existing == null) return NotFound();
            if (roleId == 2 && sessionDistrict.HasValue && existing.DistrictId != sessionDistrict.Value) return Forbid();

            _branchRepo.UpdateBranch(model); // implement in repo
            return RedirectToAction("Index");
        }

        // POST: Branch/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");

            var existing = _branchRepo.GetBranchesbyId(id);
            if (existing == null) return NotFound();

            // Branch Admins not allowed to delete branches
            if (roleId == 3) return Forbid();

            // District admin only their district
            if (roleId == 2 && sessionDistrict.HasValue && existing.DistrictId != sessionDistrict.Value) return Forbid();

            _branchRepo.DeleteBranch(id);
            return RedirectToAction("Index");
        }
    }
}
