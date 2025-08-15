using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class Company
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int Number { get; set; } // Номер
    public string? Name { get; set; } // Наименование
    public DateOnly? RegisterDate { get; set; } // Дата регистрации
}