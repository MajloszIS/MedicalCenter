using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class MedicalLeaveService : IMedicalLeaveService
    {
        private readonly IMedicalLeaveRepository _medicalLeaveRepository;
        public MedicalLeaveService(IMedicalLeaveRepository medicalLeaveRepository) 
        {
            _medicalLeaveRepository = medicalLeaveRepository;
        }
        public async Task CreateMedicalLeaveAsync(MedicalLeaveDto medicalLeaveDto)
        {
            if (medicalLeaveDto == null)
                throw new ArgumentNullException("Podano puste dane", nameof(medicalLeaveDto));

            if (medicalLeaveDto.DateTo <= medicalLeaveDto.DateFrom)
                throw new ArgumentException("Data końca musi być późniejsza niż data początku");

            var medicalLeave = new MedicalLeave
            {
                DoctorId = medicalLeaveDto.DoctorId,
                PatientId = medicalLeaveDto.PatientId,
                DateFrom = medicalLeaveDto.DateFrom,
                DateTo = medicalLeaveDto.DateTo,
                Reason = medicalLeaveDto.Reason,
                IssuedAt = DateTime.UtcNow
            };

            await _medicalLeaveRepository.AddMedicalLeaveAsync(medicalLeave);
        }
    }
}
