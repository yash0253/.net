using Microsoft.AspNetCore.Mvc;
using VisaManagementSystem.DTOs;
using VisaManagementSystem.Services;

namespace VisaManagementSystem.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }



        [HttpGet("dashboardData")]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var data = await _adminService.GetDashboardStatsAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("filter-applications")]
        public async Task<IActionResult> FilterApplications([FromQuery] VisaApplicationFilterDto dto)
        {
            try
            {
                var result = await _adminService.GetFilterApplications(dto);

                return Ok(new
                {
                    Data=result
                });
            }catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPost("Update-Status")]

        public async Task<object> UpdateApplicationStatus(ApplicationStatus dto)
        {
            try
            {
                var result = await _adminService.ChangeApplicationStatus(dto);

                return Ok(new
                {
                    message = result
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
            }

        }

    };
}
