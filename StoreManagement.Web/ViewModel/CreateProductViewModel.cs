using StoreManagement.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Web.ViewModel
{
    public class CreateProductViewModel
    {
        [Required]
        public string ProductName { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        [Required]
        public IFormFile ImageURL { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }
}
