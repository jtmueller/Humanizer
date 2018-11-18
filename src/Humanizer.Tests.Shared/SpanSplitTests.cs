#if NETCOREAPP2_1
using System;
using System.Text.RegularExpressions;
using Humanizer;
using Xunit;

namespace Humanizer.Tests
{
    public class SpanSplitTests
    {
        private static readonly Regex PascalCaseWordPartsRegex = new Regex(@"[\p{Lu}]?[\p{Ll}]+|[0-9]+[\p{Ll}]*|[\p{Lu}]+(?=[\p{Lu}][\p{Ll}]|[0-9]|\b)|[\p{Lo}]+",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

        [Fact]
        public void Basic_Split_Array_Test()
        {
            var input = "1,2,3,4,5";
            var expected = input.Split(',');
            var results = input.AsSpan().Split(',').ToArray();

            Assert.Equal(expected.Length, results.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], results[i]);
            }
        }

        [Fact]
        public void Basic_Split_Test()
        {
            var input = "1,2,3,4,5";
            var expected = input.Split(',');
            var results = input.AsSpan().Split(',').ToArray();

            Assert.Equal(expected.Length, results.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], results[i]);
            }
        }

        [Fact]
        public void Basic_SplitAny_Test()
        {
            var input = "1,2;3,4,;5,6|7|8";
            var expected = input.Split(',', ';', '|');
            var results = input.AsSpan().Split(',', ';', '|').ToArray();

            Assert.Equal(expected.Length, results.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], results[i]);
            }
        }

        [Fact]
        public void Split_Remove_Empty_Test()
        {
            var input = ",1,2,,3,,,4,5,";
            var expected = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var results = input.AsSpan().Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();

            Assert.Equal(expected.Length, results.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], results[i]);
            }
        }

        [Fact]
        public void SplitAny_Remove_Empty_Test()
        {
            var input = ";;1,,2;,;|,3;;;4,;5,6|,|7|8||";
            var splitChars = new[] { ',', ';', '|' };
            var expected = input.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            var results = input.AsSpan().Split(splitChars, StringSplitOptions.RemoveEmptyEntries).ToArray();

            Assert.Equal(expected.Length, results.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], results[i]);
            }
        }

        [Fact]
        public void Split_By_String_Test()
        {
            var input = string.Join(Environment.NewLine, new[] { "foo", "bar", "baz", "quux" });
            var expected = input.Split(Environment.NewLine);
            var results = input.AsSpan().Split(Environment.NewLine).ToArray();

            Assert.Equal(expected.Length, results.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], results[i]);
            }
        }

        [Theory]
        [InlineData("PascalCaseSentence", "Pascal Case Sentence")]
        public void Split_By_Regex_Test(string input, string expectedOutput)
        {
            var results = input.SplitRegexWord(PascalCaseWordPartsRegex).ToArray();
            Assert.Equal(expectedOutput, string.Join(' ', results));
        }
    }
}
#endif
