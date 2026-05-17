using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Patient")]
        public Guid PatientId { get; set; }

        public Patient Patient { get; set; } = null!;

        public List<CartItem> Items { get; set; } = new();
    }
}