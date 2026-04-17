using System;
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

        // Replaces characters that are unsafe for file system paths and Graphviz identifiers with '-'.
        // Multibyte (non-ASCII) characters such as Japanese, Chinese, etc. are preserved as-is.
        public static string GetSafeName(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return "NameNotDefined";
            }

            StringBuilder sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                bool isUnsafe = false;
                for (int j = 0; j < UnsafeChars.Length; j++)
                {
                    if (c == UnsafeChars[j])
                    {
                        isUnsafe = true;
                        break;
                    }
                }
                sb.Append(isUnsafe ? '-' : c);
            }

            return sb.ToString();
        }
    }
}
