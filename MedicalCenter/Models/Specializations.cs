using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Specialization
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public List<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}