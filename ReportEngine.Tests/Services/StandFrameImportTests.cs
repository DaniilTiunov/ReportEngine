using System.Collections.ObjectModel;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ModelWrappers;
using ReportEngine.Domain.Entities;

namespace ReportEngine.Tests.Services;

[TestClass]
public class StandFrameImportTests
{
    [TestMethod]
    public void ConvertForImport_ExcludesFrameGraphAndPreservesModelAndObvyazki()
    {
        var frame = new FormedFrame { Name = "Рама из Excel" };
        var obvyazka = new ObvyazkaInStand { ObvyazkaId = 42 };
        var model = new StandModel
        {
            FramesInStand = new ObservableCollection<FormedFrame> { frame }
        };
        model.ObvyazkiInStand.Add(obvyazka);

        var entity = StandDataConverter.ConvertToStandEntity(model, includeFrames: false);

        Assert.AreEqual(0, entity.StandFrames.Count);
        Assert.AreEqual(1, model.FramesInStand.Count);
        Assert.AreSame(frame, model.FramesInStand.Single());
        Assert.AreSame(obvyazka, entity.ObvyazkiInStand.Single());
    }

    [TestMethod]
    public void ConvertByDefault_PreservesExistingFrameMapping()
    {
        var frame = new FormedFrame { Id = 12, Name = "Рама из справочника" };
        var model = new StandModel
        {
            FramesInStand = new ObservableCollection<FormedFrame> { frame }
        };

        var entity = StandDataConverter.ConvertToStandEntity(model);

        Assert.AreEqual(12, entity.StandFrames.Single().FrameId);
        Assert.AreSame(frame, entity.StandFrames.Single().Frame);
    }
}
