using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities
{
    public class ContainerBatch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Привязка к проекту
        public int ProjectInfoId { get; set; }

        [ForeignKey(nameof(ProjectInfoId))]
        public virtual ProjectInfo? Project { get; set; }

        // Порядок в очереди (1 = первая, 2 = вторая и т.д.)
        public int BatchOrder { get; set; }

        // Всего контейнеров в партии (считаем синхронно при изменениях)
        public int ContainersCount { get; set; }

        // Общее кол-во стендов во всех контейнерах партии (считается при изменениях)
        public int StandsCount { get; set; }

        // Доп. поле-описание/имя очереди
        public string? Name { get; set; }

        // Коллекция контейнеров (упаковок) в этой партии
        public virtual ICollection<ContainerStand> Containers { get; set; } = new List<ContainerStand>();
    }
}