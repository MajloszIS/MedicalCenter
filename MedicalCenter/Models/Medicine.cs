using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Medicine
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        public Guid CategoryId { get; set; }

        public MedicineCategory Category { get; set; }
    }
}