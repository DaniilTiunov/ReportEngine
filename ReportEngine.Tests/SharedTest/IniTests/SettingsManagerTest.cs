using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.Tests.SharedTest.IniTests;

[TestClass]
public class SettingsmanagerTest
{
    [TestMethod]
    public void CanSaveNewPath_WhenCalled_ReturnsCorrectPath()
    {
    }

    [TestMethod]
    public void GetIniPath_WhenCalled_ReturnsCorrectPath()
    {
        // Arrange
        var path = "C:\\Work\\Prjs\\ReportEngine\\ReportEngine.App\\bin\\Debug\\net8.0-windows\\Отчёты";

        var result = SettingsManager.GetReportDirectory();

        Assert.AreEqual(path, result);
    }
}
