using ReportEngine.App.Services.Notification;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Services.Core;

public class AuditService
{
    private readonly ReAppContext _context;
    private readonly ExceptionService _exceptionService;

    public AuditService(
        ReAppContext context,
        ExceptionService exceptionService)
    {
        _exceptionService = exceptionService;
        _context = context;
    }

    public async Task LogEventAsync(
        string userSystemName,
        string action,
        string details)
    {
        await _exceptionService.SafeExecuteAsync(async () =>
        {
            var auditEvent = new AuditEvent
            {
                Id = 0,
                Timestamp = DateTime.Now,
                UserSystemName = userSystemName,
                Action = action,
                Details = details
            };

            await _context.AuditEvents.AddAsync(auditEvent);
            await _context.SaveChangesAsync();
        });
    }
}
