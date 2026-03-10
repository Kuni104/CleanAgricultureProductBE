using CleanAgricultureProductBE.DTOs.Account;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.DTOs.UserProfile;
using CleanAgricultureProductBE.Services.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.RegularExpressions;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1/admin/account")]
    [ApiController]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả tài khoản (Admin)")]
        public async Task<IActionResult> GetAllAccount([FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? keyword)
        {
            var success = "";
            var message = "";

            var result = await accountService.GetAllAccounts(page, size, keyword);

            if (result.ResultObject == null || result.ResultObject.Count == 0)
            {
                success = "true";
                message = "Không có tài khoản nào!";
            }
            else
            {
                success = "true";
                message = "Lấy tất cả tài khoản thành công!";
            }

            var response = new ResponseObjectWithPagination<List<AccountResponseDto>>
            {
                Success = success,
                Message = message,
                Data = result.ResultObject,
                Pagination = result.Pagination
            };

            return Ok(response);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [SwaggerOperation(Summary = "Tạo tài khoản mới (Role: 1.Admin | 2.Customer | 3.Staff | 4.DeliveryPerson)")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequestDto request)
        {
            if (request.PhoneNumber != null && request.PhoneNumber.Trim() != "")
            {
                var isValidPhoneNumber = Regex.IsMatch(request.PhoneNumber, @"^(?:\+84|0)\d{9}$");
                if (!isValidPhoneNumber)
                {
                    var invalidPhoneResponse = new ResponseObject<UserProfileResponseDto>
                    {
                        Success = "false",
                        Message = "Số điện thoại không hợp lệ! Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số.",
                        Data = null
                    };
                    return BadRequest(invalidPhoneResponse);
                }
            }

            var result = await accountService.CreateAccount(request);

            if (result == null)
            {
                return base.BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Email đã được sử dụng!"
                });
            }

            return base.Ok(new ResponseObject<AccountResponseDto>
            {
                Success = "true",
                Message = "Tạo tài khoản thành công",
                Data = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{accountId}")]
        [SwaggerOperation(Summary = "Thay đổi trạng thái tài khoản (Admin | Status: Active, Inactive)")]
        public async Task<IActionResult> ChangeAccountStatus([FromRoute] Guid accountId, [FromBody] ChangeAccountStatusRequestDto request)
        {
            if (request == null || request.Status.Trim() == "")
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Trạng thái không được để trống!"
                });
            }

            var result = await accountService.ChangeAccountStatus(accountId, request);
            if (result == null)
            {
                return NotFound(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Không tìm thấy tài khoản!"
                });
            }
            
            return Ok(new ResponseObject<AccountResponseDto>
            {
                Success = "true",
                Message = "Thay đổi trạng thái tài khoản thành công!",
                Data = result
            });
        }
    }
}
