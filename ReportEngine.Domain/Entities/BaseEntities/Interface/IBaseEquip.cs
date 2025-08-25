namespace ReportEngine.Domain.Entities.BaseEntities.Interface;

public interface IBaseEquip
{
    int Id { get; set; }
    string Name { get; set; }
    float Cost { get; set; }
}