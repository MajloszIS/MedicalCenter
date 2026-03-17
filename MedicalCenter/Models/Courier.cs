using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Courier
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public User User { get; set; }

        public List<Delivery> Deliveries { get; set; }
    }
}