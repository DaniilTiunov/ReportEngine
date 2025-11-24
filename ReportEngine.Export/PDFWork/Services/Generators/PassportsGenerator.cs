using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Diagnostics;




namespace ReportEngine.Export.PDFWork.Services.Generators;

public class PassportsGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public PassportsGenerator(IProjectInfoRepository projectRepository)
    {
        _projectInfoRepository = projectRepository;
    }

    public ReportType Type => ReportType.PassportsReport;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        var exeFilePath = DirectoryHelper.GetPythonExePath();
        var jsonSavePath = DirectoryHelper.GetJsonSavePath();

        var savePath = SettingsManager.GetReportDirectory();
        var fileName = ExcelReportHelper.CreateReportName("Паспорт", "pdf");
        var fullSavePath = Path.Combine(savePath, fileName);

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = exeFilePath;
        startInfo.Arguments = $"--script passport --jsonPath \"{jsonSavePath}\" --outputFilePath \"{fullSavePath}\"";
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