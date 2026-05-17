using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class DeliveryStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        public List<Delivery> Deliveries { get; set; } = new();
    }
}