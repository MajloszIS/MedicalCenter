using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAllUsersAsync();
        public Task<User> GetUserByIdAsync(Guid id);
        public Task<User> GetUserByEmailAsync(string email);
        public Task<User> GetUserByEmailWithRoleAsync(string email);
        public Task CreateUserAsync(User user);
        public Task UpdateUserAsync(User user);
        public Task DeleteUserAsync(Guid id);
    }
}
