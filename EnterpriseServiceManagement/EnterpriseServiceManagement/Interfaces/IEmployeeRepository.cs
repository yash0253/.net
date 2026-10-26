using EnterpriseServiceManagement.Models;

namespace EnterpriseServiceManagement.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();

    Task<Employee?> GetByIdAsync(Guid id);

    Task<Employee> AddAsync(Employee employee);

    Task UpdateAsync(Employee employee);

    Task DeleteAsync(Employee employee);

    Task<bool> ExistsAsync(Guid id);

    Task<bool> ExistsByEmployeeCodeAsync(
        string employeeCode,
        Guid? excludeId = null);

}