using EnterpriseServiceManagement.DTOs;
using EnterpriseServiceManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace EnterpriseServiceManagement.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employee = await _service.GetAllAsync();
        return Ok(employee);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var employee = await _service.GetByIdAsync(id);
        return Ok(employee);

    }

    [HttpPost]
    public async Task<IActionResult> Create (CreateEmployeeDto dto)
    {
        var employee = await _service.CreateAsync(dto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = employee.Id },
            employee
            );
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateEmployeeDto dto
        ){
        await _service.UpdateAsync(id, dto);
        return NoContent();

    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }


}
