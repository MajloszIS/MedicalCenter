using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class CartItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Cart")]
        public Guid CartId { get; set; }

        [ForeignKey("Medicine")]
        public Guid MedicineId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        public Cart Cart { get; set; } = null!;
        public Medicine Medicine { get; set; } = null!;
    }
}