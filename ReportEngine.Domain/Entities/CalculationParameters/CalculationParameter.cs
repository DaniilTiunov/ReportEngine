using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.Domain.Entities.CalculationParameters;

public class CalculationParameter
{
    public int Id { get; set; }

    // Название параметра, например "Время сварки одного коллектора"
    public string? Name { get; set; } = null!;

    // Значение параметра, храним как decimal для числовых данных
    public string? Value { get; set; }

    // Тип единицы измерения, например "чел/ч", "шт", "руб"
    public string? Unit { get; set; } = null!;

    // Дополнительно: описание
    public string? Description { get; set; }

    
    //группа параметра 
    public int ParameterGroupId { get; set; } 

    public CalculationParameterGroup CalculationParameterGroup { get; set; }
}
