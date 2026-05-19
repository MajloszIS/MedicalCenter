using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IReviewService
    {
        public Task AddReviewAsync(ReviewDto reviewDto);
    }
}
