#if NETCOREAPP2_1
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Humanizer
{
    // Based on: https://gist.github.com/LordJZ/e0b5245d69497f2a43a5f09c1d26e34c

    public ref struct SpanSplitEnumerable
    {
        private readonly char _separator;
        private readonly char[] _separators;
        private readonly string _separatorString;
        private readonly ReadOnlySpan<char> _span;
        private readonly StringSplitOptions _options;

        public SpanSplitEnumerable(ReadOnlySpan<char> span, char[] separators, StringSplitOptions options = StringSplitOptions.None)
        {
            _span = span;
            _separators = separators;
            _options = options;

            _separator = (char)0;
            _separatorString = string.Empty;
        }

        public SpanSplitEnumerable(ReadOnlySpan<char> span, char separator, StringSplitOptions options = StringSplitOptions.None)
        {
            _span = span;
            _separator = separator;
            _options = options;

            _separators = Array.Empty<char>();
            _separatorString = string.Empty;
        }

        public SpanSplitEnumerable(ReadOnlySpan<char> span, string separatorString, StringSplitOptions options = StringSplitOptions.None)
        {
            _span = span;
            _separatorString = separatorString;
            _options = options;

            _separators = Array.Empty<char>();
            _separator = (char)0;
        }

        public SpanSplitEnumerator GetEnumerator() => new SpanSplitEnumerator(_span, _separators, _separator, _separatorString, _options);

        /// <summary>
        ///     Converts the span enumerable to a string array.
        ///     Useful for storing results - do not use when avoiding allocations.
        /// </summary>
        public string[] ToArray()
        {
            var list = new List<string>();
            foreach (var part in this)
            {
                list.Add(part.ToString());
            }
            return list.ToArray();
        }
    }

    public ref struct SpanSplitEnumerator
    {
        static private long _sentinel;

        private readonly char _separator;
        private readonly ReadOnlySpan<char> _separators;
        private readonly string _separatorString;
        private ReadOnlySpan<char> _span;
        private readonly StringSplitOptions _options;

        internal SpanSplitEnumerator(ReadOnlySpan<char> span, ReadOnlySpan<char> separators, char separator, string separatorString, StringSplitOptions options)
        {
            _span = span;
            _separators = separators;
            _separator = separator;
            _separatorString = separatorString;
            _options = options;
            Current = default;

            if (_span.IsEmpty)
                TrailingEmptyItem = true;
        }

        private unsafe ReadOnlySpan<char> TrailingEmptyItemSentinel =>
            new ReadOnlySpan<char>(Unsafe.AsPointer(ref _sentinel), 1);

        private bool TrailingEmptyItem
        {
            get => _span == TrailingEmptyItemSentinel;
            set => _span = value ? TrailingEmptyItemSentinel : default;
        }

        public bool MoveNext()
        {
            if (TrailingEmptyItem)
            {
                TrailingEmptyItem = false;
                Current = default;
                return _options == 0;
            }

next:
            if (_span.IsEmpty)
            {
                _span = Current = default;
                return false;
            }

            int idx;
            var sepLen = 1;

            if (_separator != 0)
            {
                idx = _span.IndexOf(_separator);
            }
            else if (!_separators.IsEmpty)
            {
                idx = _span.IndexOfAny(_separators);
            }
            else if (_separatorString?.Length > 0)
            {
                idx = _span.IndexOf(_separatorString, StringComparison.Ordinal);
                sepLen = _separatorString.Length;
            }
            else
            {
                throw new InvalidOperationException("No separator specified!");
            }

            if (idx < 0)
            {
                Current = _span;
                _span = default;
            }
            else
            {
                Current = _span.Slice(0, idx);
                _span = _span.Slice(idx + sepLen);

                if (_options == StringSplitOptions.RemoveEmptyEntries && Current.IsEmpty)
                    goto next;

                if (_span.IsEmpty)
                    TrailingEmptyItem = true;
            }

            return true;
        }

        public ReadOnlySpan<char> Current { get; private set; }
    }

    public ref struct SpanSplitRegexEnumerable
    {
        private readonly ReadOnlySpan<char> _span;
        private readonly MatchCollection _matches;
        private readonly StringSplitOptions _options;

        public SpanSplitRegexEnumerable(ReadOnlySpan<char> span, MatchCollection matches, StringSplitOptions options)
        {
            _span = span;
            _matches = matches;
            _options = options;
        }

        public SpanSplitRegexEnumerator GetEnumerator() => new SpanSplitRegexEnumerator(_span, _matches, _options);

        /// <summary>
        ///     Converts the span enumerable to a string array.
        ///     Useful for storing results - do not use when avoiding allocations.
        /// </summary>
        public string[] ToArray()
        {
            var list = new List<string>();
            foreach (var part in this)
            {
                list.Add(part.ToString());
            }
            return list.ToArray();
        }
    }

    public ref struct SpanSplitRegexEnumerator
    {
        private readonly ReadOnlySpan<char> _span;
        private readonly MatchCollection _matches;
        private readonly StringSplitOptions _options;
        private int _matchIndex;

        internal SpanSplitRegexEnumerator(ReadOnlySpan<char> span, MatchCollection matches, StringSplitOptions options)
        {
            _span = span;
            _matches = matches;
            _options = options;
            _matchIndex = 0;
            Current = default;
        }

        public bool MoveNext()
        {
            if (_span.IsEmpty)
                return false;

next:
            if (_matchIndex >= _matches.Count)
                return false;

            var match = _matches[_matchIndex++];

            Current = _span.Slice(match.Index, match.Length);

            if (_options == StringSplitOptions.RemoveEmptyEntries && Current.IsEmpty)
                goto next;

            return true;
        }

        public ReadOnlySpan<char> Current { get; private set; }
    }

    public static class SpanSplitExtensions
    {
        [Pure]
        public static SpanSplitEnumerable Split(this ReadOnlySpan<char> span, char separator, StringSplitOptions options = StringSplitOptions.None)
            => new SpanSplitEnumerable(span, separator, options);

        [Pure]
        public static SpanSplitEnumerable Split(this ReadOnlySpan<char> span, string separatorString, StringSplitOptions options = StringSplitOptions.None)
            => new SpanSplitEnumerable(span, separatorString, options);

        [Pure]
        public static SpanSplitEnumerable Split(this ReadOnlySpan<char> span, char[] separators, StringSplitOptions options = StringSplitOptions.None)
           => new SpanSplitEnumerable(span, separators, options);

        [Pure]
        public static SpanSplitEnumerable Split(this ReadOnlySpan<char> span, params char[] separators)
            => new SpanSplitEnumerable(span, separators);

        [Pure]
        public static SpanSplitRegexEnumerable SplitRegexWord(this string input, Regex regex, StringSplitOptions options = StringSplitOptions.None)
        {
            var matches = regex.Matches(input);
            return new SpanSplitRegexEnumerable(input.AsSpan(), matches, options);
        }

        // TODO: SplitRegexSeparator?
    }
}
#endif
