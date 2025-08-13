using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities
{
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

        public string? FirstSensorType { get; set; } // Тип датчика
        public string? FirstSensorKKS { get; set; }//ККС Контура
        public string? FirstSensorMarkPlus { get; set; } //Марикровка +
        public string? FirstSensorMarkMinus { get; set; } //Марикровка -
        public string? SecondSensorType { get; set; } // Тип датчика
        public string? SecondSensorKKS { get; set; }//ККС Контура
        public string? SecondSensorMarkPlus { get; set; } //Марикровка +
        public string? SecondSensorMarkMinus { get; set; } //Марикровка -
        public string? ThirdSensorType { get; set; } // Тип датчика
        public string? ThirdSensorKKS { get; set; }//ККС Контура
        public string? ThirdSensorMarkPlus { get; set; } //Марикровка +
        public string? ThirdSensorMarkMinus { get; set; } //Марикровка -
    }
}
