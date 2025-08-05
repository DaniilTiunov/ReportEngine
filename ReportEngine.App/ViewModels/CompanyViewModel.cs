using ReportEngine.App.Commands;
using ReportEngine.App.Model;
using ReportEngine.App.Services;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels
{
    public class CompanyViewModel
    {
        private readonly NavigationService _navigation;
        private readonly IBaseRepository<Company> _companyRepository;
        public CompanyModel CurrentCompany { get; set; } = new();
        public CompanyViewModel(IBaseRepository<Company> companyRepository, NavigationService navigation)
        {
            InitializeCommands();

            _companyRepository = companyRepository;
            _navigation = navigation;
        }

        public void InitializeCommands()
        {
            ShowAllCompaniesCommand = new RelayCommand(OnShowAllCompaniesExecuted, CanAllCommandsExecute);
            //SaveChangesCommand = new RelayCommand(OnSaveChangesCommandExecuted, CanAllCommandsExecute);
        }

        public ICommand ShowAllCompaniesCommand { get; set; }
        public ICommand AddNewCompanyCommand { get; set; }
        public ICommand SaveChangesCommand { get; set; }
        public ICommand DeleteCompanyCommand { get; set; }
        public ICommand UpdateCompanyCommand { get; set; }
        public bool CanAllCommandsExecute(object e) => true;

        public async void OnShowAllCompaniesExecuted(object p)
        {
            await ExceptionHelper.SafeExecuteAsync(async () =>
            {
                var companies = await _companyRepository.GetAllAsync();
                CurrentCompany.AllCompanies = new ObservableCollection<Company>(companies);
            });
        }
    }
}
