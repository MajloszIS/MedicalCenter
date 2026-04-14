using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IMedicineRepository
    {
        Task<List<Medicine>> GetAllMedicinesAsync();
        Task<Medicine> GetByIdAsync(Guid id);
    }
}