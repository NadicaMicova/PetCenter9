using Microsoft.AspNetCore.Identity;

namespace PetCenter9.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
