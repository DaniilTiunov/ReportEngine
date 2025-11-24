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

        var savePath = SettingsManager.GetReportDirectory() + "\\\\";

        var fileName = ExcelReportHelper.CreateReportName("Технологические карты", ".docx");

        var fullSavePath = Path.Combine(savePath, fileName);

        var templatePath = DirectoryHelper.GetReportsTemplatePath("TechnologicalCards_template", ".docx");



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


        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = exeFilePath; // путь к .exe файлу
        startInfo.Arguments = $"--script techcard --jsonPath \"{jsonSavePath}\" --outputReportPath \"{savePath}\"";
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;



        using (Process process = Process.Start(startInfo))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
            }

            process.WaitForExit();
        }

    }
}