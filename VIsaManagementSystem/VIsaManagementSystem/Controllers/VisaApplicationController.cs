using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VisaManagementSystem.DTOs;
using VisaManagementSystem.Enums;
using VisaManagementSystem.Services;

namespace VisaManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VisaApplicationController : ControllerBase
    {
        private readonly IVisaApplicationService _visaService;
        private readonly IMessageService _messageService;
        private readonly ILogger<VisaApplicationController> _logger;

        public VisaApplicationController(
            IVisaApplicationService visaService,
            IMessageService messageService,
            ILogger<VisaApplicationController> logger)
        {
            _visaService = visaService;
            _messageService = messageService;
            _logger = logger;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForVisa([FromBody] ApplyVisaDto applyVisaDto)
        {
            try
            {
                var userId = GetUserIdFromToken();

                if (userId == null)
                    return Unauthorized("User not authorized");

                var result = await _visaService.ApplyForVisaAsync(
                    applyVisaDto,
                    userId.Value);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying for visa");

                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("my-applications")]
        public async Task<IActionResult> GetMyApplications()
        {
            try
            {
                var userId = GetUserIdFromToken();

                if (userId == null)
                    return Unauthorized("User not authorized");

                var result = await _visaService.GetUserApplicationsAsync(
                    userId.Value);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting applications");

                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("application/{referenceNumber}")]
        public async Task<IActionResult> GetApplicationByReference(
            string referenceNumber)
        {
            try
            {
                var userId = GetUserIdFromToken();

                if (userId == null)
                    return Unauthorized("User not authorized");

                var role = GetUserRoleFromToken();

                var result =
                    await _visaService.GetApplicationByReferenceAsync(
                        referenceNumber,
                        userId.Value,
                        role);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting application");

                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPost("submit/{applicationId}")]
        public async Task<IActionResult> SubmitApplication(int applicationId)
        {
            try
            {
                var userId = GetUserIdFromToken();

                if (userId == null)
                    return Unauthorized("User not authorized");

                var result =
                    await _visaService.SubmitApplicationAsync(
                        applicationId,
                        userId.Value);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting application");

                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        private int? GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                return null;

            return int.Parse(claim.Value);
        }

        private string GetUserRoleFromToken()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value
                   ?? "Applicant";
        }
    }
}