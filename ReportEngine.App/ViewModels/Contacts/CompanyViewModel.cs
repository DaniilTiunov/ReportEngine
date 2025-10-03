using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Model.Contacts;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels.Contacts;

public class CompanyViewModel
{
    private readonly IBaseRepository<Company> _companyRepository;
    private readonly INotificationService _notificationService;

    public CompanyViewModel(IBaseRepository<Company> companyRepository,
        INotificationService notificationService)
    {
        InitializeCommands();

        _companyRepository = companyRepository;
        _notificationService = notificationService;


        LoadAllCompaniesAsync();
    }

    public CompanyModel CurrentCompany { get; set; } = new();
    public ICommand LoadAllCompaniesCommand { get; set; }
    public ICommand AddNewCompanyCommand { get; set; }
    public ICommand SaveChangesCommand { get; set; }
    public ICommand DeleteCompanyCommand { get; set; }

    public void InitializeCommands()
    {
        LoadAllCompaniesCommand = new RelayCommand(OnLoadAllCompaniesExecuted, CanAllCommandsExecute);
        AddNewCompanyCommand = new RelayCommand(OnAddNewCompanyCommandExecuted, CanAllCommandsExecute);
        SaveChangesCommand = new RelayCommand(OnSaveChangesCommandExecuted, CanAllCommandsExecute);
        DeleteCompanyCommand = new RelayCommand(OnDeleteCompanyCommandExecuted, CanAllCommandsExecute);
    }

    public bool CanAllCommandsExecute(object e)
    {
        return true;
    }

    public async void OnLoadAllCompaniesExecuted(object p)
    {
        await LoadAllCompaniesAsync();
    }

    public async void OnAddNewCompanyCommandExecuted(object p)
    {
        await AddNewCompanyAsync();
    }

    public async void OnSaveChangesCommandExecuted(object p)
    {
        await SaveChangesAsync();
    }

    public async void OnDeleteCompanyCommandExecuted(object p)
    {
        await DeleteSelectedCompanyAsync();
    }

    private async Task LoadAllCompaniesAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var companies = await _companyRepository.GetAllAsync();
            CurrentCompany.AllCompanies = new ObservableCollection<Company>(companies);
        });
    }

    private async Task AddNewCompanyAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var newCompany = CurrentCompany.CreateNewCompany();
            CurrentCompany.AllCompanies.Add(newCompany);
            await _companyRepository.AddAsync(newCompany);
        });
    }

    private async Task SaveChangesAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            if (CurrentCompany.SelectedCompany != null)
            {
                await _companyRepository.UpdateAsync(CurrentCompany.SelectedCompany);
                _notificationService.ShowInfo("Изменения сохранены");
            }
        });
    }

    private async Task DeleteSelectedCompanyAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            if (CurrentCompany.SelectedCompany != null)
            {
                await _companyRepository.DeleteAsync(CurrentCompany.SelectedCompany);
                CurrentCompany.AllCompanies.Remove(CurrentCompany.SelectedCompany);
                CurrentCompany.SelectedCompany = null;
            }
        });
    }
}