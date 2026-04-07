using System.Windows;
using System.Windows.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Controls;

public partial class AuditEventsView : UserControl
{
    public AuditEventsView(AuditViewModel auditViewModel)
    {
        InitializeComponent();
        DataContext = auditViewModel;
        Loaded += AuditEventsView_Loaded;
    }

    private async void AuditEventsView_Loaded(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as AuditViewModel;
        if (vm != null)
        {
            await vm.LoadAllEventsAsync();
        }
    }
}

