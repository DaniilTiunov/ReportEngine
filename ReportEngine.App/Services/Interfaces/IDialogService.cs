using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ViewModels;
using ReportEngine.App.ViewModels.DTO;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.App.Services.Interfaces;

public interface IDialogService
{
    T? ShowEquipDialog<T>()
        where T : class, IBaseEquip, new();

    Stand ShowSelectStandDialog();

    Obvyazka? ShowObvyazkaDialog(bool dialogMode);

    IBaseEquip? ShowAllSortamentsDialog();

    string ShowCompanyDialog();

    string ShowSubjectDialog();

    FormedFrame ShowFrameDialog();

    public RenumerationInfo ShowRenumerateDialog();

    public int ShowStandCopyDialog();

    void ShowObvSettingsWindow(ProjectViewModel projectViewModel);

    void ShowEditObvSettingsWindow(ProjectViewModel projectViewModel, StandModel standModel,
        ObvyazkaInStand selectedObvyazka);

    void ShowStandsSettingsWindow(ProjectViewModel projectViewModel, bool editMode);

    void ShowEditStandsObvSettingsWindow(ProjectViewModel projectViewModel, StandModel standModel, bool editMode);

    void RunWithProgressDialog(Action action);

    Task RunWithProgressDialogAsync(Func<Task> action);
}
