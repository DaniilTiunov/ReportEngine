using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Domain.Store;
using ReportEngine.Export.DTO;
using ReportEngine.Export.ExcelWork;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.Export.PDFWork.Services.Generators;

public class PassportsGenerator : IReportGenerator
{
    private readonly ParametersStore _parametersStore;
    private readonly ProjectInfoRepository _projectInfoRepository;

    public PassportsGenerator(ProjectInfoRepository projectRepository, ParametersStore parametersStore)
    {
        _projectInfoRepository = projectRepository;
        _parametersStore = parametersStore;
    }

    public ReportType Type => ReportType.PassportsReport;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetFullProjectbyIdAsync(projectId);
        //await _parametersStore.LoadSettingsDataAsync();

        var exeFilePath = DirectoryHelper.GetPythonExePath();
        var savePath = JsonHandler.GetSaveReportDirectory(DirectoryHelper.GetConfigPath());
        var fileName = ExcelReportHelper.CreateReportName("Паспорт", "pdf");
        var fullSavePath = Path.Combine(savePath, fileName);

        var dataObject = await JsonCreator.CreateProjectJson(project, _parametersStore);
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        var jsonObject = JsonSerializer.Serialize(dataObject, options);
        var jsonSavePath = DirectoryHelper.GetJsonSavePath();
        File.WriteAllText(jsonSavePath, jsonObject, Encoding.UTF8);

        var startInfo = new ProcessStartInfo
        {
            FileName = exeFilePath,
            Arguments = $"--script passport --jsonPath \"{jsonSavePath}\" --outputFilePath \"{fullSavePath}\"",
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

    public async Task GenerateAsync(int projectId, List<Stand>? selectedStands = null)
    {
        var project = await _projectInfoRepository.GetFullProjectbyIdAsync(projectId);
        //await _parametersStore.LoadSettingsDataAsync();

        var exeFilePath = DirectoryHelper.GetPythonExePath();
        var savePath = JsonHandler.GetSaveReportDirectory(DirectoryHelper.GetConfigPath());
        var fileName = ExcelReportHelper.CreateReportName("Паспорт", "pdf");
        var fullSavePath = Path.Combine(savePath, fileName);

        var dataObject = await JsonCreator.CreateProjectJson(project, _parametersStore, selectedStands);
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        var jsonObject = JsonSerializer.Serialize(dataObject, options);
        var jsonSavePath = DirectoryHelper.GetJsonSavePath();
        File.WriteAllText(jsonSavePath, jsonObject, Encoding.UTF8);

        var startInfo = new ProcessStartInfo
        {
            FileName = exeFilePath,
            Arguments = $"--script passport --jsonPath \"{jsonSavePath}\" --outputFilePath \"{fullSavePath}\"",
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
