namespace MedicalCenter.DTOs
{
    public class UpdatePatientProfileDto : UpdateProfileDto
    {
        public string Pesel { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
