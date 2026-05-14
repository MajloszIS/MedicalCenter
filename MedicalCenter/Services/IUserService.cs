using MedicalCenter.DTOs;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace MedicalCenter.Services
{
    public interface IUserService
    {
        public Task<bool> IsUserWithThisEmailExists(string email);
        public Task<LoginResultDto> LoginAsync(LoginDto dto);
        public Task UpdateProfilePictureAsync(Guid userId, string filePath);
        public Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
        public Task<UserWithRoleDto> GetUserByEmailWithRoleAsync(string email);
        public Task<Guid> GetUserIdByDoctorIdAsync(Guid doctorId);
        public Task<Guid> GetUserIdByPatientIdAsync(Guid patientId);

    }
}
