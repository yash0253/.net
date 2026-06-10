using System.ComponentModel.DataAnnotations;

namespace VisaManagementSystem.Models
{
    public class VisaType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Fee { get; set; }

        public int ProcessingDays { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
