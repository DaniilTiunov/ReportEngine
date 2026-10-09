using System.Globalization;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Background;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.App.Services.Calculation;

public class UpdaterStandService
{
    private readonly ReAppContext _context;
    private readonly INotificationService _notificationService;
    private readonly IStandService _standService;

    public UpdaterStandService(
        ReAppContext context,
        IStandService standService,
        INotificationService notificationService)
    {
        _context = context;
        _standService = standService;
        _notificationService = notificationService;
    }

    public async Task ApplyChangesAndSaveAsync(ProjectModel project)
    {
        var changes = await GetUnprocessedChangesAsync();
        if (changes.Count == 0)
            return;

        var equipmentChanges = new List<EquipmentChange>(changes.Count);
        foreach (var change in changes)
            equipmentChanges.Add(new EquipmentChange(change, await FindChangedEquipmentAsync(change)));

        foreach (var stand in project.Stands)
            ApplyChangesToStand(stand, equipmentChanges);

        foreach (var target in await LoadStoredSnapshotsAsync(project))
        foreach (var change in equipmentChanges)
            ApplyChangesToObject(target, change);

        foreach (var change in changes)
            change.Processed = true;

        await _context.SaveChangesAsync();

        // Перезагружаем коллекции только после сохранения снимков в БД.
        await _standService.LoadObvyazkiInStandsAsync(project.Stands);
        await _standService.LoadStandsDataAsync(project.Stands);
        await _standService.LoadPurposesInStands(project.Stands);

        _notificationService.ShowInfo("Данные комплектующих обновлены!");
    }

    private static void ApplyChangesToStand(StandModel stand, IEnumerable<EquipmentChange> changes)
    {
        foreach (var change in changes)
        {
            ApplyChangesToObject(stand, change);

            foreach (var purpose in stand.AllAdditionalEquipPurposesInStand)
                ApplyChangesToObject(purpose, change);
            foreach (var purpose in stand.ObvyazkaAdditionalComponents)
                ApplyChangesToObject(purpose, change);
            foreach (var purpose in stand.AllElectricalPurposesInStand)
                ApplyChangesToObject(purpose, change);
            foreach (var purpose in stand.AllDrainagePurposesInStand)
                ApplyChangesToObject(purpose, change);
            foreach (var obvyazka in stand.ObvyazkiInStand)
                ApplyChangesToObject(obvyazka, change);
            foreach (var component in stand.FramesInStand.SelectMany(frame => frame.Components))
                ApplyChangesToObject(component, change);
        }
    }

    private async Task<List<object>> LoadStoredSnapshotsAsync(ProjectModel project)
    {
        var standIds = project.Stands.Select(stand => stand.Id).Distinct().ToArray();
        var obvyazkaIds = project.Stands.SelectMany(stand => stand.ObvyazkiInStand)
            .Select(item => item.Id).Where(id => id != 0).Distinct().ToArray();
        var additionalPurposeIds = project.Stands.SelectMany(stand => stand.AllAdditionalEquipPurposesInStand)
            .Select(item => item.Id).Where(id => id != 0).Distinct().ToArray();
        var electricalPurposeIds = project.Stands.SelectMany(stand => stand.AllElectricalPurposesInStand)
            .Select(item => item.Id).Where(id => id != 0).Distinct().ToArray();
        var drainagePurposeIds = project.Stands.SelectMany(stand => stand.AllDrainagePurposesInStand)
            .Select(item => item.Id).Where(id => id != 0).Distinct().ToArray();
        var obvyazkaPurposeIds = project.Stands.SelectMany(stand => stand.ObvyazkaAdditionalComponents)
            .Select(item => item.Id).Where(id => id != 0).Distinct().ToArray();
        var frameComponentIds = project.Stands.SelectMany(stand => stand.FramesInStand)
            .SelectMany(frame => frame.Components).Select(item => item.Id)
            .Where(id => id != 0).Distinct().ToArray();
        var containerIds = project.ContainerStandsInProject.Select(item => item.Id)
            .Where(id => id != 0).Distinct().ToArray();

        var result = new List<object>();
        result.AddRange(await _context.Stands.Where(item => standIds.Contains(item.Id)).ToListAsync());
        result.AddRange(await _context.ObvyazkiInStands.Where(item => obvyazkaIds.Contains(item.Id)).ToListAsync());
        result.AddRange(await _context.AdditionalEquipPurposes
            .Where(item => additionalPurposeIds.Contains(item.Id)).ToListAsync());
        result.AddRange(await _context.ElectricalPurposes
            .Where(item => electricalPurposeIds.Contains(item.Id)).ToListAsync());
        result.AddRange(await _context.DrainagePurposes
            .Where(item => drainagePurposeIds.Contains(item.Id)).ToListAsync());
        result.AddRange(await _context.ObvyazkaAdditionalEquipPurpose
            .Where(item => obvyazkaPurposeIds.Contains(item.Id)).ToListAsync());
        result.AddRange(await _context.FrameComponents
            .Where(item => frameComponentIds.Contains(item.Id)).ToListAsync());
        result.AddRange(await _context.ContainersStand
            .Where(item => containerIds.Contains(item.Id)).ToListAsync());

        return result;
    }

    private async Task<IBaseEquip?> FindChangedEquipmentAsync(TablesChanges change)
    {
        if (!change.EquipId.HasValue)
            return null;

        var entityType = _context.Model.GetEntityTypes()
            .FirstOrDefault(type =>
                typeof(IBaseEquip).IsAssignableFrom(type.ClrType) &&
                (string.Equals(type.GetTableName(), change.TableName, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(type.ClrType.Name, change.TableName, StringComparison.OrdinalIgnoreCase)));

        if (entityType == null)
            return null;

        return await _context.FindAsync(entityType.ClrType, change.EquipId.Value) as IBaseEquip;
    }

    private static void ApplyChangesToObject(object target, EquipmentChange change)
    {
        if (target is IBaseEquip equipment && MatchesEquipment(equipment, change))
        {
            equipment.Name = GetNewName(change) ?? equipment.Name;
            equipment.Cost = GetNewCost(change) ?? equipment.Cost;
            equipment.Weight = change.Equipment?.Weight ?? equipment.Weight;
            equipment.Measure = change.Equipment?.Measure ?? equipment.Measure;
            equipment.ExportDays = change.Equipment?.ExportDays ?? equipment.ExportDays;
        }

        ApplySnapshot(target, change, "Material", "CostPerUnit", weightProperty: "Weight",
            measureProperty: "Measure", exportDaysProperty: "ExportDays");
        ApplySnapshot(target, change, "ComponentName", "CostComponent", "ComponentId",
            measureProperty: "Measure", exportDaysProperty: "ExportDays");
        ApplySnapshot(target, change, "Name", "ContainerCost", weightProperty: "ContainerWeight");

        ApplySnapshot(target, change, "MaterialLine", "MaterialLineCostPerUnit", "MaterialLineId",
            measureProperty: "MaterialLineMeasure", exportDaysProperty: "MaterialLineExportDays");
        ApplySnapshot(target, change, "Armature", "ArmatureCostPerUnit", "ArmatureId",
            measureProperty: "ArmatureMeasure", exportDaysProperty: "ArmatureExportDays");
        ApplySnapshot(target, change, "TreeSocket", "TreeSocketMaterialCostPerUnit", "TreeSocketId",
            measureProperty: "TreeSocketMaterialMeasure", exportDaysProperty: "TreeSocketExportDays");
        ApplySnapshot(target, change, "KMCH", "KMCHCostPerUnit", "KMCHId",
            measureProperty: "KMCHMeasure", exportDaysProperty: "KMCHExportDays");
    }

    private static void ApplySnapshot(
        object target,
        EquipmentChange change,
        string nameProperty,
        string costProperty,
        string? equipmentIdProperty = null,
        string? weightProperty = null,
        string? measureProperty = null,
        string? exportDaysProperty = null)
    {
        var type = target.GetType();
        var name = type.GetProperty(nameProperty);
        if (name?.CanRead != true || name.CanWrite != true)
            return;

        var currentName = name.GetValue(target) as string;
        var cost = type.GetProperty(costProperty);
        var currentCost = TryGetFloat(cost?.GetValue(target));

        var idMatches = false;
        if (equipmentIdProperty != null && change.Change.EquipId.HasValue)
        {
            var id = type.GetProperty(equipmentIdProperty)?.GetValue(target);
            idMatches = id != null && Convert.ToInt32(id) == change.Change.EquipId.Value;
        }

        var nameMatches = !string.IsNullOrEmpty(change.Change.OldName) &&
                          string.Equals(currentName, change.Change.OldName, StringComparison.Ordinal) &&
                          (!change.Change.OldCost.HasValue || cost == null || currentCost == change.Change.OldCost);
        var costMatches = string.IsNullOrEmpty(change.Change.OldName) &&
                          change.Change.OldCost.HasValue && currentCost == change.Change.OldCost;

        if (!idMatches && !nameMatches && !costMatches)
            return;

        name.SetValue(target, GetNewName(change) ?? currentName);
        SetValue(target, cost, GetNewCost(change));
        SetValue(target, type.GetProperty(weightProperty ?? string.Empty), change.Equipment?.Weight);
        SetValue(target, type.GetProperty(measureProperty ?? string.Empty), change.Equipment?.Measure);
        SetValue(target, type.GetProperty(exportDaysProperty ?? string.Empty), change.Equipment?.ExportDays);
    }

    private static bool MatchesEquipment(IBaseEquip equipment, EquipmentChange change)
    {
        if (change.Change.EquipId.HasValue && equipment.Id != change.Change.EquipId.Value)
            return false;

        return string.IsNullOrEmpty(change.Change.OldName) ||
               string.Equals(equipment.Name, change.Change.OldName, StringComparison.Ordinal);
    }

    private static string? GetNewName(EquipmentChange change)
    {
        return change.Equipment?.Name ?? change.Change.NewName;
    }

    private static float? GetNewCost(EquipmentChange change)
    {
        return change.Equipment?.Cost ?? change.Change.NewCost;
    }

    private static float? TryGetFloat(object? value)
    {
        if (value == null)
            return null;
        if (value is float number)
            return number;
        if (float.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.CurrentCulture, out number))
            return number;
        return float.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out number)
            ? number
            : null;
    }

    private static void SetValue(object target, PropertyInfo? property, object? value)
    {
        if (property?.CanWrite != true || value == null)
            return;

        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        var converted = targetType == typeof(string)
            ? Convert.ToString(value, CultureInfo.CurrentCulture)
            : Convert.ChangeType(value, targetType, CultureInfo.CurrentCulture);
        property.SetValue(target, converted);
    }

    private async Task<List<TablesChanges>> GetUnprocessedChangesAsync()
    {
        return await _context.TablesChanges
            .Where(change => change.Processed == false)
            .OrderBy(change => change.ChangedAt)
            .ToListAsync();
    }

    private sealed record EquipmentChange(TablesChanges Change, IBaseEquip? Equipment);
}
