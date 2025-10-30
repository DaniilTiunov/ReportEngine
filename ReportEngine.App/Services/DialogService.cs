using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Display;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels;
using ReportEngine.App.ViewModels.Contacts;
using ReportEngine.App.ViewModels.FormedEquips;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

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
            var window = factory.CreateWindow<T>(true);


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

    public IBaseEquip? ShowAllSortamentsDialog()
    {
        try
        {
            IBaseEquip? selected = null;
            using var scope = _serviceProvider.CreateScope();
            var viewModel = scope.ServiceProvider.GetRequiredService<AllSortamentsViewModel>();

            viewModel.SelectionHandler = item => { selected = item; };

            var window = new AllSortamentsView(viewModel, true);

            window.EquipDataGrid.IsReadOnly = true;

            window.ShowDialog();

            return selected;
        }
        catch (Exception ex)
        {
            MessageBoxHelper.ShowError(ex.Message);
            return null;
        }
    }

    public string ShowCompanyDialog()
    {
        try
        {
            string selected = null;

            var viewModel = _serviceProvider.GetRequiredService<CompanyViewModel>();

            viewModel.SelectedItem = item => { selected = item; };

            var window = new CompanyView(viewModel, true);
            window.CompaniesDataGrid.IsReadOnly = true;

            window.ShowDialog();
            return selected;
        }
        catch (Exception ex)
        {
            MessageBoxHelper.ShowError(ex.Message);
            return string.Empty;
        }
    }

    public string ShowSubjectDialog()
    {
        try
        {
            string selected = null;

            var viewModel = _serviceProvider.GetRequiredService<SubjectViewModel>();

            viewModel.SelectedItem = item => { selected = item; };

            var window = new SubjectsView(viewModel, true);
            window.SubjectsDataGrid.IsReadOnly = true;

            window.ShowDialog();
            return selected;
        }
        catch (Exception ex)
        {
            MessageBoxHelper.ShowError(ex.Message);
            return string.Empty;
        }
    }

    public FormedFrame ShowFrameDialog()
    {
        try
        {
            FormedFrame selected = null;

            var viewModel = _serviceProvider.GetRequiredService<FormedFrameViewModel>();
            var window = new FrameDialogView(viewModel);

            viewModel.SelectedItem = item => { selected = item; };

            window.FrameDataGrid.IsReadOnly = true;
            window.ShowDialog();

            return selected;
        }
        catch (Exception ex)
        {
            MessageBoxHelper.ShowError(ex.Message);
            return null;
        }
    }
}
