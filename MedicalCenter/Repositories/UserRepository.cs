using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return _context.Users.ToListAsync();
        }
        public Task<User> GetUserByIdAsync(Guid id)
        {
            return _context.Users.SingleOrDefaultAsync(u => u.Id == id);  
        }
        public Task CreateUserAsync(User user)
        {     
            _context.Users.Add(user);
            return _context.SaveChangesAsync();
        }
        public Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            return _context.SaveChangesAsync();
        }
        public async Task DeleteUserAsync(Guid id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
