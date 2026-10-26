
using EnterpriseServiceManagement.Identity;
using EnterpriseServiceManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseServiceManagement.Data;

public class ESMDbContext
    : IdentityDbContext<AppUser, AppRole, Guid>
{
    public ESMDbContext(
        DbContextOptions<ESMDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Identity tables
        base.OnModelCreating(modelBuilder);

        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(d => d.Code)
                .IsRequired()
                .HasMaxLength(10);

            entity.HasIndex(d => d.Code)
                .IsUnique();
        });

        // Employee configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.EmployeeCode)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(e => e.EmployeeCode)
                .IsUnique();

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Permission)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(p => new { p.RoleId, p.Permission })
                .IsUnique();

            entity.HasOne(p => p.Role)
                .WithMany(r => r.Permissions)
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}


//ModelBuilder is an EF Core object that you use to tell Entity Framework how your C# classes should be converted into database tables.
//- HasOne(e => e.Department) — each employee has one related department.
//- WithMany(d => d.Employees) — a department can have many employees.
//- HasForeignKey(e => e.DepartmentId) — the employee's DepartmentId connects it to the department.
//- IsRequired() — every employee must have a department.
//- OnDelete(DeleteBehavior.Restrict) — prevents deleting a department while employees still reference it.