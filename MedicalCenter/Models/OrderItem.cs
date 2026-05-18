using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }

        [ForeignKey("Medicine")]
        public Guid MedicineId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        public Order Order { get; set; } = null!;
        public Medicine Medicine { get; set; } = null!;
    }
}