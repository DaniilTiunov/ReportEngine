using System.Collections.ObjectModel;
using System.Windows.Input;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Model;
using ReportEngine.App.Services;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels;

public class ObvyazkaViewModel
{
    private readonly IDialogService _dialogService;
    private readonly NavigationService _navigation;
    private readonly IObvyazkaRepository _obvyazkaRepository;

    public ObvyazkaViewModel(IObvyazkaRepository obvyazkaRepository, NavigationService navigation,
        IDialogService dialogService)
    {
        _obvyazkaRepository = obvyazkaRepository;
        _navigation = navigation;
        _dialogService = dialogService;
        InitializeCommands();
    }

    public Action<Obvyazka>? SelectionHandler { get; set; }

    public ObvyazkaModel CurrentObvyazka { get; set; } = new();

    public ICommand ShowAllObvyazkaCommand { get; set; }

    public void InitializeCommands()
    {
        ShowAllObvyazkaCommand = new RelayCommand(OnShowAllObvyazkiCommandExecuted, CanAllCommandsExecute);
    }

    public bool CanAllCommandsExecute(object e)
    {
        return true;
    }

    #region Команды

    public async void OnShowAllObvyazkiCommandExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(ShowAllObvyazkiAsync);
    }

    #endregion

    #region Методы

    public async Task ShowAllObvyazkiAsync()
    {
        var obvyazki = await _obvyazkaRepository.GetAllAsync();
        CurrentObvyazka.Obvyazki = new ObservableCollection<Obvyazka>(obvyazki);
    }

    #endregion
}