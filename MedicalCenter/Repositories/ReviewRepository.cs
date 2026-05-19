using MedicalCenter.Data;
using MedicalCenter.Models;
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
        public Task<List<Review>> GetReviewsByDoctorIdAsync(Guid doctorId)
        {
            return _context.Reviews
                .Where(r => r.DoctorId == doctorId)
                .ToListAsync();
        }
        public Task<Review?> GetReviewByIdAsync(Guid reviewId)
        {
            var review = _context.Reviews
                .Where(r => r.Id == reviewId)
                .FirstOrDefaultAsync();
            return review;
        }
        public async Task DeleteReviewAsync(Guid reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

    }
}
