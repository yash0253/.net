using EnterpriseServiceManagement.DTOs;
using EnterpriseServiceManagement.Exceptions;
using EnterpriseServiceManagement.Interfaces;
using EnterpriseServiceManagement.Models;

namespace EnterpriseServiceManagement.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;

    public DepartmentService(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DepartmentResponseDto>> GetAllAsync()
    {
        var departments = await _repository.GetAllAsync();

        return departments.Select(MapToResponse);
    }

    public async Task<DepartmentResponseDto> GetByIdAsync(Guid id)
    {
        var department = await _repository.GetByIdAsync(id);

        if (department == null)
            throw new NotFoundException(
                $"Department with ID '{id}' was not found.");

        return MapToResponse(department);
    }

    public async Task<DepartmentResponseDto> CreateAsync(
        CreateDepartmentDto dto)
    {
        var code = dto.Code.Trim().ToUpperInvariant();

        var codeExists = await _repository.ExistsByCodeAsync(code);

        if (codeExists)
        {
            throw new ConflictException(
                $"Department with code '{code}' already exists.");
        }

        var department = new Department
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Code = code,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(department);

        return MapToResponse(created);
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateDepartmentDto dto)
    {
        var department = await _repository.GetByIdAsync(id);

        if (department == null)
        {
            throw new NotFoundException(
                $"Department with ID '{id}' was not found.");
        }

        var code = dto.Code.Trim().ToUpperInvariant();

        var codeExists = await _repository.ExistsByCodeAsync(
            code,
            id);

        if (codeExists)
        {
            throw new ConflictException(
                $"Department with code '{code}' already exists.");
        }

        department.Name = dto.Name.Trim();
        department.Code = code;
        department.IsActive = dto.IsActive;

        await _repository.UpdateAsync(department);
    }

    public async Task DeleteAsync(Guid id)
    {
        var department = await _repository.GetByIdAsync(id);

        if (department == null)
        {
            throw new NotFoundException(
                $"Department with ID '{id}' was not found.");
        }

        await _repository.DeleteAsync(department);
    }

    private static DepartmentResponseDto MapToResponse(
        Department department)
    {
        return new DepartmentResponseDto
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            IsActive = department.IsActive,
            CreatedAt = department.CreatedAt
        };
    }
}