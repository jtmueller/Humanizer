using System;

using BenchmarkDotNet.Attributes;

namespace Humanizer.Benchmarks
{
    [ClrJob(true), CoreJob, MemoryDiagnoser]
    public class StringHumanizeBenchmarks
    {
        // hard-coded seed ensures the same random strings are generated each time.
        private const int RAND_SEED = 967;

        private readonly Random _random = new Random(RAND_SEED);
        private string _input;

        [Params(10, 100, 1000)]
        public int StringLen;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _input = StringGenerator.Generate(_random, StringLen, includeSpaces:true, includeSeparators:true, pascalCase:false, maxWordLen: 9);
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
