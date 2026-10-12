using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using StoreManagement.Web.Data;
using StoreManagement.Web.Models;
using StoreManagement.Web.Services;
using StoreManagement.Web.ViewModel;

namespace StoreManagement.Web.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext _db;
        private IEmailService _emailService;
        public CategoryController(ApplicationDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public IActionResult Index(string searchKey)
        {
            var categories = _db.Categories.Where(x => !x.IsDeleted && (x.CategoryName.Contains(searchKey) || string.IsNullOrWhiteSpace(searchKey))).ToList();
            ViewBag.searchKey = searchKey;
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryViewModel input)
        {
            var nameIsExist = _db.Categories.Any(x => x.CategoryName == input.CategoryName && !x.IsDeleted);
            if (nameIsExist)
            {
                TempData["msg"] = "e:Category Name is already Exist!";
                return View(input);
            }
            if (ModelState.IsValid)
            {
                var category = new Category();
                category.CategoryName = input.CategoryName;
                category.CreatedAt = DateTime.Now;
                _db.Categories.Add(category);
                _db.SaveChanges();

                var admins = _db.Admins.Where(x => !x.IsDeleted).ToList();
                foreach (var admin in admins)
                {
                    await _emailService.Send(admin.Email, "New Category", $"Category with Name {category.CategoryName} was added.");
                }

                TempData["msg"] = "s:Item added successfully";
                return RedirectToAction("Index");
            }
            return View(input);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _db.Categories.SingleOrDefault(x => x.CategoryId == id && !x.IsDeleted);
            if(category == null)
            {
                return NotFound();
            }
            var vmUpdate = new UpdateCategoryViewModel();
            vmUpdate.CategoryId = category.CategoryId;
            vmUpdate.CategoryName = category.CategoryName;
            return View(vmUpdate);
        }

        [HttpPost]
        public IActionResult Update(UpdateCategoryViewModel input)
        {
            if (ModelState.IsValid)
            {
                var category = _db.Categories.SingleOrDefault(x => x.CategoryId == input.CategoryId && !x.IsDeleted);
                if (category == null)
                {
                    return NotFound();
                }
                category.CategoryName = input.CategoryName;
                _db.Categories.Update(category);
                _db.SaveChanges();
                TempData["msg"] = "s:Item Updated successfully";
                return RedirectToAction("Index");
            }
            return View(input);
        }

        public IActionResult Delete(int id)
        {
            var category = _db.Categories.SingleOrDefault(x => x.CategoryId == id && !x.IsDeleted);
            if(category == null)
            {
                return NotFound();
            }
            category.IsDeleted = true;
            _db.Categories.Update(category);
            _db.SaveChanges();
            TempData["msg"] = "s:Item Deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
