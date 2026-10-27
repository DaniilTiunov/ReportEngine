namespace ReportEngine.Import.Dto;

public class FrameDto
{
    //Наименование
    public string Name { get; set; } = "";

    //Тип рамы
    public string FrameType { get; set; } = "";

    //Ширина
    public string Width { get; set; } = "";

    //Высота
    public string Height { get; set; } = "";

    //Глубина
    public string Depth { get; set; } = "";

    //Вес
    public string Weight { get; set; } = "";

    //Описание по КД
    public string Description { get; set; } = "";


    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Name)
               && string.IsNullOrWhiteSpace(FrameType)
               && string.IsNullOrWhiteSpace(Width)
               && string.IsNullOrWhiteSpace(Height)
               && string.IsNullOrWhiteSpace(Depth)
               && string.IsNullOrWhiteSpace(Weight)
               && string.IsNullOrWhiteSpace(Description);
    }
}