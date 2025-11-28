using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Export.DTO
{
    public class ProjectJsonObject
    {
        public string? SeniorEngineer {  get; set; } //ведущий инженер
        public string? ResponsibleForAccept { get; set; } //ответственный за приемку
        public string? SecondLevelSpecialist { get; set; } //специалист второго уровня
        public string? OSiL { get; set; } //представитель ОСиЛ
        public int Id { get; set; }
        public int Number { get; set; } //№п/п
        public string? Description { get; set; } //Обозначение КД
        public DateOnly CreationDate { get; set; } //Дата запроса
        public string? Company { get; set; } //Заказчик
        public string? Object { get; set; } //Объект
        public int StandCount { get; set; } //Кол-во стендов
        public decimal Cost { get; set; } //Стоимость
        public ProjectStatus Status { get; set; } //Статус
        public DateOnly StartDate { get; set; } //Старта проекта
        public DateOnly OutOfProduction { get; set; } //Выход из производства
        public DateOnly EndDate { get; set; } //Окончание догвора
        public string? OrderCustomer { get; set; } //Заказ покупателя
        public string? RequestProduction { get; set; } //Заявка на производство
        public string? MarkPlus { get; set; } //Маркировка +
        public string? MarkMinus { get; set; } //Маркировка -
        public bool IsGalvanized { get; set; } //Оцинковка
        public float HumanCost { get; set; } //Трудозатраты
        public string? Manager { get; set; } //Руоводитель

        public ICollection<StandJsonObject> Stands { get; set; } = new List<StandJsonObject>();
    }
}
