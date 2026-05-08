namespace MedicalCenter.DTOs
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? NewPassword { get; set; }
        public string? ProfilePicturePath { get; set; }
    }
}
