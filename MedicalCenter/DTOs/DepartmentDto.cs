using MedicalCenter.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.DTOs
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public List<DoctorDepartment>? DoctorDepartments { get; set; }
    }
}
