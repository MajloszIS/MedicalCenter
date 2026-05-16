using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IMedicineService
    {
        Task<List<MedicineDto>> GetAllMedicineAsync();
        Task<List<MedicineDto>> GetAvailableMedicinesAsync();
    }
}