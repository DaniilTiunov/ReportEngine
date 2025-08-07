using ReportEngine.Shared.Config.Directory;

namespace ReportEngine.Tests.SharedTest.Directory
{
    [TestClass]
    public class DirectoryHelperTest
    {
        [TestMethod]
        public void GetImagesPath_WhenCalled_ReturnsCorrectPath()
        {
            // Arrange
            string imageName = "1";
            string expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "Obvyazka", imageName + ".jpg");

            // Act
            string result = DirectoryHelper.GetImagesPath(imageName);

            // Assert
            Assert.AreEqual(expectedPath, result);
        }
    }
}
