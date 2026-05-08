namespace MedicalCenter.DTOs
{
    public class UpdateDoctorProfileDto : UpdateProfileDto
    {
        public string LicenseNumber { get; set; }
        public string SpecializationName { get; set; }
    }
}
