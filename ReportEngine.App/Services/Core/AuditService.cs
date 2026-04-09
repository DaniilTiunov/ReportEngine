using ReportEngine.App.AppHelpers;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Services.Core;

public class AuditService
{
    private readonly ReAppContext _context;

    public AuditService(ReAppContext context)
    {
        _context = context;
    }

    public async Task LogEventAsync(
        string userSystemName,
        string action,
        string details)
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
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
