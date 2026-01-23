namespace ReportEngine.App.ViewModels.DTO
{
    public struct RenumerationInfo
    {
        public int FromNumber { get; set; }
        public int ToNumber { get; set; }
        public string Prefix { get; set; }
        public string Postfix { get; set; }
        public int? StartValue { get; set; }
        public int? Step { get; set; }
        public int StartValueLength { get; set; }
    }
}
