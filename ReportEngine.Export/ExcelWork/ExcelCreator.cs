using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

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
