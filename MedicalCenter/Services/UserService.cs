using Humanizer;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

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

            if (user.PasswordHash == null)
            {
                return null; // konto Google - nie można logować przez formularz
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

        public async Task UpdateProfilePictureAsync(Guid userId, string pictureFilePath)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return;

            user.ProfilePicturePath = pictureFilePath;
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;
            if(!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateUserAsync(user);
            return true;
        }
        public async Task<UserWithRoleDto> GetUserByEmailWithRoleAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailWithRoleAsync(email);
            if (user == null)
            {
                return null;
            }

            var result = new UserWithRoleDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleName = user.Role.Name
            };

            return result;
        }
    }
}
