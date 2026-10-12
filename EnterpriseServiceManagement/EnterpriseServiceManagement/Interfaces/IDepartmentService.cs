using EnterpriseServiceManagement.DTOs;

namespace EnterpriseServiceManagement.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentResponseDto>> GetAllAsync();

    Task<DepartmentResponseDto> GetByIdAsync(Guid id);

    Task<DepartmentResponseDto> CreateAsync(CreateDepartmentDto dto);

    Task UpdateAsync(Guid id, UpdateDepartmentDto dto);

    Task DeleteAsync(Guid id);
}