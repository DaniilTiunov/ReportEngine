namespace ReportEngine.Import.Dto;

public class StandDto
{
    public string? Number { get; set; } // Нопер ПП
    public string? KKSCode { get; set; } // Код ККС
    public string? Design { get; set; } // Обозначение стэнда
    public string? Devices { get; set; } // Приборы
    public string? BraceType { get; set; } // Тип крепления датчика
    public string? Width { get; set; } // Ширна
    public string? SerialNumber { get; set; } //Серийный номер
    public string? Weight { get; set; } // Масса
    public string? StandSummCost { get; set; } // Сумма стенда
    public string? ObvyazkaType { get; set; } // Тип обвязки
    public string? NN { get; set; } // NN
    public string? MaterialLine { get; set; } // Материал линии
    public string? Armature { get; set; } // Араматура
    public string? TreeSocket { get; set; } // Тройник
    public string? KMCH { get; set; } // КМЧ
    public string? DesigneStand { get; set; } //Описание
    public string? Comments { get; set; } // Комментарий
    public byte[]? ImageData { get; set; } // Изображение
}