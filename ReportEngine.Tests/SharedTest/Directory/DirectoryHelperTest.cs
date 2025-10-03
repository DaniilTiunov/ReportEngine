using ReportEngine.Shared.Config.Directory;

namespace ReportEngine.Tests.SharedTest.Directory;

[TestClass]
public class DirectoryHelperTest
{
    [TestMethod]
    public void GetImagesPath_WhenCalled_ReturnsCorrectPath()
    {
        // Arrange
        var imageName = "1";
        var expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "Obvyazka",
            imageName + ".jpg");

        // Act
        var result = DirectoryHelper.GetImagesPath(imageName);

        // Assert
        Assert.AreEqual(expectedPath, result);
    }

    [TestMethod]
    public void GetReportFolder_WhenCalled_ReturnsCorrectPath()
    {
        // Arrange
        var expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExcelTemplates", "Тара" + ".xlsx");

        // Act
        var result = DirectoryHelper.GetReportsTemplatePath("Тара", ".xlsx");

        Assert.AreEqual(expectedPath, result);
    }
}