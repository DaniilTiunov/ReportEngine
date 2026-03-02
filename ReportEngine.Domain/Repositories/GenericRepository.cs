using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
using System.Linq.Expressions;

namespace ReportEngine.Domain.Repositories
{
    public class GenericRepository


    {
        private readonly ReAppContext _context;
        private readonly Dictionary<string, Type> _entityNameTypePairs;

        public GenericRepository(ReAppContext context)
        {
            _context = context;

            _entityNameTypePairs = new Dictionary<string, Type>();
            InitializePairs();
        }



        public void InitializePairs()
        {
           var typesList = new List<Type>()
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

            foreach (var type in typesList) 
            {
                _entityNameTypePairs.Add(type.Name, type);
            }

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






    }
}
