using System;
using System.Globalization;
using System.Text.RegularExpressions;

#if NETCOREAPP2_1
using System.Buffers;
using System.Text;
#else
using System.Linq;
#endif

namespace Humanizer
{
    /// <summary>
    /// Contains extension methods for humanizing string values.
    /// </summary>
    public static class StringHumanizeExtensions
    {
        private static readonly Regex PascalCaseWordPartsRegex;
        private static readonly Regex FreestandingSpacingCharRegex;
        private static readonly char[] UnderscoreDashSeparators;

        static StringHumanizeExtensions()
        {
            PascalCaseWordPartsRegex = new Regex(@"[\p{Lu}]?[\p{Ll}]+|[0-9]+[\p{Ll}]*|[\p{Lu}]+(?=[\p{Lu}][\p{Ll}]|[0-9]|\b)|[\p{Lo}]+",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture | RegexOptionsUtil.Compiled);
            FreestandingSpacingCharRegex = new Regex(@"\s[-_]|[-_]\s", RegexOptionsUtil.Compiled);
            UnderscoreDashSeparators = new[] { ' ', '-', '_' };
        }

#if NETCOREAPP2_1

        /// <summary>
        /// Humanizes the input string; e.g. Underscored_input_String_is_turned_INTO_sentence -> 'Underscored input String is turned INTO sentence'
        /// </summary>
        /// <param name="input">The string to be humanized</param>
        /// <param name="casing">The desired casing for the output</param>
        public static string Humanize(this string input, LetterCasing? casing = null)
        {
            var builder = new StringBuilder(input.Length);
            Humanize(input, builder, casing);
            return builder.ToString();
        }

        /// <summary>
        /// Humanizes the input string; e.g. Underscored_input_String_is_turned_INTO_sentence -> 'Underscored input String is turned INTO sentence'
        /// </summary>
        public static void Humanize(this ReadOnlySpan<char> input, StringBuilder output, LetterCasing? casing = null) => 
            Humanize(input.ToString(), output, casing);

        /// <summary>
        /// Humanizes the input string; e.g. Underscored_input_String_is_turned_INTO_sentence -> 'Underscored input String is turned INTO sentence'
        /// </summary>
        public static void Humanize(this string input, StringBuilder output, LetterCasing? casing = null)
        {
            var foo = input.AsMemory();
            var chars = input.AsSpan();
            var startLen = output.Length;

            // if input is all capitals (e.g. an acronym) then return it without change
            if (AllCapitals(chars))
            {
                output.Append(input);
            }
            else if (FreestandingSpacingCharRegex.IsMatch(input))
            {
                // if input contains a dash or underscore which preceeds or follows a space (or both, e.g. free-standing)
                // remove the dash/underscore and run it through FromPascalCase

                var wordIndex = 0;
                foreach (var word in chars.Split(UnderscoreDashSeparators, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (wordIndex > 0)
                        output.Append(' ');

                    FromPascalCase(word.ToString(), output, isFullSentence: false);
                    wordIndex++;
                }
            }
            else if (input.Contains('_') || input.Contains('-'))
            {
                var wordIndex = 0;
                foreach (var word in chars.Split(UnderscoreDashSeparators, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (wordIndex > 0)
                        output.Append(' ');
                    output.Append(word);
                    wordIndex++;
                }
            }
            else
            {
                FromPascalCase(input, output);
            }

            if (casing.HasValue)
            {
                ApplyCase(output, startLen, casing.Value);
            }
        }

        private static void ApplyCase(StringBuilder builder, int startIndex, LetterCasing casing)
        {
            var length = builder.Length - startIndex;
            var pool = ArrayPool<char>.Shared;
            var chars = pool.Rent(length);
            try
            {
                var span = chars.AsSpan(0, length);
                builder.CopyTo(startIndex, span, length);

                span.ApplyCase(casing);

                builder.Remove(startIndex, length);
                builder.Append(span);
            }
            finally
            {
                pool.Return(chars);
            }
        }

        private static void FromPascalCase(string input, StringBuilder output, bool isFullSentence = true)
        {
            var ti = CultureInfo.CurrentCulture.TextInfo;
            var wordIndex = 0;

            foreach (var word in input.SplitRegexWord(PascalCaseWordPartsRegex))
            {
                if (wordIndex > 0)
                    output.Append(' ');

                if (AllCapitals(word) && (word.Length > 1 || !isFullSentence || (word.Length == 1 && word[0] == 'I')))
                {
                    // don't adjust the case
                    output.Append(word);
                }
                else
                {
                    for (var j = 0; j < word.Length; j++)
                    {
                        if (isFullSentence && wordIndex == 0 && j == 0)
                            output.Append(ti.ToUpper(word[j]));
                        else
                            output.Append(ti.ToLower(word[j]));
                    }
                }

                wordIndex++;
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

#else

        /// <summary>
        /// Humanizes the input string; e.g. Underscored_input_String_is_turned_INTO_sentence -> 'Underscored input String is turned INTO sentence'
        /// </summary>
        /// <param name="input">The string to be humanized</param>
        /// <returns></returns>
        public static string Humanize(this string input)
        {
            // if input is all capitals (e.g. an acronym) then return it without change
            if (input.ToCharArray().All(char.IsUpper))
            {
                return input;
            }

            // if input contains a dash or underscore which preceeds or follows a space (or both, e.g. free-standing)
            // remove the dash/underscore and run it through FromPascalCase
            if (FreestandingSpacingCharRegex.IsMatch(input))
            {
                return FromPascalCase(FromUnderscoreDashSeparatedWords(input));
            }

            if (input.Contains("_") || input.Contains("-"))
            {
                return FromUnderscoreDashSeparatedWords(input);
            }

            return FromPascalCase(input);
        }

        /// <summary>
        /// Humanized the input string based on the provided casing
        /// </summary>
        /// <param name="input">The string to be humanized</param>
        /// <param name="casing">The desired casing for the output</param>
        /// <returns></returns>
        public static string Humanize(this string input, LetterCasing casing)
        {
            return input.Humanize().ApplyCase(casing);
        }

        private static string FromUnderscoreDashSeparatedWords(string input)
        {
            return string.Join(" ", input.Split(new[] { '_', '-' }));
        }

        private static string FromPascalCase(string input)
        {
            var result = string.Join(" ", PascalCaseWordPartsRegex
                .Matches(input).Cast<Match>()
                .Select(match => match.Value.ToCharArray().All(char.IsUpper) &&
                    (match.Value.Length > 1 || (match.Index > 0 && input[match.Index - 1] == ' ') || match.Value == "I")
                    ? match.Value
                    : match.Value.ToLower()));

            return result.Length > 0 ? char.ToUpper(result[0]) +
                result.Substring(1, result.Length - 1) : result;
        }
#endif
    }
}
