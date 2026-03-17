using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<User> Users { get; set; }
    }
}