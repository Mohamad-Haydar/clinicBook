using BenchmarkDotNet.Running;
using NBench;

namespace StressTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<DoctorAvailabilityStressTests>(new CustomConfig());
        }
    }
}
