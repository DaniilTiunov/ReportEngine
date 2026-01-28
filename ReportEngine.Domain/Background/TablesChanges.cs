using System.ComponentModel.DataAnnotations;

namespace ReportEngine.Domain.Background
{
    public class TablesChanges
    {
        [Key]
        public int Id { get; set; }

        public string? TableName { get; set; } = string.Empty;
        public int? EquipId { get; set; }
        public string? OldName { get; set; }
        public string? NewName { get; set; }
        public bool? Processed { get; set; }
        public DateTime? ChangedAt { get; set; }
    }
}
