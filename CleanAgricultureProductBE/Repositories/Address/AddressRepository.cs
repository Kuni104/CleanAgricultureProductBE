using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;
using AddressModel = CleanAgricultureProductBE.Models.Address;

namespace CleanAgricultureProductBE.Repositories.Address
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _context;

        public AddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AddressModel> CreateAsync(AddressModel address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<List<AddressModel>> GetByUserProfileIdAsync(Guid userProfileId)
        {
            return await _context.Addresses
                .Where(a => a.UserProfileId == userProfileId)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();
        }

        public async Task<AddressModel?> GetByIdAsync(Guid addressId)
        {
            return await _context.Addresses.FindAsync(addressId);
        }

        public async Task<AddressModel> UpdateAsync(AddressModel address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<bool> DeleteAsync(Guid addressId)
        {
            var address = await GetByIdAsync(addressId);
            if (address == null) return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AddressModel?> GetDefaultAddressByUserProfileIdAsync(Guid userProfileId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserProfileId == userProfileId && a.IsDefault);
        }

        public async Task UnsetDefaultAddressAsync(Guid userProfileId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserProfileId == userProfileId && a.IsDefault)
                .ToListAsync();

            foreach (var address in addresses)
            {
                address.IsDefault = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}
