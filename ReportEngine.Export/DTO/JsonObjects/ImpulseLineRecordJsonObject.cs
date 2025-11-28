using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Export.DTO.JsonObjects
{



    public class WireRecord {
        public string? Circuit {  get; set; }
        public string? Mark { get; set; }
        public string? ElectricBox { get; set; }
        public string? Terminal { get; set; }
        public WireRecord(string circuit, string mark, string electricBox, string terminal) 
        {
            Circuit = circuit;
            Mark = mark;
            ElectricBox = electricBox;
            Terminal = terminal;
        }
    }



    public class ImpulseLineRecordJsonObject
    {
        public string? Name { get; set; }
        public string? CodeKKS { get; set; }
        public List<WireRecord> Wires { get; set; } = new List<WireRecord>();
        public string? Annotation {  get; set; }
    }
}
