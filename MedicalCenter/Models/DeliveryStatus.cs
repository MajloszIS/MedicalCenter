using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class DeliveryStatus
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public List<Delivery> Deliveries { get; set; }
    }
}