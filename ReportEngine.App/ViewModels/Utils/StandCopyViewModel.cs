using System.Windows.Input;
using ReportEngine.App.Commands;
using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.ViewModels.Utils;

public class StandCopyViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;
    private int _copyCount = 1;

    public StandCopyViewModel(INotificationService notificationService)
    {
        ApplyCommand = new RelayCommand(OnApplyCommandExecuted, _ => true);
        _notificationService = notificationService;
    }

    public int CopyCount
    {
        get => _copyCount;
        set => Set(ref _copyCount, value);
    }

    public ICommand ApplyCommand { get; set; }

    public Action<int> ResultHandler { get; set; }

    public bool ValidateData()
    {
        if (CopyCount < 1)
        {
            _notificationService.ShowError("Некорретные данные");
            return false;
        }

        return true;
    }

    public async void OnApplyCommandExecuted(object sender)
    {
        ResultHandler?.Invoke(CopyCount);
    }
}
