using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.AppHelpers
{
    public static class StandsListHelper
    {
        public static List<Stand> SelectedStands {  get; set; } = new();

        public static List<Stand> ReturnSelectedStands(IEnumerable<StandModel> standsModels)
        {
            var standsEntities = new List<Stand>();

            foreach (var standModel in standsModels)
            {
                var standEntity = StandDataConverter.ConvertToStandEntity(standModel);

                standsEntities.Add(standEntity);
            }
            
            return standsEntities;
        }

        public static void GetSelectedStands(IEnumerable<StandModel> standsModels)
        {
            SelectedStands.Clear();

            foreach (var standModel in standsModels)
            {
                SelectedStands.Add(StandDataConverter.ConvertToStandEntity(standModel));
            }

            var popa = SelectedStands;
        }
    }
}
