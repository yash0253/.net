
namespace EnterpriseServiceManagement.Helpers;

public static class PermissionConstants
{
    public static class Employees
    {
        public const string Read = "Employees.Read";
        public const string ReadSelf = "Employees.ReadSelf";
        public const string Create = "Employees.Create";
        public const string Update = "Employees.Update";
        public const string Delete = "Employees.Delete";
    }

    public static class Departments
    {
        public const string Read = "Departments.Read";
        public const string Create = "Departments.Create";
        public const string Update = "Departments.Update";
        public const string Delete = "Departments.Delete";
    }

    public static class Roles
    {
        public const string Read = "Roles.Read";
        public const string Manage = "Roles.Manage";
    }

    public static readonly IReadOnlySet<string> All =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Employee permissions
            Employees.Read,
            Employees.ReadSelf,
            Employees.Create,
            Employees.Update,
            Employees.Delete,

            // Department permissions
            Departments.Read,
            Departments.Create,
            Departments.Update,
            Departments.Delete,

            // Role permissions
            Roles.Read,
            Roles.Manage
        };
}


//Its value is "Employees.Read".
//But if a client sends "Employees.DeleteAll", that permission isn't in PermissionConstants.All, so the API should reject it.