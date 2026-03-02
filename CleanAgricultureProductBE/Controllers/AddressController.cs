using CleanAgricultureProductBE.DTOs.Address;
using CleanAgricultureProductBE.Services.Address;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/me/addresses")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var addresses = await _addressService.GetAllAddressesAsync(email);
                return Ok(new { success = "true", message = "Get addresses successfully", data = addresses });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(Guid id)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var address = await _addressService.GetAddressByIdAsync(email, id);
                return Ok(new { success = "true", message = "Get address successfully", data = address });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] AddressRequestDto dto)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var address = await _addressService.CreateAddressAsync(email, dto);
                return Ok(new { success = "true", message = "Address created successfully", data = address });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] AddressRequestDto dto)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var address = await _addressService.UpdateAddressAsync(email, id, dto);
                return Ok(new { success = "true", message = "Address updated successfully", data = address });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(Guid id, [FromQuery] bool confirm = false)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                await _addressService.DeleteAddressAsync(email, id, confirm);
                return Ok(new { success = "true", message = "Address deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/set-default")]
        public async Task<IActionResult> SetDefaultAddress(Guid id)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var address = await _addressService.SetDefaultAddressAsync(email, id);
                return Ok(new { success = "true", message = "Default address set successfully", data = address });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
