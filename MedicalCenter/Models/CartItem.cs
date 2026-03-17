using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class CartItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CartId { get; set; }
        public Guid MedicineId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        public Cart Cart { get; set; }
        public Medicine Medicine { get; set; }
    }
}