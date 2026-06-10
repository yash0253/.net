namespace VisaManagementSystem.DTOs
{
    public class VisaApplicationFilterDto
    {
        public string? Status { get; set; }
        public string? SearchTerm { get; set; }
        public string? PassportNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
