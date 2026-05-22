using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class AddressService : IAddressService
    {
        private readonly IPatientRepository _patientRepository;
        public AddressService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<AddressDto> GetAddressByPatientIdAsync(Guid patientId)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(patientId) ?? throw new Exception("Błąd");

            var addressDto = new AddressDto
            {
                Street = patient.Address?.Street,
                HouseNumber = patient.Address?.HouseNumber,
                ApartmentNumber = patient.Address?.ApartmentNumber,
                PostalCode = patient.Address?.PostalCode,
                City = patient.Address?.City
            };

            return addressDto;
        }
        public async Task UpdateAddressAsync(Guid patientId, AddressDto dto)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(patientId) ?? throw new Exception("Błąd");

            if (dto == null)
                throw new Exception("Wprowadź dane do adresu");

            if (patient.Address == null)
            {
                patient.Address = new Address
                {
                    Street = dto.Street,
                    HouseNumber = dto.HouseNumber,
                    ApartmentNumber = dto.ApartmentNumber,
                    PostalCode = dto.PostalCode,
                    City = dto.City
                };
            }
            else
            {
                patient.Address.Street = dto.Street;
                patient.Address.HouseNumber = dto.HouseNumber;
                patient.Address.ApartmentNumber = dto.ApartmentNumber;
                patient.Address.PostalCode = dto.PostalCode;
                patient.Address.City = dto.City;
            }

            await _patientRepository.UpdatePatientAsync(patient);
        }

    }
}
