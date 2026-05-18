using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IAddressService
    {
        public Task<AddressDto> GetAddressByPatientIdAsync(Guid patientId);
        public Task UpdateAddressAsync(Guid id, AddressDto dto);
    }
}
