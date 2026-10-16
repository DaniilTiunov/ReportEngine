using System.Text;
using ReportEngine.Import.Models;
using ReportEngine.Import.Services;

namespace ReportApp.Benchmarks;

public static class Program
{
    private static async Task Main(string[] args)
    {
        string jsonFilePath = "";
        
        using var host = Startup.BuildHost(args, jsonFilePath);

        await host.StartAsync();
    }
    
    private static void PrintProjectHeader(ProjectInfoJsonModel projectInfo)
    {
        Console.WriteLine();
        Console.WriteLine("====== ПРОЕКТ =======", ConsoleColor.Green);
        Console.WriteLine($"  Обозначение КД : {projectInfo.DesignationKD ?? "<пусто>"}");
        Console.WriteLine($"  Объект         : {projectInfo.Object}");
        Console.WriteLine($"  Клиент         : {projectInfo.Client}");
        Console.WriteLine($"  Статус         : {projectInfo.Status}");
        Console.WriteLine($"  Дата запроса   : {projectInfo.RequestDate:yyyy-MM-dd}");
        Console.WriteLine($"  Дата отгрузки  : {(projectInfo.ShipmentDate.HasValue ? projectInfo.ShipmentDate.Value.ToString("yyyy-MM-dd") : "<нет>")}");
        Console.WriteLine($"  Стендов (план) : {projectInfo.StandsCount}");
        Console.WriteLine();
    }
    
    private static void PrintStand(StandJsonModel stand)
    {
        Console.WriteLine();
        Console.WriteLine($"====== СТЕНД В ПРОЕКТЕ С КД: {stand.Designation} =======", ConsoleColor.Green);
        Console.WriteLine($"Id: {stand.StandId}");
        Console.WriteLine($"Обозначение: {stand.Designation}");
        Console.WriteLine($"Материалов: {stand.MaterialsCount}");
        Console.WriteLine();
    }

    private static void PrintMaterials(MaterialJsonModel material, StandJsonModel stand)
    {
        Console.WriteLine();
        Console.WriteLine($"====== МАТЕРИАЛЫ В СТЕНДЕ с КД: {stand.Designation} =======", ConsoleColor.Green);
        Console.WriteLine($"Узел: {material.Node}");
        Console.WriteLine($"ПозицияУзла: {material.NodePosition}");
        Console.WriteLine($"Элемент: {material.Element}");
        Console.WriteLine($"Материал_id: {material.MaterialId}");
        Console.WriteLine($"Материал: {material.MaterialName}");
        Console.WriteLine($"Кол-во: {material.Quantity}");
        Console.WriteLine($"Ед. изм: {material.Unit}");
        Console.WriteLine($"Цена: {material.Price}");
        Console.WriteLine();
    }
}