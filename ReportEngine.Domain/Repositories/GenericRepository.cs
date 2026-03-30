using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Braces;
using ReportEngine.Domain.Entities.Drainage;
using ReportEngine.Domain.Entities.ElectricComponents;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Entities.Pipes;

namespace ReportEngine.Domain.Repositories;

public class GenericRepository
{
    private readonly ReAppContext _context;
    private readonly Dictionary<string, Type> _entityNameTypePairs;
    private readonly Dictionary<Type, Func<Expression, Task<IBaseEquip?>>> _handlers;

    public GenericRepository(ReAppContext context)
    {
        _context = context;

        _entityNameTypePairs = new Dictionary<string, Type>();
        InitializeEntityPairs();

        _handlers = new Dictionary<Type, Func<Expression, Task<IBaseEquip?>>>();
        InitializeHandlers();
    }

    private void InitializeEntityPairs()
    {
        var typesList = new List<Type>
        {
            typeof(CarbonPipe),
            typeof(HeaterPipe),
            typeof(StainlessPipe),
            typeof(CarbonArmature),
            typeof(HeaterArmature),
            typeof(StainlessArmature),
            typeof(CarbonSocket),
            typeof(HeaterSocket),
            typeof(StainlessSocket),
            typeof(Drainage),
            typeof(FrameDetail),
            typeof(PillarEqiup),
            typeof(FrameRoll),
            typeof(BoxesBrace),
            typeof(DrainageBrace),
            typeof(SensorBrace),
            typeof(CabelBoxe),
            typeof(CabelInput),
            typeof(CabelProduction),
            typeof(CabelProtection),
            typeof(Heater),
            typeof(Other),
            typeof(Container)
        };

        foreach (var type in typesList) _entityNameTypePairs.Add(type.Name, type);
    }

    private void InitializeHandlers()
    {
        _handlers.Add(typeof(CarbonPipe), async expr => await GetAsync((Expression<Func<CarbonPipe, bool>>)expr));
        _handlers.Add(typeof(HeaterPipe), async expr => await GetAsync((Expression<Func<HeaterPipe, bool>>)expr));
        _handlers.Add(typeof(StainlessPipe), async expr => await GetAsync((Expression<Func<StainlessPipe, bool>>)expr));
        _handlers.Add(typeof(CarbonArmature),
            async expr => await GetAsync((Expression<Func<CarbonArmature, bool>>)expr));
        _handlers.Add(typeof(HeaterArmature),
            async expr => await GetAsync((Expression<Func<HeaterArmature, bool>>)expr));
        _handlers.Add(typeof(StainlessArmature),
            async expr => await GetAsync((Expression<Func<StainlessArmature, bool>>)expr));
        _handlers.Add(typeof(CarbonSocket), async expr => await GetAsync((Expression<Func<CarbonSocket, bool>>)expr));
        _handlers.Add(typeof(HeaterSocket), async expr => await GetAsync((Expression<Func<HeaterSocket, bool>>)expr));
        _handlers.Add(typeof(StainlessSocket),
            async expr => await GetAsync((Expression<Func<StainlessSocket, bool>>)expr));
        _handlers.Add(typeof(Drainage), async expr => await GetAsync((Expression<Func<Drainage, bool>>)expr));
        _handlers.Add(typeof(FrameDetail), async expr => await GetAsync((Expression<Func<FrameDetail, bool>>)expr));
        _handlers.Add(typeof(PillarEqiup), async expr => await GetAsync((Expression<Func<PillarEqiup, bool>>)expr));
        _handlers.Add(typeof(FrameRoll), async expr => await GetAsync((Expression<Func<FrameRoll, bool>>)expr));
        _handlers.Add(typeof(BoxesBrace), async expr => await GetAsync((Expression<Func<BoxesBrace, bool>>)expr));
        _handlers.Add(typeof(DrainageBrace), async expr => await GetAsync((Expression<Func<DrainageBrace, bool>>)expr));
        _handlers.Add(typeof(SensorBrace), async expr => await GetAsync((Expression<Func<SensorBrace, bool>>)expr));
        _handlers.Add(typeof(CabelBoxe), async expr => await GetAsync((Expression<Func<CabelBoxe, bool>>)expr));
        _handlers.Add(typeof(CabelInput), async expr => await GetAsync((Expression<Func<CabelInput, bool>>)expr));
        _handlers.Add(typeof(CabelProduction),
            async expr => await GetAsync((Expression<Func<CabelProduction, bool>>)expr));
        _handlers.Add(typeof(CabelProtection),
            async expr => await GetAsync((Expression<Func<CabelProtection, bool>>)expr));
        _handlers.Add(typeof(Heater), async expr => await GetAsync((Expression<Func<Heater, bool>>)expr));
        _handlers.Add(typeof(Other), async expr => await GetAsync((Expression<Func<Other, bool>>)expr));
        _handlers.Add(typeof(Container), async expr => await GetAsync((Expression<Func<Container, bool>>)expr));
    }

    public async Task<T?> GetAsync<T>(
        Expression<Func<T, bool>> predicate)
        where T : class
    {
        return await _context.Set<T>()
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<List<T>> GetAllAsync<T>(Func<IQueryable<T>, IQueryable<T>> query)
        where T : class
    {
        return await query(_context.Set<T>().AsNoTracking())
            .ToListAsync();
    }

    public Type? GetEntityTypeByName(string name)
    {
        Type? resultType = null;

        return _entityNameTypePairs.TryGetValue(name, out resultType) ? resultType : null;
    }

    public async Task<IBaseEquip?> GetByNameAsync(Type entityType, string propertyName, object value)
    {
        if (value == null)
            return null;

        var predicate = CreateEqualsExpression(entityType, propertyName, value);

        if (_handlers.TryGetValue(entityType, out var handler)) return await handler(predicate);

        return null;
    }

    private LambdaExpression CreateEqualsExpression(Type entityType, string propertyName, object value)
    {
        var parameter = Expression.Parameter(entityType, "x");
        var property = Expression.Property(parameter, propertyName);
        var constant = Expression.Constant(value);
        var equals = Expression.Equal(property, constant);

        return Expression.Lambda(equals, parameter);
    }
}
