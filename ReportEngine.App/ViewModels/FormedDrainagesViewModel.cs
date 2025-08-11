using ReportEngine.App.Commands;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Drainage;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class FormedDrainagesViewModel
    {
        private readonly IFormedDrainagesRepository _formedDrainagesRepository;
        private readonly IGenericBaseRepository<Drainage, Drainage> _genericEquipRepository;
        public FormedDrainagesModel FormedDrainagesModel { get; set; } = new();

        // Для справочника дренажей
        public ObservableCollection<Drainage> AllDrainages { get; set; } = new();
        public Drainage SelectedDrainageFromCatalog { get; set; }

        public FormedDrainagesViewModel(
            IFormedDrainagesRepository formedDrainagesRepository,
            IGenericBaseRepository<Drainage, Drainage> genericEquipRepository)
        {
            _formedDrainagesRepository = formedDrainagesRepository;
            _genericEquipRepository = genericEquipRepository;
            InitializeCommands();
            LoadDataAsync();
        }

        public void InitializeCommands()
        {
            ShowAllFormedDrainagesCommand = new RelayCommand(OnShowAllFormedDrainagesCommandExecuted, CanAllCommandsExecute);
            AddFormedDrainageCommand = new RelayCommand(OnAddFormedDrainageCommandExecuted, CanAllCommandsExecute);
            DeleteFormedDrainageCommand = new RelayCommand(OnDeleteFormedDrainageCommandExecuted, CanAllCommandsExecute);
            AddPurposeCommand = new RelayCommand(OnAddPurposeCommandExecuted, CanAllCommandsExecute);
            DeletePurposeCommand = new RelayCommand(OnDeletePurposeCommandExecuted, CanAllCommandsExecute);
            InsertMaterialCommand = new RelayCommand(OnInsertMaterialCommandExecuted, CanAllCommandsExecute);
        }

        public async void LoadDataAsync()
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var details = await _genericEquipRepository.GetAllAsync();
                var formedDrainages = await _formedDrainagesRepository.GetAllAsync();

                AllDrainages = new ObservableCollection<Drainage>(details);
                FormedDrainagesModel.AllFormedDrainage = new ObservableCollection<FormedDrainage>(formedDrainages);
                
            });
        }

        public ICommand ShowAllFormedDrainagesCommand { get; set; }
        public ICommand AddFormedDrainageCommand { get; set; }
        public ICommand DeleteFormedDrainageCommand { get; set; }
        public ICommand AddPurposeCommand { get; set; }
        public ICommand DeletePurposeCommand { get; set; }
        public ICommand InsertMaterialCommand { get; set; }
        public bool CanAllCommandsExecute(object arg) => true;

        public async void OnShowAllFormedDrainagesCommandExecuted(object obj)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var formedDrainages = await _formedDrainagesRepository.GetAllWithPurposesAsync();
                FormedDrainagesModel.AllFormedDrainage = new ObservableCollection<FormedDrainage>(formedDrainages);
            });
        }

        public async void OnAddFormedDrainageCommandExecuted(object obj)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var newDrainage = FormedDrainagesModel.CreateNewFormedDrainage("Новый дренаж");
                await _formedDrainagesRepository.AddAsync(newDrainage);
                FormedDrainagesModel.AllFormedDrainage.Add(newDrainage);
                FormedDrainagesModel.SelectedFormedDrainage = newDrainage;
            });
        }

        public async void OnDeleteFormedDrainageCommandExecuted(object obj)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var selected = FormedDrainagesModel.SelectedFormedDrainage;
                if (selected == null) return;
                await _formedDrainagesRepository.DeleteAsync(selected);
                FormedDrainagesModel.AllFormedDrainage.Remove(selected);
                FormedDrainagesModel.SelectedFormedDrainage = null;
            });
        }

        public async void OnAddPurposeCommandExecuted(object obj)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var drainage = FormedDrainagesModel.SelectedFormedDrainage;
                if (drainage == null) return;
                var newPurpose = FormedDrainagesModel.CreateNewPurpose("Новое назначение", "", null);
                drainage.Purposes.Add(newPurpose);
                FormedDrainagesModel.Purposes.Add(newPurpose);
                FormedDrainagesModel.SelectedPurpose = newPurpose;
                await _formedDrainagesRepository.UpdateAsync(drainage);
            });
        }

        public async void OnDeletePurposeCommandExecuted(object obj)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var drainage = FormedDrainagesModel.SelectedFormedDrainage;
                var purpose = FormedDrainagesModel.SelectedPurpose;
                if (drainage == null || purpose == null) return;
                drainage.Purposes.Remove(purpose);
                FormedDrainagesModel.Purposes.Remove(purpose);
                FormedDrainagesModel.SelectedPurpose = null;
                await _formedDrainagesRepository.UpdateAsync(drainage);
            });
        }

        // Вставить материал из справочника в назначение
        public async void OnInsertMaterialCommandExecuted(object obj)
        {
            var selectedPurpose = FormedDrainagesModel.SelectedPurpose;
            if (selectedPurpose != null && SelectedDrainageFromCatalog != null)
            {
                selectedPurpose.Material = SelectedDrainageFromCatalog.Name;
                // Если нужно, можно сразу обновить в БД:
                await _formedDrainagesRepository.UpdateAsync(FormedDrainagesModel.SelectedFormedDrainage);
            }
        }
    }
}