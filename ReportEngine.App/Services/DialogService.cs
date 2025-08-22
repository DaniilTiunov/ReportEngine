using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Display;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Shared.Helpers;

namespace ReportEngine.App.Services;

public class DialogService : IDialogService
{
    private readonly IServiceProvider _serviceProvider;

    public DialogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T? ShowEquipDialog<T>()
        where T : class, IBaseEquip, new()
    {
        try
        {
            T selectedItem = null;
            var factory = _serviceProvider.GetRequiredService<GenericEquipWindowFactory>();
            var window = factory.CreateWindow<T>();

            if (window.DataContext is GenericEquipViewModel<T> viewModel)
                viewModel.SelectionHandler = item =>
                {
                    selectedItem = item;
                    window.Close();
                };
            window.ShowDialog();
            return selectedItem;
        }
        catch (Exception ex)
        {
            MessageBoxHelper.ShowError(ex.Message);
            return null;
        }
    }

    public Obvyazka? ShowObvyazkaDialog()
    {
        try
        {
            Obvyazka? selectedObvyazka = null;
            var obvyazkaViewModel = _serviceProvider.GetRequiredService<ObvyazkaViewModel>();

            obvyazkaViewModel.SelectionHandler = obv => selectedObvyazka = obv;

            var window = new ObvyazkiView(obvyazkaViewModel);
            
            window.ShowDialog();
            return selectedObvyazka;
        }
        catch (Exception e)
        {
            MessageBoxHelper.ShowError(e.Message);
            return null;
        }
    }
}