using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class PrescriptionItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PrescriptionId { get; set; }
        public Guid MedicineId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        public Prescription Prescription { get; set; }
        public Medicine Medicine { get; set; }
    }
}