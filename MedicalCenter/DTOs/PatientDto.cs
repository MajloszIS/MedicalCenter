namespace MedicalCenter.DTOs
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Phone { get; set; }
        public string? Pesel { get; set; }
    }
}
