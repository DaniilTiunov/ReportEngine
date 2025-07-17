using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities
{
    public class ProjectInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Number { get; set; } //№п/п
        public string? Description { get; set; } //Обозначение КД
        public DateTime CreationDate { get; set; } //Дата запроса
        public string? Company { get; set; } //Заказчик
        public string? Object { get; set; } //Объект
        public int StandCount { get; set; } //Кол-во стендов
        public float Cost { get; set; } //Стоимость
        public enum Status { Started, NotStarted, Completed } //Статус
        public DateTime StartDate { get; set; } //Старта проекта
        public DateTime OutOfProduction { get; set; } //Выход из производства
        public DateTime EndDate { get; set; } //Окончание догвора
        public string? OrderCustomer { get; set; } //Заказ покупателя
        public string? RequestProduction { get; set; } //Заявка на производство
        public string? MarkPlus {  get; set; } //Маркировка +
        public string? MarkMinus { get; set; } //Маркировка -
        public bool isGalvanized { get; set; } //Оцинковка
    }
}
