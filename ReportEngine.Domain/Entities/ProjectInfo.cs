namespace ReportEngine.Domain.Entities
{
    public class ProjectInfo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Company { get; set; }
        public bool isStarted { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Cost { get; set; }
    }
}
