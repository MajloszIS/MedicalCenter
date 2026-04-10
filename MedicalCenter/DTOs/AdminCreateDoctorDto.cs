namespace MedicalCenter.DTOs
{
    public class AdminCreateDoctorDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public string LicenseNumber { get; set; }
        public string SpecializationName { get; set; }
    }
}
