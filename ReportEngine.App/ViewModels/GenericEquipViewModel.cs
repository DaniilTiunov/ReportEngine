using ReportEngine.App.Commands;
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
    /// <typeparam name="TEquip">Тип оборудования, который является классом и имеет публичный конструктор без параметров.</typeparam>
    public class GenericEquipViewModel<T, TEquip> : BaseViewModel
        where T : IBaseEquip // Ограничение: T должен реализовывать интерфейс IBaseEquip
        where TEquip : class, new() // Ограничение: TEquip должен быть классом и иметь публичный конструктор без параметров
    {
        private readonly IGenericBaseRepository<T, TEquip> _genericEquipRepository; // Репозиторий для работы с данными оборудования

        /// <summary>
        /// Модель для управления коллекцией оборудования и выбранным элементом оборудования.
        /// </summary>
        public GenericEquipModel<T, TEquip> GenericEquipModel { get; set; } = new GenericEquipModel<T, TEquip>();

        /// <summary>
        /// Инициализирует новый экземпляр класса GenericEquipViewModel.
        /// </summary>
        /// <param name="genericEquipRepository">Репозиторий для работы с данными оборудования.</param>
        public GenericEquipViewModel(IGenericBaseRepository<T, TEquip> genericEquipRepository)
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
            ShowAllEquipCommand = new RelayCommand(OnShowAllEquipCommandExecuted, CanAllCommandsExecute);
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
                GenericEquipModel.BaseEquips = new ObservableCollection<T>(baseEquips);
            });
        }

        /// <summary>
        /// Команда для добавления нового оборудования.
        /// </summary>
        public ICommand AddNewEquipCommand { get; set; }

        /// <summary>
        /// Команда для удаления оборудования.
        /// </summary>
        public ICommand RemoveEquipCommand { get; set; }
    }
}
