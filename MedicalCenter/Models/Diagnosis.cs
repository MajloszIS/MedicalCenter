using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Diagnosis
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MedicalRecordId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public MedicalRecord MedicalRecord { get; set; }

        public List<Treatment> Treatments { get; set; } = new List<Treatment>();
    }
}