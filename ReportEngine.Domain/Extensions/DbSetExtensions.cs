using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;

namespace ReportEngine.Domain.Extensions;

public static class DbSetExtensions
{
    public static IQueryable<IBaseEquip> SetTable(this DbContext context, EquipReferenceType type)
    {
        var propertyName = type.ToString() + "s";
        var property = context.GetType().GetProperty(propertyName);
        return property.GetValue(context) as IQueryable<IBaseEquip>;
    }
}
