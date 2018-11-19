using System;

using BenchmarkDotNet.Attributes;

namespace Humanizer.Benchmarks
{
    [ClrJob(true), CoreJob, MemoryDiagnoser]
    public class TransformersBenchmarks
    {
        // hard-coded seed ensures the same random strings are generated each time.
        private const int RAND_SEED = 17432;

        private readonly Random _random = new Random(RAND_SEED);
        private string _input;

        [Params(10, 100, 1000)]
        public int StringLen;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _input = StringGenerator.Generate(_random, StringLen, includeSpaces: true, includeSymbols: true);
        }

        [Benchmark]
        public void AllTransforms()
        {
            _input.Transform(To.LowerCase, To.UpperCase, To.SentenceCase, To.TitleCase);
        }

        [Benchmark]
        public void LowerCase()
        {
            _input.Transform(To.LowerCase);
        }

        [Benchmark]
        public void UpperCase()
        {
            _input.Transform(To.UpperCase);
        }

        [Benchmark]
        public void SentenceCase()
        {
            _input.Transform(To.SentenceCase);
        }

        [Benchmark]
        public void TitleCase()
        {
            _input.Transform(To.TitleCase);
        }
    }
}
