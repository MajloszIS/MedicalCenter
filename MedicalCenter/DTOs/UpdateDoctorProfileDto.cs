namespace MedicalCenter.DTOs
{
    public class UpdateDoctorProfileDto : UpdateProfileDto
    {
        public string? LicenseNumber { get; set; } = string.Empty;
        public string? SpecializationName { get; set; } = string.Empty;
    }
}
