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
            var ti = CultureInfo.CurrentCulture.TextInfo;
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = ti.ToUpper(chars[i]);
            }
        }
#endif
    }
}
