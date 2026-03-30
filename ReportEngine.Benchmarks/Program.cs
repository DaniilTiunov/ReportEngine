using BenchmarkDotNet.Running;
using DocumentFormat.OpenXml.Wordprocessing;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.CalculationParameters;
using ReportEngine.Domain.Repositories;
﻿using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;

namespace ReportEngine.Benchmarks;

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

                var newParameters = new List<CalculationParameter>()
                {
                    new CalculationParameter()
                    {
                        Name = "Стоимость пескоструйных работ",
                        Value = "900",
                        Unit = "руб/ч",
                        Description = "Стоимость пескоструйных работ"
                    },
                    new CalculationParameter()
                    {
                        Name = "Время пескоструйных работ",
                        Value = "1.2",
                        Unit = "чел*ч",
                        Description = "Время пескоструйных работ"
                    },
 
                };



                foreach (var parameter in newParameters)
                {

                   await rep.AddParameterToGroup(parameter, CalculationParameterType.SandBlastCost);
                }

            }
            ;
        }

        
}

