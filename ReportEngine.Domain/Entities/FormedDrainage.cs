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

        public virtual ICollection<DrainagePurpose> Purposes { get; set; } = new List<DrainagePurpose>();
    }
}
