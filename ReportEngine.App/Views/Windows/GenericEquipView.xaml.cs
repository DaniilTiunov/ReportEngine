using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using ReportEngine.App.Services.Notification;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для GenericEquipView.xaml
/// </summary>
public partial class GenericEquipView : MetroWindow
{
    private readonly ExceptionService _exceptionService;
    private readonly bool _isDialog;
    private bool _allowEdit;

    public GenericEquipView(
        ExceptionService exceptionService,
        bool IsDialog = false)
    {
        InitializeComponent();
        _isDialog = IsDialog;
        _exceptionService = exceptionService;
    }

    private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
        if (!_allowEdit)
            e.Cancel = true;
    }

    private void SelectEquip_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        _exceptionService.SafeExecute(() =>
        {
            if (!_isDialog)
            {
                if (sender is not DataGrid grid)
                    return;

                _allowEdit = true;

                if (grid.CurrentCell != null) grid.BeginEdit();

                _allowEdit = false;
            }

            var type = DataContext.GetType();

            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(GenericEquipViewModel<>))
                return;

            dynamic viewModel = DataContext;
            viewModel.SelectCommand.Execute(null);
        });
    }
}