using ReportEngine.App.Commands;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class FormedDrainagesViewModel
    {
        private readonly IFormedDrainagesRepository _formedDrainagesRepository;
        public FormedDrainagesModel FormedDrainagesModel { get; set; } = new();

        public FormedDrainagesViewModel(IFormedDrainagesRepository formedDrainagesRepository)
        {
            _formedDrainagesRepository = formedDrainagesRepository;
            InitializeCommands();
        }

        public void InitializeCommands()
        {
            ShowAllFormedDrainagesCommand = new RelayCommand(OnShowAllFormedDrainagesCommandExecuted, CanAllCommandsExecute);
            AddFormedDrainageCommand = new RelayCommand(OnAddFormedDrainageCommandExecuted, CanAllCommandsExecute);
            DeleteFormedDrainageCommand = new RelayCommand(OnDeleteFormedDrainageCommandExecuted, CanAllCommandsExecute);
            AddPurposeCommand = new RelayCommand(OnAddPurposeCommandExecuted, CanAllCommandsExecute);
            DeletePurposeCommand = new RelayCommand(OnDeletePurposeCommandExecuted, CanAllCommandsExecute);
        }

        public ICommand ShowAllFormedDrainagesCommand { get; set; }
        public ICommand AddFormedDrainageCommand { get; set; }
        public ICommand DeleteFormedDrainageCommand { get; set; }
        public ICommand AddPurposeCommand { get; set; }
        public ICommand DeletePurposeCommand { get; set; }
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
                await _formedDrainagesRepository.UpdateAsync(drainage);
            });
        }
    }
}
