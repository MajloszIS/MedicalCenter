using Humanizer;
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
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User> GetUserByEmailWithRoleAsync(string email)
        {
            return await _context.Users.Include(u => u.Role).Include(u => u.Patient).FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User> GetUserByDoctorIdAsync(Guid doctorId)
        {
            var user = await _context.Users.Include(u => u.Doctor).FirstOrDefaultAsync(u => u.Doctor.Id == doctorId);
            return user;
        }
        public async Task<User> GetUserByPatientIdAsync(Guid patientId)
        {
            var user = await _context.Users.Include(u => u.Patient).FirstOrDefaultAsync(u => u.Patient.Id == patientId);
            return user;
        }
        public async Task<User> GetUserByCourierIdAsync(Guid courierId)
        {
            var user = await _context.Users.Include(u => u.Courier).FirstOrDefaultAsync(u => u.Courier.Id == courierId);
            return user;
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
