
using System.ComponentModel.DataAnnotations;

namespace VisaManagementSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string Role { get; set; } = "Applicant";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
