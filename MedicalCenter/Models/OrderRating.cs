using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class OrderRating
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Patient")]
        public Guid PatientId { get; set; }

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; } = null!;
        public Order Order { get; set; } = null!;
    }
}