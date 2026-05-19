using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IReviewRepository
    {
        public Task AddReviewAsync(Review review);
        public Task<bool> ExistsAsync(Guid patientId, Guid doctorId);
    }
}
