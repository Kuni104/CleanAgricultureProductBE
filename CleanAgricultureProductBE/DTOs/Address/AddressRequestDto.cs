namespace CleanAgricultureProductBE.DTOs.Address
{
    public class AddressRequestDto
    {
        public string? RecipientName { get; set; }
        public string? RecipientPhone { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? AddressDetail { get; set; }
        public bool? IsDefault { get; set; }
    }
}
