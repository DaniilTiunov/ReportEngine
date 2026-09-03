using ReportEngine.App.Model.StandsModel;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.App.Services.Converters
{
    public class ConverterService
    {
        private readonly ReAppContext _context;
        public ConverterService(ReAppContext context)
        {
            _context = context;
        }

        public async Task ConvertObvyazkaInStandDataToStandEquips(StandModel stand, ObvyazkaInStand obv)
        {
            stand.MaterialLineEquip = await GetBaseEquip(obv.MaterialLineType, obv.MaterialLineId);
            stand.TreeSocketEquip = await GetBaseEquip(obv.TreeSocketType, obv.TreeSocketId);
            stand.KMCHEquip = await GetBaseEquip(obv.KMCHType, obv.KMCHId);
            stand.ArmatureEquip = await GetBaseEquip(obv.ArmatureType, obv.ArmatureId);
        }



        public void ConvertStandEquipsToObvyazkaInStandData(StandModel stand, ObvyazkaInStand obv)
        {
            obv.MaterialLineId = stand.MaterialLineEquip?.Id;
            obv.MaterialLineType = stand.MaterialLineEquip?.GetType().AssemblyQualifiedName;

            obv.TreeSocketId = stand.TreeSocketEquip?.Id;
            obv.TreeSocketType = stand.TreeSocketEquip?.GetType().AssemblyQualifiedName;

            obv.KMCHId = stand.KMCHEquip?.Id;
            obv.KMCHType = stand.KMCHEquip?.GetType().AssemblyQualifiedName;

            obv.ArmatureId = stand.ArmatureEquip?.Id;
            obv.ArmatureType = stand.ArmatureEquip?.GetType().AssemblyQualifiedName;
        }



        private async Task<IBaseEquip?> GetBaseEquip(string? typeName, int? id)
        {
            if (string.IsNullOrEmpty(typeName) || !id.HasValue)
                return null;

            var entityType = Type.GetType(typeName);

            if (entityType == null)
                return null;

            return await _context.FindAsync(entityType, id) as IBaseEquip;
        }

    }
}
