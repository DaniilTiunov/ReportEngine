using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ReportEngine.Shared.Config.IniHelpers;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettings;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net48)]
public class IniReaderBenchmarkss
{
    [Benchmark]
    public void LoadSettings()
    {
        CalculationSettingsManager.Load<StandSettings, StandSettingsData>();
    }
}
