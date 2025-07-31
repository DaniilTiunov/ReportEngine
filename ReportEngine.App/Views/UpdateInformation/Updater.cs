using ReportEngine.App.Display;
using System.Diagnostics;
using System.IO;

namespace ReportEngine.App.Views.UpdateInformation
{
    public class Updater
    {
        public static void CheckForUpdate(string versionOnServerPath, string localVersionPath)
        {
            string versionOnServer = File.ReadAllText(versionOnServerPath);

            if (versionOnServer != localVersionPath)
            {
                if (MessageBoxHelper.ShowConfirmation(
                        $"Доступна новая версия приложения\n" +
                        $"Новая версия: {versionOnServer}\n" +
                        $"Ваша версия: {localVersionPath}\n" +
                        "Обновить приложение?\n",
                        "Обновление"))
                {
                    Process.Start("explorer.exe", @"T:\\00 ОКП АСУ\\01 Группа разработки ПО\\Тиунов\\Progs");
                }
            }
            else
            {
                MessageBoxHelper.ShowInfo($"Вы используете последнюю версию: {localVersionPath}");
            }
        }
    }
}
