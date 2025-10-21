using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.App.Services.Interfaces;

public interface IDialogService
{
    T? ShowEquipDialog<T>()
        where T : class, IBaseEquip, new();

    Obvyazka? ShowObvyazkaDialog();

    IBaseEquip? ShowAllSortamentsDialog();

    string ShowCompanyDialog();
}