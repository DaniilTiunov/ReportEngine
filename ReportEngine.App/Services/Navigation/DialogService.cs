using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels;
using ReportEngine.App.ViewModels.Contacts;
using ReportEngine.App.ViewModels.DTO;
using ReportEngine.App.ViewModels.FormedEquips;
using ReportEngine.App.ViewModels.Utils;
using ReportEngine.App.Views.Utils;
using ReportEngine.App.Views.Windows;
using ReportEngine.App.Views.Windows.Dialog;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.Services.Navigation;

public class DialogService : IDialogService
{
    private readonly INotificationService _notificationService;
    private readonly IProjectInfoRepository _projectInfoRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly IStandService _standService;

    public DialogService(
        IServiceProvider serviceProvider,
        IStandService standService,
        INotificationService notificationService,
        IProjectInfoRepository projectInfoRepository)
    {
        _serviceProvider = serviceProvider;
        _standService = standService;
        _notificationService = notificationService;
        _projectInfoRepository = projectInfoRepository;
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
            _notificationService.ShowError(ex.Message);
            return null;
        }
    }

    public Obvyazka? ShowObvyazkaDialog(bool dialogMode)
    {
        try
        {
            Obvyazka? selectedObvyazka = null;
            var obvyazkaViewModel = _serviceProvider.GetRequiredService<ObvyazkaViewModel>();

            obvyazkaViewModel.SelectionHandler = obv => selectedObvyazka = obv;

            var window = new ObvyazkiView(obvyazkaViewModel);

            if (dialogMode)
            {
                window.CreateObvPanel.Visibility = Visibility.Collapsed;
                window.PanelColumn.Width = new GridLength(0);
                window.Width = 1000;
            }

            window.ShowDialog();
            return selectedObvyazka;
        }
        catch (Exception e)
        {
            _notificationService.ShowError(e.Message);
            return null;
        }
    }

    public IBaseEquip? ShowAllSortamentsDialog(object equipType = null)
    {
        try
        {
            IBaseEquip? selected = null;
            using var scope = _serviceProvider.CreateScope();
            var viewModel = scope.ServiceProvider.GetRequiredService<AllSortamentsViewModel>();

            viewModel.SelectionHandler = item => { selected = item; };

            var window = new AllSortamentsView(viewModel, true);

            OpenCurrentTab(equipType, window);

            window.EquipDataGrid.IsReadOnly = true;

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.ShowDialog();

            return selected;
        }
        catch (Exception ex)
        {
            _notificationService.ShowError(ex.Message);
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
            _notificationService.ShowError(ex.Message);
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
            _notificationService.ShowError(ex.Message);
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
            _notificationService.ShowError(ex.Message);
            return null;
        }
    }

    public RenumerationInfo ShowRenumerateDialog()
    {
        var resultData = new RenumerationInfo
        {
            FromNumber = -1,
            ToNumber = -1,
            Prefix = "",
            Postfix = "",
            StartValue = null,
            Step = null,
            StartValueLength = 0
        };

        try
        {
            var viewModel = _serviceProvider.GetRequiredService<RenumeratorViewModel>();
            var window = new RenumerateView(viewModel);

            viewModel.ResultHandler = info => { resultData = info; };

            window.ShowDialog();

            return resultData;
        }
        catch (Exception ex)
        {
            _notificationService.ShowError(ex.Message);
            return resultData;
        }
    }

    public int ShowStandCopyDialog()
    {
        var resultCopyCount = 0;

        try
        {
            var viewModel = _serviceProvider.GetRequiredService<StandCopyViewModel>();
            var window = new StandCopyView(viewModel);

            viewModel.ResultHandler = copyCount => { resultCopyCount = copyCount; };

            window.ShowDialog();

            return resultCopyCount;
        }
        catch (Exception ex)
        {
            _notificationService.ShowError(ex.Message);
            return resultCopyCount;
        }
    }

    public void ShowObvSettingsWindow(ProjectViewModel projectViewModel)
    {
        var window = new ObvSettingsView();

        window.DataContext = projectViewModel;

        projectViewModel.CurrentStandModel.SelectedObvyazkaInStand = new ObvyazkaInStand();

        window.ShowDialog();
    }

    public void ShowEditObvSettingsWindow(ProjectViewModel projectViewModel,
        StandModel standModel,
        ObvyazkaInStand selectedObvyazka)
    {
        var window = new ObvSettingsView();

        window.DataContext = projectViewModel;

        _standService.FillStandFieldsFromObvyazka(standModel, selectedObvyazka);

        window.ShowDialog();
    }

    public void ShowStandsSettingsWindow(ProjectViewModel projectViewModel, bool editMode)
    {
        try
        {
            var window = new StandsSettingsView(projectViewModel);

            window.DataContext = projectViewModel;

            if (!editMode)
            {
                window.CreateStandButton.Visibility = Visibility.Visible;
                window.EditStandbutton.Visibility = Visibility.Hidden;
            }

            projectViewModel.CurrentProjectModel.SelectedStand = null;
            projectViewModel.NewStand = new StandModel();

            var stands = projectViewModel.CurrentProjectModel.Stands;
            if (stands == null || !stands.Any())
            {
                projectViewModel.NewStand.Number = 1;
            }
            else
            {
                var maxStandNumber = stands.Max(s => s.Number);
                projectViewModel.NewStand.Number = maxStandNumber + 1;
            }

            window.ShowDialog();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError(ex.Message);
        }
    }

    public void ShowEditStandsObvSettingsWindow(ProjectViewModel projectViewModel, StandModel standModel, bool editMode)
    {
        try
        {
            var window = new StandsSettingsView(projectViewModel);

            window.DataContext = projectViewModel;

            if (editMode)
            {
                window.CreateStandButton.Visibility = Visibility.Hidden;
                window.EditStandbutton.Visibility = Visibility.Visible;
            }

            projectViewModel.CurrentProjectModel.SelectedStand = standModel;
            projectViewModel.OnFillStandFieldsFromSelectedStandCommandExecuted(new object());


            window.ShowDialog();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError(ex.Message);
        }
    }

    public void RunWithProgressDialog(Action action)
    {
        var progressDialog = new ProgressDialog();
        var owner = Application.Current.MainWindow;

        progressDialog.Show();

        try
        {
            action();
        }
        finally
        {
            progressDialog.Close();
        }
    }

    public async Task RunWithProgressDialogAsync(Func<Task> action)
    {
        var progressDialog = new ProgressDialog();

        progressDialog.Show();

        try
        {
            await action();
        }
        finally
        {
            progressDialog.Close();
        }
    }

    public Stand ShowSelectStandDialog()
    {
        var vm = new AllStandsViewModel(_projectInfoRepository);
        var view = new AllStandsView(vm);

        view.DataContext = vm;

        var result = view.ShowDialog();

        if (result == true)
            return vm.SelectedResult;

        return null;
    }

    private void OpenCurrentTab(object equipType, AllSortamentsView window)
    {
        switch (equipType)
        {
            case ElectricalPurpose:
                window.MainTabControl.SelectedIndex = 6;
                break;
            case DrainagePurpose:
                window.MainTabControl.SelectedIndex = 3;
                break;
        }
    }
}
