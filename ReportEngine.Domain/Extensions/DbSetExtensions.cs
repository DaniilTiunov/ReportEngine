using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.Domain.Extensions;

public static class DbSetExtensions
{
    public static IQueryable<IBaseEquip>? SetTable(this DbContext context, Type type)
    {
        var setMethod = context.GetType().GetMethod("Set", Type.EmptyTypes);
        var genericMethod = setMethod?.MakeGenericMethod(type);
        var setResult = genericMethod?.Invoke(context, null);
        return setResult as IQueryable<IBaseEquip>;
    }
}
