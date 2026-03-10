using CleanAgricultureProductBE.DTOs.Address;

namespace CleanAgricultureProductBE.Services.Address
{
    public interface IAddressService
    {
        Task<AddressResponseDto> CreateAddressAsync(string accountEmail, AddressRequestDto dto);
        Task<List<AddressResponseDto>> GetAllAddressesAsync(string accountEmail);
        Task<AddressResponseDto> GetAddressByIdAsync(string accountEmail, Guid addressId);
        Task<AddressResponseDto> UpdateAddressAsync(string accountEmail, Guid addressId, AddressRequestDto dto);
        Task<bool> DeleteAddressAsync(string accountEmail, Guid addressId);
        Task<AddressResponseDto> SetDefaultAddressAsync(string accountEmail, Guid addressId);
    }
}
