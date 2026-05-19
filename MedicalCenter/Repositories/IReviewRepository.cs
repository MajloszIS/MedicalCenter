using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IReviewRepository
    {
        public Task AddReviewAsync(Review review);
        public Task<bool> ExistsAsync(Guid patientId, Guid doctorId);
        public Task<List<Review>> GetReviewsByDoctorIdAsync(Guid doctorId);
        public Task<Review?> GetReviewByIdAsync(Guid reviewId);
        public Task DeleteReviewAsync(Guid reviewId);

    }
}
