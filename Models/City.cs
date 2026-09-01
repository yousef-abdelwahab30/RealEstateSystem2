using System.ComponentModel.DataAnnotations;

namespace RealEstateSystem.Models
{
    public class City
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; }

        [StringLength(80)]
        public string Governorate { get; set; }

        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
