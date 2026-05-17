using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [StringLength(11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL musi mieć 11 cyfr")]
        public string? Pesel { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        public Guid? AddressId { get; set; }
        public Address? Address { get; set; } 

        public User User { get; set; } = null!;
    }
}