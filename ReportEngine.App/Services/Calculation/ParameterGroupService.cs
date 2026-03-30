using System.Collections.ObjectModel;
using Mapster;
using ReportEngine.App.Extensions;
using ReportEngine.App.Model.CalculationModels;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities.CalculationParameters;
using ReportEngine.Domain.Repositories;

namespace ReportEngine.App.Services.Calculation;

public class ParameterGroupService
{
    private readonly CalculationRepository _calculationRepository;

    public ParameterGroupService(CalculationRepository calculationRepository)
    {
        _calculationRepository = calculationRepository;
    }

    public async Task<ObservableCollection<CalculationParameter>> GetParametersAsync(
        CalculationParameterType parametersType)
    {
        var entity = await _calculationRepository.GetGroupByTypeAsync(parametersType);

        return entity.Parameters.ToObservable();
    }

    public async Task UpdateGroupAsync(CalculationParameterType parametersType)
    {

    }

    public async Task AddNewParameterAsync(CalculationParameter parameter, CalculationParameterType parametersType)
    {

    }

    public async Task RemoveParameterFromGroupAsync(CalculationParameter parameter,
        CalculationParameterType parametersType)
    {

    }
}
