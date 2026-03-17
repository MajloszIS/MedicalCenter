using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class MedicalRecord
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }

        public List<Diagnosis> Diagnoses { get; set; }
    }
}