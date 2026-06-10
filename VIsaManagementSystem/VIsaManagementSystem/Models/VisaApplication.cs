using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisaManagementSystem.Models
{
    public class VisaApplication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ReferenceNumber { get; set; } = string.Empty;

        public int UserId { get; set; }

        public int VisaTypeId { get; set; }

        public string Status { get; set; } = "DRAFT";

        public string? FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? PassportNumber { get; set; }

        public DateTime? IntendedArrivalDate { get; set; }

        public DateTime? IntendedDepartureDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? SubmissionDate { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("VisaTypeId")]
        public VisaType? VisaType { get; set; }
        public string Gender { get; internal set; }
        public DateTime PassportExpiryDate { get; internal set; }
        public string PurposeOfVisit { get; internal set; }
        public string CurrentAddress { get; internal set; }
        public string EmploymentStatus { get; internal set; }
        public string PhoneNumber { get; internal set; }
        public string? EmployerName { get; internal set; }
        public decimal? AnnualIncome { get; internal set; }
    }
}
