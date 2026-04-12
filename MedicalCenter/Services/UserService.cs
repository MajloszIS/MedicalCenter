using Humanizer;
using MedicalCenter.DTOs;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository) 
        { 
            _userRepository = userRepository;
        }
        public async Task<bool> IsUserWithThisEmailExists(string email)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<LoginResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetUserByEmailWithRoleAsync(dto.Email);
            if (user == null)
            {
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return null;
            }

            var result = new LoginResultDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                RoleName = user.Role.Name
            };

            return result;
        }
    }
}
