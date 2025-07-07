using Excel = Microsoft.Office.Interop.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Export.ExcelWork
{
    public class ExcelCreater
    {
        private readonly IBaseRepository<User> _userRepository;

        public ExcelCreater(IBaseRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateExcelFile()
        {
            Excel.Application excel = new Excel.Application
            {
                Visible = true,
                SheetsInNewWorkbook = 1
            };

            Excel.Workbook workbook = excel.Workbooks.Add(Type.Missing);

            excel.DisplayAlerts = false;

            Excel.Worksheet sheet = (Excel.Worksheet)excel.Worksheets.get_Item(1);
            //Название листа (вкладки снизу)
            sheet.Name = "Лист1";

            User user = await _userRepository.GetByIdAsync(2);
            sheet.Range["A1"].Value = user?.ToString();
        }
    }
}
