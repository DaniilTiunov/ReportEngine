namespace ReportEngine.Domain.Entities;

public class AuditEvent
{
    public int Id { get; set; } // PK
    public DateTime? Timestamp { get; set; }
    public string? UserSystemName { get; set; }
    public string? Action { get; set; } = string.Empty;
    public string? Details { get; set; }
}
