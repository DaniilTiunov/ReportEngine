using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Shared.Helpers
{
    public static class FloatExtensions
    {
        public static float Ceiling(this float value)
        {
            return MathF.Ceiling(value);
        }

        public static float Round(this float value, int decimals)
        {
            return MathF.Round(value, decimals);
        }

        public static float? Ceiling(this float? value)
        {
            return value.HasValue ? MathF.Ceiling(value.Value) : null;
        }

        public static float? Floor(this float? value)
        {
            return value.HasValue ? MathF.Floor(value.Value) : null;
        }

        public static float? Round(this float? value, int decimals)
        {
            return value.HasValue ? (float)MathF.Round(value.Value, decimals) : null;
        }

    }
}
