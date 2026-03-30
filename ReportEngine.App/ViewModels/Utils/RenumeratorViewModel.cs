using System.Windows.Input;
using ReportEngine.App.Commands;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels.DTO;

namespace ReportEngine.App.ViewModels.Utils;

public class RenumeratorViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;

    private int _fromNumber = 1;

    private string _postfix = "";
    private string _prefix = "01-01.";
    private string _startValue = "001";
    private string _step = "1";
    private int _toNumber = 2;

    public RenumeratorViewModel(INotificationService notificationService)
    {
        ApplyCommand = new RelayCommand(OnApplyCommandExecuted, _ => true);
        _notificationService = notificationService;
    }

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

    public string StartValue
    {
        get => _startValue;
        set => Set(ref _startValue, value);
    }

    public string Step
    {
        get => _step;
        set => Set(ref _step, value);
    }

    public ICommand ApplyCommand { get; set; }
    public Action<RenumerationInfo> ResultHandler { get; set; }

    public bool ValidateData()
    {
        if (FromNumber > ToNumber || FromNumber < 1 || ToNumber < 1)
        {
            _notificationService.ShowError("Некорректный диапазон № стендов");
            return false;
        }

        if (string.IsNullOrEmpty(StartValue) || !int.TryParse(StartValue, out _))
        {
            _notificationService.ShowError("Некорректное cтартовое значение");
            return false;
        }

        if (string.IsNullOrEmpty(Step) || !int.TryParse(Step, out _))
        {
            _notificationService.ShowError("Некорректное значение шага");
            return false;
        }

        return true;
    }

    public async void OnApplyCommandExecuted(object sender)
    {
        ResultHandler?.Invoke(new RenumerationInfo
        {
            FromNumber = FromNumber,
            ToNumber = ToNumber,
            Prefix = Prefix,
            Postfix = Postfix,
            StartValue = int.TryParse(StartValue, out var resultStartValue) ? resultStartValue : null,
            Step = int.TryParse(Step, out var resultStepValue) ? resultStepValue : null,
            StartValueLength = StartValue.Length
        });
    }
}
