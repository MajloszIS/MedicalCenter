using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class CourierService : ICourierService
    {
        private readonly ICourierRepository _courierRepository;
        public CourierService(ICourierRepository courierRepository)
        {
            _courierRepository = courierRepository;
        }

        private async Task<CourierDto> MapToCourierDtoAsync(Courier courier)
        {
            var courierDto = new CourierDto
            {
                Id = courier.Id,
                FirstName = courier.User.FirstName,
                LastName = courier.User.LastName,
                Phone = courier.User.Phone ?? string.Empty,
            };
            return courierDto;
        }

        public async Task<List<CourierDto>> GetAllCourierAsync()
        {
            var couriers = await _courierRepository.GetAllCourierAsync(); 
            var courierDtos = new List<CourierDto>();
            foreach (var courier in couriers)
            {
                courierDtos.Add(await MapToCourierDtoAsync(courier));
            }
            return courierDtos;
        }   
        public async Task<CourierDto> GetCourierByIdAsync(Guid id)
        {
            var courier = await _courierRepository.GetCourierByIdAsync(id);   
            return await MapToCourierDtoAsync(courier);
        }
        public async Task<CourierDto> GetCourierByUserIdAsync(Guid userId)
        {
            var courier = await _courierRepository.GetCourierByUserIdAsync(userId);
            return await MapToCourierDtoAsync(courier);
        }

        /*
        public Task CreateCourierAsync(AdminCreateCourierDto dto)
        {

        }
        public Task DeleteCourierAsync(Guid CourierId)
        {

        }
        public Task<UpdateProfileDto> GetCourierProfileAsync(Guid id)
        {

        }
        public Task UpdateCourierProfileAsync(Guid id, UpdateProfileDto dto
        {

        }

        */
    }
}
