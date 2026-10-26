using EnterpriseServiceManagement.DTOs;
using EnterpriseServiceManagement.Exceptions;
using EnterpriseServiceManagement.Interfaces;
using EnterpriseServiceManagement.Models;

namespace EnterpriseServiceManagement.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IDepartmentRepository _departmentRepository;

    public EmployeeService(
        IEmployeeRepository repository,
        IDepartmentRepository departmentRepository)
    {
        _repository = repository;
        _departmentRepository = departmentRepository;
    }

    // Get all employees
    public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync()
    {
        var employees = await _repository.GetAllAsync();

        return employees.Select(MapToResponse);
    }

    // Get employee by ID
    public async Task<EmployeeResponseDto> GetByIdAsync(Guid id)
    {
        var employee = await _repository.GetByIdAsync(id);

        if (employee == null)
        {
            throw new NotFoundException(
                $"Employee with ID '{id}' was not found.");
        }

        return MapToResponse(employee);
    }

    // Create employee
    public async Task<EmployeeResponseDto> CreateAsync(
        CreateEmployeeDto dto)
    {
        var employeeCode = dto.EmployeeCode.Trim().ToUpperInvariant();

        // Check for duplicate employee code
        if (await _repository.ExistsByEmployeeCodeAsync(employeeCode))
        {
            throw new ConflictException(
                $"Employee code '{employeeCode}' already exists.");
        }

        // Check whether the department exists
        var department = await _departmentRepository
            .GetByIdAsync(dto.DepartmentId);

        if (department == null)
        {
            throw new NotFoundException(
                $"Department with ID '{dto.DepartmentId}' was not found.");
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            EmployeeCode = employeeCode,
            DepartmentId = dto.DepartmentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Department = department
        };

        var created = await _repository.AddAsync(employee);

        return MapToResponse(created);
    }

    // Update employee
    public async Task UpdateAsync(
        Guid id,
        UpdateEmployeeDto dto)
    {
        var employee = await _repository.GetByIdAsync(id);

        if (employee == null)
        {
            throw new NotFoundException(
                $"Employee with ID '{id}' was not found.");
        }

        var employeeCode = dto.EmployeeCode.Trim().ToUpperInvariant();

        // Check for duplicate code, excluding this employee
        if (await _repository.ExistsByEmployeeCodeAsync(employeeCode, id))
        {
            throw new ConflictException(
                $"Employee code '{employeeCode}' already exists.");
        }

        // Check whether the new department exists
        var department = await _departmentRepository
            .GetByIdAsync(dto.DepartmentId);

        if (department == null)
        {
            throw new NotFoundException(
                $"Department with ID '{dto.DepartmentId}' was not found.");
        }

        employee.FirstName = dto.FirstName.Trim();
        employee.LastName = dto.LastName.Trim();
        employee.Email = dto.Email.Trim();
        employee.EmployeeCode = employeeCode;
        employee.DepartmentId = dto.DepartmentId;
        employee.Department = department;
        employee.IsActive = dto.IsActive;

        await _repository.UpdateAsync(employee);
    }

    // Delete employee
    public async Task DeleteAsync(Guid id)
    {
        var employee = await _repository.GetByIdAsync(id);

        if (employee == null)
        {
            throw new NotFoundException(
                $"Employee with ID '{id}' was not found.");
        }

        await _repository.DeleteAsync(employee);
    }

    // Convert Employee model into EmployeeResponseDto
    private static EmployeeResponseDto MapToResponse(Employee employee)
    {
        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            EmployeeCode = employee.EmployeeCode,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department.Name,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt
        };
    }
}