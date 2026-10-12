using EnterpriseServiceManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseServiceManagement.Data;

public class ESMDbContext : DbContext
{
    public ESMDbContext(
        DbContextOptions<ESMDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

//ModelBuilder is an EF Core object that you use to tell Entity Framework how your C# classes should be converted into database tables.