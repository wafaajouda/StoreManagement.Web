namespace StoreManagement.Web.ViewModel
{
    public class UpdateProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public string? ImageURL { get; set; }

        public IFormFile? ImageFile { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }
}
