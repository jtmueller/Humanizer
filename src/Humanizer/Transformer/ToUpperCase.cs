using System;
using System.Globalization;

namespace Humanizer
{
    internal class ToUpperCase : IStringTransformer
    {
        public string Transform(string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToUpper(input);
        }

#if NETCOREAPP2_1
        public void Transform(Span<char> chars)
        {
            var ci = CultureInfo.CurrentCulture;
            MemoryExtensions.ToUpper(chars, chars, ci);
        }
#endif
    }
}
