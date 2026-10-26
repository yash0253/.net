using EnterpriseServiceManagement.DTOs;

namespace EnterpriseServiceManagement.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeResponseDto>> GetAllAsync();

    Task<EmployeeResponseDto> GetByIdAsync(Guid id);

    Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto);

    Task UpdateAsync(Guid id, UpdateEmployeeDto dto);

    Task DeleteAsync(Guid id);
}