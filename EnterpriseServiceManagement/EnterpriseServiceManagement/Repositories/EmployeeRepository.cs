using EnterpriseServiceManagement.Data;
using EnterpriseServiceManagement.Interfaces;
using EnterpriseServiceManagement.Models;
using Microsoft.EntityFrameworkCore;




namespace EnterpriseServiceManagement.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ESMDbContext _context;

    public EmployeeRepository(ESMDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .Include(e => e.Department)
            .AsNoTracking()
            .ToListAsync();

    }

    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);

    }

    public async Task<Employee> AddAsync (Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        return employee;
    }

    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Employee employee)
    {
        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Employees
            .AnyAsync(e => e.Id == id);
    }

    public async Task <bool> ExistsByEmployeeCodeAsync(
        string employeeCode,
        Guid? excludeId =null
        )
    {
        return await _context.Employees
            .AnyAsync(e =>
            e.EmployeeCode == employeeCode &&
            (!excludeId.HasValue || e.Id != excludeId.Value)
            );
    }

}
