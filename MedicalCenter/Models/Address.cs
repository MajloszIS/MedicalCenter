using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [StringLength(100)]
        public string? Street { get; set; }

        [StringLength(10)]
        public string? HouseNumber { get; set; }

        [StringLength(10)]
        public string? ApartmentNumber { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Kod pocztowy w formacie XX-XXX")]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? City { get; set; }
    }
}