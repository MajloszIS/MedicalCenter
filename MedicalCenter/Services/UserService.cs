using Humanizer;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MedicalCenter.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IConfiguration configuration) 
        { 
            _userRepository = userRepository;
            _configuration = configuration;
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

        public async Task<Guid> GetUserIdByDoctorIdAsync(Guid doctorId)
        {
            var user = await _userRepository.GetUserByDoctorIdAsync(doctorId);
            if (user == null)
            {
                return Guid.Empty;
            }
            return user.Id;
        }
        public async Task<Guid> GetUserIdByPatientIdAsync(Guid patientId)
        {
            var user = await _userRepository.GetUserByPatientIdAsync(patientId);
            if (user == null)
            {
                return Guid.Empty;
            }
            return user.Id;
        }
        public async Task<Guid> GetUserIdByCourierIdAsync(Guid courierId)
        {
            var user = await _userRepository.GetUserByCourierIdAsync(courierId);
            if (user == null)
            {
                return Guid.Empty;
            }
            return user.Id;
        }
        public LoginResponseDto GenerateJwtToken(LoginResultDto user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.RoleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDto
            {
                Token = tokenString,
                ExpiresAt = expires,
                FullName = user.FullName,
                RoleName = user.RoleName
            };
        }
        public async Task<UpdateProfileDto> GetUserProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            var userProfile = new UpdateProfileDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            return userProfile;
        }
        public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return;
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Phone = dto.Phone ?? user.Phone;
            user.Email = dto.Email ?? user.Email;
            await _userRepository.UpdateUserAsync(user);
        }
    }
}
