namespace MedicalCenter.DTOs
{
    public class DoctorProfileDto : ProfileDto
    {
        public string? LicenseNumber { get; set; }
        public string? SpecializationName { get; set; }
        public List<Guid>? SelectedDepartmentIds { get; set; }
        public List<DepartmentDto>? SelectedDepartment { get; set; }
    }
}
