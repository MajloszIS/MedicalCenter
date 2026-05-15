using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface ICourierRepository
    {
        public Task<List<Courier>> GetAllCourierAsync();
        public Task<Courier> GetCourierByIdAsync(Guid id);
        public Task<Courier> GetCourierByUserIdAsync(Guid userId);
        public Task CreateCourierAsync(Courier courier);
        public Task UpdateCourierAsync(Guid id, Courier courier);
        public Task DeleteCourierAsync(Guid CourierId);
    }
}
