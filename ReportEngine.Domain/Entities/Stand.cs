using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities
{
    public class Stand
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Внешний ключ для проекта
        public int ProjectInfoId { get; set; }
        [ForeignKey("ProjectInfoId")]
        public virtual ProjectInfo Project { get; set; }

        public int Number { get; set; } //Нопер ПП
        public string KKSCode { get; set; } //Код ККС
        public string Design { get; set; } //Обозначение стэнда
        
    }
}
