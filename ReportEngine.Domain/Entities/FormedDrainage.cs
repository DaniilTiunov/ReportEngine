using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities
{
    public class FormedDrainage
    {
        [Key]
        public int Id { get; set; }
        public int StandId { get; set; }
        [ForeignKey("StandId")]
        public virtual Stand Stand { get; set; }

        public string Purpose { get; set; } // Назначение: "Клапан", "Труба" и т.д.
        public string Material { get; set; } // Просто текстовое поле для материала
        public float Quantity { get; set; } // Количество, например 0.6
    }
}
