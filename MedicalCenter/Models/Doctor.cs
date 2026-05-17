using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Doctor
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Numer licencji jest wymagany")]
        [StringLength(20, ErrorMessage = "Numer licencji może mieć maksymalnie 20 znaków")]
        public string LicenseNumber { get; set; } = null!;
        public Guid SpecializationId { get; set; }

        public Specialization Specialization { get; set; } = null!;
        public List<DoctorDepartment> DoctorDepartments { get; set; } = new();
        public User User { get; set; } = null!;
    }
}