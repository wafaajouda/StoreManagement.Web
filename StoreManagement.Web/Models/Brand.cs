using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Web.Models
{
    public class Brand : BaseEntity
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
