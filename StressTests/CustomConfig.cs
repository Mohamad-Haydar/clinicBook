using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Perfolizer.Horology;

public class CustomConfig : ManualConfig
{
    public CustomConfig()
    {
        AddJob(Job.Default
            .WithWarmupCount(1) // How many warmup iterations
            .WithIterationTime(TimeInterval.FromMilliseconds(500)) // Single iteration duration (500ms in this case)
            .WithMaxIterationCount(300) // Maximum number of iterations (can simulate requests)
            .WithIterationCount(5)); // Number of iteration batches
    }
}