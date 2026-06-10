namespace VisaManagementSystem.DTOs
{
    public class ApplicationResponseDto
    {
        public int Id { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string VisaTypeName { get; set; } = string.Empty;
        public string VisaTypeCode { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public DateTime AppliedDate { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public DateTime? SubmissionDate { get; set; }
    }
}
