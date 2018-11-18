using System;
using System.Linq;

using BenchmarkDotNet.Attributes;

namespace Humanizer.Benchmarks
{
    [ClrJob(true), CoreJob, MemoryDiagnoser]
    public class TransformersBenchmarks
    {
        // hard-coded seed ensures the same random strings are generated each time.
        private const int RAND_SEED = 17432;

        private static readonly char[] _alphabet = 
            Enumerable.Repeat((int)' ', 12)
                .Concat(Enumerable.Range('a', 'z' - 'a'))
                .Concat(Enumerable.Range('A', 'Z' - 'A'))
                .Concat(Enumerable.Range('0', '9' - '0'))
                .Concat(new int[] { '.', ',', '(', ')', '!', '$' })
                .Select(x => (char)x)
                .ToArray();

        private readonly Random _random = new Random(RAND_SEED);
        private string _input;

        [Params(10, 100, 1000)]
        public int StringLen;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var chars = new char[StringLen];
            for (var i = 0; i < StringLen; i++)
            {
                chars[i] = _alphabet[_random.Next(0, _alphabet.Length)];
            }
            _input = new string(chars);
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
