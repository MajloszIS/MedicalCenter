using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IMedicineService
    {
        Task<List<MedicineDto>> GetAvailableMedicinesAsync();
    }
}