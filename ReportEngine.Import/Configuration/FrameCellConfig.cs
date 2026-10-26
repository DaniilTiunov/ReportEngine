using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Import.Configuration;

public class FrameCellConfig
{
    public string ColumnName { get; set; } = "B";
    public string ColumnFrameType { get; set; } = "C";
    public string ColumnWidth { get; set; } = "D";
    public string ColumnHeight { get; set; } = "E";
    public string ColumnDepth { get; set; } = "F";
    public string ColumnWeight { get; set; } = "G";
    public string ColumnDescription { get; set; } = "H";
}
