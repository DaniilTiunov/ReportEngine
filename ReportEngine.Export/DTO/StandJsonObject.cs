using ReportEngine.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReportEngine.Export.DTO
{
    public class StandJsonObject
    {
        public int Number { get; set; } // Номер ПП
        public string? KKSCode { get; set; } // Код ККС
        public string? Designation { get; set; } // Обозначение стэнда
        public int Devices { get; set; } // Приборы
        public string? BraceType { get; set; } // Тип крепления датчика
        public float Width { get; set; } // Ширина
        public string? SerialNumber { get; set; } //Серийный номер
        public float Weight { get; set; } // Масса
        public decimal StandSummCost { get; set; } // Сумма стенда
        public string? ObvyazkaType { get; set; } // Тип обвязки
        public int NN { get; set; } // NN
        public string? MaterialLine { get; set; } // Материал линии
        public string? Armature { get; set; } // Арматура
        public string? TreeSocket { get; set; } // Тройник
        public string? KMCH { get; set; } // КМЧ
        public string? Description { get; set; } //Описание
        public string? Comments { get; set; } // Комментарий
        public int? ContainerStandId { get; set; }

        public byte[]? ImageData { get; set; } // Изображение

        public string? ImageType { get; set; } // MIME-тип ("image/png")

        public List<FrameRecordJsonObject> Frames { get; set; } = new List<FrameRecordJsonObject>(); //рамы

        public List<PartRecordJsonObject> FrameParts { get; set; } = new List<PartRecordJsonObject>(); //основные материалы рамы стенда

        public List<PartRecordJsonObject> MountParts { get; set; } = new List<PartRecordJsonObject>(); //монтажные части

        public List<PartRecordJsonObject> DrainageParts { get; set; } = new List<PartRecordJsonObject>(); //дренаж, продувка

        public List<PartRecordJsonObject> ElectricParts { get; set; } = new List<PartRecordJsonObject>(); //электрические компоненты

        public List<string> ImpulseLines { get; set; } = new List<string>(); //импульсные линии в пределах стенда
    }


}

