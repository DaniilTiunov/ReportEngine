namespace ReportEngine.Domain.Entities.BaseEntities.Interface;

public interface IBaseEquip
{
    int Id { get; set; }
    string Name { get; set; }
    string Measure { get; set; }
    float Cost { get; set; }
    int ExportDays { get; set; }
}
