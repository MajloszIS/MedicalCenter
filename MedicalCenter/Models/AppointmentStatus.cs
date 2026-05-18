using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class AppointmentStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        public List<Appointment> Appointments { get; set; } = new();
    }
}