using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class ObvyazkaInStand
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int StandId { get; set; }

    [ForeignKey("StandId")] public virtual Stand Stand { get; set; }

    public int ObvyazkaId { get; set; }

    [ForeignKey("ObvyazkaId")] public virtual Obvyazka Obvyazka { get; set; }

    public string? ObvyazkaName { get; set; }
    public string? MaterialLine { get; set; }
    
    public float? MaterialLineCount { get; set; }
    public string? TreeSocket { get; set; }
    public float? TreeSocketMaterialCount { get; set; }
    public string? KMCH { get; set; }
    public float? KMCHCount { get; set; }
    public string? Armature { get; set; }
    public float? ArmatureCount { get; set; }
    public int? NN { get; set; }

    public float LineLength { get; set; } // Длина линии
    public float ZraCount { get; set; } //Количество ЗРА
    public float TreeSocketCount { get; set; } //Кол-во тройников
    public int Sensor { get; set; } //Датчики
    public string SensorType { get; set; } //Тип датчиков
    public float Clamp { get; set; } //Хомуты
    public float WidthOnFrame { get; set; } //Длина на раме
    public int OtherLineCount { get; set; } //Колво др. линий
    public float Weight { get; set; } //Масса
    public float HumanCost { get; set; } //Трудозатраты чел/час
    public string ImageName { get; set; } //Название картинки

    // Датчики (до 3-х)
    public string? FirstSensorType { get; set; }
    public string? FirstSensorKKS { get; set; }
    public string? FirstSensorMarkPlus { get; set; }
    public string? FirstSensorMarkMinus { get; set; }
    public string? FirstDescription { get; set; }

    public string? SecondSensorType { get; set; }
    public string? SecondSensorKKS { get; set; }
    public string? SecondSensorMarkPlus { get; set; }
    public string? SecondSensorMarkMinus { get; set; }
    public string? SecondDescription { get; set; }

    public string? ThirdSensorType { get; set; }
    public string? ThirdSensorKKS { get; set; }
    public string? ThirdSensorMarkPlus { get; set; }
    public string? ThirdSensorMarkMinus { get; set; }
    public string? ThirdDescription { get; set; }
}