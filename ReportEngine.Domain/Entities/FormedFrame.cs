using ReportEngine.Domain.Entities.BaseEntities.Interface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities
{
    public class FormedFrame
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } // Название рамы
        public string FrameType { get; set; } // Тип рамы
        public float Width { get; set; } // Ширина рамы
        public float Height { get; set; } // Высота рамы
        public float Depth { get; set; } // Глубина рамы
        public float Weight { get; set; } // Масса рамы
        public string Designe { get; set; } // Обозначение по КД рамы

        public virtual ICollection<IBaseEquip> Components { get; set; } = new List<IBaseEquip>(); // Компоненты рамы
    }
}
