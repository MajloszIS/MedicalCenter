using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IMedicalLeaveRepository
    {
        public Task AddMedicalLeaveAsync(MedicalLeave medicalLeave);
    }
}
