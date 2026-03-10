using CleanAgricultureProductBE.DTOs.Address;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.Address;
using System.Text.RegularExpressions;
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

            ValidateAddressDto(dto, isCreate: true);

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
                throw new Exception("Không tìm thấy địa chỉ");

            return MapToDto(address);
        }

        public async Task<AddressResponseDto> UpdateAddressAsync(string accountEmail, Guid addressId, AddressRequestDto dto)
        {
            var userProfileId = await GetUserProfileIdByEmail(accountEmail);
            var address = await _addressRepo.GetByIdAsync(addressId);

            if (address == null || address.UserProfileId != userProfileId)
                throw new Exception("Không tìm thấy địa chỉ");

            ValidateAddressDto(dto, isCreate: false);

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

        public async Task<bool> DeleteAddressAsync(string accountEmail, Guid addressId)
        {
            var userProfileId = await GetUserProfileIdByEmail(accountEmail);
            var address = await _addressRepo.GetByIdAsync(addressId);

            if (address == null || address.UserProfileId != userProfileId)
                throw new Exception("Không tìm thấy địa chỉ");

            var wasDefault = address.IsDefault;

            var result = await _addressRepo.DeleteAsync(addressId);

            if (wasDefault && result)
            {
                var mostRecent = await _addressRepo.GetMostRecentByUserProfileIdAsync(userProfileId, addressId);
                if (mostRecent != null)
                {
                    mostRecent.IsDefault = true;
                    await _addressRepo.UpdateAsync(mostRecent);
                }
            }

            return result;
        }

        public async Task<AddressResponseDto> SetDefaultAddressAsync(string accountEmail, Guid addressId)
        {
            var userProfileId = await GetUserProfileIdByEmail(accountEmail);
            var address = await _addressRepo.GetByIdAsync(addressId);

            if (address == null || address.UserProfileId != userProfileId)
                throw new Exception("Không tìm thấy địa chỉ");

            await _addressRepo.UnsetDefaultAddressAsync(userProfileId);

            address.IsDefault = true;
            var updated = await _addressRepo.UpdateAsync(address);

            return MapToDto(updated);
        }

        private async Task<Guid> GetUserProfileIdByEmail(string email)
        {
            var account = await _accountRepo.GetByEmailAsync(email);
            if (account == null)
                throw new Exception("Không tìm thấy tài khoản");

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

        private static void ValidateAddressDto(AddressRequestDto dto, bool isCreate)
        {
            if (isCreate)
            {
                if (string.IsNullOrWhiteSpace(dto.RecipientName))
                    throw new Exception("Tên người nhận không được để trống");

                if (string.IsNullOrWhiteSpace(dto.RecipientPhone))
                    throw new Exception("Số điện thoại người nhận không được để trống");

                if (string.IsNullOrWhiteSpace(dto.City))
                    throw new Exception("Thành phố không được để trống");

                if (string.IsNullOrWhiteSpace(dto.District))
                    throw new Exception("Quận/Huyện không được để trống");

                if (string.IsNullOrWhiteSpace(dto.Ward))
                    throw new Exception("Phường/Xã không được để trống");

                if (string.IsNullOrWhiteSpace(dto.AddressDetail))
                    throw new Exception("Chi tiết địa chỉ không được để trống");
            }

            if (!string.IsNullOrWhiteSpace(dto.RecipientName) && dto.RecipientName.Trim().Length > 100)
                throw new Exception("Tên người nhận không được vượt quá 100 ký tự");

            if (!string.IsNullOrWhiteSpace(dto.RecipientPhone))
            {
                var phoneRegex = new Regex(@"^(0|\+84)\d{9}$");
                if (!phoneRegex.IsMatch(dto.RecipientPhone.Trim()))
                    throw new Exception("Số điện thoại không hợp lệ. Phải bắt đầu bằng 0 hoặc +84 và có 10 chữ số");
            }

            if (!string.IsNullOrWhiteSpace(dto.AddressDetail) && dto.AddressDetail.Trim().Length > 500)
                throw new Exception("Chi tiết địa chỉ không được vượt quá 500 ký tự");
        }
    }
}
