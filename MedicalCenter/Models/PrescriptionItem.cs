using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class PrescriptionItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Prescription")]
        public Guid PrescriptionId { get; set; }

        [ForeignKey("Medicine")]
        public Guid MedicineId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        [StringLength(100)]
        public string? Notes { get; set; }

        public Prescription Prescription { get; set; } = null!;
        public Medicine Medicine { get; set; } = null!;
    }
}