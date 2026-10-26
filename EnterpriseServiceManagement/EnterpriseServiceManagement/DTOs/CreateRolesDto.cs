
using System.ComponentModel.DataAnnotations;

namespace EnterpriseServiceManagement.DTOs;

public class CreateRoleDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public List<string> Permissions { get; set; } = new();
}
