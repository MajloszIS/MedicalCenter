using MedicalCenter.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Invoice
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        [Required]
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public decimal Amount { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal TaxAmount { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        public string StripePaymentId { get; set; } = null!;
    }
}


