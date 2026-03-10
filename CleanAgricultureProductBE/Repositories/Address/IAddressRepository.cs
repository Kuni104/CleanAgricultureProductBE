using AddressModel = CleanAgricultureProductBE.Models.Address;

namespace CleanAgricultureProductBE.Repositories.Address
{
    public interface IAddressRepository
    {
        Task<AddressModel> CreateAsync(AddressModel address);
        Task<List<AddressModel>> GetByUserProfileIdAsync(Guid userProfileId);
        Task<AddressModel?> GetByIdAsync(Guid addressId);
        Task<AddressModel> UpdateAsync(AddressModel address);
        Task<bool> DeleteAsync(Guid addressId);
        Task<AddressModel?> GetDefaultAddressByUserProfileIdAsync(Guid userProfileId);
        Task UnsetDefaultAddressAsync(Guid userProfileId);
        Task<AddressModel?> GetMostRecentByUserProfileIdAsync(Guid userProfileId, Guid excludeAddressId);
    }
}
