namespace ReportEngine.Domain.Entities;

public class CalculationParameter
{
    public int Id { get; set; }

    // Название параметра, например "Время сварки одного коллектора"
    public string? Name { get; set; } = null!;

    // Значение параметра, храним как decimal для числовых данных
    public float? Value { get; set; }

    // Тип единицы измерения, например "чел/ч", "шт", "руб"
    public string? Unit { get; set; } = null!;

    // Дополнительно: описание
    public string? Description { get; set; }
}
