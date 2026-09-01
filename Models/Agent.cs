using System.ComponentModel.DataAnnotations;

namespace RealEstateSystem.Models
{
    public class Agent
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required, StringLength(20)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [EmailAddress, StringLength(150)]
        public string Email { get; set; }

        [StringLength(150)]
        [Display(Name = "Agency Name")]
        public string AgencyName { get; set; }

        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
