using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Domain.Entities.CalculationParameters
{
    public class CalculationParameterGroup
    {
        [Key] public int Id { get; set; }
        public CalculationParameterType SettingsType { get; set; }

        public string? Name {  get; set; }

        public List<CalculationParameter> Parameters { get; set; } = new List<CalculationParameter>();

    }
}
