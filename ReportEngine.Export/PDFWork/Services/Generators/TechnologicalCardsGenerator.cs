using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.DTO;
using ReportEngine.Export.ExcelWork;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;



namespace ReportEngine.Export.PDFWork.Services.Generators;

public class TechnologicalCardsGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public TechnologicalCardsGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    public ReportType Type => ReportType.TechnologicalCards;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        var dataObject = JsonCreator.CreateProjectJson(project);
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        string jsonObject = JsonSerializer.Serialize(dataObject, options);
        var jsonSavePath = DirectoryHelper.GetJsonSavePath();
        File.WriteAllText(jsonSavePath, jsonObject, Encoding.UTF8);

        var exeFilePath = DirectoryHelper.GetPythonExePath();


        var savePath = SettingsManager.GetReportDirectory();
        var fileName = ExcelReportHelper.CreateReportName("Технологические карты", "pdf");
        var fullSavePath = Path.Combine(savePath, fileName);

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = exeFilePath;
        startInfo.Arguments = $"--script techcard --jsonPath \"{jsonSavePath}\" --outputFilePath \"{fullSavePath}\"";
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = true;

        using (Process process = Process.Start(startInfo))
        {
            string scriptOutput = "";

            using (StreamReader reader = process.StandardOutput)
            {
                scriptOutput = reader.ReadToEnd();
            }

            process.WaitForExit();

            var result = JsonSerializer.Deserialize<PythonScriptResult>(scriptOutput);

            string outputMessage = "";
            if (!result.Success)
            {
                outputMessage = "Возникло исключение в Python скрипте\n";
                outputMessage += "--------------------------------\n";
                outputMessage += $"Тип ошибки: {result.Error.Type}\n";
                outputMessage += $"Сообщение: {result.Error.Message}\n";
                outputMessage += $"Трассировка: {result.Error.Traceback}\n";
                throw new Exception(outputMessage);
            }
            else
            {
                outputMessage = "Python скрипт выполнен успешно";
            }

            Debug.WriteLine(outputMessage);

        }

    }
}