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
        public Action<(int,int)> ResultHandler {  get; set; }

        public RenumeratorViewModel(INotificationService notificationService)
        {
            ApplyCommand = new RelayCommand(OnApplyCommandExecuted, _ => true);
            _notificationService = notificationService;
        }

        public (int,int) IncorrectNumbers
        {
            get => (-1, -1);
        }



        public bool ValidateNumbers()
        {
            if (FromNumber < 1 || ToNumber < 1)
            {
                _notificationService.ShowError("Некорректные значения");
                return false;
            }

            if (FromNumber > ToNumber)
            {
                _notificationService.ShowError("Неверный диапазон");
                return false;
            }

            return true;
        }


        public async void OnApplyCommandExecuted(object sender)
        {
            ResultHandler?.Invoke((FromNumber,ToNumber));

            Debug.WriteLine("SRAKA");
        }
    }
}
