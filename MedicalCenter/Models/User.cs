using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalCenter.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;


        [StringLength(255)]
        public string? PasswordHash { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[\p{L}\s'-]+$", ErrorMessage = "Tylko litery, spacje, łączniki i apostrofy")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[\p{L}\s'-]+$", ErrorMessage = "Tylko litery, spacje, łączniki i apostrofy")]
        public string LastName { get; set; } = null!;

        [Phone]
        public string? Phone { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public string? ProfilePicturePath { get; set; }

        public Role Role { get; set; } = null!;

        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
        public Courier? Courier { get; set; }
    }
}