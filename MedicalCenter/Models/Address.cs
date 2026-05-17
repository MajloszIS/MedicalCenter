using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Ulica jest wymagana")]
        [StringLength(100)]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "Numer domu jest wymagany")]
        [StringLength(10)]
        public string HouseNumber { get; set; } = null!;

        [StringLength(10)]
        public string? ApartmentNumber { get; set; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany")]
        [StringLength(10)]
        [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Kod pocztowy w formacie XX-XXX")]
        public string PostalCode { get; set; } = null!;

        [Required(ErrorMessage = "Miasto jest wymagane")]
        [StringLength(100)]
        public string City { get; set; } = null!;
    }
}