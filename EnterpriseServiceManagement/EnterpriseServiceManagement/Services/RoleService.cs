
using EnterpriseServiceManagement.Data;
using EnterpriseServiceManagement.DTOs;
using EnterpriseServiceManagement.Helpers;
using EnterpriseServiceManagement.Identity;
using EnterpriseServiceManagement.Interfaces;
using EnterpriseServiceManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace EnterpriseServiceManagement.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ESMDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public RoleService(
        RoleManager<AppRole> roleManager,
        ESMDbContext context,
        UserManager<AppUser> userManager)
    {
        _roleManager = roleManager;
        _context = context;
        _userManager = userManager;
    }

    public async Task<RoleResponseDto> CreateAsync(CreateRoleDto dto)
    {
        var name = dto.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.");

        if (await _roleManager.RoleExistsAsync(name))
            throw new InvalidOperationException(
                $"The role '{name}' already exists.");

        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new ArgumentException("Role description is required.");

        if (dto.Permissions is null || dto.Permissions.Count == 0)
            throw new ArgumentException(
                "At least one permission is required.");

        var permissions = dto.Permissions
            .Select(p => p?.Trim() ?? string.Empty)
            .ToList();

        // Reject unknown permissions.
        var invalidPermissions = permissions
            .Where(p => !PermissionConstants.All.Contains(p))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (invalidPermissions.Count > 0)
        {
            throw new ArgumentException(
                $"Invalid permissions: {string.Join(", ", invalidPermissions)}");
        }

        // Remove duplicates, ignoring case.
        permissions = permissions
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var role = new AppRole
        {
            Name = name,
            Description = dto.Description.Trim()
        };

        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(errors);
        }

        try
        {
            foreach (var permission in permissions)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = role.Id,
                    Permission = permission
                });
            }

            await _context.SaveChangesAsync();
        }
        catch
        {
            // Avoid leaving a role behind if saving its permissions fails.
            await _roleManager.DeleteAsync(role);
            throw;
        }

        return new RoleResponseDto
        {
            Id = role.Id,
            Name = role.Name!,
            Description = role.Description,
            Permissions = permissions
        };
    }

    public async Task AssignRoleAsync (Guid userId,string roleName)
    {
        if(string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name is required.");

        var user =await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");

        }

        roleName =roleName.Trim();

        var role =await _roleManager.FindByNameAsync(roleName);

        if(role ==null)
            throw new KeyNotFoundException("Role not found");

        if (await _userManager.IsInRoleAsync(user,role.Name!))
                throw new InvalidOperationException($"User is already assigned to the role '{role.Name}'.");

        var result =await _userManager.AddToRoleAsync(user, role.Name!);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }



         

    }
}


//	RoleManager<AppRole> is a built-in ASP.NET Core Identity class that manages roles stored in your SQL Server database.
//It provides methods such as:
//Method Purpose
//CreateAsync(role) Creates a new role
//RoleExistsAsync(name)   Checks whether a role exists
//FindByNameAsync(name)   Finds a role by name
//UpdateAsync(role)   Updates a role
//DeleteAsync(role)   Deletes a role
//We don't implement RoleManager ourselves. ASP.NET Core Identity provides it.

//    Step 2: .Where(p => !PermissionConstants.All.Contains(p))
//Where() filters a collection based on a condition.
//- Contains(p) checks whether the allowed list contains permission p.
//- ! means NOT.
//- Therefore, !Contains(p) selects permissions that are not in the allowed list.
//Suppose the cleaned permissions are:
//var permissions = new List<string>
//{
//    "Users.Read",
//    "Users.Delete",
//    "Users.Hack",
//    "Unknown.Permission",
//    "Users.Hack"
//};


//    The result after Where() is:
//{
//    "Users.Hack",
//    "Unknown.Permission",
//    "Users.Hack"
//}


//The valid permissions are excluded; only invalid permissions remain.
//Step 3: .Distinct(StringComparer.OrdinalIgnoreCase)
//Distinct() removes duplicate values.
//StringComparer.OrdinalIgnoreCase tells C# to compare strings without considering uppercase and lowercase differences.
//For example:
//var items = new List<string>
//{
//    "Users.Hack",
//    "users.hack",
//    "Unknown.Permission"
//};

//    var uniqueItems = items
//        .Distinct(StringComparer.OrdinalIgnoreCase)
//        .ToList();


//    Result:
//{
//    "Users.Hack",
//    "Unknown.Permission"
//}


//"Users.Hack" and "users.hack" are treated as duplicates.
//Step 4: .ToList()
//Finally, ToList() materializes the filtered, distinct results into a list.
//Your final invalidPermissions list might look like this:
//{
//    "Users.Hack",
//    "Unknown.Permission"
//}