using System.ComponentModel.DataAnnotations;

namespace RealEstateSystem.Models
{
    public class PropertyImage
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public int DisplayOrder { get; set; }
    }
}
