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


        static async Task Main(string[] args)
        {

            var optionsBuilder = new DbContextOptionsBuilder<ReAppContext>();
            optionsBuilder.UseNpgsql(
                "Host=172.16.0.210;Port=5432;Database=reportengine;Username=postgres;Password=postgres");

        var reAppContext = new ReAppContext(optionsBuilder.Options);

            using (reAppContext)
            {
                var rep = new CalculationRepository(reAppContext);

                var newParameters = new List<CalculationParameter>()
                {
                    new CalculationParameter()
                    {
                        Name = "Ведущий инженер",
                        Value = "С.А. Кокшаров",
                        Unit = null,
                        Description = "Ведущий инженер"
                    },
                    new CalculationParameter()
                    {
                        Name = "Ответственный за приёмку",
                        Value = "Д.А. Ефремов",
                        Unit = null,
                        Description = "Ответственный за приёмку"
                    },
                    new CalculationParameter()
                    {
                        Name = "Специалист 2 уровня",
                        Value = "Д.А. Ефремов",
                        Unit = null,
                        Description = "Специалист 2 уровня"
                    },
                    new CalculationParameter()
                    {
                        Name = "Представитель Осил",
                        Value = "М.Е. Радченко",
                        Unit = null,
                        Description = "Представитель Осил",                      
                    },
                };


                var group = new CalculationParameterGroup()
                {
                    SettingsType = CalculationParameterType.StandCost
                };

                foreach (var parameter in newParameters)
                {

                   await rep.AddParameterToGroup(parameter, group);
                }

            }
            ;
        }

        ;
    }
}
