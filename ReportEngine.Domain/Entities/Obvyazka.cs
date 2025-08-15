using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class Obvyazka
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int Number { get; set; } // Номер
    public float LineLength { get; set; } // Длина линии
    public float ZraCount { get; set; } //Количество ЗРА
    public float TreeSocket { get; set; } //Тройники
    public int Sensor { get; set; } //Датчики
    public string SensorType { get; set; } //Тип датчиков
    public float Clamp { get; set; } //Хомуты
    public float WidthOnFrame { get; set; } //Длина на раме
    public int OtherLineCount { get; set; } //Колво др. линий
    public float Weight { get; set; } //Масса
    public float HumanCost { get; set; } //Трудозатраты чел/час
    public string ImageName { get; set; } //Название картинки
}