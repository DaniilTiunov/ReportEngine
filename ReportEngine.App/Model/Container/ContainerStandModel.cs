using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReportEngine.App.Model.Container;

public class ContainerStandModel
{
    public int Id { get; set; }

    // Привязка к проекту
    public int ProjectInfoId { get; set; }

    // Код/имя упаковки (маркировка)
    public string? Name { get; set; }

    // Кол-во стендов в упаковке
    public int StandsCount { get; set; }

    // Суммарная масса стендов в упаковке (кг)
    public float StandsWeight { get; set; }

    // Вес пустого контейнера/ящика (кг)
    public float? ContainerWeight { get; set; }

    // Описание
    public string? Description { get; set; }

    // Ссылка на партию (batch)
    public int? ContainerBatchId { get; set; }

    // Список идентификаторов стендов, находящихся в упаковке
    public IList<int> StandIds { get; set; } = new List<int>();
}
