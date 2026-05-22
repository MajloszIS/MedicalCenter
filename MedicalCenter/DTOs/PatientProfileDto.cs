namespace MedicalCenter.DTOs
{
    public class PatientProfileDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ProfilePicturePath { get; set; }
        public string? Pesel { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}