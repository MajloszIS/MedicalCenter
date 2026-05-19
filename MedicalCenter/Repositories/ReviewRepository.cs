using MedicalCenter.Models;
using MedicalCenter.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;
        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid patientId, Guid doctorId)
        {
            return await _context.Reviews.AnyAsync(r => r.PatientId == patientId && r.DoctorId == doctorId);
        }
    }
}
