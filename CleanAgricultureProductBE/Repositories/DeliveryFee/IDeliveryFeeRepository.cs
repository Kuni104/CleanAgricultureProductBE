
namespace CleanAgricultureProductBE.Repositories.DeliveryFee
{
    public interface IDeliveryFeeRepository
    {
        public Task<Models.DeliveryFee?> GetDeliveryFeeById(Guid deliveryFeeId);
        public Task<List<Models.DeliveryFee>> GetDeliveryFeeList();
        public Task AddDeliveryFee(Models.DeliveryFee newDeliveryFee);
        public Task UpdateDeliveryFee(Models.DeliveryFee updatedDeliveryFee);
        public Task DeleteDeliveryFee(Models.DeliveryFee deliveryFeeToRemove);
    }
}
