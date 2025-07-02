using Microsoft.Extensions.Logging;

namespace ReportEngine.Domain.Logging
{
    public  class DomainLogger
    {
        private  ILoggerFactory _loggerFactory;
        
        public  void InitLogger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
    }
}
