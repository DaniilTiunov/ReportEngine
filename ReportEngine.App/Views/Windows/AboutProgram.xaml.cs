using System.IO;
using System.Text.Json;

using System.Windows;
using ReportEngine.App.Services.Notification;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для AboutProgram.xaml
/// </summary>
public partial class AboutProgram : Window
{
    private readonly ExceptionService _exceptionService;


    public string Version { get; private set; } = String.Empty;

    public AboutProgram(ExceptionService exceptionService)
    {
        InitializeComponent();
        _exceptionService = exceptionService;
        LoadLastUpdate();
        DataContext = this; // Устанавливаем DataContext
    }


    private void LoadLastUpdate()
    {
        _exceptionService.SafeExecute(() =>
        {
            var filePath = DirectoryHelper.GetUpdateInfoPath();

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);

                var updates = JsonSerializer.Deserialize<List<UpdateInfo>>(json);

                if (updates == null)
                {
                    throw new Exception("Не удалось загрузить список обновлений");
                }

                var lastUpdate = updates
                    .Where(u => DateTime.TryParse(u.Date, out _))
                    .OrderByDescending(u => DateTime.Parse(u.Date))
                    .FirstOrDefault();


                if (lastUpdate == null || (string.IsNullOrEmpty(lastUpdate.Version) && string.IsNullOrEmpty(lastUpdate.Date)))
                {
                    throw new Exception("Не удалось определить версию приложения");
                }

                Version = $"Версия приложения:\n{lastUpdate.Version} от {lastUpdate.Date}";
            }
            else
            {
                throw new Exception("Не удалось загрузить список обновлений");
            }
        });
    }


    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
