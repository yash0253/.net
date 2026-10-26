
using EnterpriseServiceManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace EnterpriseServiceManagement.Identity;

public class AppRole : IdentityRole<Guid>
{
    public string Description { get; set; } = string.Empty;

    public ICollection<RolePermission> Permissions { get; set; }
        = new List<RolePermission>();
}
