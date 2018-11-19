using System;
using System.Linq;

namespace Humanizer.Benchmarks
{
    internal static class StringGenerator
    {
        private static readonly char[] _separators = new[] { '_', '-' };
        private static readonly char[] _separatorsWithSpace = new[] { '_', '-', ' ' };
        private static readonly char[] _lowerCase =
            Enumerable.Range('a', 'z' - 'a').Select(c => (char)c).ToArray();
        private static readonly char[] _upperCase =
            Enumerable.Range('A', 'Z' - 'A').Select(c => (char)c).ToArray();
        private static readonly char[] _numbers =
            Enumerable.Range('0', '9' - '0').Select(c => (char)c).ToArray();
        private static readonly char[] _symbols =
            new[] { '.', ',', '(', ')', '"', '!', '$' };
        private static readonly char[] _alphaNumeric =
            _lowerCase.Concat(_upperCase).Concat(_numbers).ToArray();
        private static readonly char[] _alphaNumericLower =
            _lowerCase.Concat(_numbers).ToArray();
        private static readonly char[] _alphaNumericSymbols =
            _lowerCase.Concat(_upperCase).Concat(_numbers).Concat(_symbols).ToArray();
        private static readonly char[] _alphaNumericSymbolsLower =
            _lowerCase.Concat(_numbers).Concat(_symbols).ToArray();

        public static string Generate(Random random, int length, bool includeSpaces = false, bool includeSeparators = false, bool pascalCase = false, bool includeSymbols = false, int maxWordLen = 7)
        {
            var wordLen = maxWordLen;
            var capitalizeNextLetter = true;
            var lastWasSpace = false;
            var chars = new char[length];
            for (var i = 0; i < length; i++)
            {
                if (i == 0 || i % wordLen == 0)
                {
                    wordLen = random.Next(1, maxWordLen + 1);

                    // at the beginning of a word
                    if (!lastWasSpace && i > 0)
                    {
                        capitalizeNextLetter = i == 0 || pascalCase;

                        if (includeSpaces && includeSeparators)
                        {
                            chars[i] = _separatorsWithSpace[random.Next(0, _separatorsWithSpace.Length)];
                            lastWasSpace = true;
                            continue;
                        }
                        else if (includeSpaces)
                        {
                            chars[i] = ' ';
                            lastWasSpace = true;
                            continue;
                        }
                        else if (includeSeparators)
                        {
                            chars[i] = _separators[random.Next(0, _separators.Length)];
                            lastWasSpace = true;
                            continue;
                        }
                    }
                }

                if (capitalizeNextLetter)
                {
                    chars[i] = _upperCase[random.Next(0, _upperCase.Length)];
                    capitalizeNextLetter = false;
                }
                else if (includeSymbols)
                {
                    if (pascalCase)
                        chars[i] = _alphaNumericSymbolsLower[random.Next(0, _alphaNumericSymbolsLower.Length)];
                    else
                        chars[i] = _alphaNumericSymbols[random.Next(0, _alphaNumericSymbols.Length)];
                }
                else
                {
                    if (pascalCase)
                        chars[i] = _alphaNumericLower[random.Next(0, _alphaNumericLower.Length)];
                    else
                        chars[i] = _alphaNumeric[random.Next(0, _alphaNumeric.Length)];
                }
                lastWasSpace = false;
            }

            return new string(chars);
        }
    }
}
