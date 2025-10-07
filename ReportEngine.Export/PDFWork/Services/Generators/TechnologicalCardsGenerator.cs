using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Generators.DTO;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using DOCXT = DocxTemplater;

namespace ReportEngine.Export.PDFWork.Services.Generators;

public class TechnologicalCardsGenerator : IReportGenerator
{
    private readonly static string _savePath = SettingsManager.GetReportDirectory();
    private readonly static string _fileName = "Технологические карты___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + ".docx";
    private readonly static string _fileNamePdf = "Технологические карты___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + ".pdf";
    private readonly static string _fullSavePath = Path.Combine(_savePath, _fileNamePdf);

    private readonly IProjectInfoRepository _projectInfoRepository;

    public TechnologicalCardsGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    public ReportType Type => ReportType.TechnologicalCards;

    public async Task GenerateAsync(int projectId)
    {
        var data = await _projectInfoRepository.GetStandsByIdAsync(projectId);

        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigurePage(page, data);
            });
        });

        document.GeneratePdf(_fullSavePath);
        document.ShowInCompanion();
    }
    private void ConfigurePage(PageDescriptor page, ProjectInfo data)
    {
        page.PageColor(Colors.White);
        page.Content().Element(c => CreateTable(c, data));
    }
    private void CreateTable(IContainer container, ProjectInfo data)
    {
        container.Table(table =>
        {
            ConfigureTableColumns(table);
            CreateTableHeader(table, data);
            FillTableRows(table);
        });
    }
    private void ConfigureTableColumns(TableDescriptor table)
    {
        table.ColumnsDefinition(columns =>
        {
            columns.ConstantColumn(50);
            columns.RelativeColumn();
            columns.ConstantColumn(125);
        });
    }
    private void CreateTableHeader(TableDescriptor table, ProjectInfo data)
    {
        table.Header(header =>
        {
            header.Cell().BorderBottom(2).Padding(8).Text("#");
            header.Cell().BorderBottom(2).Padding(8).Text(data.Stands.First().KMCH);
            header.Cell().BorderBottom(2).Padding(8).AlignRight().Text("Price");
        });
    }
    private void FillTableRows(TableDescriptor table)
    {
        foreach (var i in Enumerable.Range(0, 6))
        {
            var price = Math.Round(Random.Shared.NextDouble() * 100, 2);

            table.Cell().Padding(8).Text($"{i + 1}");
            table.Cell().Padding(8).Text(Placeholders.Label());
            table.Cell().Padding(8).AlignRight().Text($"${price}");
        }
    }
}