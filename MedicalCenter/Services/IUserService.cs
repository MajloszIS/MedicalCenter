using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IUserService
    {
        public Task<bool> IsUserWithThisEmailExists(string email);
        public Task<LoginResultDto> LoginAsync(LoginDto dto);
    }
}
