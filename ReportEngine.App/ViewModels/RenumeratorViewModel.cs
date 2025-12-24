using System.Diagnostics;
using System.Windows.Input;
using ReportEngine.App.Commands;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels.DTO;

namespace ReportEngine.App.ViewModels
{
    public class RenumeratorViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;

        private int _fromNumber = 1;
        private int _toNumber = 2;
        private string _prefix = "Текст до номера";
        private string _postfix = "Текст после номера";

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

        public string Prefix
        {
            get => _prefix;
            set => Set(ref _prefix, value);
        }

        public string Postfix
        {
            get => _postfix;
            set => Set(ref _postfix, value);
        }

        public ICommand ApplyCommand { get; set; }
        public Action<RenumerationInfo> ResultHandler {  get; set; }

        public RenumeratorViewModel(INotificationService notificationService)
        {
            ApplyCommand = new RelayCommand(OnApplyCommandExecuted, _ => true);
            _notificationService = notificationService;
        }

        public bool ValidateData()
        {
            if (FromNumber < 1 || ToNumber < 1)
            {
                _notificationService.ShowError("Некорректные значения № стендов");
                return false;
            }

            if (FromNumber > ToNumber)
            {
                _notificationService.ShowError("Некорректный диапазон № стендов");
                return false;
            }

            return true;
        }


        public async void OnApplyCommandExecuted(object sender)
        {
            ResultHandler?.Invoke(new RenumerationInfo()
            {
                FromNumber = FromNumber,
                ToNumber = ToNumber,
                Prefix = Prefix,
                Postfix = Postfix
            });
        }
    }
}
