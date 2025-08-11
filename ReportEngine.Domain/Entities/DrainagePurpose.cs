using System.ComponentModel.DataAnnotations;

namespace ReportEngine.Domain.Entities
{
    public class DrainagePurpose
    {
        [Key]
        public int Id { get; set; }
        public string Purpose { get; set; } // Например, "Клапан", "Труба"

        // Внешний ключ на дренаж
        public int FormedDrainageId { get; set; }
        public virtual FormedDrainage FormedDrainage { get; set; }
    }
}
