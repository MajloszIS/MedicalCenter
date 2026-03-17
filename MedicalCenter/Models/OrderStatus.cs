using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class OrderStatus
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public List<Order> Orders { get; set; }
    }
}