namespace ReportEngine.Shared.Config.JsonHelpers
{
    public class UpdateInfo
    {
        public string Version { get; set; }
        public string Date { get; set; }
        public UpdateSections Sections { get; set; } = new();
    }

    public class UpdateSections
    {
        public List<string> Added { get; set; } = new();
        public List<string> Changed { get; set; } = new();
        public List<string> Fixed { get; set; } = new();
    }
}
