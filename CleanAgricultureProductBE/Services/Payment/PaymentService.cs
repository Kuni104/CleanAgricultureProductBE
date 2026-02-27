using CleanAgricultureProductBE.Repositories.Payment;

namespace CleanAgricultureProductBE.Services.Payment
{
    public class PaymentService(IPaymentRepository paymentRepository) : IPaymentService
    {
        public Task CreatePayment()
        {
            throw new NotImplementedException();
        }
        public async Task HandlePayment(string paymentId, string transactionCode)
        {
           var payment = await paymentRepository.GetPaymentById(Guid.Parse(paymentId));
            if (payment == null)
            {
                throw new Exception($"Payment with ID {paymentId} not found.");
            }
            // Cập nhật trạng thái thanh toán thành công
            payment.PaymentStatus = "Completed";
            payment.TransactionCode = transactionCode;
            await paymentRepository.UpdatePayment(payment);
        }
    }
}
