using System.ComponentModel.DataAnnotations;

namespace RealEstateSystem.Models
{
    public class PropertyType
    {
        public int Id { get; set; }

        [Required, StringLength(60)]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
