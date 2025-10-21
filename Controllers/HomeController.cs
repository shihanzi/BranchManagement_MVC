using AhadiyyaMVC.DataAccess;
using AhadiyyaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AhadiyyaMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private StaffRepository _userRepository;
        public HomeController(ILogger<HomeController> logger, StaffRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            var users = _userRepository.GetAllUsers(); // fetch all users
            return View(users);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
