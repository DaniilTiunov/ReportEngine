using System.Windows;

namespace ReportEngine.App.Services.Interfaces;

public interface IWindowWithViewModel<out TViewModel>
{
    TViewModel ViewModel { get; }

    Window? Owner { get; set; }

    bool? ShowDialog();
}
