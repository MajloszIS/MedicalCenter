using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class CourierService : ICourierService
    {
        private readonly ICourierRepository _courierRepository;
        private readonly IUserRepository _userRepository;

        public CourierService(ICourierRepository courierRepository, IUserRepository userRepository)
        {
            _courierRepository = courierRepository;
            _userRepository = userRepository;
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

        public async Task CreateCourierAsync(AdminCreateDto dto)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                PasswordHash = passwordHash,
                RoleId = 4
            };

            await _userRepository.CreateUserAsync(user);

            var courier = new Courier 
            {
                UserId = user.Id
            };

            await _courierRepository.CreateCourierAsync(courier);
        }
        public async Task DeleteCourierAsync(Guid CourierId)
        {
            var courier = await _courierRepository.GetCourierByIdAsync(CourierId);
            if (courier == null)
            {
                throw new Exception("Courier not found");
            }
            await _courierRepository.DeleteCourierAsync(CourierId);
        }
        public async Task<UpdateProfileDto> GetCourierProfileAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            var courier = await _courierRepository.GetCourierByUserIdAsync(id);
            if (courier == null)
            {
                throw new Exception("Courier not found");
            }
            var courierProfileDto = new UpdateProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Email = user.Email,
                ProfilePicturePath = user.ProfilePicturePath,
            };
            return courierProfileDto;
        }
        public async Task UpdateCourierProfileAsync(Guid id, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            var courier = await _courierRepository.GetCourierByUserIdAsync(id);

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Phone = dto.Phone ?? user.Phone;
            user.Email = dto.Email ?? user.Email;

            await _userRepository.UpdateUserAsync(user);
            await _courierRepository.UpdateCourierAsync(id, courier);
        }

    }
}
