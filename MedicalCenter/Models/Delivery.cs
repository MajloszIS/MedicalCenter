using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Delivery
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }

        [ForeignKey("Courier")]
        public Guid? CourierId { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public DeliveryStatus Status { get; set; } = null!;
        public Order Order { get; set; } = null!;
        public Courier? Courier { get; set; }
    }
}