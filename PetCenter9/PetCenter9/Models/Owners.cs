using System.ComponentModel.DataAnnotations;

namespace PetCenter9.Models
{
    public class Owners
    {
        [Key]
        public int OwnersId { get; set; }

        public string? OwnerPictureURL { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        public int Age { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        public virtual ICollection<Pets> Pet { get; set; }
        public Owners()
        {
            Pet = new List<Pets>();
        }
    }
}
