
namespace CleanAgricultureProductBE.Repositories.DeliveryFee
{
    public interface IDeliveryFeeRepository
    {
        public Task<List<Models.DeliveryFee>> GetDeliveryFeeList();
        Task AddDeliveryFee(Models.DeliveryFee newDeliveryFee);
    }
}
