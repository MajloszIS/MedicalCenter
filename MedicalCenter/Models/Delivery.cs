using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Delivery
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }
        public Guid? CourierId { get; set; }

        public Guid StatusId { get; set; }
        public DeliveryStatus Status { get; set; }

        public Order Order { get; set; }
        public Courier Courier { get; set; }
    }
}