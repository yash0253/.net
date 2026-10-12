using EnterpriseServiceManagement.Models;

namespace EnterpriseServiceManagement.Interfaces;

public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync();

    Task<Department?> GetByIdAsync(Guid id);

    Task<Department> AddAsync(Department department);

    Task UpdateAsync(Department department);

    Task DeleteAsync(Department department);

    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null);

}