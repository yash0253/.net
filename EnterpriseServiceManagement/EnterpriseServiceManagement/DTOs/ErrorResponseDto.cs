namespace EnterpriseServiceManagement.DTOs;

public class ErrorResponseDto
{
    public bool Success { get; set; } = false;

    public string Message { get; set; } = string.Empty;

    public Dictionary<string, string[]>? Errors { get; set; }

    public string TraceId { get; set; } = string.Empty;
}