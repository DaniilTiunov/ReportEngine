using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Export.Formatting.Interfaces
{
    public interface IUserFormat
    {
        string ToStringFull(string format);
        string ToStringShort(string format);
        

    }
}
