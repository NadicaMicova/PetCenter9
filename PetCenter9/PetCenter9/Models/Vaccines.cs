using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PetCenter9.Models
{
    public class Vaccines
    {
        [Key]
        public int VaccinesId { get; set; }

        [Required(ErrorMessage = "Vaccine name is required")]
        public string Name { get; set; }
        [ForeignKey("Pets")]
        public int Pets { get; set; }
        public virtual ICollection<Pets> Pet { get; set; }
        public Vaccines()
        {
            Pet = new List<Pets>();
        }
    }
}
