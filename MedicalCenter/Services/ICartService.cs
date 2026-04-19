namespace MedicalCenter.Services
{
    public interface ICartService
    {
        Task AddToCartAsync(Guid patientId, Guid medicineId, int quantity);
        Task CreateOrderFromCartAsync(Guid patientId);
    }
}