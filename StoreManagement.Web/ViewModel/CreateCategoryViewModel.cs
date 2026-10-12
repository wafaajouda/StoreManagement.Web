using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Web.ViewModel
{
    public class CreateCategoryViewModel
    {
        [Required]
        public string CategoryName { get; set; }
    }
}
