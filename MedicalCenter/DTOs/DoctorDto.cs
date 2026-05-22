namespace MedicalCenter.DTOs;

public class DoctorDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Phone { get; set; }
    public string SpecializationName { get; set; }
    public List<DepartmentDto>? SelectedDepartment { get; set; }
}