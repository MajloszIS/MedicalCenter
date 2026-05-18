using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Department
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        public List<DoctorDepartment> DoctorDepartments { get; set; } = new List<DoctorDepartment>();
    }
}