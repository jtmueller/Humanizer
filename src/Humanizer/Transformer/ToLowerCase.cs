using System;
using System.Globalization;

namespace Humanizer
{
    internal class ToLowerCase : IStringTransformer
    {
        public string Transform(string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToLower(input);
        }

#if NETCOREAPP2_1
        public void Transform(Span<char> chars)
        {
            var ci = CultureInfo.CurrentCulture;
            MemoryExtensions.ToLower(chars, chars, ci);
        }
#endif
    }
}
