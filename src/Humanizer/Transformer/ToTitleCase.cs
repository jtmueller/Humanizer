using System;
using System.Text.RegularExpressions;

#if NETCOREAPP2_1
using System.Buffers;
#else
using System.Linq;
#endif

namespace Humanizer
{
    internal class ToTitleCase : IStringTransformer
    {
        private const string _wordsRegex = @"(\w|[^\u0000-\u007F])+'?\w*";

#if NETCOREAPP2_1
        public string Transform(string input)
        {
            var pool = ArrayPool<char>.Shared;
            var chars = pool.Rent(input.Length);
            try
            {
                var output = chars.AsSpan(0, input.Length);
                input.AsSpan().CopyTo(output);

                var matches = Regex.Matches(input, _wordsRegex);
                foreach (Match match in matches)
                {
                    var word = output.Slice(match.Index, match.Length);
                    if (!AllCapitals(word))
                    {
                        ReplaceWithTitleCase(word);
                    }
                }

                return output.ToString();
            }
            finally
            {
                pool.Return(chars);
            }
        }

        public void Transform(Span<char> chars)
        {
            var matches = Regex.Matches(chars.ToString(), _wordsRegex);
            foreach (Match match in matches)
            {
                var word = chars.Slice(match.Index, match.Length);
                if (!AllCapitals(word))
                {
                    ReplaceWithTitleCase(word);
                }
            }
        }

        private static bool AllCapitals(ReadOnlySpan<char> word)
        {
            for (var i = 0; i < word.Length; i++)
            {
                if (!char.IsUpper(word[i]))
                    return false;
            }
            return true;
        }

        private static void ReplaceWithTitleCase(Span<char> word)
        {
            if (word.Length == 0)
                return;
            word[0] = char.ToUpper(word[0]);

            if (word.Length > 1)
            {
                for (var i = 1; i < word.Length; i++)
                {
                    word[i] = char.ToLower(word[i]);
                }
            }
        }
#else
        public string Transform(string input)
        {
            var result = input;
            var matches = Regex.Matches(input, _wordsRegex);
            foreach (Match word in matches)
            {
                if (!AllCapitals(word.Value))
                {
                    result = ReplaceWithTitleCase(word, result);
                }
            }

            return result;
        }

        private static bool AllCapitals(string input)
        {
            return input.ToCharArray().All(char.IsUpper);
        }

        private static string ReplaceWithTitleCase(Match word, string source)
        {
            var wordToConvert = word.Value;
            var replacement = char.ToUpper(wordToConvert[0]) + wordToConvert.Remove(0, 1).ToLower();
            return source.Substring(0, word.Index) + replacement + source.Substring(word.Index + word.Length);
        }
#endif
    }
}
