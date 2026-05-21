using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class MedicalLeave
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Patient")]
        public Guid PatientId { get; set; }

        [ForeignKey("Doctor")]
        public Guid DoctorId { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        [Required, StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        public DateTime IssuedAt { get; set; }

        public Patient Patient { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
    }
}


