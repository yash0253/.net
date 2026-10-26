using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EnterpriseServiceManagement.Models;

public class Department
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public ICollection<Employee> Employees { get; set; }
       = new List<Employee>();
}

//A collection/list of employees belonging to this department.
//and the users is not StoredProcedure in table columns is not createad just we can acess using department.employee we can acess 