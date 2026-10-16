using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Web.Data;
using StoreManagement.Web.Models;
using StoreManagement.Web.ViewModel;

namespace StoreManagement.Web.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class BrandController : Controller
    {
        private ApplicationDbContext _db;
        public BrandController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string searchKey)
        {
            var brands = _db.Brands.Where(x => !x.IsDeleted && (x.Name.Contains(searchKey) || string.IsNullOrWhiteSpace(searchKey))).ToList();
            ViewBag.searchKey = searchKey; 
            return View(brands);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateBrandViewModel input)
        {
            var NameIsExist = _db.Brands.Any(x => !x.IsDeleted && x.Name == input.Name);
            if(NameIsExist)
            {
                TempData["msg"] = "e: Brand Name is already exist!";
                return View(input);
            }
            if(ModelState.IsValid)
            {
                var brand = new Brand();
                brand.Name = input.Name;
                brand.CreatedAt = DateTime.Now;
                _db.Brands.Add(brand);
                _db.SaveChanges();
                TempData["msg"] = "s: Brand Added Successfully";
                return RedirectToAction("Index");
            }
            return View(input);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var brand = _db.Brands.SingleOrDefault(x => x.Id == id && !x.IsDeleted);
            if(brand == null)
            {
                return NotFound();
            }
            var brandVm = new UpdateBrandViewModel();
            brandVm.Id = brand.Id;
            brandVm.Name = brand.Name;
            return View(brandVm);
        }

        [HttpPost]
        public IActionResult Update(UpdateBrandViewModel input)
        {
            if (ModelState.IsValid)
            {
                var brand = _db.Brands.SingleOrDefault(x => x.Id == input.Id && !x.IsDeleted);
                if (brand == null)
                {
                    return NotFound();
                }

                brand.Name = input.Name;
                _db.Brands.Update(brand);
                _db.SaveChanges();
                TempData["msg"] = "s: Brand updated Successfully";
                return RedirectToAction("Index");
            }
            return View(input);
        }

        public IActionResult Delete(int id)
        {
            var brand = _db.Brands.SingleOrDefault(x => x.Id == id && !x.IsDeleted);
            brand.IsDeleted = true;
            _db.Brands.Update(brand);
            _db.SaveChanges();
            TempData["msg"] = "s: Brand deleted Successfully";
            return RedirectToAction("Index");
        }

    }
}
