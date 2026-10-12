using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EnterpriseServiceManagement.Models;

public class Employee
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string EmployeeCode { get; set; } = string.Empty;

    public Guid DepartmentId { get; set; }

    //This stores which department the employee belongs to.
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Department Department { get; set; } = null!;
}

//Step 3 — Navigation Property

//This is another new concept:

//public Department Department { get; set; } = null!;

//This is called a navigation property.

//It allows us to navigate from:

//Employee
//   ↓
//Department

//For example, conceptually:

//employee.Department.Name

//could give:

//IT