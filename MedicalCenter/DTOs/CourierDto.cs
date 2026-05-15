using MedicalCenter.Models;

namespace MedicalCenter.DTOs
{
    public class CourierDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }
}
