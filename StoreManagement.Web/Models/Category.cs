using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Web.Models
{
    public class Category : BaseEntity
    {
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
