namespace ReportEngine.Export.DTO
{
    public class PythonScriptResult
    {
        public bool Success { get; set; }
        public PythonError? Error { get; set; }
    }

    public class PythonError
    {
        public string? Type { get; set; }
        public string? Message { get; set; }
        public string? Traceback { get; set; }
    }
}
