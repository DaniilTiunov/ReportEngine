using System.Collections.ObjectModel;
using System.Windows.Input;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Model.Contacts;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels.Contacts;

public class SubjectViewModel
{
    private readonly IBaseRepository<Subject> _subjectsRepository;
    private readonly INotificationService _notificationService;

    public SubjectViewModel(IBaseRepository<Subject> subjectsRepository,
        INotificationService notificationService)
    {
        InitializeCommands();
        
        _subjectsRepository = subjectsRepository;
        _notificationService = notificationService;
    }
    
    public Action<string> SelectedItem { get; set; }
    public SubjectModel CurrentSubject { get; set; } = new();
    public ICommand LoadAllSubjectsCommand { get; set; }
    public ICommand AddNewSubjectCommand { get; set; }
    public ICommand SaveChangesCommand { get; set; }
    public ICommand DeleteSubjectCommand { get; set; }
    
    public void InitializeCommands()
    {
        LoadAllSubjectsCommand = new RelayCommand(OnLoadAllSubjectsExecuted, CanAllCommandsExecute);
        AddNewSubjectCommand = new RelayCommand(OnAddNewSubjectCommandExecuted, CanAllCommandsExecute);
        SaveChangesCommand = new RelayCommand(OnSaveChangesCommandExecuted, CanAllCommandsExecute);
        DeleteSubjectCommand = new RelayCommand(OnDeleteSubjectCommandExecuted, CanAllCommandsExecute);
    }

    public bool CanAllCommandsExecute(object e)
    {
        return true;
    }

    public async void OnLoadAllSubjectsExecuted(object p)
    {
        await LoadAllSubjectsAsync();
    }

    public async void OnAddNewSubjectCommandExecuted(object p)
    {
        await AddNewSubjectAsync();
    }

    public async void OnSaveChangesCommandExecuted(object p)
    {
        await SaveChangesAsync();
    }

    public async void OnDeleteSubjectCommandExecuted(object p)
    {
        await DeleteSelectedSubjectAsync();
    }

    public async Task LoadAllSubjectsAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var subjects = await _subjectsRepository.GetAllAsync();
            CurrentSubject.AllSubjects = new ObservableCollection<Subject>(subjects);
        });
    }

    private async Task AddNewSubjectAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            var newSubject = CurrentSubject.CreateNewSubject();
            CurrentSubject.AllSubjects.Add(newSubject);
            await _subjectsRepository.AddAsync(newSubject);
        });
    }

    private async Task SaveChangesAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            if (CurrentSubject.SelectedSubject != null)
            {
                await _subjectsRepository.UpdateAsync(CurrentSubject.SelectedSubject);
                _notificationService.ShowInfo("Изменения сохранены");
            }
        });
    }

    private async Task DeleteSelectedSubjectAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            if (CurrentSubject.SelectedSubject != null)
            {
                await _subjectsRepository.DeleteAsync(CurrentSubject.SelectedSubject);
                CurrentSubject.AllSubjects.Remove(CurrentSubject.SelectedSubject);
                CurrentSubject.SelectedSubject = null;
            }
        });
    }
}