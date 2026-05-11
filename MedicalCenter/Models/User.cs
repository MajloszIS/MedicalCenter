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
        public string Email { get; set; }


        [StringLength(255, MinimumLength = 6)]
        public string? PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Only letters allowed")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Only letters allowed")]
        public string LastName { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public string? ProfilePicturePath { get; set; }

        public Role Role { get; set; }

        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
    }
}