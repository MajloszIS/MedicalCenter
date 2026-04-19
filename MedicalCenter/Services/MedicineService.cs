using MedicalCenter.DTOs;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _medicineRepo;

        public MedicineService(IMedicineRepository medicineRepo)
        {
            _medicineRepo = medicineRepo;
        }

        public async Task<List<MedicineDto>> GetAvailableMedicinesAsync()
        {
            var medicines = await _medicineRepo.GetAllMedicinesAsync();

            var medicineDtos = medicines.Select(m => new MedicineDto
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                CategoryName = m.Category?.Name ?? "Brak kategorii"
            }).ToList();

            return medicineDtos;
        }
    }
}