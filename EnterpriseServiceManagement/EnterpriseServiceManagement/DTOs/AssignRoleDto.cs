
using System.ComponentModel.DataAnnotations;

namespace EnterpriseServiceManagement.DTOs;

public class AssignRoleDto
{
    [Required]
    public string RoleName { get; set; } = string.Empty;
}
