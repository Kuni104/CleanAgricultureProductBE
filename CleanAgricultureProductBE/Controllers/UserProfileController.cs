using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.DTOs.UserProfile;
using CleanAgricultureProductBE.Services.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/me/profile")]
    [ApiController]
    public class UserProfileController(IUserProfileService userProfileService) : ControllerBase
    {
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var success = "";
            var message = "";
            var result = await userProfileService.GetUserProfile(accountEmail!);
            if (result == null)
            {
                success = "false";
                message = "Không tìm thấy thông tin người dùng!";
            }
            else
            {
                success = "true";
                message = "Lấy thông tin người dùng thành công!";
            }
            var response = new ResponseObject<UserProfileResponseDto>
            {
                Success = success,
                Message = message,
                Data = result
            };
            return Ok(response);
        }

        [Authorize (Roles = "Customer")]
        [HttpPatch]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileRequestDto request)
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var success = "";
            var message = "";
            var result = await userProfileService.UpdateUserProfile(accountEmail!, request);
            if (result == null)
            {
                success = "false";
                message = "Không tìm thấy thông tin người dùng!";
            }
            else
            {
                success = "true";
                message = "Lấy thông tin người dùng thành công!";
            }
            var response = new ResponseObject<UserProfileResponseDto>
            {
                Success = success,
                Message = message,
                Data = result
            };
            return Ok(response);
        }
    }
}
