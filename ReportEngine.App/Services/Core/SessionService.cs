using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Services.Core;

public static class SessionService
{
    public static User? CurrentUser { get; set; }
}