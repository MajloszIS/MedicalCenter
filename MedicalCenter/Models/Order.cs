using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PatientId { get; set; }

        [Range(0, 100000)]
        public decimal TotalPrice { get; set; }

        public Guid StatusId { get; set; }
        public OrderStatus Status { get; set; }

        public Patient Patient { get; set; }

        public List<OrderItem> Items { get; set; }
    }
}