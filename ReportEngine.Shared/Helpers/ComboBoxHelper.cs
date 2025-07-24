using ReportEngine.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Shared.Helpers
{
    public class ComboBoxHelper
    {
        public static TEnum ComboBoxChangedValue<TEnum>(string status) where TEnum : Enum
        {
            return (TEnum)Enum.Parse(typeof(TEnum), status);
        }
    }
}
