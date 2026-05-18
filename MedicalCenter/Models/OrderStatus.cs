using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class OrderStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        public List<Order> Orders { get; set; } = new();
    }
}