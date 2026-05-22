using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.DTOs
{
    public class MedicalLeaveDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        [Required, StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        public DateTime IssuedAt { get; set; }

        public string PatientFullName { get; set; } = string.Empty;
        public string DoctorFulLName { get; set; } = string.Empty;

    }
}
