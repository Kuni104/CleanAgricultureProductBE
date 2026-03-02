namespace CleanAgricultureProductBE.DTOs.Address
{
    public class AddressResponseDto
    {
        public Guid AddressId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientPhone { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
