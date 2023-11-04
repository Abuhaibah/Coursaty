using Coursaty.Data;
using Coursaty.Models;
using Coursaty.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Coursaty.Controllers
{
    [Authorize]

    public class DashboardController : Controller
    {
        #region Configration
        private AppDbContext _db;
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;


        public DashboardController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager, ILogger<AccountController> logger, AppDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;


            _db = db;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Instructor")]
        public IActionResult Instructor()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
		{
			var isAdmin = User.IsInRole("Admin"); 
			if (isAdmin)
			{
				var adminName = User.Identity.Name;
				ViewBag.AdminName = adminName; 
			}
			return View();
		}
        [Authorize(Roles = "Student")]
        public IActionResult Student()
		{
			return View();
		}

        [HttpGet]
        [AllowAnonymous]
        public IActionResult UserInstructor(string role)
        {
            List<AddUserViewModel> userViewModels;

            if (role == "Student")
            {
                var studentUsers = _userManager.GetUsersInRoleAsync("Student").Result;
                userViewModels = CreateViewModels(studentUsers, "Student");
            }
            else if (role == "Instructor")
            {
                var instructorUsers = _userManager.GetUsersInRoleAsync("Instructor").Result;
                userViewModels = CreateViewModels(instructorUsers, "Instructor");
            }
            else
            {
                userViewModels = new List<AddUserViewModel>();
            }

            return View(userViewModels);
        }
        [AllowAnonymous]
        private List<AddUserViewModel> CreateViewModels(IEnumerable<IdentityUser> users, string role)
        {
            var userViewModels = new List<AddUserViewModel>();

            foreach (var user in users)
            {
                var model = new AddUserViewModel
                {
                    Email = user.Email,
                    Mobile = user.PhoneNumber,
                    FirstName = _userManager.GetClaimsAsync(user).Result.FirstOrDefault(c => c.Type == "FirstName")?.Value,
                    LastName = _userManager.GetClaimsAsync(user).Result.FirstOrDefault(c => c.Type == "LastName")?.Value,
                    Role = role
                };
                userViewModels.Add(model);
            }

            return userViewModels;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int userId)
        {
            var user = _db.AddUser.FirstOrDefault(u => u.userId == userId);

            if (user == null)
            {
                return NotFound();
            }

            var editModel = new EditUserViewModel
            {
                userId = user.userId,
                Email = user.Email,
                Mobile = user.Mobile,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            return View(editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.AddUser.FirstOrDefault(u => u.userId == model.userId);

                if (user == null)
                {
                    return NotFound(); 
                }

                user.Email = model.Email;
                user.Mobile = model.Mobile;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                _db.Update(user);
                await _db.SaveChangesAsync();

                return RedirectToAction("UserInstructor", new { role = user.Role });
            }

            return View(model);
        }
        [AllowAnonymous]
        public IActionResult Info(int userId)
        {
            var user = _db.AddUser.FirstOrDefault(u => u.userId == userId);

            if (user == null)
            {
                return NotFound(); 
            }

            var infoModel = new UserInfoViewModel
            {
                Email = user.Email,
                Mobile = user.Mobile,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            return View(infoModel);
        }
        [Authorize(Roles = "Admin,Instructor")]

        public IActionResult AddCourse()
        {
            var instructors = _userManager.GetUsersInRoleAsync("Instructor").Result;

            var instructorList = instructors.Select(i => new SelectListItem
            {
                Text = i.UserName,
                Value = i.Id
            }).ToList();

            ViewBag.Instructors = instructorList;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]

        public async Task<IActionResult> AddCourse(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
              
                model.InstructorName = model.InstructorName;

                _db.Courses.Add(model);
                _db.SaveChanges();

                return RedirectToAction(nameof(CourseList));
            }

            ViewBag.Instructors = GetInstructorsSelectList();
            return View(model);
        }
        [Authorize(Roles = "Admin,Instructor")]

        private List<SelectListItem> GetInstructorsSelectList()
        {
            var instructors = _userManager.GetUsersInRoleAsync("Instructor").Result;
            return instructors.Select(i => new SelectListItem
            {
                Text = i.UserName,
                Value = i.Id
            }).ToList();
        }

        [AllowAnonymous]
        public IActionResult CourseList()
        {
            var usersInstructors = new UsersInstructors
            {
                Courses = _db.Courses.ToList(),
            };

            return View(usersInstructors);
        }


        [Authorize(Roles = "Admin,Instructor")]

        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var usersInstructors = new UsersInstructors
            {
                Courses = new List<CourseViewModel> { course }
            };

            return View(usersInstructors);
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]

        public async Task<IActionResult> EditCourse(UsersInstructors usersInstructors)
        {
            if (ModelState.IsValid)
            {
                var course = usersInstructors.Courses.FirstOrDefault();
                if (course == null)
                {
                    return NotFound();
                }

                var existingCourse = await _db.Courses.FindAsync(course.CourseId);
                if (existingCourse is null)
                {
                    return NotFound();
                }

                existingCourse.CourseName = course.CourseName;
                existingCourse.InstructorName = course.InstructorName;

                await _db.SaveChangesAsync();

                return RedirectToAction("CourseList");
            }

            return View(usersInstructors);
        }


        [Authorize(Roles = "Admin,Instructor")]

        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var courseViewModel = new CourseViewModel
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                CourseDescription = course.CourseDescription,
                CourseStartDate = course.CourseStartDate,
                CourseImg = course.CourseImg,
                CourseHours = course.CourseHours,
                InstructorName = course.InstructorName
            };

            return View(courseViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]

        public async Task<IActionResult> DeleteCourse(int id, IdentityRole r)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();

            return RedirectToAction("CourseList");
        }
        [AllowAnonymous]
        public IActionResult DetailsCourse(int id)
        {
            var course = _db.Courses.Find(id);

            if (course == null)
            {
                return NotFound();
            }
            var studentsInCourse = _db.AddUser.Where(u => u.CourseId == id && u.Role == "Student").ToList();

            var usersInstructors = new UsersInstructors
            {
                Courses = new List<CourseViewModel> { course },
                AddUsers = studentsInCourse
            };

            return View(usersInstructors);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Instructor")]

        public IActionResult AddStudent(int courseId)
        {
            var course = _db.Courses.Find(courseId);

            var studentModel = new AddUserViewModel
            {
                CourseId = courseId,
                Role = "Student" 
            };

            return View(studentModel);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Instructor")]

        public IActionResult ChooseStudents(int courseId)
        {
            var course = _db.Courses.Find(courseId);

            var availableStudents = _userManager.GetUsersInRoleAsync("Student").Result;

            var model = new ChooseStudentsViewModel
            {
                CourseId = courseId,
                CourseName = course.CourseName,
                AvailableStudents = availableStudents.Select(user => new AddUserViewModel
                {
                    Email = user.Email,
                    FirstName = _userManager.GetClaimsAsync(user).Result.FirstOrDefault(c => c.Type == "FirstName")?.Value,
                    LastName = _userManager.GetClaimsAsync(user).Result.FirstOrDefault(c => c.Type == "LastName")?.Value
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]

        public IActionResult AddSelectedStudents(int courseId, List<int> selectedStudents)
        {
            try
            {
                Console.WriteLine($"Received courseId: {courseId}");
                Console.WriteLine("Received student IDs:");
                foreach (var studentId in selectedStudents)
                {
                    Console.WriteLine(studentId);
                }

                var course = _db.Courses.Find(courseId);

                if (course == null)
                {
                    return NotFound();
                }

                foreach (var studentId in selectedStudents)
                {
                    var student = _db.AddUser.Find(studentId);

                    if (student != null)
                    {
                        student.CourseId = courseId;
                    }
                    else
                    {
                        return NotFound($"Student with ID {studentId} not found.");
                    }
                }

                _db.SaveChanges();

                return RedirectToAction("DetailsCourse", new { id = courseId });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        [AllowAnonymous]
        public IActionResult CarouselList()
        {
            var carouselItems = _db.CarouselItems.ToList();

            var usersInstructors = new UsersInstructors
            {
                CarouselItems = carouselItems
            };

            return View(usersInstructors);
        }



        [HttpGet]
        [Authorize(Roles = "Admin")]

        public IActionResult AddCarousel()
        {
            var usersInstructors = new UsersInstructors();
            usersInstructors.CarouselItems.Add(new CarouselItem());
            return View(usersInstructors);
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]

        public IActionResult AddCarousel(UsersInstructors usersInstructors, List<IFormFile> imageFiles)
        {
            if (imageFiles != null && imageFiles.Count > 0)
            {
                try
                {
                    foreach (var imageFile in imageFiles)
                    {
                        if (imageFile.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                imageFile.CopyTo(memoryStream);
                                var carouselItem = new CarouselItem
                                {
                                    ImageData = Convert.ToBase64String(memoryStream.ToArray())
                                };

                                usersInstructors.CarouselItems.Add(carouselItem);
                            }
                        }
                    }

                    _db.Update(usersInstructors);
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error saving to the database: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No images provided or image sizes are zero.");
            }

            usersInstructors.CarouselItems = _db.CarouselItems.ToList();
            return View(usersInstructors);
        }






        [Authorize(Roles = "Admin")]

        public IActionResult EditCarousel(int id)
        {
            var item = _db.CarouselItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public IActionResult EditCarousel(CarouselItem item)
        {
            if (ModelState.IsValid)
            {
                _db.CarouselItems.Update(item);
                _db.SaveChanges();
                return RedirectToAction("CarouselList");
            }
            return View(item);
        }
        [Authorize(Roles = "Admin")]

        public IActionResult DeleteCarousel(int id)
        {
            var item = _db.CarouselItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public IActionResult DeleteCarouselConfirmed(int id)
        {
            var item = _db.CarouselItems.Find(id);
            if (item != null)
            {
                _db.CarouselItems.Remove(item);
                _db.SaveChanges();
            }
            return RedirectToAction("CarouselList");
        }




    }








}

