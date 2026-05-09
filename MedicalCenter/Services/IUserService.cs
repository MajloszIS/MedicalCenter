using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IUserService
    {
        public Task<bool> IsUserWithThisEmailExists(string email);
        public Task<LoginResultDto> LoginAsync(LoginDto dto);
        public Task UpdateProfilePictureAsync(Guid userId, string filePath);
        public Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
    }
}
