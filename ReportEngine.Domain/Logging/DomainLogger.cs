using Microsoft.Extensions.Logging;

namespace ReportEngine.Domain.Logging
{
    public static class DomainLogger
    {
        private static ILoggerFactory _loggerFactory;
        
        public static void InitLogger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
    }
}
