using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PetCenter9.Models
{
    public class Pets
    {
        [Key]
        public int PetsId { get; set; }
        public string? PetPictureURL { get; set; }

        [Required(ErrorMessage = "Pet name is required")]
        public string Name { get; set; }

        [Range(0, 50, ErrorMessage = "Age must be between 0 and 50")]
        public int Age { get; set; }
        [ForeignKey("Owner")]
        public int OwnersId { get; set; }
        public Owners? Owners { get; set; }
        public virtual ICollection<Vaccines> Vaccine { get; set; }
        public Pets()
        {
            Vaccine = new List<Vaccines>();
        }
    }
}
