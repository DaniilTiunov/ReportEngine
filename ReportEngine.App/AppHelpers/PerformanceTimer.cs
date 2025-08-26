using System.Diagnostics;

namespace ReportEngine.App.AppHelpers;

public class PerformanceTimer
{
    public static void Measure(string name, Action action)
    {
        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();
        Console.WriteLine($"{name} выполнено за {sw.ElapsedMilliseconds} мс");
        Debug.WriteLine($"{name} выполнено за {sw.ElapsedMilliseconds} мс");
    }

    public static async Task MeasureAsync(string name, Func<Task> action)
    {
        var sw = Stopwatch.StartNew();
        await action();
        sw.Stop();
        Console.WriteLine($"{name} выполнено за {sw.ElapsedMilliseconds} мс");
        Debug.WriteLine($"{name} выполнено за {sw.ElapsedMilliseconds} мс");
    }
}