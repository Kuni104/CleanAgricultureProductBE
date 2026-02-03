namespace CleanAgricultureProductBE.Models
{
    public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }
        public string MethodName { get; set; } = string.Empty;
        /*------------------------------------------------------------------------------------------------------------------------*/
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
