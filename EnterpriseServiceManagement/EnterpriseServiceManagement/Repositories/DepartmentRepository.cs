using EnterpriseServiceManagement.Data;
using EnterpriseServiceManagement.Interfaces;
using EnterpriseServiceManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseServiceManagement.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ESMDbContext _context;

        public DepartmentRepository(ESMDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(Guid id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Department> AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();

            return department;
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Department department)
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Departments
                .AnyAsync(d => d.Id == id);
        }

        public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null)
        {
            return await _context.Departments
                .AnyAsync(d =>
                d.Code == code &&
                (!excludeId.HasValue || d.Id != excludeId.Value));

        }
    }
}