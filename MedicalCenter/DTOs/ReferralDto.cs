using MedicalCenter.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.DTOs
{
    public class ReferralDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }

        [Required, MaxLength(100)]
        public string TargetSpecialization { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime IssuedDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        // zamiast encji - tylko stringi do wyświetlania
        public string PatientFullName { get; set; } = string.Empty;
        public string DoctorFullName { get; set; } = string.Empty;
    }
}
