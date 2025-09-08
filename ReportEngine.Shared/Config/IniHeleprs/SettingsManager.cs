using IniParser;
using IniParser.Model;
using ReportEngine.Shared.Config.Directory;

namespace ReportEngine.Shared.Config.IniHeleprs;

public class SettingsManager
{
    private static readonly string _iniFile = DirectoryHelper.GetIniConfigPath();
    private static readonly FileIniDataParser _parser = new();
    private static IniData _iniData;

    public static string GetReportDirectory()
    {
        _iniData = _parser.ReadFile(_iniFile);
        return _iniData["SaveReportDirectory"]["savePath"];
    }

    public static void SetReportDirectory(string newPath)
    {
        _iniData = _parser.ReadFile(_iniFile);
        _iniData["SaveReportDirectory"]["savePath"] = newPath;
        _parser.WriteFile(_iniFile, _iniData);
    }
}