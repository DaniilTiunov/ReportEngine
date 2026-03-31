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
                        Name = "Швеллер",
                        Value = "Швеллер ШП 32х16 ТУ 36.22.21.00.021-91",
                        Unit = null,
                        Description = "Швеллер по умолчанию"
                    },

                    new CalculationParameter()
                    {
                        Name = "Шильдик",
                        Value = "Шильдик",
                        Unit = null,
                        Description = "Шильдик по умолчанию"
                    },
                    new CalculationParameter()
                    {
                        Name = "Табличка",
                        Value = "Табличка",
                        Unit = null,
                        Description = "Табличка по умолчанию"
                    },
                    new CalculationParameter()
                    {
                        Name = "Оцинковка",
                        Value = "Оцинкование рамы",
                        Unit = null,
                        Description = "Оцинковка рам по умолчанию"
                    },
                    new CalculationParameter()
                    {
                        Name = "Кронштейн",
                        Value = "Кронштейн под установку разделителя сред ЭП-С.01.02.01",
                        Unit = null,
                        Description = "Кронштейн по умолчанию"
                    },
                    new CalculationParameter()
                    {
                        Name = "Кронштейн универсальный",
                        Value = "Кронштейн универсальный ЭП-П.054.0000.001",
                        Unit = null,
                        Description = "Кронштейн универсальный по умолчанию"
                    },
                    new CalculationParameter()
                    {
                        Name = "Кронштейн перепадчика",
                        Value = "Кронштейн датчика \"перепадника\" ЭП-П.040.0000.004",
                        Unit = null,
                        Description = "Кронштейн перепадчика по умолчанию"
                    },
                    new CalculationParameter()
                    {
                        Name = "Кронштейн абсолютника",
                        Value = "Кронштейн датчика \"абсолютника\" ЭП-П.050.0000.001",
                        Unit = null,
                        Description = "Кронштейн абсолютника по умолчанию"
                    },
       
                    new CalculationParameter()
                    {
                        Name = "Кабель 6 мм",
                        Value = "Провод ПуГВ  1х6 ж/з",
                        Unit = null,
                        Description = "Кабель 6 мм по умолчанию"
                    },
                    new CalculationParameter()
                    {
                        Name = "Кол-во кабеля 6 мм",
                        Value = "2",
                        Unit = "м",
                        Description = "Количество кабеля 6 мм по умолчанию"
                    },
                    new CalculationParameter()
                    {
                        Name = "Кабель 4 мм",
                        Value = "Провод ПуГВ 1х4 ж/з",
                        Unit = null,
                        Description = "Кабель 4 мм по умолчанию"
                    },
                    new CalculationParameter()
                    {
                        Name = "Сигнальный кабель",
                        Value = "Кабель МКЭШнг(А)-LS 3х0,75",
                        Unit = null,
                        Description = "Сигнальный кабель по умолчанию"
                    },
                    new CalculationParameter()
                    {
                        Name = "Хомуты",
                        Value = "Хомут U обр. оцинк. Для трубы диам. 12-17 мм (3/8'' M6)",
                        Unit = null,
                        Description = "Хомуты по умолчанию"
                    },
                    new CalculationParameter()
                    {
                        Name = "Клемма",
                        Value = "Клемма UK-2,5N  арт. 3003347",
                        Unit = null,
                        Description = "Клемма по умолчанию"
                    }

                };



                foreach (var parameter in newParameters)
                {

                   await rep.AddParameterToGroup(parameter, CalculationParameterType.Equipments);
                }

            }
            ;
        }

        
}

