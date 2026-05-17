using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Prescription
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("MedicalRecord")]
        public Guid MedicalRecordId { get; set; }

        [ForeignKey("Doctor")]
        public Guid DoctorId { get; set; }

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        public MedicalRecord MedicalRecord { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;

        public List<PrescriptionItem> Items { get; set; } = new();
    }
}