using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string LicenseNumber { get; set; }

        public int SpecializationId { get; set; }

        public User User { get; set; }
    }
}
