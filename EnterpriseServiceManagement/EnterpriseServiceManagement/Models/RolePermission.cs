
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnterpriseServiceManagement.Identity;

namespace EnterpriseServiceManagement.Models;

public class RolePermission
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Permission { get; set; } = string.Empty;

    public Guid RoleId { get; set; }

    [ForeignKey(nameof(RoleId))] //It tells Entity Framework Core that the Role navigation property uses RoleId as its foreign key.
    public AppRole Role { get; set; } = null!;
}
