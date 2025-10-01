using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;
using System.Collections.ObjectModel;

namespace ReportEngine.App.Model.Contacts;

public class CompanyModel : BaseViewModel
{
    private ObservableCollection<Company> _allCompanies;
    private string _name;
    private int _number;
    private DateOnly _registerDate;
    private Company _selectedCompany;

    public string Name
    {
        get => _name;
        set => Set(ref _name, value);
    }

    public int Number
    {
        get => _number;
        set => Set(ref _number, value);
    }

    public DateOnly RegisterDate
    {
        get => _registerDate;
        set => Set(ref _registerDate, value);
    }

    public Company SelectedCompany
    {
        get => _selectedCompany;
        set => Set(ref _selectedCompany, value);
    }

    public ObservableCollection<Company> AllCompanies
    {
        get => _allCompanies;
        set => Set(ref _allCompanies, value);
    }

    public Company CreateNewCompany()
    {
        return new Company
        {
            Name = Name,
            Number = Number,
            RegisterDate = RegisterDate
        };
    }
}