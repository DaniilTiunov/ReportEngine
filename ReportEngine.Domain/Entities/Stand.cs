using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class Stand
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // Внешний ключ для проекта
    public int ProjectInfoId { get; set; }

    [ForeignKey("ProjectInfoId")] public virtual ProjectInfo Project { get; set; }

    public int Number { get; set; } //Нопер ПП
    public string? KKSCode { get; set; } //Код ККС
    public string? Design { get; set; } //Обозначение стэнда
    public int Devices { get; set; } //Приборы
    public string? BraceType { get; set; } //Тип крепления датчика
    public float Width { get; set; } //Ширна
    public string? SerialNumber { get; set; } //Серийный номер
    public float Weight { get; set; } //Масса
    public decimal StandSummCost { get; set; } //Сумма стенда
    public int ObvyazkaType { get; set; } //Тип обвязки
    public int NN { get; set; } //NN
    public string? MaterialLine { get; set; } //Материал линии
    public string? Armature { get; set; } // Араматура
    public string? TreeSocket { get; set; } // Тройник
    public string? KMCH { get; set; } // КМЧ
    public string? DesigneStand { get; set; } //Описание
    public string? Comments { get; set; } // Комментарий

    public virtual ICollection<StandFrame> StandFrames { get; set; } = new List<StandFrame>();
    public virtual ICollection<ObvyazkaInStand> ObvyazkiInStand { get; set; } = new List<ObvyazkaInStand>();
    public virtual ICollection<StandDrainage> StandDrainages { get; set; } = new List<StandDrainage>();

    public virtual ICollection<StandAdditionalEquip> StandAdditionalEquips { get; set; } =
        new List<StandAdditionalEquip>();

    public virtual ICollection<StandElectricalComponent> StandElectricalComponent { get; set; } =
        new List<StandElectricalComponent>();
}