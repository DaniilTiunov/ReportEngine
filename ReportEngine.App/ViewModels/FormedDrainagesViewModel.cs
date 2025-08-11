using ReportEngine.App.Commands;
using ReportEngine.App.Model;
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
        }

        public void InitializeCommands()
        {
            ShowAllFormedDrainagesCommand = new RelayCommand(OnShowAllFormedDrainagesCommandExecuted, CanAllCommandsExecute);
            AddFormedDrainagesCommand = new RelayCommand(OnAddFormedDrainagesCommandExecuted, CanAllCommandsExecute);
            DeleteFormedDrainagesCommand = new RelayCommand(OnDeleteFormedDrainagesCommandExecuted, CanAllCommandsExecute);
            UpdateFormedDrainagesCommand = new RelayCommand(OnUpdateFormedDrainagesCommandExecuted, CanAllCommandsExecute);
        }
        public ICommand ShowAllFormedDrainagesCommand { get; set; }
        public ICommand AddFormedDrainagesCommand { get; set; }
        public ICommand DeleteFormedDrainagesCommand { get; set; }
        public ICommand UpdateFormedDrainagesCommand { get; set; }
        public bool CanAllCommandsExecute(object arg) => true;
        public async void OnShowAllFormedDrainagesCommandExecuted(object obj)
        {
            await LoadAllFormedDrainagesAsync();
        }
        public async void OnAddFormedDrainagesCommandExecuted(object obj)
        {
            await AddNewFormedDrainagesAsync();
        }
        public async void OnDeleteFormedDrainagesCommandExecuted(object obj)
        {
            throw new NotImplementedException();
        }
        public async void OnUpdateFormedDrainagesCommandExecuted(object obj)
        {
            throw new NotImplementedException();
        }

        private async Task LoadAllFormedDrainagesAsync()
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var formedDrainages = await _formedDrainagesRepository.GetAllAsync();
                FormedDrainagesModel.AllFormedDrainage = new ObservableCollection<FormedDrainage>(formedDrainages);
            });
        }
        private async Task AddNewFormedDrainagesAsync()
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var newFormedDrainages = FormedDrainagesModel.CreateNewFormedDrainages();
                FormedDrainagesModel.AllFormedDrainage.Add(newFormedDrainages);
                await _formedDrainagesRepository.AddAsync(newFormedDrainages);
            });
        }
    }
}
