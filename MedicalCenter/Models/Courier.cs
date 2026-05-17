using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Courier
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [StringLength(10, ErrorMessage = "Numer rejestracyjny może mieć maksymalnie 10 znaków")]
        public string? VehicleRegistration { get; set; }

        public User User { get; set; } = null!;

        public List<Delivery> Deliveries { get; set; } = new();
    }
}