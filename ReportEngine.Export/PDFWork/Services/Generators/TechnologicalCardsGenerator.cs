using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Domain.Store;
using ReportEngine.Export.DTO;
using ReportEngine.Export.ExcelWork;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ReportEngine.Export.PDFWork.Services.Generators;

public class TechnologicalCardsGenerator : IReportGenerator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IProjectInfoRepository _projectInfoRepository;
    private readonly ParametersStore _parametersStore;


    public TechnologicalCardsGenerator(IProjectInfoRepository projectInfoRepository, ParametersStore parametersStore, IServiceProvider serviceProvider)
    {
        _projectInfoRepository = projectInfoRepository;
        _parametersStore = parametersStore;
        _serviceProvider = serviceProvider;
    }

    public ReportType Type => ReportType.TechnologicalCards;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);
        await _parametersStore.LoadSettingsDataAsync();

        var dataObject = await JsonCreator.CreateProjectJson(project, _parametersStore,null);

        //впихиваем доп опции генерации
        dataObject.ReportSettings = _serviceProvider.GetRequiredService<ReportSettings>();

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        var jsonObject = JsonSerializer.Serialize(dataObject, options);
        var jsonSavePath = DirectoryHelper.GetJsonSavePath();
        File.WriteAllText(jsonSavePath, jsonObject, Encoding.UTF8);

        var exeFilePath = DirectoryHelper.GetPythonExePath();

        var savePath = SettingsManager.GetReportDirectory();
        var fileName = ExcelReportHelper.CreateReportName("Технологические карты", "pdf");
        var fullSavePath = Path.Combine(savePath, fileName);

        var startInfo = new ProcessStartInfo
        {
            FileName = exeFilePath,
            Arguments = $"--script techcard --jsonPath \"{jsonSavePath}\" --outputFilePath \"{fullSavePath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (var process = Process.Start(startInfo))
        {
            var scriptOutput = "";

            using (var reader = process.StandardOutput)
            {
                scriptOutput = reader.ReadToEnd();
            }

            process.WaitForExit();

            var result = JsonSerializer.Deserialize<PythonScriptResult>(scriptOutput);

            var outputMessage = "";
            if (!result.Success)
            {
                outputMessage = "Возникло исключение в Python скрипте\n";
                outputMessage += "--------------------------------\n";
                outputMessage += $"Тип ошибки: {result.Error.Type}\n";
                outputMessage += $"Сообщение: {result.Error.Message}\n";
                outputMessage += $"Трассировка: {result.Error.Traceback}\n";
                throw new Exception(outputMessage);
            }

            outputMessage = "Python скрипт выполнен успешно";

            Debug.WriteLine(outputMessage);
        }
    }


    //перегрузка для выбранных стендов
    public async Task GenerateAsync(int projectId, List<Stand>? selectedStands = null)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);
        await _parametersStore.LoadSettingsDataAsync();

        var dataObject = await JsonCreator.CreateProjectJson(project, _parametersStore, selectedStands);

        //впихиваем доп опции генерации
        dataObject.ReportSettings = _serviceProvider.GetRequiredService<ReportSettings>();

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        var jsonObject = JsonSerializer.Serialize(dataObject, options);
        var jsonSavePath = DirectoryHelper.GetJsonSavePath();
        File.WriteAllText(jsonSavePath, jsonObject, Encoding.UTF8);

        var exeFilePath = DirectoryHelper.GetPythonExePath();

        var savePath = SettingsManager.GetReportDirectory();
        var fileName = ExcelReportHelper.CreateReportName("Технологические карты", "pdf");
        var fullSavePath = Path.Combine(savePath, fileName);

        var startInfo = new ProcessStartInfo
        {
            FileName = exeFilePath,
            Arguments = $"--script techcard --jsonPath \"{jsonSavePath}\" --outputFilePath \"{fullSavePath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (var process = Process.Start(startInfo))
        {
            var scriptOutput = "";

            using (var reader = process.StandardOutput)
            {
                scriptOutput = reader.ReadToEnd();
            }

            process.WaitForExit();

            var result = JsonSerializer.Deserialize<PythonScriptResult>(scriptOutput);

            var outputMessage = "";
            if (!result.Success)
            {
                outputMessage = "Возникло исключение в Python скрипте\n";
                outputMessage += "--------------------------------\n";
                outputMessage += $"Тип ошибки: {result.Error.Type}\n";
                outputMessage += $"Сообщение: {result.Error.Message}\n";
                outputMessage += $"Трассировка: {result.Error.Traceback}\n";
                throw new Exception(outputMessage);
            }

            outputMessage = "Python скрипт выполнен успешно";

            Debug.WriteLine(outputMessage);
        }
    }
}
