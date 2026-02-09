namespace CleanAgricultureProductBE.Services.Order
{
    public interface IOrderService
    {
        public Task<string> PlaceOrder(string accountId);
    }
}
