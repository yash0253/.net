using Microsoft.EntityFrameworkCore;
using VisaManagementSystem.Models;
using VIsaManagementSystem.Models;


namespace VisaManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<VisaType> VisaTypes { get; set; }
        public DbSet<VisaApplication> VisaApplications { get; set; }
        public DbSet<SystemMessage> SystemMessages { get; set; }

        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<ApplicationRemark> AppliactionRemark { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<VisaApplication>()
                .HasIndex(v => v.ReferenceNumber)
                .IsUnique();

            modelBuilder.Entity<VisaType>().HasData(
                new VisaType { Id = 1, Name = "Tourist Visa", Code = "TV", Fee = 100, ProcessingDays = 7 },
                new VisaType { Id = 2, Name = "Business Visa", Code = "BV", Fee = 150, ProcessingDays = 10 },
                new VisaType { Id = 3, Name = "Student Visa", Code = "SV", Fee = 200, ProcessingDays = 15 },
                new VisaType { Id = 4, Name = "Work Visa", Code = "WV", Fee = 300, ProcessingDays = 20 },
                new VisaType { Id = 5, Name = "Transit Visa", Code = "TRV", Fee = 50, ProcessingDays = 3 }
            );
        }
    }
}