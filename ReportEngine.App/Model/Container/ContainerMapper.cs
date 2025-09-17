using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Model.Container;

public static class ContainerMapper
{
    public static ContainerBatch ToEntity(this ContainerBatchModel model)
    {
        if (model == null) return null;

        var entity = new ContainerBatch
        {
            Id = model.Id,
            ProjectInfoId = model.ProjectInfoId,
            BatchOrder = model.BatchOrder,
            Name = model.Name,
            ContainersCount = model.ContainersCount,
            StandsCount = model.StandsCount
        };

        if (model.Containers != null && model.Containers.Any())
        {
            foreach (var c in model.Containers)
            {
                var containerEntity = c.ToEntity();
                containerEntity.ContainerBatchId = model.Id == 0 ? null : model.Id;
                entity.Containers.Add(containerEntity);
            }
        }

        return entity;
    }

    public static ContainerBatchModel ToModel(this ContainerBatch entity)
    {
        if (entity == null) return null;

        var model = new ContainerBatchModel
        {
            Id = entity.Id,
            ProjectInfoId = entity.ProjectInfoId,
            BatchOrder = entity.BatchOrder,
            Name = entity.Name,
            ContainersCount = entity.Containers?.Count ?? 0,
            StandsCount = entity.Containers?.Sum(c => c.StandsCount) ?? 0
        };

        if (entity.Containers != null && entity.Containers.Any())
        {
            model.Containers = entity.Containers.Select(c => c.ToModel()).ToList();
        }

        return model;
    }

    // Container: Model -> Entity
    public static ContainerStand ToEntity(this ContainerStandModel model)
    {
        if (model == null) return null;

        var entity = new ContainerStand
        {
            Id = model.Id,
            ProjectInfoId = model.ProjectInfoId,
            Name = model.Name,
            StandsCount = model.StandsCount,
            StandsWeight = model.StandsWeight,
            ContainerWeight = model.ContainerWeight,
            Description = model.Description,
            ContainerBatchId = model.ContainerBatchId
        };

        // stands are not materialized here (we keep only ids in model). Repository will attach stands by ids.
        return entity;
    }

    // Container: Entity -> Model
    public static ContainerStandModel ToModel(this ContainerStand entity)
    {
        if (entity == null) return null;

        var model = new ContainerStandModel
        {
            Id = entity.Id,
            ProjectInfoId = entity.ProjectInfoId,
            Name = entity.Name,
            StandsCount = entity.StandsCount,
            StandsWeight = entity.StandsWeight,
            ContainerWeight = entity.ContainerWeight,
            Description = entity.Description,
            ContainerBatchId = entity.ContainerBatchId,
            StandIds = entity.Stands?.Select(s => s.Id).ToList() ?? new List<int>()
        };

        return model;
    }
}
