using ReportEngine.App.Model;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.ModelWrappers
{
    public static class StandDataConverter
    {
        public static StandModel ConvertToStandModel(Stand stand)
        {
            return new StandModel
            {
                Id = stand.Id,
                ProjectId = stand.ProjectInfoId, // обязательно копируем ProjectId
                KKSCode = stand.KKSCode,
                Design = stand.Design,
                BraceType = stand.BraceType,
                Devices = stand.Devices,
                Width = stand.Width,
                SerialNumber = stand.SerialNumber,
                Weight = stand.Weight,
                StandSummCost = stand.StandSummCost,
                ObvyazkaType = stand.ObvyazkaType,
                NN = stand.NN,
                MaterialLine = stand.MaterialLine,
                Armature = stand.Armature,
                TreeSocket = stand.TreeSocket,
                KMCH = stand.KMCH,
            };
        }
        public static Stand ConvertToStandEntity(StandModel model)
        {
            return new Stand
            {
                Id = model.Id,
                ProjectInfoId = model.ProjectId, // обязательно копируем ProjectId
                KKSCode = model.KKSCode,
                Design = model.Design,
                BraceType = model.BraceType,
                Devices = model.Devices,
                Width = model.Width,
                SerialNumber = model.SerialNumber,
                Weight = model.Weight,
                StandSummCost = model.StandSummCost,
                ObvyazkaType = model.ObvyazkaType,
                NN = model.NN,
                MaterialLine = model.MaterialLine,
                Armature = model.Armature,
                TreeSocket = model.TreeSocket,
                KMCH = model.KMCH,
            };
        }
    }
}
