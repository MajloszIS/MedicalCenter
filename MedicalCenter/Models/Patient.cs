using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Pesel { get; set; }

        public DateTime BirthDate { get; set; }

        public User User { get; set; }
    }
}
