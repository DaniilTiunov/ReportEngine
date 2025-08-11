using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities
{
    public class FormedDrainage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; } 
        public string? Material { get; set; } // Просто текстовое поле для материала
        public float? Quantity { get; set; } // Количество
        public virtual ICollection<DrainagePurpose> Purposes { get; set; } = new List<DrainagePurpose>();
    }
}
