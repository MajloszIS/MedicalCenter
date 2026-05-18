using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Medicine
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [Range(0.01, 10000)]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }

        public MedicineCategory Category { get; set; } = null!;
    }
}