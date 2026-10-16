using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Web.ViewModel
{
    public class CreateBrandViewModel
    {
        [Required] 
        public string Name { get; set; }
    }
}
