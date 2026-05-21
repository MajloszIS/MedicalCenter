using MedicalCenter.Data;
using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public class MedicalLeaveRepository : IMedicalLeaveRepository
    {
        private readonly AppDbContext _context;
        public MedicalLeaveRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddMedicalLeaveAsync(MedicalLeave medicalLeave)
        {
            await _context.MedicalLeaves.AddAsync(medicalLeave);
            await _context.SaveChangesAsync();   
        }
    }
}
