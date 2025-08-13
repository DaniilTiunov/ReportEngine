namespace ReportEngine.App.Model.StandsModel
{
    public class StandFrameModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public StandFrameModel() { }

        public StandFrameModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
