namespace StoreManagement.Web.Models
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
