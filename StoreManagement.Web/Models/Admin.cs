namespace StoreManagement.Web.Models
{
    public class Admin : BaseEntity
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string Email { get; set; }
        public string Phone {  get; set; }
    }
}
