using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.CalculationParameters;
using ReportEngine.Domain.Repositories;

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

            var paramKeyPairs = new Dictionary<string, string>()
               {
                    {"Ведущий инженер","LeadEngineer"} ,
                    {"Ответственный за приёмку","AcceptanceSupervisor"} ,
                    {"Специалист 2 уровня","SpecialistL2"} ,
                    {"Представитель Осил","OsilRep"} ,
                    {"Стоимость пескоструйных работ","SandblastingCost"} ,
                    {"Время пескоструйных работ","SandblastingTime"} ,
                    {"Стоимость электромонтажа","ElectricalInstallCost"} ,
                    {"Время монтажа 1 провода","WireInstallTime"} ,
                    {"Время монтажа 1 кабеля","CableInstallTime"} ,
                    {"Стоимость изготовления обвязок","PipeworkFabCost"} ,
                    {"Стоимость изготовления коллектора","ManifoldFabCost"} ,
                    {"Стоимость испытаний стенда","TestBenchTestCost"} ,
                    {"Стоимость общей проверки стенда","TestBenchInspectCost"} ,
                    {"Стоимость оцинковки стенда","TestBenchGalvCost"} ,
                    {"Время проверки 1 стенда","StandInspectTime"} ,
                    {"Время заключительных работ","FinalWorkTime"} ,
                    {"Время сверления 1 отверстия","HoleDrillTime"} ,
                    {"Время сварки коллектора","CollectorWeldTime"} ,
                    {"Время проведения всех испытаний","AllTestsTime"} ,
                    {"Время подготовки всего оборудования","EquipmentPrepTime"} ,
                    {"Время сверления 1 отверстия в шине","BusHoleDrillTime"} ,
                    {"Время монтажа 1 ввода","InputInstallTime"} ,
                    {"Время других операций","OtherOpsTime"} ,
                    {"Стоимость изготовления рамы","FrameFabCost"} ,
                    {"Стоимость покраски стенда","TestBenchPaintCost"} ,
                    {"Время производства 1 рамы","FrameProdTime"} ,
                    {"Время покраски 1 рамы","FramePaintTime"} ,
                    {"Время подготовки 1 рамы","FramePrepTime"} ,
                    {"Время покраски 1 обвязки","PipeworkPaintTime"} ,
                    {"Материал №1","MaterialOne"} ,
                    {"Кол-во материала №1","MaterialOneQuantity"} ,
                    {"Материал №2","MaterialTwo"} ,
                    {"Кол-во материала №2","MaterialTwoQuantity"} ,
                    {"Стоимость изготовления 1 разборной рамы","DisassemblableFrameCost"} ,
                    {"Время изготовления 1 разборной рамы","DisassemblableFrameFabTime"} ,
                    {"Стоимость изготовления 1 стойки","StandFabCost"} ,
                    {"Время изготовления 1 стойки","StandFabTime"} ,
                    {"Стоимость работы на 1 шкаф","CabinetWorkCost"} ,
                    {"Время подготовки 1 шкафа","CabinetPrepTime"} ,
                    {"Швеллер","ChannelBar"} ,
                    {"Шильдик","Nameplate"} ,
                    {"Табличка","LabelPlate"} ,
                    {"Оцинковка","Galvanizing"} ,
                    {"Кронштейн","Bracket"} ,
                    {"Кронштейн универсальный","UniversalBracket"} ,
                    {"Кронштейн перепадчика","DiffPressureBracket"} ,
                    {"Кронштейн абсолютника","AbsPressureBracket"} ,
                    {"Кабель 6 мм","Cable6mm"} ,
                    {"Кол-во кабеля 6 мм","Cable6mmQuantity"} ,
                    {"Кабель 4 мм","Cable4mm"} ,
                    {"Сигнальный кабель","SignalCable"} ,
                    {"Хомуты","Clamps"} ,
                    {"Клемма","Terminal"} };

            var groups = await rep.GetAllGroupsAsync();
            var tempList = new List<CalculationParameterGroup?>();  

            foreach (var group in groups)
            {
                var tempGroup = await rep.GetGroupByTypeAsync(group.SettingsType);
                tempList.Add(tempGroup);    
            }

            var allParams = tempList.SelectMany(gr => gr.Parameters);



            foreach (var par in allParams)
            {
                
                par.Key = paramKeyPairs[par.Name];
            }

            foreach(var gr in tempList)
            {
               
                await rep.UpdateParametersInGroup(gr.SettingsType, gr.Parameters);
            }
            
        }
            ;
    }


}

