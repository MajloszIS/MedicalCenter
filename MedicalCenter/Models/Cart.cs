using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PatientId { get; set; }

        public Patient Patient { get; set; }

        public List<CartItem> Items { get; set; }
    }
}