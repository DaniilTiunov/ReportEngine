using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities.CalculationParameters;

public class CalculationParameter
{
    public int Id { get; set; }

    //уникальный текстовый ключ параметра
    public string? Key { get; set; }

    // Название параметра, например "Время сварки одного коллектора"
    public string? Name { get; set; } = null!;

    // Значение параметра, храним как decimal для числовых данных
    public string? Value { get; set; }

    // Тип единицы измерения, например "чел/ч", "шт", "руб"
    public string? Unit { get; set; } = null!;

    //Описание параметра
    public string? Description { get; set; }

    //группа параметра
    public int ParameterGroupId { get; set; }

    [ForeignKey(nameof(ParameterGroupId))]
    public virtual CalculationParameterGroup CalculationParameterGroup { get; set; }

    public int? EquipReferenceId { get; set; }

    public string? EquipReferenceType { get; set; }
}
