using System.Text;
using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Store;
using ReportEngine.Export.DTO.JsonObjects;
using ReportEngine.Export.ExcelWork;

public class Program
{
    private static async Task Main(string[] args)
    {
        var conString = "Host=172.16.0.210;Port=5432;Database=reportengine;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<ReAppContext>()
            .UseNpgsql(conString)
            .Options;

        using (var context = new ReAppContext(options))
        {
            var calculationRepository = new CalculationRepository(context);
            var parameterStore = new ParametersStore(calculationRepository);
            var projectRepository = new ProjectInfoRepository(context);

            await parameterStore.LoadSettingsDataAsync();

            // Создаем генератор
            var generator = new TechnologicalCardsMarkdownGenerator(
                projectRepository,
                parameterStore);


            var projectId = 254; // Укажите ID вашего проекта

            Console.WriteLine("🚀 Генерация технологических карт в Markdown...");
            await generator.GenerateAsync(projectId);

            Console.WriteLine("✅ Готово!");
        }
    }
}


public class TechnologicalCardsMarkdownGenerator
{
    private readonly ParametersStore _parametersStore;
    private readonly ProjectInfoRepository _projectInfoRepository;

    public TechnologicalCardsMarkdownGenerator(
        ProjectInfoRepository projectInfoRepository,
        ParametersStore parametersStore)
    {
        _projectInfoRepository = projectInfoRepository;
        _parametersStore = parametersStore;
    }

    public async Task GenerateAsync(int projectId, List<Stand>? selectedStands = null)
    {
        var project = await _projectInfoRepository.GetFullProjectbyIdAsync(projectId);
        var dataObject = await JsonCreator.CreateProjectJson(project, _parametersStore, selectedStands);

        var markdown = GenerateMarkdown(dataObject);

        var fileName = $"Технологические карты_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.md";
        var savePath = Path.Combine(GetSaveDirectory(), fileName);

        await File.WriteAllTextAsync(savePath, markdown, Encoding.UTF8);
        Console.WriteLine($"✅ Технологические карты сохранены: {savePath}");
    }

    private string GenerateMarkdown(ProjectJsonObject project)
    {
        var sb = new StringBuilder();

        // Заголовок
        sb.AppendLine("# Технологические карты");
        sb.AppendLine();
        sb.AppendLine($"**Проект:** {project.Description}");
        sb.AppendLine($"**Номер проекта:** {project.Number}");
        sb.AppendLine($"**Заказчик:** {project.OrderCustomer}");
        sb.AppendLine($"**Дата:** {DateTime.Now:dd.MM.yyyy}");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // Карты для каждого стенда
        foreach (var stand in project.Stands)
        {
            sb.AppendLine(GenerateStandCard(stand, project));
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private string GenerateStandCard(StandJsonObject stand, ProjectJsonObject project)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"## Стенд датчиков КИПиА {stand.Designation}");
        sb.AppendLine();

        // Общая информация
        sb.AppendLine("### Общая информация");
        sb.AppendLine();
        sb.AppendLine("| Параметр | Значение |");
        sb.AppendLine("|----------|----------|");
        sb.AppendLine($"| **Обозначение по КД** | {stand.Designation} |");
        sb.AppendLine($"| **Код KKS** | {stand.KKSCode} |");
        sb.AppendLine($"| **Заводской номер** | {stand.SerialNumber} |");
        sb.AppendLine($"| **Размер стенда, мм** | {stand.Width} |");
        sb.AppendLine($"| **Тип обвязки** | {stand.ObvyazkaType} |");
        sb.AppendLine($"| **Материал линии** | {stand.MaterialLine} |");
        sb.AppendLine($"| **Арматура** | {stand.Armature} |");
        sb.AppendLine($"| **Тип покрытия** | {(project.IsGalvanized ? "Оцинковка" : "Покраска")} |");
        sb.AppendLine();

        // Рамы
        if (stand.Frames.Any())
        {
            sb.AppendLine("### Рамы");
            sb.AppendLine();
            sb.AppendLine("| Рама, мм | Обозначение по КД | Кол-во, шт |");
            sb.AppendLine("|----------|-------------------|------------|");
            foreach (var frame in stand.Frames)
                sb.AppendLine($"| {frame.Width} | {frame.DocName} | {frame.Quantity} |");
            sb.AppendLine();
        }

        // Основные материалы рамы
        if (stand.FrameParts.Any())
        {
            sb.AppendLine("### Основные материалы рамы");
            sb.AppendLine();
            sb.AppendLine("| Наименование | Ед. изм. | Норм. | Факт. |");
            sb.AppendLine("|--------------|----------|-------|-------|");
            foreach (var part in stand.FrameParts) sb.AppendLine($"| {part.Name} | {part.Unit} | {part.Quantity} |  |");
            sb.AppendLine();
        }

        // Монтажные части
        if (stand.MountParts.Any())
        {
            sb.AppendLine("### Комплект монтажных частей");
            sb.AppendLine();
            sb.AppendLine("| Наименование | Ед. изм. | Норм. | Факт. |");
            sb.AppendLine("|--------------|----------|-------|-------|");
            foreach (var part in stand.MountParts) sb.AppendLine($"| {part.Name} | {part.Unit} | {part.Quantity} |  |");
            sb.AppendLine();
        }

        // Дренаж
        if (stand.DrainageParts.Any())
        {
            sb.AppendLine("### Дренаж и/или продувка");
            sb.AppendLine();
            sb.AppendLine("| Наименование | Ед. изм. | Норм. | Факт. |");
            sb.AppendLine("|--------------|----------|-------|-------|");
            foreach (var part in stand.DrainageParts)
                sb.AppendLine($"| {part.Name} | {part.Unit} | {part.Quantity} |  |");
            sb.AppendLine();
        }

        // Электрические компоненты
        if (stand.ElectricParts.Any())
        {
            sb.AppendLine("### Электрические компоненты");
            sb.AppendLine();
            sb.AppendLine("| Наименование | Ед. изм. | Норм. | Факт. |");
            sb.AppendLine("|--------------|----------|-------|-------|");
            foreach (var part in stand.ElectricParts)
                sb.AppendLine($"| {part.Name} | {part.Unit} | {part.Quantity} |  |");
            sb.AppendLine();
        }

        // Импульсные линии
        if (stand.ImpulseLines.Any())
        {
            sb.AppendLine("### Импульсные линии");
            sb.AppendLine();
            sb.AppendLine("| № | Наименование и код KKS | Цепь | Маркировка | Коробка | Клеммы | Примечание |");
            sb.AppendLine("|---|------------------------|------|------------|---------|--------|------------|");

            var lineNumber = 1;
            foreach (var line in stand.ImpulseLines)
            {
                var wires = line.Wires.ToList();
                for (var i = 0; i < wires.Count; i++)
                {
                    var wire = wires[i];
                    var number = i == 0 ? lineNumber.ToString() : "";
                    var name = i == 0 ? line.Name : "";
                    var kks = i == 0 ? line.CodeKKS : "";
                    var note = i == 0 ? line.Annotation : "";

                    sb.AppendLine(
                        $"| {number} | {name}<br/>{kks} | {wire.Circuit} | {wire.Mark} | {wire.ElectricBox} | {wire.Terminal} | {note} |");
                }

                lineNumber++;
            }

            sb.AppendLine();
        }

        // Чертеж
        if (stand.ImageData != null)
        {
            sb.AppendLine("### Чертеж стенда");
            sb.AppendLine();
            sb.AppendLine($"![Чертеж стенда {stand.Designation}](data:image/png;base64,{stand.ImageData})");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private string GetSaveDirectory()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var reportPath = Path.Combine(currentDir, "Reports", "Markdown");
        Directory.CreateDirectory(reportPath);
        return reportPath;
    }
}