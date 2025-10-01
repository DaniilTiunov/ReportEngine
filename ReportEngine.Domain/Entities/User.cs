using ReportEngine.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportEngine.Domain.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? SecondName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneContact { get; set; }
    public string? Email { get; set; }
    public string? Position { get; set; }
    public string? Cabinet { get; set; }

    public string? UserLogin { get; set; }
    public SystemRole? SystemRole { get; set; }
}