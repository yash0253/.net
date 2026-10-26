using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Security.Principal;

namespace EnterpriseServiceManagement.Identity;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}


//IdentityUser<Guid> gives you standard identity fields such as user ID, username, email, and password hash.
//FullName stores the user's display name.
//IsActive lets your application disable a user account.