using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Street { get; set; }

        [Required]
        [StringLength(10)]
        public string HouseNumber { get; set; }

        [StringLength(10)]
        public string ApartmentNumber { get; set; }

        [Required]
        [StringLength(10)]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        public User User { get; set; }
    }
}
