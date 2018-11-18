namespace Humanizer
{
    using System;

    /// <summary>
    /// Can transform a string
    /// </summary>
    public interface IStringTransformer
    {
        /// <summary>
        /// Transforms the input
        /// </summary>
        /// <param name="input">String to be transformed</param>
        string Transform(string input);

#if NETCOREAPP2_1
        /// <summary>
        /// Transforms the input in-place, writing the changes directly back to the input span.
        /// Does not support transformations that change the string length.
        /// </summary>
        /// <param name="chars">The characters to be transformed.</param>
        void Transform(Span<char> chars);
#endif
    }
}
