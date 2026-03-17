using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class MedicineCategory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public List<Medicine> Medicines { get; set; }
    }
}