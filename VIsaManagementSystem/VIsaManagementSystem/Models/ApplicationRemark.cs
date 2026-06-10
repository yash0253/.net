using VisaManagementSystem.Models;

public class ApplicationRemark
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public int AdminId { get; set; }
    public string Remark { get; set; } = string.Empty;
    public string RemarkType { get; set; } = "Internal"; // Internal, ToUser
    public DateTime CreatedAt { get; set; }

    public VisaApplication? Application { get; set; }
    public User? Admin { get; set; }
}