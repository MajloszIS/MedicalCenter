using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Diagnosis
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("MedicalRecord")]
        public Guid MedicalRecordId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        public DateTime DiagnosedAt { get; set; } = DateTime.UtcNow;

        public MedicalRecord MedicalRecord { get; set; } = null!;

        public List<Treatment> Treatments { get; set; } = new();
    }
}