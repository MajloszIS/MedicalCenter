using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Prescription
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MedicalRecordId { get; set; }
        public Guid DoctorId { get; set; }

        public MedicalRecord MedicalRecord { get; set; }
        public Doctor Doctor { get; set; }

        public List<PrescriptionItem> Items { get; set; }
    }
}