using System.Collections.ObjectModel;
using Mapster;
using ReportEngine.App.Extensions;
using ReportEngine.App.Model.CalculationModels;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities.CalculationParameters;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;
using ReportEngine.Domain.Repositories;

namespace ReportEngine.App.Services.Calculation;

public class ParameterGroupService
{
    private readonly CalculationRepository _calculationRepository;
    private readonly INotificationService _notificationService;

    public ParameterGroupService(
        CalculationRepository calculationRepository,
        INotificationService notificationService)
    {
        _calculationRepository = calculationRepository;
        _notificationService = notificationService;
    }

    public async Task<ObservableCollection<CalculationParameter>> GetParametersAsync(
        CalculationParameterType parametersType)
    {
        var entity = await _calculationRepository.GetGroupByTypeAsync(parametersType);

        return entity.Parameters.ToObservable();
    }

    public async Task UpdateGroupAsync(CalculationParameterType parametersType, IEnumerable<CalculationParameter> updatedParameters)
    {
        await _calculationRepository.UpdateParametersInGroup(parametersType, updatedParameters);
        _notificationService.ShowInfo("Параметры успешно обновлены!");
    }

    public async Task AddNewParameterAsync(CalculationParameter parameter, CalculationParameterType parametersType)
    {
        await _calculationRepository.AddParameterToGroup(parameter, parametersType);
        _notificationService.ShowInfo("Параметр успешно добавлен");
    }

    public async Task RemoveParameterFromGroupAsync(CalculationParameter parameter,
        CalculationParameterType parametersType)
    {
        await _calculationRepository.DeleteParameterFromGroup(parameter, parametersType);
        _notificationService.ShowInfo("Параметр успешно удалён");
    }
}
