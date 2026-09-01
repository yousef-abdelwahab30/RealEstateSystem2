using System.ComponentModel.DataAnnotations;
using RealEstateSystem.Models.Enums;

namespace RealEstateSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Display(Name = "Property")]
        public int PropertyId { get; set; }
        public Property Property { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Visitor Name")]
        public string VisitorName { get; set; }

        [Required, StringLength(20)]
        [Display(Name = "Visitor Phone")]
        public string VisitorPhone { get; set; }

        [Display(Name = "Visit Date")]
        [DataType(DataType.DateTime)]
        public DateTime RequestedDate { get; set; }

        public AppointmentStatus Status { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }
    }
}
