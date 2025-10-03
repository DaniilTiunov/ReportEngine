using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Model.FormedEquipsModels;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Drainage;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels.FormedEquips;

public class FormedDrainagesViewModel : BaseViewModel
{
    private readonly IFormedDrainagesRepository _formedDrainagesRepository;
    private readonly IGenericBaseRepository<Drainage, Drainage> _genericEquipRepository;

    public FormedDrainagesViewModel(
        IFormedDrainagesRepository formedDrainagesRepository,
        IGenericBaseRepository<Drainage, Drainage> genericEquipRepository)
    {
        _formedDrainagesRepository = formedDrainagesRepository;
        _genericEquipRepository = genericEquipRepository;

        InitializeCommands();
        LoadDataAsync();
    }

    public FormedDrainagesModel FormedDrainagesModel { get; } = new();

    public ICommand AddFormedDrainageCommand { get; set; }
    public ICommand SaveChangesCommand { get; set; }
    public ICommand DeleteFormedDrainageCommand { get; set; }
    public ICommand AddPurposeCommand { get; set; }
    public ICommand DeletePurposeCommand { get; set; }
    public ICommand InsertMaterialCommand { get; set; }

    public void InitializeCommands()
    {
        AddFormedDrainageCommand = new RelayCommand(OnAddFormedDrainageExecuted, CanAllCommandsExecute);
        DeleteFormedDrainageCommand = new RelayCommand(OnDeleteFormedDrainageExecuted, CanAllCommandsExecute);
        AddPurposeCommand = new RelayCommand(OnAddPurposeExecuted, CanAllCommandsExecute);
        DeletePurposeCommand = new RelayCommand(OnDeletePurposeExecuted, CanAllCommandsExecute);
        InsertMaterialCommand = new RelayCommand(OnInsertMaterialExecuted, CanAllCommandsExecute);
        SaveChangesCommand = new RelayCommand(OnSaveChangesExecuted, CanAllCommandsExecute);
    }

    public bool CanAllCommandsExecute(object p)
    {
        return true;
    }

    public async void LoadDataAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(LoadDataInternalAsync);
    }

    public async void OnAddFormedDrainageExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(AddFormedDrainageAsync);
    }

    public async void OnDeleteFormedDrainageExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(DeleteFormedDrainageAsync);
    }

    public async void OnAddPurposeExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(AddPurposeAsync);
    }

    public async void OnDeletePurposeExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(DeletePurposeAsync);
    }

    public async void OnInsertMaterialExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(InsertMaterialAsync);
    }

    public async void OnSaveChangesExecuted(object p)
    {
        await ExceptionHelper.SafeExecuteAsync(SaveChangesAsync);
    }

    #region Методы

    private async Task LoadDataInternalAsync()
    {
        var catalog = await _genericEquipRepository.GetAllAsync();
        var formed = await _formedDrainagesRepository.GetAllWithPurposesAsync();

        FormedDrainagesModel.DrainageDetails = new ObservableCollection<Drainage>(catalog);
        FormedDrainagesModel.AllFormedDrainage = new ObservableCollection<FormedDrainage>(formed);
    }

    private async Task AddFormedDrainageAsync()
    {
        var newDrainage = FormedDrainagesModel.CreateNewFormedDrainage();
        await _formedDrainagesRepository.AddAsync(newDrainage);
        FormedDrainagesModel.AllFormedDrainage.Add(newDrainage);
        FormedDrainagesModel.SelectedFormedDrainage = newDrainage;
        FormedDrainagesModel.NewFormedDrainage = new FormedDrainage();
    }

    private async Task DeleteFormedDrainageAsync()
    {
        var selected = FormedDrainagesModel.SelectedFormedDrainage;
        if (selected == null) return;
        await _formedDrainagesRepository.DeleteAsync(selected);
        FormedDrainagesModel.AllFormedDrainage.Remove(selected);
        FormedDrainagesModel.SelectedFormedDrainage = null;
    }

    private async Task AddPurposeAsync()
    {
        var drainage = FormedDrainagesModel.SelectedFormedDrainage;
        if (drainage == null) return;
        var newPurpose = FormedDrainagesModel.CreateNewPurpose();

        if (drainage.Purposes == null)
            drainage.Purposes = new List<DrainagePurpose>();

        drainage.Purposes.Add(newPurpose);
        FormedDrainagesModel.RefreshPurposes();
        FormedDrainagesModel.SelectedPurpose = newPurpose;
        await _formedDrainagesRepository.UpdateAsync(drainage);
    }

    private async Task DeletePurposeAsync()
    {
        var drainage = FormedDrainagesModel.SelectedFormedDrainage;
        var purpose = FormedDrainagesModel.SelectedPurpose;
        if (drainage == null || purpose == null) return;
        drainage.Purposes.Remove(purpose);
        FormedDrainagesModel.RefreshPurposes();
        FormedDrainagesModel.SelectedPurpose = null;
        await _formedDrainagesRepository.UpdateAsync(drainage);
    }

    private async Task InsertMaterialAsync()
    {
        var selectedPurpose = FormedDrainagesModel.SelectedPurpose;
        var selectedMaterial = FormedDrainagesModel.SelectedDrainageDetail;
        if (selectedPurpose != null && selectedMaterial != null)
        {
            selectedPurpose.Material = selectedMaterial.Name;
            FormedDrainagesModel.RefreshPurposes();
            await _formedDrainagesRepository.UpdateAsync(FormedDrainagesModel.SelectedFormedDrainage);
        }
    }

    private async Task SaveChangesAsync()
    {
        if (FormedDrainagesModel.SelectedFormedDrainage != null)
            await _formedDrainagesRepository.UpdateAsync(FormedDrainagesModel.SelectedFormedDrainage);
    }

    #endregion
}