
using EnterpriseServiceManagement.DTOs;
using EnterpriseServiceManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseServiceManagement.Controllers;

[ApiController]
[Route("api/roles")]
//[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleDto dto)
    {
        try
        {
            var role = await _roleService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(Create),
                new { id = role.Id },
                role);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
    [HttpPost("{roleName}/users/{userId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole(
    string roleName,
    Guid userId)
    {
        try
        {
            await _roleService.AssignRoleAsync(userId, roleName);

            return Ok(new
            {
                success = true,
                message = $"Role '{roleName}' assigned successfully."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
