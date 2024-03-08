using Microsoft.AspNetCore.Identity;

namespace DelightFoods_Live.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
