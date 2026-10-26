
namespace EnterpriseServiceManagement.DTOs;

public class RoleResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> Permissions { get; set; } = new();
}
