
using EnterpriseServiceManagement.DTOs;
using EnterpriseServiceManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseServiceManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var message = await _authService.RegisterAsync(dto);

            return Ok(new
            {
                success = true,
                message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var response = await _authService.LoginAsync(dto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Invalid email or password."
            });
        }
    }
}
