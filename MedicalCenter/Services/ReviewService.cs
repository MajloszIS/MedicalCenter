using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }
        public async Task AddReviewAsync(ReviewDto reviewDto)
        {
            if (await _reviewRepository.ExistsAsync(reviewDto.PatientId, reviewDto.DoctorId))
            {
                throw new InvalidOperationException("Już wystawiłeś opinię temu lekarzowi.");
            }

            var review = new Review
            {
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                DoctorId = reviewDto.DoctorId,
                PatientId = reviewDto.PatientId,
            };
            await _reviewRepository.AddReviewAsync(review);
            
        }
    }
}
