using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities
{
    public class ContainerStand
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Внешний ключ на проект (опционально — контейнер привязан к проекту)
        public int ProjectInfoId { get; set; }

        [ForeignKey(nameof(ProjectInfoId))]
        public virtual ProjectInfo Project { get; set; }

        // Имя ящика/контейнера
        public string? Name { get; set; }

        // Дополнительное описание (при необходимости)
        public string? Description { get; set; }

        // Набор стендов внутри этого контейнера
        public virtual ICollection<Stand> Stands { get; set; } = new List<Stand>();
    }
}
