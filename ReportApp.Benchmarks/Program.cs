// See https://aka.ms/new-console-template for more information
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.InkML;
using Gee.External.Capstone.X86;
using Iced.Intel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.CalculationParameters;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;
using ReportEngine.Domain.Entities.ElectricComponents;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories;
using TraceReloggerLib;


public class Program
{
    private static async Task Main(string[] args)
    {

        var optionsBuilder = new DbContextOptionsBuilder<ReAppContext>();
        optionsBuilder.UseNpgsql(
            "Host=172.16.10.230;Port=5432;Database=reportengine;Username=postgres;Password=postgres");

        var reAppContext = new ReAppContext(optionsBuilder.Options);

        using (reAppContext)
        {
            var rep = new CalculationRepository(reAppContext);
           

            //формируем список всех значений Enum
            var enumNamesInfo = Enum.GetValues(typeof(EquipReferenceType))
                .Cast<EquipReferenceType>()
                .Select(e => new 
                {
                    enumValue = e,
                    enumValueName = e.ToString()
                })
                .ToList();

            //формируем список всех Entity
            var entityNamesInfo = reAppContext.Model.GetEntityTypes()
                .Select(type => new
                {
                    entityName = type.ShortName(),
                    entity = type
                })
                .ToList();


            //объединяем коллекции по имени
            var joinResults = enumNamesInfo.Join(
                entityNamesInfo,
                k1 => k1.enumValueName,
                k2 => k2.entityName,
                (k1, k2) => new
                {
                    enumValue = k1.enumValue,
                    entityType = k2.entity.ClrType
                })
                .ToList();


            //проверяем, что не объединилось
            var mismatchResults = enumNamesInfo.ExceptBy(
                    joinResults.Select(res=>res.enumValue.ToString()),
                    e => e.enumValueName)
                .ToList();


            //словарь итоговый
            var typeEntityPairs = new Dictionary<EquipReferenceType, object?>();


            foreach (var joinResult in joinResults) 
            {
                // Получаем DbSet через рефлексию
                var dbSetMethod = reAppContext.GetType().GetMethod("Set", Type.EmptyTypes);
                var genericMethod = dbSetMethod?.MakeGenericMethod(joinResult.entityType);
                var dbSet = genericMethod?.Invoke(reAppContext, null);

                typeEntityPairs.Add(joinResult.enumValue, dbSet);
            }




            var zhopa = typeEntityPairs[EquipReferenceType.CabelBoxe];
            var sraka = zhopa as DbSet<CabelBoxe>;

 


            ;


        }
    }
}


