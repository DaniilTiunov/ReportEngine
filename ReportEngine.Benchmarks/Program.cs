using BenchmarkDotNet.Running;

namespace ReportEngine.Benchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<IniReaderBenchmarks>(
            new BenchmarkDotNet.Configs.DebugInProcessConfig());
        }
    }
}
