using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class MedicalRecord
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Patient")]
        public Guid PatientId { get; set; }

        [ForeignKey("Doctor")]
        public Guid DoctorId { get; set; }

        public Patient Patient { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;

        public List<Diagnosis> Diagnoses { get; set; } = new();
        public List<Prescription> Prescriptions { get; set; } = new();
    }
}