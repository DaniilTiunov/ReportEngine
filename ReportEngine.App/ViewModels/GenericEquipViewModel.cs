using ReportEngine.App.Commands;
using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    /// <summary>
    /// Обобщенная ViewModel для управления оборудованием.
    /// </summary>
    /// <typeparam name="T">Тип, реализующий интерфейс IBaseEquip.</typeparam>
    public class GenericEquipViewModel<T> : BaseViewModel
        where T : class, IBaseEquip, new() // Ограничение: T должен реализовывать интерфейс IBaseEquip
    {
        public Action<T> SelectionHandler { get; set; }
        private readonly IGenericBaseRepository<T, T> _genericEquipRepository; // Репозиторий для работы с данными оборудования
        /// <summary>
        /// Модель для управления коллекцией оборудования и выбранным элементом оборудования.
        /// </summary>
        public GenericEquipModel<T, T> GenericEquipModel { get; set; } = new GenericEquipModel<T, T>();
        /// <summary>
        /// Инициализирует новый экземпляр класса GenericEquipViewModel.
        /// </summary>
        /// <param name="genericEquipRepository">Репозиторий для работы с данными оборудования.</param>
        public GenericEquipViewModel(IGenericBaseRepository<T, T> genericEquipRepository)
        {
            InitializeCommands(); // Инициализируем команды
            _genericEquipRepository = genericEquipRepository; // Устанавливаем репозиторий
        }
        /// <summary>
        /// Инициализирует команды для ViewModel.
        /// </summary>
        public void InitializeCommands()
        {
            // Инициализируем команду для отображения всего оборудования
            SelectCommand = new RelayCommand(OnSelect, CanAllCommandsExecute);
            ShowAllEquipCommand = new RelayCommand(OnShowAllEquipCommandExecuted, CanAllCommandsExecute);
            SaveChangesEquipCommand = new RelayCommand(OnSaveChangesCommandExecuted, CanAllCommandsExecute);
            RemoveEquipCommand = new RelayCommand(OnRemoveEquipCommandExecuted, CanAllCommandsExecute);
        }
        /// <summary>
        /// Команда для отображения всего оборудования.
        /// </summary>
        public ICommand ShowAllEquipCommand { get; set; }
        /// <summary>
        /// Определяет, может ли команда для отображения всего оборудования быть выполнена.
        /// </summary>
        /// <param name="e">Параметр команды.</param>
        /// <returns>Всегда возвращает true, указывая, что команда всегда может быть выполнена.</returns>
        public bool CanAllCommandsExecute(object e) => true;
        /// <summary>
        /// Выполняет команду для отображения всего оборудования.
        /// </summary>
        /// <param name="e">Параметр команды.</param>
        public async void OnShowAllEquipCommandExecuted(object e)
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
        public ICommand SelectCommand { get; set; }
        private void OnSelect(object e)
        {
            if (GenericEquipModel.SelectedBaseEquip != null)
            {
                SelectionHandler?.Invoke(GenericEquipModel.SelectedBaseEquip);
            }
        }
        /// <summary>
        /// Команда для добавления нового оборудования.
        /// </summary>
        public ICommand SaveChangesEquipCommand { get; set; }
        public async void OnSaveChangesCommandExecuted(object e)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                foreach (var equip in GenericEquipModel.BaseEquips)
                {
                    if (equip.Id == 0) // Если Id равен 0, значит это новый объект
                    {
                        await _genericEquipRepository.AddAsync(equip);
                    }
                    else // Иначе обновляем существующий объект
                    {
                        await _genericEquipRepository.UpdateAsync(equip);
                    }
                }
                //OnShowAllEquipCommandExecuted(null); // Обновить список
            });
        }
        /// <summary>
        /// Команда для удаления оборудования.
        /// </summary>
        public ICommand RemoveEquipCommand { get; set; }
        public async void OnRemoveEquipCommandExecuted(object e)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var selectedEquip = GenericEquipModel.SelectedBaseEquip;
                if(selectedEquip == null)
                {
                    MessageBoxHelper.ShowInfo("Пожалуйста, выберите оборудование для удаления.");
                    return;
                }
                    
                await _genericEquipRepository.DeleteAsync(selectedEquip);
                GenericEquipModel.BaseEquips.Remove(selectedEquip); // Удаляем из коллекции
                GenericEquipModel.SelectedBaseEquip = default; // Сбросить выбор
            });
        }
    }
}