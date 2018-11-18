using System;

#if NETCOREAPP2_1
using System.Buffers;
#else
using System.Linq;
#endif

namespace Humanizer
{
    /// <summary>
    /// A portal to string transformation using IStringTransformer
    /// </summary>
    public static class To
    {
#if NETCOREAPP2_1
        /// <summary>
        /// Transforms a string using the provided transformers. Transformations are applied in the provided order.
        /// </summary>
        public static string Transform(this string input, params IStringTransformer[] transformers)
        {
            var pool = ArrayPool<char>.Shared;
            var chars = pool.Rent(input.Length);
            try
            {
                var output = chars.AsSpan(0, input.Length);
                Transform(input.AsSpan(), output);
                return output.ToString();
            }
            finally
            {
                pool.Return(chars);
            }
        }

        /// <summary>
        /// Transforms a char-span using the provided transformers. Transformations are applied in the provided order.
        /// </summary>
        public static void Transform(this ReadOnlySpan<char> input, Span<char> output, params IStringTransformer[] transformers)
        {
            if (output.Length < input.Length)
                throw new ArgumentException("Output length must equal or exceed input length.");

            input.CopyTo(output);

            for (var i = 0; i < transformers.Length; i++)
            {
                transformers[i].Transform(output);
            }
        }
#else
        /// <summary>
        /// Transforms a string using the provided transformers. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="transformers"></param>
        /// <returns></returns>
        public static string Transform(this string input, params IStringTransformer[] transformers)
        {
            return transformers.Aggregate(input, (current, stringTransformer) => stringTransformer.Transform(current));
        }
#endif

        /// <summary>
        /// Changes string to title case
        /// </summary>
        /// <example>
        /// "INvalid caSEs arE corrected" -> "Invalid Cases Are Corrected"
        /// </example>
        public static readonly IStringTransformer TitleCase = new ToTitleCase();

        /// <summary>
        /// Changes the string to lower case
        /// </summary>
        /// <example>
        /// "Sentence casing" -> "sentence casing"
        /// </example>
        public static readonly IStringTransformer LowerCase = new ToLowerCase();

        /// <summary>
        /// Changes the string to upper case
        /// </summary>
        /// <example>
        /// "lower case statement" -> "LOWER CASE STATEMENT"
        /// </example>
        public static readonly IStringTransformer UpperCase = new ToUpperCase();

        /// <summary>
        /// Changes the string to sentence case
        /// </summary>
        /// <example>
        /// "lower case statement" -> "Lower case statement"
        /// </example>
        public static readonly IStringTransformer SentenceCase = new ToSentenceCase();
    }
}
