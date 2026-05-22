namespace MedicalCenter.DTOs
{
    public class PatientRegisterDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Phone { get; set; }

        public required string Pesel { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
