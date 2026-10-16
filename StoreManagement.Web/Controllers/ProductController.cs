using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Web.Data;
using StoreManagement.Web.Models;
using StoreManagement.Web.Services;
using StoreManagement.Web.ViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StoreManagement.Web.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class ProductController : Controller
    {
        private ApplicationDbContext _db;
        private IFileService _fileService;
        private IEmailService _emailService;

        public ProductController(ApplicationDbContext db, IFileService fileService, IEmailService emailService)
        {
            _db = db;
            _fileService = fileService;
            _emailService = emailService;
        }
        [Authorize]
        public IActionResult Index(string searchKey)
        {
            var products = _db.Products
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Where(x => !x.IsDeleted && (x.ProductName.Contains(searchKey) || string.IsNullOrWhiteSpace(searchKey)))
                .ToList();
            ViewBag.searchKey = searchKey;
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["categoryList"] = new SelectList(_db.Categories.Where(x => !x.IsDeleted).ToList(), "CategoryId", "CategoryName");
            ViewData["brandList"] = new SelectList(_db.Brands.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel input)
        {
            var nameIsExist = _db.Products.Any(x => x.ProductName == input.ProductName && !x.IsDeleted);
            if(nameIsExist)
            {
                TempData["msg"] = "e:Product Name is already Exist!";
                ViewData["categoryList"] = new SelectList(_db.Categories.Where(x => !x.IsDeleted).ToList(), "CategoryId", "CategoryName");
                ViewData["brandList"] = new SelectList(_db.Brands.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
                return View(input);
            }
            if (ModelState.IsValid)
            {
                var product = new Product();
                product.ProductName = input.ProductName;
                product.Description = input.Description;
                product.Price = input.Price;
                
                if(input.ImageURL != null)
                {
                    product.ImageURL = await _fileService.SaveFile(input.ImageURL, "Images");
                }
                
                product.CategoryId = input.CategoryId;
                product.BrandId = input.BrandId;
                product.CreatedAt = DateTime.Now;
                _db.Products.Add(product);
                _db.SaveChanges();

                var admins = _db.Admins.Where(x => !x.IsDeleted).ToList();
                foreach (var admin in admins)
                {
                    await _emailService.Send(admin.Email, "New Product", $"Product with Name {product.ProductName} was added.");
                }

                TempData["msg"] = "s:Product was added successfully";
                return RedirectToAction("Index");
            }
            ViewData["categoryList"] = new SelectList(_db.Categories.Where(x => !x.IsDeleted).ToList(), "CategoryId", "CategoryName");
            ViewData["brandList"] = new SelectList(_db.Brands.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
            return View(input);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            ViewData["categoryList"] = new SelectList(_db.Categories.Where(x => !x.IsDeleted).ToList(), "CategoryId", "CategoryName");
            ViewData["brandList"] = new SelectList(_db.Brands.Where(x => !x.IsDeleted).ToList(), "Id", "Name");

            var product = _db.Products.SingleOrDefault(x => x.ProductId == id && !x.IsDeleted);
            if (product == null)
            {
                return NotFound();
            }
            var vmUpdate = new UpdateProductViewModel();
            vmUpdate.ProductId = product.ProductId;
            vmUpdate.ProductName = product.ProductName;
            vmUpdate.Description = product.Description;
            vmUpdate.ImageURL = product.ImageURL;
            vmUpdate.Price = product.Price;
            vmUpdate.CategoryId = product.CategoryId;
            vmUpdate.BrandId = product.BrandId;
            return View(vmUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductViewModel input)
        {
            if (ModelState.IsValid)
            {
                var product = _db.Products.SingleOrDefault(x => x.ProductId == input.ProductId && !x.IsDeleted);
                if (product == null)
                {
                    return NotFound();
                }
                product.ProductName = input.ProductName;
                product.Description = input.Description;

                if(input.ImageFile != null && input.ImageFile.Length > 0)
                {
                    product.ImageURL = await _fileService.SaveFile(input.ImageFile, "Images");
                }

                product.Price = input.Price;
                product.CategoryId = input.CategoryId;
                product.BrandId = input.BrandId;
                _db.Products.Update(product);
                _db.SaveChanges();
                TempData["msg"] = "s:Product Updated successfully";
                return RedirectToAction("Index");
            }
            ViewData["categoryList"] = new SelectList(_db.Categories.Where(x => !x.IsDeleted).ToList(), "CategoryId", "CategoryName");
            ViewData["brandList"] = new SelectList(_db.Brands.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
            return View(input);
        }

        public IActionResult Delete(int id)
        {
            var product = _db.Products.SingleOrDefault(x => x.ProductId == id && !x.IsDeleted);
            if (product == null)
            {
                return NotFound();
            }
            product.IsDeleted = true;
            _db.Products.Update(product);
            _db.SaveChanges();
            TempData["msg"] = "s:Product Deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
