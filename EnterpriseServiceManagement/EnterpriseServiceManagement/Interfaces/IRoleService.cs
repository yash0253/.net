
using EnterpriseServiceManagement.DTOs;

namespace EnterpriseServiceManagement.Interfaces;

public interface IRoleService
{
    Task<RoleResponseDto> CreateAsync(CreateRoleDto dto);
    Task AssignRoleAsync(Guid userId, string roleName);

}
