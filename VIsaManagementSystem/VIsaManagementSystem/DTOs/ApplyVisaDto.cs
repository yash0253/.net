using System.ComponentModel.DataAnnotations;
using VisaManagementSystem.Models;

namespace VisaManagementSystem.DTOs
{
    public class ApplyVisaDto
    {
        [Required(ErrorMessage = "Visa Type is required")]
        public int VisaTypeId { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Passport Number is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Passport Number must be between 6 and 20 characters")]
        public string PassportNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Passport Expiry Date is required")]
        public DateTime PassportExpiryDate { get; set; }

        [Required(ErrorMessage = "Intended Arrival Date is required")]
        public DateTime IntendedArrivalDate { get; set; }

        [Required(ErrorMessage = "Intended Departure Date is required")]
        public DateTime IntendedDepartureDate { get; set; }

        [Required(ErrorMessage = "Purpose of Visit is required")]
        [StringLength(500, ErrorMessage = "Purpose cannot exceed 500 characters")]
        public string PurposeOfVisit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Current Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string CurrentAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Employment Status is required")]
        [RegularExpression("^(Employed|Unemployed|Self-Employed|Student|Retired)$",
            ErrorMessage = "Invalid Employment Status")]
        public string EmploymentStatus { get; set; } = string.Empty;

        public string? EmployerName { get; set; }

        [Range(0, 10000000, ErrorMessage = "Annual Income must be between 0 and 10,000,000")]
        public decimal? AnnualIncome { get; set; }

        // Optional fields
        public string? PreviousTravelHistory { get; set; }
        public bool HasPreviousVisaRefusal { get; set; } = false;
        public string? RefusalReason { get; set; }
    }
}


