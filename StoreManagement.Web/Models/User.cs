using Microsoft.AspNetCore.Identity;
using StoreManagement.Web.Enums;

namespace StoreManagement.Web.Models
{
    public class User : IdentityUser
    {
        public bool IsDelete { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public UserType UserType { get; set; }
    }
}
