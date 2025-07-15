using Microsoft.Win32;
using ReportEngine.Shared.Config.JsonHelpers;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ReportEngine.App.Views.UpdateInformation
{
    public class Updater
    {
        public static void CheckForUpdate(string versionOnServerPath, string localVersionPath)
        {
            string versionOnServer = File.ReadAllText(versionOnServerPath);

            if (versionOnServer != localVersionPath)
            {
                if(MessageBox.Show($"Доступна новая версия приложения\n" +
                    $"Новая версия: {versionOnServer}\n" + $"Ваша версия: {localVersionPath}\n" + "Обновить приложение?\n", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    Process.Start("explorer.exe", @"T:\00 ОКП АСУ\01 Группа разработки ПО\Тиунов\Progs");
                
            }
            else
            {
                MessageBox.Show($"Вы используете последнюю версию: {localVersionPath}", "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
        }
    }
}
