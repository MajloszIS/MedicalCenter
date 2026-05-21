using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IMedicalLeaveService
    {
        public Task CreateMedicalLeaveAsync(MedicalLeaveDto medicalLeaveDto);
    }
}
