using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Verbox.Extensions
{
    internal static class StringExtensions
    {
        public static string PascalToDash(this string pascal)
        {
            return string.Join('-',
                               pascal.SplitPascal()
                                     .Select(token => token.ToLower()));
        }

        private static IEnumerable<string> SplitPascal(this string pascal)
        {
            var tokens = new List<string>();
            StringBuilder builder = new StringBuilder().Append(pascal[0]);
            for (var i = 1; i < pascal.Length; ++i)
            {
                if (char.IsUpper(pascal[i]))
                {
                    tokens.Add(builder.ToString());
                    builder.Clear();
                }
                builder.Append(pascal[i]);
            }
            tokens.Add(builder.ToString());
            return tokens;
        }

        public static string SeparatedToPascal(this string separated)
        {
            var tokens = new List<string>();
            var builder = new StringBuilder();
            foreach (char c in separated)
            {
                if (char.IsLetter(c) || char.IsDigit(c))
                {
                    builder.Append(c);
                }
                else if (builder.Length > 0)
                {
                    tokens.Add(builder.ToString());
                    builder.Clear();
                }
            }
            return string.Concat(tokens.Select(ToTitle));
        }

        public static string SeparatedToPascal(this string separated,
                                               params char[] separators)
        {
            return string.Concat(separated.Split(separators)
                                          .Select(ToTitle));
        }

        public static string DashToPascal(this string dash)
        {
            return dash.SeparatedToPascal('-');
        }

        private static string ToTitle(this string s)
        {
            return $"{char.ToUpper(s[0])}{s[1..].ToLower()}";
        }

        public static string JoinMeaningful(this string separator,
                                          params object[] values)
        {
            IEnumerable<object> filtered = values.Where(v => string.IsNullOrWhiteSpace(v?.ToString()) == false);
            return filtered.Any() ? string.Join(separator, filtered) : null;
        }

        public static string JoinMeaningful(this char separator,
                                          params object[] values)
        {
            return separator.ToString().JoinMeaningful(values);
        }
    }
}
