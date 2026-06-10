namespace VIsaManagementSystem.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int? ApplicationId { get; set; }
        public int? UserId { get; set; }
        public string Action { get; set; } = string.Empty; // STATUS_CHANGE, REMARK_ADDED, DOCUMENT_REQUESTED
        public string EntityType { get; set; } = string.Empty; // Application, User, Document
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
