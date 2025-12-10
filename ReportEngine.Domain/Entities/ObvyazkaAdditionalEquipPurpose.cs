using ReportEngine.Domain.Entities.BaseEntities.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Domain.Entities
{
    public class ObvyazkaAdditionalEquipPurpose : IPurposeEntity
    {
        public string? Purpose { get; set; }
        public string? Material { get; set; }
        public float? Quantity { get; set; }
        public float? CostPerUnit { get; set; }
        public string? Measure { get; set; }
        public int? ExportDays { get; set; }

        [Key] public int Id { get; set; }
    }
}
