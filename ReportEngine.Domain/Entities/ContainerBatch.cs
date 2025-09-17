using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities
{
    public class ContainerBatch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // Внешний ключ на проект (опционально — контейнер привязан к проекту)
        public int ProjectInfoId { get; set; }
        [ForeignKey(nameof(ProjectInfoId))]
        public virtual ProjectInfo Project { get; set; }
        public int? ContainersCount { get; set; }
        public int? StandsCount { get; set; }
        // Набор упаковок внутри этого контейнера
        public virtual ICollection<ContainerStand> Containers { get; set; } = new List<ContainerStand>();
    }
}
