using System;
using System.Linq;
using System.Text;

namespace Humanizer
{
    /// <summary>
    /// Contains extension methods for dehumanizing strings.
    /// </summary>
    public static class StringDehumanizeExtensions
    {
#if NETCOREAPP2_1
        /// <summary>
        /// Dehumanizes a string; e.g. 'some string', 'Some String', 'Some string' -> 'SomeString'
        /// </summary>
        /// <param name="input">The string to be dehumanized</param>
        /// <returns></returns>
        public static string Dehumanize(this string input) => Dehumanize(input.AsSpan());

        /// <summary>
        /// Dehumanizes a string; e.g. 'some string', 'Some String', 'Some string' -> 'SomeString'
        /// </summary>
        /// <param name="input">The string to be dehumanized</param>
        /// <returns></returns>
        public static string Dehumanize(this ReadOnlySpan<char> input)
        {
            var output = new StringBuilder(input.Length);
            foreach (var word in input.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                word.Humanize(output, LetterCasing.Title);
            }

            for (var i = output.Length - 1; i >= 0; i--)
            {
                if (output[i] == ' ')
                    output.Remove(i, 1);
            }

            return output.ToString();
        }
#else
        /// <summary>
        /// Dehumanizes a string; e.g. 'some string', 'Some String', 'Some string' -> 'SomeString'
        /// </summary>
        /// <param name="input">The string to be dehumanized</param>
        /// <returns></returns>
        public static string Dehumanize(this string input)
        {
            var titlizedWords = input.Split(' ').Select(word => word.Humanize(LetterCasing.Title));
            return string.Join("", titlizedWords).Replace(" ", "");
        }
#endif
    }
}
