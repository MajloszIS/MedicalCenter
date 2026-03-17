using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Doctor
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string LicenseNumber { get; set; }

        [Required]
        public Guid SpecializationId { get; set; }
        public Specialization Specialization { get; set; }

        public List<DoctorDepartment> DoctorDepartments { get; set; }

        public User User { get; set; }
    }
}