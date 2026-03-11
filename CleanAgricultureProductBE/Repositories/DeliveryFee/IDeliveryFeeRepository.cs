
namespace CleanAgricultureProductBE.Repositories.DeliveryFee
{
    public interface IDeliveryFeeRepository
    {
        public Task<Models.DeliveryFee?> GetDeliveryFeeById(Guid deliveryFeeId);
        public Task<Models.DeliveryFee?> GetDeliveryFeeByWard(string ward);
        public Task<Models.DeliveryFee?> GetHighestDeliveryFee();
        public Task<bool> CheckDeliveryFee(string city, string ward, string district);
        public Task<List<Models.DeliveryFee>> GetDeliveryFeeList();
        public Task AddDeliveryFee(Models.DeliveryFee newDeliveryFee);
        public Task UpdateDeliveryFee(Models.DeliveryFee updatedDeliveryFee);
        public Task DeleteDeliveryFee(Models.DeliveryFee deliveryFeeToRemove);
    }
}
