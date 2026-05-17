using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Patient")]
        public Guid PatientId { get; set; }

        [ForeignKey("Doctor")]
        public Guid DoctorId { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }

        [Range(15, 120, ErrorMessage = "Czas trwania od 15 do 120 minut")]
        public int DurationMinutes { get; set; } = 30;

        [StringLength(500)]
        public string Description { get; set; } = null!;

        [StringLength(1000)]
        public string? Notes { get; set; }

        public Patient Patient { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
        public AppointmentStatus Status { get; set; } = null!;
    }
}