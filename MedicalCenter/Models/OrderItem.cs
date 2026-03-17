using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }
        public Guid MedicineId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        public Order Order { get; set; }
        public Medicine Medicine { get; set; }
    }
}