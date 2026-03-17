using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL must be 11 digits")]
        public string Pesel { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public User User { get; set; }
    }
}