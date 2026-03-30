using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Domain.Entities.CalculationParameters
{
    public enum CalculationParameterType
    {
        HumanCost, //Трудозатраты
        ElectricCost,  //Электрика
        SandBlastCost, //Пескоструй
        FrameCost, // Рама
        StandCost, // Стенды
        Equipments, // Комплектующие
    }

}
