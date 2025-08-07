using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities
{
    public class FormedFrame
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Description("Название рамы")]
        public string Name { get; set; }
        [Description("Тип рамы")]
        public string FrameType { get; set; } 
        [Description("Ширина рамы в м")]
        public float Width { get; set; } 
        [Description("Высота рамы")]
        public float Height { get; set; }
        [Description("Глубина рамы")]
        public float Depth { get; set; }
        [Description("Масса рамы")]
        public float Weight { get; set; }
        [Description("Обозначение по КД рамы")]
        public string Designe { get; set; }
        [Description("Рамные комплектующие")]
        public virtual ICollection<BaseFrame> BaseFrameComponents { get; set; } = new List<BaseFrame>(); // Компоненты рамы
        public virtual ICollection<BaseEquip> BaseEquipComponents { get; set; } = new List<BaseEquip>(); // Возможные компоненты рамы
        public virtual ICollection<BaseElectricComponent> BaseElectricComponents { get; set; } = new List<BaseElectricComponent>(); // Возможные компоненты рамы


    }
}
