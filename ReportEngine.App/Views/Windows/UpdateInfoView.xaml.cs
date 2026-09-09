using System.IO;
using System.Text.Json;
using MahApps.Metro.Controls;
using ReportEngine.App.Services.Notification;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.Models;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для UpdateInfoView.xaml
/// </summary>
public partial class UpdateInfoView : MetroWindow
{
    private readonly ExceptionService _exceptionService;

    public UpdateInfoView(ExceptionService exceptionService)
    {
        InitializeComponent();
        _exceptionService = exceptionService;
        LoadUpdateHistory();
        DataContext = this;
    }

    public List<UpdateInfo> Updates { get; set; }

    private void LoadUpdateHistory()
    {
        _exceptionService.SafeExecute(() =>
        {
            var filePath = DirectoryHelper.GetUpdateInfoPath();

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);

                var updates = JsonSerializer.Deserialize<List<UpdateInfo>>(json);

                UpdatesList.ItemsSource = updates;
            }
            else
            {
                Updates = new List<UpdateInfo>();
            }
        });
    }
}