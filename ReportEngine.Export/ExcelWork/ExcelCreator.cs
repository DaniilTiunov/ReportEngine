using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using Excel = Microsoft.Office.Interop.Excel;

namespace ReportEngine.Export.ExcelWork
{
    public class ExcelCreator
    {
        private readonly IBaseRepository<User> _userRepository;

        public ExcelCreator(IBaseRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }       
    }
}
