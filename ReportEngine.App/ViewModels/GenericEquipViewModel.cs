using System.Collections.ObjectModel;
using System.Windows.Input;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;

namespace ReportEngine.App.ViewModels;

/// <summary>
///     Обобщенная ViewModel для управления оборудованием.
/// </summary>
/// <typeparam name="T">Тип, реализующий интерфейс IBaseEquip.</typeparam>
public class GenericEquipViewModel<T> : BaseViewModel
    where T : class, IBaseEquip, new() // Ограничение: T должен реализовывать интерфейс IBaseEquip
{
    private readonly IGenericBaseRepository<T, T>
        _genericEquipRepository; // Репозиторий для работы с данными оборудования

    /// <summary>
    ///     Инициализирует новый экземпляр класса GenericEquipViewModel.
    /// </summary>
    /// <param name="genericEquipRepository">Репозиторий для работы с данными оборудования.</param>
    public GenericEquipViewModel(IGenericBaseRepository<T, T> genericEquipRepository)
    {
        InitializeCommands(); // Инициализируем команды
        _genericEquipRepository = genericEquipRepository; // Устанавливаем репозиторий
    }

    public Action<T> SelectionHandler { get; set; }

    /// <summary>
    ///     Модель для управления коллекцией оборудования и выбранным элементом оборудования.
    /// </summary>
    public GenericEquipModel<T, T> GenericEquipModel { get; set; } = new();

    /// <summary>
    ///     Инициализирует команды для ViewModel.
    /// </summary>
    public void InitializeCommands()
    {
        // Инициализируем команду для отображения всего оборудования
        SelectCommand = new RelayCommand(OnSelect, CanAllCommandsExecute);
        ShowAllEquipCommand = new RelayCommand(OnShowAllEquipCommandExecuted, CanAllCommandsExecute);
        SaveChangesEquipCommand = new RelayCommand(OnSaveChangesCommandExecuted, CanAllCommandsExecute);
        RemoveEquipCommand = new RelayCommand(OnRemoveEquipCommandExecuted, CanAllCommandsExecute);
        AddNewEquipCommand = new RelayCommand(OnAddNewEquipCommandExecuted, CanAllCommandsExecute);
    }

    #region Команды для работы с оборудованием

    public ICommand AddNewEquipCommand { get; set; }

    /// <summary>
    ///     Команда для отображения всего оборудования.
    /// </summary>
    public ICommand ShowAllEquipCommand { get; set; }

    /// <summary>
    ///     Определяет, может ли команда для отображения всего оборудования быть выполнена.
    /// </summary>
    /// <param name="e">Параметр команды.</param>
    /// <returns>Всегда возвращает true, указывая, что команда всегда может быть выполнена.</returns>
    public bool CanAllCommandsExecute(object e)
    {
        return true;
    }

    /// <summary>
    ///     Выполняет команду для отображения всего оборудования.
    /// </summary>
    /// <param name="e">Параметр команды.</param>
    public async void OnShowAllEquipCommandExecuted(object e)
    {
        await LoadAllBaseEquipsAsync();
    }

    public ICommand SelectCommand { get; set; }

    private void OnSelect(object e)
    {
        if (GenericEquipModel.SelectedBaseEquip != null) SelectionHandler?.Invoke(GenericEquipModel.SelectedBaseEquip);
    }

    /// <summary>
    ///     Команда для добавления нового оборудования.
    /// </summary>
    public ICommand SaveChangesEquipCommand { get; set; }

    public async void OnSaveChangesCommandExecuted(object e)
    {
        await SaveChangesAsync();
    }

    /// <summary>
    ///     Команда для удаления оборудования.
    /// </summary>
    public ICommand RemoveEquipCommand { get; set; }

    public async void OnRemoveEquipCommandExecuted(object e)
    {
        await RemoveSelectedBaseEquipAsync();
    }

    public async void OnAddNewEquipCommandExecuted(object e)
    {
        await AddBaseEquipAsync();
    }

    #endregion

    #region Методы для работы с оборудованием

    /// <summary>
    ///     Метод для загрузки всего оборудования.
    /// </summary>
    private async Task LoadAllBaseEquipsAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var items = await _genericEquipRepository.GetAllAsync();
            var baseEquips = items.OfType<T>().ToList();
            if (GenericEquipModel.BaseEquips != null)
            {
                GenericEquipModel.BaseEquips.Clear();
                foreach (var equip in baseEquips)
                    GenericEquipModel.BaseEquips.Add(equip);
            }
            else
            {
                GenericEquipModel.BaseEquips = new ObservableCollection<T>(baseEquips);
            }
        });
    }

    /// <summary>
    ///     Метод для добавления.
    /// </summary>
    private async Task AddBaseEquipAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var newEquip = new T();
            GenericEquipModel.BaseEquips.Add(newEquip);
            GenericEquipModel.SelectedBaseEquip = newEquip;
        });
    }

    /// <summary>
    ///     Метод для сохранения изменений.
    /// </summary>
    private async Task SaveChangesAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            foreach (var equip in GenericEquipModel.BaseEquips)
            {
                // Проверяем обязательные поля
                if (string.IsNullOrWhiteSpace(equip.Name))
                {
                    MessageBoxHelper.ShowInfo("Поле 'Name' обязательно для заполнения.");
                    continue;
                }

                if (equip.Id == 0)
                    await _genericEquipRepository.AddAsync(equip);
                else
                    await _genericEquipRepository.UpdateAsync(equip);
            }
        });
    }

    /// <summary>
    ///     Метод для удаления выбранного оборудования.
    /// </summary>
    private async Task RemoveSelectedBaseEquipAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var selectedEquip = GenericEquipModel.SelectedBaseEquip;
            if (selectedEquip == null)
            {
                MessageBoxHelper.ShowInfo("Пожалуйста, выберите оборудование для удаления.");
                return;
            }

            await _genericEquipRepository.DeleteAsync(selectedEquip);
            GenericEquipModel.BaseEquips.Remove(selectedEquip); // Удаляем из коллекции
            GenericEquipModel.SelectedBaseEquip = default; // Сбросить выбор
        });
    }

    #endregion
}