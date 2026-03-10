using CleanAgricultureProductBE.DTOs.Address;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.Services.Address;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1/me/addresses")]
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
        [SwaggerOperation(Summary = "Lấy danh sách địa chỉ của tôi")]
        public async Task<IActionResult> GetAllAddresses()
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var addresses = await _addressService.GetAllAddressesAsync(email);
                return Ok(new ResponseObject<List<AddressResponseDto>> { 
                    Success = "true", 
                    Message = "Lấy các địa chỉ thành công!", 
                    Data = addresses });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message,
                });
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy địa chỉ theo ID")]
        public async Task<IActionResult> GetAddressById(Guid id)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var address = await _addressService.GetAddressByIdAsync(email, id);
                return Ok(new ResponseObject<AddressResponseDto>
                {
                    Success = "true",
                    Message = "Lấy địa chỉ thành công!",
                    Data = address
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo địa chỉ mới")]
        public async Task<IActionResult> CreateAddress([FromBody] AddressRequestDto dto)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var address = await _addressService.CreateAddressAsync(email, dto);
                return Ok(new ResponseObject<AddressResponseDto>
                {
                    Success = "true",
                    Message = "Địa chỉ được tạo thành công!",
                    Data = address
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string> 
                { 
                    Success = "false",
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật địa chỉ")]
        public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] AddressRequestDto dto)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var address = await _addressService.UpdateAddressAsync(email, id, dto);
                return Ok(new ResponseObject<AddressResponseDto>
                {
                    Success = "true",
                    Message = "Địa chỉ được cập nhật thành công!",
                    Data = address
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa địa chỉ")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                await _addressService.DeleteAddressAsync(email, id);
                return Ok(new ResponseObject<string>
                {
                    Success = "true",
                    Message = "Địa chỉ đã được xóa thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string> 
                { 
                    Success = "false", 
                    Message = ex.Message 
                });
            }
        }

        [HttpPatch("{id}/set-default")]
        [SwaggerOperation(Summary = "Đặt địa chỉ mặc định")]
        public async Task<IActionResult> SetDefaultAddress(Guid id)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var address = await _addressService.SetDefaultAddressAsync(email, id);
                return Ok(new ResponseObject<AddressResponseDto>
                {
                    Success = "true",
                    Message = "Địa chỉ đã được đặt làm mặc định thành công!",
                    Data = address
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string> 
                { 
                    Success = "false",
                    Message = ex.Message 
                });
            }
        }
    }
}
