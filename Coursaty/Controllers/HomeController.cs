using Microsoft.AspNetCore.Mvc;
using Coursaty.Models;
using System.Diagnostics;
using System.Collections.Generic;
using Coursaty.Data;

namespace Coursaty.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _db;

        public HomeController (AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            UsersInstructors model = new UsersInstructors()
            {
                CarouselItems = _db.CarouselItems.ToList(),
                Courses = _db.Courses.ToList(),
                AddUsers = _db.AddUser.ToList(),
                MenuItems = _db.MenuItems.ToList(),


            };

            return View(model);
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
