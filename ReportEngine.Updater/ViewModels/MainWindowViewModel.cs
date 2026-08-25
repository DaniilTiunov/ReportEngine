using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Updater.Commands;
using ReportEngine.Updater.ViewModels.Base;
using ReportEngine.Updater.Views;

namespace ReportEngine.Updater.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider;
    
    [ObservableProperty]
    private object _currentView;

    public MainWindowViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        
        NavigateCommand = new RelayCommand(Navigate);
    }
    
    public ICommand NavigateCommand { get; set; }

    private void Navigate(object obj)
    {
        switch (obj)
        {
            case "Home":
                CurrentView  = _serviceProvider.GetRequiredService<HomeView>();
                break;
            case "Versions":
                CurrentView  = _serviceProvider.GetRequiredService<VersionsView>();
                break;
            case "Settings":
                CurrentView  = _serviceProvider.GetRequiredService<SettingsView>();
                break;
        }
    }
}