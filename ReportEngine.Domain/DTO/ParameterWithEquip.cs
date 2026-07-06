using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.CalculationParameters;

namespace ReportEngine.Domain.DTO;

public class ParameterWithEquip
{
    public CalculationParameter? Parameter { get; set; }
    public IBaseEquip? Equipment { get; set; }
}
