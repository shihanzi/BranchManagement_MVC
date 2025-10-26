using AhadiyyaMVC.DataAccess;
using AhadiyyaMVC.Models;
using Microsoft.AspNetCore.Mvc;

public class DistrictController : Controller
{
    private readonly DistrictRepository _districtRepo;
    public DistrictController(DistrictRepository districtRepo)
    {
        _districtRepo = districtRepo;
    }
    public IActionResult Index()
    {
        var model = _districtRepo.GetDistricts();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(District model)
    {
        int? roleId = HttpContext.Session.GetInt32("RoleId");

        // Branch Admins not allowed
        //if (roleId == 1)
        //    return Forbid();

        // Enforce DistrictAdmin restriction: they can only create branches in their own district
        //if (roleId == 1 && roleId.HasValue && model.DistrictId != roleId.Value)
        //{
        //    ModelState.AddModelError("", "Admin can only create district.");
        //}

        //if (!ModelState.IsValid)
        //{
        //    ViewBag.Districts = (roleId == 1 && roleId.HasValue)
        //        ? _districtRepo.GetDistricts().Where(d => d.Id == roleId.Value).ToList()
        //        : _districtRepo.GetDistricts();

        //    return View(model);
        //}

        _districtRepo.AddDistrict(model); // implement in repo
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        int? roleId = HttpContext.Session.GetInt32("RoleId");
        //int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");

        //// Branch Admins not allowed
        //if (roleId == 3)
        //    return RedirectToAction("Index", "Staff");

        var model = _districtRepo.GetDistrictsbyId(id);
        //if (model == null) return NotFound();

        //// District Admin may only edit branches in their district
        //if (roleId == 2 && sessionDistrict.HasValue && model.DistrictId != sessionDistrict.Value)
        //    return Forbid();

        //// prepare districts dropdown
        //if (roleId == 2 && sessionDistrict.HasValue)
        //    ViewBag.Districts = _districtRepo.GetDistricts().Where(d => d.Id == sessionDistrict.Value).ToList();
        //else
        //    ViewBag.Districts = _districtRepo.GetDistricts();

        return View(model);
    }

    public IActionResult Create()
    {
        //int? roleId = HttpContext.Session.GetInt32("RoleId");
        //int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");
        //int? sessionBranch = HttpContext.Session.GetInt32("BranchId");

        // Branch Admins shouldn't be here at all
        //if (roleId == 1)
        //    return RedirectToAction("Create", "District");

        // Fill District dropdown according to role:
        //if (roleId == 1 && sessionDistrict.HasValue) // DistrictAdmin -> only their district
        //{
        //    ViewBag.Districts = _districtRepo.GetDistricts().Where(d => d.Id == sessionDistrict.Value).ToList();
        //}
        //else // SuperAdmin or others -> all districts
        //{
            ViewBag.Districts = _districtRepo.GetDistricts();
        //}

        return View(new District());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        int? roleId = HttpContext.Session.GetInt32("RoleId");
        int? sessionDistrict = HttpContext.Session.GetInt32("DistrictId");

        var existing = _districtRepo.GetDistrictsbyId(id);
        if (existing == null) return NotFound();

        // Branch Admins not allowed to delete branches
        if (roleId == 3) return Forbid();

        // District admin only their district
        //if (roleId == 2 && sessionDistrict.HasValue && existing.DistrictId != sessionDistrict.Value) return Forbid();

        _districtRepo.DeleteDistricts(id);
        return RedirectToAction("Index");
    }

}
