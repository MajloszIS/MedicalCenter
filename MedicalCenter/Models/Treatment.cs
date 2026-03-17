using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Treatment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DiagnosisId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public Diagnosis Diagnosis { get; set; }
    }
}