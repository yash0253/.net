using System.ComponentModel.DataAnnotations;

namespace VisaManagementSystem.Models
{
    public class SystemMessage
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string MessageCode { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string MessageText { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Language { get; set; } = "en";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}