using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Model;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels;

public class ObvyazkaViewModel
{
    private readonly INotificationService _notificationService;
    private readonly IObvyazkaRepository _obvyazkaRepository;

    public ObvyazkaViewModel(IObvyazkaRepository obvyazkaRepository, INotificationService notificationService)
    {
        _obvyazkaRepository = obvyazkaRepository;
        _notificationService = notificationService;
        InitializeCommands();
    }

    public Action<Obvyazka>? SelectionHandler { get; set; }
    public ObvyazkaModel CurrentObvyazka { get; set; } = new();
    public ICommand ShowAllObvyazkaCommand { get; set; }
    public ICommand AddNewObvyazkaCommand { get; set; }
    public ICommand UpdateChangesCommand { get; set; }
    public ICommand DeleteObvyazkaCommand { get; set; }

    public void InitializeCommands()
    {
        ShowAllObvyazkaCommand = new RelayCommand(OnShowAllObvyazkiCommandExecuted, CanAllCommandsExecute);
        UpdateChangesCommand = new RelayCommand(OnUpdateChangesExecuted, CanAllCommandsExecute);
        DeleteObvyazkaCommand = new RelayCommand(OnDeleteObvyazkaExecuted, CanAllCommandsExecute);
        AddNewObvyazkaCommand = new RelayCommand(OnAddNewObvyazkaCommandExecuted, CanAllCommandsExecute);
    }

    public bool CanAllCommandsExecute(object e)
    {
        return true;
    }

    #region Команды

    public async void OnShowAllObvyazkiCommandExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(ShowAllObvyazkiAsync);
    }

    public async void OnAddNewObvyazkaCommandExecuted(object e)
    {
        try
        {
            var newObvyazka = new Obvyazka
            {
                Number = CurrentObvyazka.SelectedObvyazka.Number,
                LineLength = CurrentObvyazka.SelectedObvyazka.LineLength,
                ZraCount = CurrentObvyazka.SelectedObvyazka.ZraCount,
                TreeSocket = CurrentObvyazka.SelectedObvyazka.TreeSocket,
                KMCHCount = CurrentObvyazka.SelectedObvyazka.KMCHCount,
                Sensor = CurrentObvyazka.SelectedObvyazka.Sensor,
                SensorType = CurrentObvyazka.SelectedObvyazka.SensorType,
                Clamp = CurrentObvyazka.SelectedObvyazka.Clamp,
                WidthOnFrame = CurrentObvyazka.SelectedObvyazka.WidthOnFrame,
                OtherLineCount = CurrentObvyazka.SelectedObvyazka.OtherLineCount,
                Weight = CurrentObvyazka.SelectedObvyazka.Weight,
                HumanCost = CurrentObvyazka.SelectedObvyazka.HumanCost,
                ImageName = CurrentObvyazka.SelectedObvyazka.ImageName
            };

            await AddNewObvyazkaAsync(newObvyazka);

            CurrentObvyazka.Obvyazki.Add(newObvyazka);
            CurrentObvyazka.SelectedObvyazka = newObvyazka;
            _notificationService.ShowInfo("Новая обвязка добавлена");

            await ShowAllObvyazkiAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при добавлении обвязки: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    public async void OnUpdateChangesExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () => await UpdateObvyazkaAsync(CurrentObvyazka.SelectedObvyazka));
        _notificationService.ShowInfo("Изменения выбранной обвязки сохранены");
    }

    public async void OnDeleteObvyazkaExecuted(object e)
    {
        await ExceptionHelper.SafeExecuteAsync(async () => await DeleteObvyazkaAsync(CurrentObvyazka.SelectedObvyazka));
        CurrentObvyazka.Obvyazki.Remove(CurrentObvyazka.SelectedObvyazka);
        _notificationService.ShowInfo("Выбранная обвязка удалена");
    }

    #endregion

    #region Методы

    public async Task ShowAllObvyazkiAsync()
    {
        var obvyazki = await _obvyazkaRepository.GetAllAsync();
        CurrentObvyazka.Obvyazki = new ObservableCollection<Obvyazka>(obvyazki);
    }

    public async Task AddNewObvyazkaAsync(Obvyazka obvyazka)
    {
        await _obvyazkaRepository.AddAsync(obvyazka);
    }

    public async Task UpdateObvyazkaAsync(Obvyazka obvyazka)
    {
        await _obvyazkaRepository.UpdateAsync(obvyazka);
    }

    public async Task DeleteObvyazkaAsync(Obvyazka obvyazka)
    {
        await _obvyazkaRepository.DeleteAsync(obvyazka);
    }

    #endregion
}
