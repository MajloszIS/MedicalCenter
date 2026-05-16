namespace MedicalCenter.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
    }
}
