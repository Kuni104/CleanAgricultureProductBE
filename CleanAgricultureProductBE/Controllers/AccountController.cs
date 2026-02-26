using CleanAgricultureProductBE.DTOs.Account;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1/admin/account")]
    [ApiController]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        //[Authorize(Roles = "Admin")]
        [HttpGet]
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

            var response = new ResponseObject<List<AccountResponseDto>>
            {
                Success = success,
                Message = message,
                Data = result.ResultObject,
                Pagination = result.Pagination
            };

            return Ok(response);

        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequestDto request)
        {

            var result = await accountService.CreateAccount(request);

            if (result == null)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Email đã được sử dụng!"
                });
            }

            return Ok(new ResponseObject<AccountResponseDto>
            {
                Success = "true",
                Message = "Tạo tài khoản thành công",
                Data = result
            });
        }

        [HttpPatch]
        public async Task<IActionResult> ChangeAccountStatus()
        {
            return Ok();
        }
    }
}
