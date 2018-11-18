using System;

namespace Humanizer
{
    /// <summary>
    /// ApplyCase method to allow changing the case of a sentence easily
    /// </summary>
    public static class CasingExtensions
    {
        /// <summary>
        /// Changes the casing of the provided input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="casing"></param>
        /// <returns></returns>
        public static string ApplyCase(this string input, LetterCasing casing)
        {
            switch (casing)
            {
                case LetterCasing.Title:
                    return input.Transform(To.TitleCase);

                case LetterCasing.LowerCase:
                    return input.Transform(To.LowerCase);

                case LetterCasing.AllCaps:
                    return input.Transform(To.UpperCase);

                case LetterCasing.Sentence:
                    return input.Transform(To.SentenceCase);

                default:
                    throw new ArgumentOutOfRangeException(nameof(casing));
            }
        }

#if NETCOREAPP2_1
        /// <summary>
        /// Changes the casing of the provided input
        /// </summary>
        public static void ApplyCase(this Span<char> chars, LetterCasing casing)
        {
            switch (casing)
            {
                case LetterCasing.Title:
                    chars.Transform(To.TitleCase);
                    break;

                case LetterCasing.LowerCase:
                    chars.Transform(To.LowerCase);
                    break;

                case LetterCasing.AllCaps:
                    chars.Transform(To.UpperCase);
                    break;

                case LetterCasing.Sentence:
                    chars.Transform(To.SentenceCase);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(casing));
            }
        }
#endif
    }
}
