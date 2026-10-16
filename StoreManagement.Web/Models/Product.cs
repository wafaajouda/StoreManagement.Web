using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Web.Models
{
    public class Product : BaseEntity
    {
        public int ProductId { get; set; }
        [Required]
        public  string ProductName { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }

        public string ImageURL { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }
    }
}
