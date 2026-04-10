using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.DTOs
{
    public class PatientRegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public string Pesel { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
