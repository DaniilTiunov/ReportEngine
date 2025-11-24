using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;



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

        var dataObject = ExcelReportHelper.CreateProjectJson(project);
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        string jsonObject = JsonSerializer.Serialize(dataObject, options);
        var jsonFileName = DirectoryHelper.GetGeneratedJsonPath();
        File.WriteAllText(jsonFileName, jsonObject, Encoding.UTF8);

        var exeFilePath = DirectoryHelper.GetPythonExePath();
        var jsonSavePath = DirectoryHelper.GetJsonSavePath();

        var savePath = SettingsManager.GetReportDirectory();
        var fileName = ExcelReportHelper.CreateReportName("Технологические карты", "pdf");
        var fullSavePath = Path.Combine(savePath, fileName);


        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = exeFilePath;
        startInfo.Arguments = $"--script techcard --jsonPath \"{jsonSavePath}\" --outputFilePath \"{fullSavePath}\"";
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = false;


        using (Process process = Process.Start(startInfo))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                Debug.Print("Результат выполнения скрипта:" + result);
            }

            process.WaitForExit();
        }

    }
}