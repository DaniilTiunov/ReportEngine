using System.Diagnostics;
using System.Windows.Input;
using ReportEngine.App.Commands;
using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.ViewModels
{
    public class RenumeratorViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;

        private int _fromNumber;
        private int _toNumber;

        public int FromNumber
        {
            get => _fromNumber;
            set => Set(ref _fromNumber, value);
        }
        public int ToNumber
        {
            get => _toNumber;
            set => Set(ref _toNumber, value);
        }

        public ICommand ApplyCommand { get; set; }
        public Action<(int,int)> SelectionHandler {  get; set; }


        public RenumeratorViewModel(INotificationService notificationService)
        {
            ApplyCommand = new RelayCommand(OnApplyCommandExecuted, _ => true);
            _notificationService = notificationService;
        }



        public async void OnApplyCommandExecuted(object sender)
        {
            //SelectionHandler.Invoke();

            Debug.WriteLine("OnApply выполнился");
        }
    }
}
