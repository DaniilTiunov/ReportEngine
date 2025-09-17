using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReportEngine.App.Model.Container;

public class ContainerBatchModel
{
    public int Id { get; set; }

    // Привязка к проекту
    public int ProjectInfoId { get; set; }

    // Порядок в очереди (1 = первая, 2 = вторая и т.д.)
    public int BatchOrder { get; set; }

    // Имя/описание партии
    public string? Name { get; set; }

    // Всего контейнеров в партии
    public int ContainersCount { get; set; }

    // Общее кол-во стендов во всех контейнерах партии
    public int StandsCount { get; set; }

    // Контейнеры в партии (без полной развёрнутой информации по стендам,
    // в модели хранится список id стендов в контейнере)
    public IList<ContainerStandModel> Containers { get; set; } = new List<ContainerStandModel>();
}
