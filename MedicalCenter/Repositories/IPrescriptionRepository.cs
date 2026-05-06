using MedicalCenter.Models;
using MedicalCenter.DTOs;   

namespace MedicalCenter.Repositories
{
    public interface IPrescriptionRepository
    {
        public Task<Prescription> GetByIdAsync(Guid id);
        public Task CreatePrescriptionAsync(Prescription prescription);
        public Task UpdatePrescriptionAsync(Prescription prescription);
        public Task DeletePrescriptionAsync(Prescription prescription);
    }
}
