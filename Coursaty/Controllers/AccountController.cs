using System.Security.Claims;
using System.Threading.Tasks;
using Coursaty.Data;
using Coursaty.Models;
using Coursaty.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Coursaty.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        #region Configration
        private AppDbContext _db;
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;


        public AccountController(UserManager<IdentityUser> userManager,
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


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.Mobile
                };
                var result = await _userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    await _userManager.AddClaimAsync(user, new Claim("FirstName", model.FirstName));
                    await _userManager.AddClaimAsync(user, new Claim("LastName", model.LastName));
                    return RedirectToAction("RoleList", "Account");

                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return View(model);
            }
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var result = await _signInManager.PasswordSignInAsync(model.Email!, model.Password!, model.RememberMe, false);


                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Dashboard");
                }

                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);

            }

            return View(model);
        }
        [AllowAnonymous]

        public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Login", "Account");
		}
        #region Roles
        [Authorize(Roles = "Admin")]
        public IActionResult RoleList()
        {
            var usersInstructors = new UsersInstructors
            {
                IdentityRoles = _roleManager.Roles.ToList()
            };

            return View(usersInstructors);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                ModelState.AddModelError("", "Not Created");
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var model = new UsersInstructors();

            var editRoleViewModel = new EditRoleViewModel
            {
                RoleName = role.Name,
                RoleId = role.Id
            };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    editRoleViewModel.Users.Add(user.Email);
                }
            }

            model.EditRoleViewModels = new List<EditRoleViewModel> { editRoleViewModel };

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId!);
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            return View(role);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole(string id, IdentityRole r)
        {

            if (id == null)
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            var res = await _roleManager.DeleteAsync(role!);
            if (res.Succeeded)
            {
                return RedirectToAction(nameof(RoleList));
            }
            return View(role);

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            var usersInstructors = new UsersInstructors();
            var userRoleViewModels = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                var userRoleModel = new UserRoleViewModel
                {
                    UserName = user.UserName,
                    UserId = user.Id,
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleModel.isSelected = true;
                }
                else
                {
                    userRoleModel.isSelected = false;
                }
                userRoleViewModels.Add(userRoleModel);
            }

            usersInstructors.UserRoleViewModels = userRoleViewModels;

            return View(usersInstructors); 
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserRole(string id, UsersInstructors usersInstructors) 
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            IdentityResult result = null;

            for (int i = 0; i < usersInstructors.UserRoleViewModels.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(usersInstructors.UserRoleViewModels[i].UserId);
                if (user == null)
                {
                    return NotFound();
                }

                if (usersInstructors.UserRoleViewModels[i].isSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!usersInstructors.UserRoleViewModels[i].isSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            if (result != null && result.Succeeded)
            {
                return RedirectToAction("EditRole", new { id = id });
            }

            ModelState.AddModelError("", "Failed to update user roles.");
            return View(usersInstructors); 
        }



        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion
    }
}
