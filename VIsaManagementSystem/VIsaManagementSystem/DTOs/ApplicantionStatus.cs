namespace VisaManagementSystem.DTOs
{
    public class ApplicationStatus
    {
        public required int ApplicationId { get; set; }
        public required string Status { get; set; }

        public string? Reason { get; set; }


    }
}
