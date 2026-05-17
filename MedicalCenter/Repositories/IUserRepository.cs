using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAllUsersAsync();
        public Task<User> GetUserByIdAsync(Guid id);
        public Task<User> GetUserByEmailAsync(string email);
        public Task<User> GetUserByEmailWithRoleAsync(string email);
        public Task<User> GetUserByDoctorIdAsync(Guid doctorId);
        public Task<User> GetUserByPatientIdAsync(Guid patientId);
        public Task<User> GetUserByCourierIdAsync(Guid courierId);
        public Task CreateUserAsync(User user);
        public Task UpdateUserAsync(User user);
        public Task DeleteUserAsync(Guid id);
    }
}
