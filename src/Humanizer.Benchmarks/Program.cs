using BenchmarkDotNet.Running;

namespace Humanizer.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TransformersBenchmarks>();
        }
    }
}
