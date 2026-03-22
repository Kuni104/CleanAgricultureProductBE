using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.CycleSchedule;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.CycleSchedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class CycleScheduleController(ICycleScheduleService cycleScheduleService) : ControllerBase
    {
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("cycle-schedule")]
        [SwaggerOperation(Summary = "Lấy lịch tất cả lịch xoay tua (Admin, Staff)")]
        public async Task<IActionResult> GetCycleSchedulesAdmin([FromQuery] int? page, [FromQuery] int? size)
        {
            try
            {
            var message = "";
            var result = await cycleScheduleService.GetCycleSchedulesAdmin(page, size);

            if (result == null)
            {
                message = "Không có lịch xoay tua nào";
            }
            else
            {
                message = "Lấy lịch xoay tua thành công";
            }

            return Ok(new ResponseObjectWithPagination<List<CycleScheduleResponseDto>>
            {
                Success = "true",
                Message = message,
                Data = result!.ResultObject,
                Pagination = result.Pagination
            });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string> { Success = "false", Message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("cycle-schedule/{cycleScheduleId}")]
        [SwaggerOperation(Summary = "Lấy lịch xoay tua (Admin, Staff)")]
        public async Task<IActionResult> GetCycleScheduleByIdAdmin([FromRoute] Guid cycleScheduleId)
        {
            var message = "";
            var result = await cycleScheduleService.GetCycleScheduleByIdAdmin(cycleScheduleId);

            if (result == null)
            {
                return BadRequest( new ResponseObject<CycleScheduleResponseDto>
                {
                    Success = "true",
                    Message = "Không có đơn hàng nào với id này",
                    Data = result
                });
            }
            else
            {
                message = "Lấy lịch xoay tua thành công";
            }

            return Ok(new ResponseObject<CycleScheduleResponseDto>
            {
                Success = "true",
                Message = message,
                Data = result
            });
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPatch("cycle-schedule/cancel/{cycleScheduleId}")]
        [SwaggerOperation(Summary = "Hủy giao hàng tuần hoàn (Admin, Staff)")]
        public async Task<IActionResult> CancelCycleScheduleByIdAdmin([FromRoute] Guid cycleScheduleId)
        {
            var message = "";
            var result = await cycleScheduleService.CancelCycleScheduleByIdAdmin(cycleScheduleId);

            if (result == null)
            {
                return BadRequest(new ResponseObject<CycleScheduleResponseDto>
                {
                    Success = "true",
                    Message = "Không có đơn hàng nào với id này",
                    Data = result
                });
            }
            else
            {
                message = "Hủy giao hàng tuần hoàn thành công";
            }

            return Ok(new ResponseObject<CycleScheduleResponseDto>
            {
                Success = "true",
                Message = message,
                Data = result
            });
        }


        //CUSTOMER
        [Authorize(Roles = "Customer")]
        [HttpGet("me/cycle-schedule/")]
        [SwaggerOperation(Summary = "Lấy lịch tất cả lịch xoay tua (Customer)")]
        public async Task<IActionResult> GetCycleSchedules([FromQuery] int? page, [FromQuery] int? size)
        {
            try
            {
                var accountEmail = User.FindFirstValue(ClaimTypes.Email);

                var message = "";
                var result = await cycleScheduleService.GetCycleSchedules(accountEmail!, page, size);

                if (result == null)
                {
                    message = "Không có lịch xoay tua nào";
                }
                else
                {
                    message = "Lấy lịch xoay tua thành công";
                }

                return Ok(new ResponseObjectWithPagination<List<CycleScheduleResponseDto>>
                {
                    Success = "true",
                    Message = message,
                    Data = result!.ResultObject,
                    Pagination = result.Pagination
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string> { Success = "false", Message = ex.Message });
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("me/cycle-schedule/{cycleScheduleId}")]
        [SwaggerOperation(Summary = "Lấy lịch xoay tua (Customer)")]
        public async Task<IActionResult> GetCycleScheduleById([FromRoute] Guid cycleScheduleId)
        {
            var message = "";

            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cycleScheduleService.GetCycleScheduleById(accountEmail!, cycleScheduleId);

            if (result == null)
            {
                return BadRequest(new ResponseObject<CycleScheduleResponseDto>
                {
                    Success = "true",
                    Message = "Không có đơn hàng nào với id này",
                    Data = result
                });
            }
            else
            {
                message = "Lấy lịch xoay tua thành công";
            }

            return Ok(new ResponseObject<CycleScheduleResponseDto>
            {
                Success = "true",
                Message = message,
                Data = result
            });
        }

        [Authorize(Roles = "Customer")]
        [HttpPatch("me/cycle-schedule/cancel/{cycleScheduleId}")]
        [SwaggerOperation(Summary = "Hủy giao hàng tuần hoàn (Customer)")]
        public async Task<IActionResult> CancelCycleScheduleById([FromRoute] Guid cycleScheduleId)
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var message = "";
            var result = await cycleScheduleService.CancelCycleScheduleById(accountEmail!, cycleScheduleId);

            if (result == null)
            {
                return BadRequest(new ResponseObject<CycleScheduleResponseDto>
                {
                    Success = "true",
                    Message = "Không có đơn hàng nào với id này",
                    Data = result
                });
            }
            else
            {
                message = "Hủy giao hàng tuần hoàn thành công";
            }

            return Ok(new ResponseObject<CycleScheduleResponseDto>
            {
                Success = "true",
                Message = message,
                Data = result
            });
        }
    }
}
