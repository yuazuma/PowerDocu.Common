using System;
using System.Globalization;
using System.Text;

namespace PowerDocu.Common
{
    public static class CharsetHelper
    {
        private static readonly char[] UnsafeChars =
        {
            ':',
            '?',
            '<',
            '>',
            '/',
            '|',
            ',',
            '*',
            '&',
            '"',
            '#'
        };

        // Converts a name to a filesystem-safe string.
        // Umlauts and similar diacritics are reduced to their base ASCII letter (e.g. ae -> a).
        // All other non-ASCII characters are encoded as their Unicode code point in hex (e.g. Chinese -> U6CE8),
        // preserving uniqueness even when names have the same character count.
        // Unsafe filesystem/graphviz characters are replaced with '-'.
        public static string GetSafeName(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return "NameNotDefined";
            }

            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder(normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                // Strip combining diacritical marks (e.g. ae -> a)
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                    continue;

                char normalized = c;
                // Re-normalize to FormC char-by-char is not possible, so we handle
                // ASCII safety and unsafe-char replacement in a single pass below.
                // Non-ASCII characters that survived diacritical stripping are encoded
                // as their Unicode code point in hex to preserve name uniqueness.
                if (normalized > 127)
                {
                    // Encode non-ASCII characters as their Unicode code point (e.g. Chinese -> 'U6CE8').
                    // This preserves uniqueness across different multibyte characters with the same
                    // character count, while remaining safe for file/folder names and the graphviz library.
                    if (char.IsHighSurrogate(normalized) && i + 1 < normalizedString.Length && char.IsLowSurrogate(normalizedString[i + 1]))
                    {
                        // Handle surrogate pairs (code points above U+FFFF, e.g. emoji)
                        int codePoint = char.ConvertToUtf32(normalized, normalizedString[i + 1]);
                        sb.Append($"U{codePoint:X5}");
                        i++; // skip low surrogate
                    }
                    else
                    {
                        sb.Append($"U{(int)normalized:X4}");
                    }
                    continue;
                }

                // Replace all unsafe characters with '-'
                bool isUnsafe = false;
                for (int j = 0; j < UnsafeChars.Length; j++)
                {
                    if (normalized == UnsafeChars[j])
                    {
                        isUnsafe = true;
                        break;
                    }
                }
                sb.Append(isUnsafe ? '-' : normalized);
            }

            return sb.ToString();
        }
    }
}

