using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Web.Data;
using StoreManagement.Web.Models;
using StoreManagement.Web.ViewModel;

namespace StoreManagement.Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        
        public UserController(ApplicationDbContext db, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = _db.Users.Where(x => !x.IsDelete).Select(x => new UserViewModel()
            {
                Id = x.Id,
                UserName = x.Email,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                CreatedAt = x.CreatedAt
            }).ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel input)
        {
            var user = new User();
            user.CreatedAt = DateTime.Now;
            user.UserName = input.Email;
            user.Email = input.Email;
            user.PhoneNumber = input.PhoneNumber;
            user.UserType = input.UserType;

            var result = await _userManager.CreateAsync(user, input.Password);
            if(input.UserType == Enums.UserType.Admin)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }else if (input.UserType == Enums.UserType.Employee)
            {
                await _userManager.AddToRoleAsync(user, "Employee");
            }

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(input);
        }

        public IActionResult Delete(string Id)
        {
            var user = _db.Users.SingleOrDefault(x => x.Id == Id && !x.IsDelete);
            user.IsDelete = true;
            _db.Users.Update(user);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> InitRoles()
        {
            if (!_db.Roles.Any())
            {
                var roles = new List<string>();
                roles.Add("Admin");
                roles.Add("Employee");
                foreach (var role in roles)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            return RedirectToAction("Index");
        }
    }
}
