using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class DoctorDepartment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DoctorId { get; set; }
        public Guid DepartmentId { get; set; }

        public Doctor Doctor { get; set; }
        public Department Department { get; set; }
    }
}