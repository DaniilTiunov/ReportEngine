using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class ContainerStand
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // Привязка к проекту (опционально)
    public int ProjectInfoId { get; set; }

    [ForeignKey(nameof(ProjectInfoId))] public virtual ProjectInfo? Project { get; set; }

    // Код/имя упаковки (маркировка)
    public string? Name { get; set; }

    // Кол-во стендов в упаковке
    public int StandsCount { get; set; }

    // Суммарная масса стендов в упаковке (кг)
    public float StandsWeight { get; set; }

    // Вес контейнера/ящика (кг)
    public float? ContainerWeight { get; set; }

    // Описание
    public string? Description { get; set; }

    public float? ContainerCost { get; set; }

    // Ссылка на партию (batch)
    public int? ContainerBatchId { get; set; }

    [ForeignKey(nameof(ContainerBatchId))] public virtual ContainerBatch? Batch { get; set; }

    // Список стендов внутри упаковки
    public virtual ICollection<Stand> Stands { get; set; } = new List<Stand>();
}
