using ReportEngine.App.Commands;
using ReportEngine.App.Model;
using ReportEngine.App.Services;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class ObvyazkaViewModel
    {
        private readonly IObvyazkaRepository _obvyazkaRepository;
        private readonly NavigationService _navigation;
        public ObvyazkaModel CurrentObvyazka { get; set; } = new();

        public ObvyazkaViewModel(IObvyazkaRepository obvyazkaRepository, NavigationService navigation)
        {
            _obvyazkaRepository = obvyazkaRepository;
            _navigation = navigation;
            InitializeCommands();
        }

        public void InitializeCommands()
        {
            ShowAllObvyazkaCommand = new RelayCommand(OnShowAllObvyazkiCommandExecuted, CanAllCommandsExecute);
        }

        public bool CanAllCommandsExecute(object e) => true;
        public ICommand ShowAllObvyazkaCommand { get; set; }
        public async void OnShowAllObvyazkiCommandExecuted(object p)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var obvyazki = await _obvyazkaRepository.GetAllAsync();
                CurrentObvyazka.Obvyazki = new ObservableCollection<Obvyazka>(obvyazki);
            });
        }
    }
}
