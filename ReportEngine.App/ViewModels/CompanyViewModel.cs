using ReportEngine.App.Model;
using ReportEngine.App.Services;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
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
            //AddNewCompanyCommand = new RelayCommand(OnAddNewCompanyCommandExecuted, CanAllCommandsExecute);
            //SaveChangesCommand = new RelayCommand(OnSaveChangesCommandExecuted, CanAllCommandsExecute);
        }

        public ICommand AddNewCompanyCommand { get; set; }
        public ICommand SaveChangesCommand { get; set; }
        public ICommand DeleteCompanyCommand { get; set; }
        public ICommand UpdateCompanyCommand { get; set; }
        public bool CanAllCommandsExecute(object e) => true;
    }
}
