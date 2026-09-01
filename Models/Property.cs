using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RealEstateSystem.Models.Enums;

namespace RealEstateSystem.Models
{
    public class Property
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Property Type")]
        public int PropertyTypeId { get; set; }
        public PropertyType PropertyType { get; set; }

        [Display(Name = "City")]
        public int CityId { get; set; }
        public City City { get; set; }

        [Display(Name = "Agent")]
        public int AgentId { get; set; }
        public Agent Agent { get; set; }

        [Required, StringLength(300)]
        public string Address { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1000000000)]
        public decimal Price { get; set; }

        [Range(1, 100000)]
        [Display(Name = "Area (m2)")]
        public int Area { get; set; }

        [Range(0, 20)]
        public int Bedrooms { get; set; }

        [Range(0, 20)]
        public int Bathrooms { get; set; }

        [Display(Name = "For Sale / Rent")]
        public ListingType ListingType { get; set; }

        public PropertyStatus Status { get; set; }

        [Display(Name = "Furnished")]
        public bool IsFurnished { get; set; }

        [Display(Name = "Main Image")]
        public string MainImageUrl { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile ImageFile { get; set; }

        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
