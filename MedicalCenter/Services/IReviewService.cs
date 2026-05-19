using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IReviewService
    {
        public Task AddReviewAsync(ReviewDto reviewDto);
        public Task<List<ReviewDto>> GetReviewsByDoctorIdAsync(Guid doctorId);
        public Task<ReviewDto> GetReviewByIdAsync(Guid reviewId);
        public Task DeleteReviewAsync(Guid reviewId);
    }
}
