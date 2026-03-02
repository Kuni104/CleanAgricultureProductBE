using CleanAgricultureProductBE.DTOs.Address;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.Address;
using AddressModel = CleanAgricultureProductBE.Models.Address;

namespace CleanAgricultureProductBE.Services.Address
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepo;
        private readonly IAccountRepository _accountRepo;

        public AddressService(IAddressRepository addressRepo, IAccountRepository accountRepo)
        {
            _addressRepo = addressRepo;
            _accountRepo = accountRepo;
        }

        public async Task<AddressResponseDto> CreateAddressAsync(string accountEmail, AddressRequestDto dto)
        {
            var userProfileId = await GetUserProfileIdByEmail(accountEmail);

            if (dto.IsDefault.HasValue && dto.IsDefault.Value)
            {
                await _addressRepo.UnsetDefaultAddressAsync(userProfileId);
            }

            var address = new AddressModel
            {
                AddressId = Guid.NewGuid(),
                UserProfileId = userProfileId,
                RecipientName = dto.RecipientName ?? string.Empty,
                RecipientPhone = dto.RecipientPhone ?? string.Empty,
                Ward = dto.Ward ?? string.Empty,
                District = dto.District ?? string.Empty,
                City = dto.City ?? string.Empty,
                AddressDetail = dto.AddressDetail ?? string.Empty,
                IsDefault = dto.IsDefault ?? false
            };

            var created = await _addressRepo.CreateAsync(address);

            return MapToDto(created);
        }

        public async Task<List<AddressResponseDto>> GetAllAddressesAsync(string accountEmail)
        {
            var userProfileId = await GetUserProfileIdByEmail(accountEmail);
            var addresses = await _addressRepo.GetByUserProfileIdAsync(userProfileId);

            return addresses.Select(MapToDto).ToList();
        }

        public async Task<AddressResponseDto> GetAddressByIdAsync(string accountEmail, Guid addressId)
        {
            var userProfileId = await GetUserProfileIdByEmail(accountEmail);
            var address = await _addressRepo.GetByIdAsync(addressId);

            if (address == null || address.UserProfileId != userProfileId)
                throw new Exception("Address not found");

            return MapToDto(address);
        }

        public async Task<AddressResponseDto> UpdateAddressAsync(string accountEmail, Guid addressId, AddressRequestDto dto)
        {
            var userProfileId = await GetUserProfileIdByEmail(accountEmail);
            var address = await _addressRepo.GetByIdAsync(addressId);

            if (address == null || address.UserProfileId != userProfileId)
                throw new Exception("Address not found");

            if (dto.IsDefault.HasValue && dto.IsDefault.Value && !address.IsDefault)
            {
                await _addressRepo.UnsetDefaultAddressAsync(userProfileId);
            }

            address.RecipientName = dto.RecipientName ?? address.RecipientName;
            address.RecipientPhone = dto.RecipientPhone ?? address.RecipientPhone;
            address.Ward = dto.Ward ?? address.Ward;
            address.District = dto.District ?? address.District;
            address.City = dto.City ?? address.City;
            address.AddressDetail = dto.AddressDetail ?? address.AddressDetail;
            if (dto.IsDefault.HasValue) address.IsDefault = dto.IsDefault.Value;

            var updated = await _addressRepo.UpdateAsync(address);

            return MapToDto(updated);
        }

        public async Task<bool> DeleteAddressAsync(string accountEmail, Guid addressId, bool confirm)
        {
            if (!confirm)
                throw new Exception("Delete confirmation required");

            var userProfileId = await GetUserProfileIdByEmail(accountEmail);
            var address = await _addressRepo.GetByIdAsync(addressId);

            if (address == null || address.UserProfileId != userProfileId)
                throw new Exception("Address not found");

            return await _addressRepo.DeleteAsync(addressId);
        }

        public async Task<AddressResponseDto> SetDefaultAddressAsync(string accountEmail, Guid addressId)
        {
            var userProfileId = await GetUserProfileIdByEmail(accountEmail);
            var address = await _addressRepo.GetByIdAsync(addressId);

            if (address == null || address.UserProfileId != userProfileId)
                throw new Exception("Address not found");

            await _addressRepo.UnsetDefaultAddressAsync(userProfileId);

            address.IsDefault = true;
            var updated = await _addressRepo.UpdateAsync(address);

            return MapToDto(updated);
        }

        private async Task<Guid> GetUserProfileIdByEmail(string email)
        {
            var account = await _accountRepo.GetByEmailAsync(email);
            if (account == null)
                throw new Exception("Account not found");

            return account.UserProfile.UserProfileId;
        }

        private AddressResponseDto MapToDto(AddressModel address)
        {
            return new AddressResponseDto
            {
                AddressId = address.AddressId,
                RecipientName = address.RecipientName,
                RecipientPhone = address.RecipientPhone,
                Ward = address.Ward,
                District = address.District,
                City = address.City,
                AddressDetail = address.AddressDetail,
                IsDefault = address.IsDefault
            };
        }
    }
}
