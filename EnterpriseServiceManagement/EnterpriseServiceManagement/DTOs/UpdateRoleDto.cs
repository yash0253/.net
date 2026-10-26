
using System.ComponentModel.DataAnnotations;

namespace EnterpriseServiceManagement.DTOs;

public class UpdateRoleDto
{
    [Required]
    [StringLength(250)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public List<string> Permissions { get; set; } = new();
}
