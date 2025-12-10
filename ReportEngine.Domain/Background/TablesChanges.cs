using System.ComponentModel.DataAnnotations;

namespace ReportEngine.Domain.Background
{
    public class TablesChanges
    {
        [Key]
        public int Id { get; set; }

        public string TableName { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; }
    }
}
