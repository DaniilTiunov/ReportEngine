using ReportEngine.App.Config.JsonHelpers;
using System.IO;
using System.Windows;

namespace ReportEngine.App.UpdateInformation
{
    public class Updater
    {
        public static void CheckForUpdate(string versionOnServerPath, string localVersionPath)
        {
            string versionOnServer = File.ReadAllText(versionOnServerPath);

            if (versionOnServer != localVersionPath)
            {
                MessageBox.Show($"Доступна новая версия приложения\n" +
                    $"Новая версия: {versionOnServer}" + $"Ваша версия: {localVersionPath}", "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
