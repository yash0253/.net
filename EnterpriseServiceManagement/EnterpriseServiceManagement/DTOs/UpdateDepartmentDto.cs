namespace EnterpriseServiceManagement.DTOs;

public class UpdateDepartmentDto
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}