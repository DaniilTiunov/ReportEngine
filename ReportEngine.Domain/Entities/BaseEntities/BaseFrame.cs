using ReportEngine.Domain.Entities.BaseEntities.Interface;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities.BaseEntities
{
    public class BaseFrame : IBaseEquip
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public float Cost { get; set; }
        public string Measure { get; set; }
        public int ExportDays { get; set; }
    }
}
