using System.Diagnostics;

namespace ReportEngine.Shared.Utils;

public static class PerformanceTimer
{
    public static void Measure(string name, Action action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        var sw = Stopwatch.StartNew();
        try
        {
            action();
        }
        finally
        {
            sw.Stop();
            Debug.WriteLine($"{name} выполнено за {sw.ElapsedMilliseconds} мс");
        }
    }

    public static async Task MeasureAsync(string name, Func<Task> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        var sw = Stopwatch.StartNew();
        try
        {
            await action().ConfigureAwait(false);
        }
        finally
        {
            sw.Stop();
            Debug.WriteLine($"{name} выполнено за {sw.ElapsedMilliseconds} мс");
        }
    }
}