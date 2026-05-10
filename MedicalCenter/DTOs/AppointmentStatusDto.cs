using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.DTOs
{
    public class AppointmentStatusDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
