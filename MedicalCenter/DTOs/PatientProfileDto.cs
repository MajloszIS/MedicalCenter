namespace MedicalCenter.DTOs
{
    public class PatientProfileDto : ProfileDto
    {
        public string? Pesel { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}