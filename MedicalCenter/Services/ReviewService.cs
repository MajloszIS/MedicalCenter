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
        public async Task<List<ReviewDto>> GetReviewsByDoctorIdAsync(Guid doctorId)
        {
            var reviews = await _reviewRepository.GetReviewsByDoctorIdAsync(doctorId);

            if (reviews == null || !reviews.Any())
            {
                throw new NullReferenceException("Nie znaleziono opinii dla tego lekarza.");
            }

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                DoctorId = r.DoctorId,
                PatientId = r.PatientId,
            }).ToList();
        }
        public async Task<ReviewDto> GetReviewByIdAsync(Guid reviewId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
            {
                throw new NullReferenceException("Nie znaleziono opinii o podanym ID.");
            }
            return new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                DoctorId = review.DoctorId,
                PatientId = review.PatientId,
            };
        }
        public async Task DeleteReviewAsync(Guid reviewId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
            {
                throw new NullReferenceException("Nie znaleziono opinii o podanym ID.");
            }
            await _reviewRepository.DeleteReviewAsync(reviewId);
        }
    }
}
