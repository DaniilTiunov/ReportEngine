namespace ReportEngine.Import.Dto;

public class ProjectInfoDto
{
    public string? Number { get; set; } //№п/п
    public string? Description { get; set; } //Обозначение КД
    public string? CreationDate { get; set; } //Дата запроса
    public string? Company { get; set; } //Заказчик
    public string? Object { get; set; } //Объект
    public string? StandCount { get; set; } //Кол-во стендов
    public string? Cost { get; set; } //Стоимость
    public string? Status { get; set; } //Статус
    public string? StartDate { get; set; } //Старта проекта
    public string? OutOfProduction { get; set; } //Выход из производства
    public string? EndDate { get; set; } //Окончание догвора
    public string? OrderCustomer { get; set; } //Заказ покупателя
    public string? RequestProduction { get; set; } //Заявка на производство
    public string? MarkPlus { get; set; } //Маркировка +
    public string? MarkMinus { get; set; } //Маркировка -
    public string? IsGalvanized { get; set; } //Оцинковка
    public string? HumanCost { get; set; } //Трудозатраты
    public string? Manager { get; set; } //Руоводитель
}