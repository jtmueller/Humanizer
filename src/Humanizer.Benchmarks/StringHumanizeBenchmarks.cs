using System;
using System.Linq;

using BenchmarkDotNet.Attributes;

namespace Humanizer.Benchmarks
{
    [ClrJob(true), CoreJob, MemoryDiagnoser]
    public class StringHumanizeBenchmarks
    {
        // hard-coded seed ensures the same random strings are generated each time.
        private const int RAND_SEED = 11917;

        private static readonly char[] _alphabet =
            Enumerable.Repeat(new int[] { '_', '-', ' ' }, 10).SelectMany(x => x)
                .Concat(Enumerable.Range('a', 'z' - 'a'))
                .Concat(Enumerable.Range('A', 'Z' - 'A'))
                .Concat(Enumerable.Range('0', '9' - '0'))
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
        public void Humanize()
        {
            _input.Humanize();
        }

        [Benchmark]
        public void HumanizeToLowerCase()
        {
            _input.Humanize(LetterCasing.LowerCase);
        }

        [Benchmark]
        public void HumanizeToUpperCase()
        {
            _input.Humanize(LetterCasing.AllCaps);
        }

        [Benchmark]
        public void HumanizeToSentenceCase()
        {
            _input.Humanize(LetterCasing.Sentence);
        }

        [Benchmark]
        public void HumanizeToTitleCase()
        {
            _input.Humanize(LetterCasing.Title);
        }
    }
}
