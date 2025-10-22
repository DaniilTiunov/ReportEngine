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
