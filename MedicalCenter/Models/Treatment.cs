using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Treatment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Diagnosis")]
        public Guid DiagnosisId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        public Diagnosis Diagnosis { get; set; } = null!;
    }
}