using System.ComponentModel.DataAnnotations;
namespace MedicalCenter.DTOs
{
    public class ReviewDto
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
