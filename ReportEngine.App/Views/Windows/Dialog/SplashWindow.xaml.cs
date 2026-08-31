using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.App.Views.Windows.Dialog;

[INotifyPropertyChanged]
public partial class SplashWindow : Window
{
    [ObservableProperty] private ReleaseChannel _channel;

    [ObservableProperty] private string _version;

    public SplashWindow()
    {
        DataContext = this;
        InitializeComponent();

        SetApplicationVersion();
    }

    private void SetApplicationVersion()
    {
        var filePath = DirectoryHelper.GetUpdateInfoPath();
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            var updates = JsonSerializer.Deserialize<List<UpdateInfo>>(json, options);
            var update = updates?.FirstOrDefault();

            Version = update.Version;
            Channel = update.Channel;
        }
    }

    public void CheckDbStatus(ReAppContext dbContext)
    {
        StatusText.Text = "Проверка подключения к базе данных...";

        if (dbContext.Database.CanConnect())
            StatusText.Text = "Подключение к базе данных установлено...";
        else
            StatusText.Text = "Подключение к базе данных не установлено...";
    }

    public void SetStatusText(string statusText)
    {
        StatusText.Text = statusText;
    }
}