using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Export.Mapping
{
    public static class ExcelReportHelper
    {
        public static string CreateReportName(string prefix, string fileExtension)
        {
            return prefix + "___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + "." + fileExtension;
        }

       
    }
}
