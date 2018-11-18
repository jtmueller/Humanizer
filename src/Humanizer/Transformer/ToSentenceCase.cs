using System;

#if NETCOREAPP2_1
using System.Buffers;
#endif

namespace Humanizer
{
    internal class ToSentenceCase : IStringTransformer
    {
#if NETCOREAPP2_1
        public string Transform(string input)
        {
            var pool = ArrayPool<char>.Shared;
            var chars = pool.Rent(input.Length);
            try
            {
                var output = chars.AsSpan(0, input.Length);
                input.AsSpan().CopyTo(output);
                Transform(output);
                return output.ToString();
            }
            finally
            {
                pool.Return(chars);
            }
        }

        public void Transform(Span<char> chars)
        {
            if (chars.Length == 0)
                return;

            chars[0] = char.ToUpper(chars[0]);
        }
#else
        public string Transform(string input)
        {
            if (input.Length >= 1)
            {
                return string.Concat(input.Substring(0, 1).ToUpper(), input.Substring(1));
            }

            return input.ToUpper();
        }
#endif
    }
}
