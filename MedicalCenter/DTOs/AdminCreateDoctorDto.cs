namespace MedicalCenter.DTOs
{
    public class AdminCreateDoctorDto : AdminCreateDto
    {
        public string LicenseNumber { get; set; }
        public string SpecializationName { get; set; }
    }
}
