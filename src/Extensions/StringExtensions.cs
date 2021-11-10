using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Verbox.Extensions
{
    public static class StringExtensions
    {
        public static string PascalToDash(this string pascal)
        {
            return string.Join('-', pascal.SplitPascal()
                                          .Select(token => token.ToLower()));
        }

        private static IEnumerable<string> SplitPascal(this string pascal)
        {
            var tokens = new List<string>();
            StringBuilder builder = new StringBuilder().Append(pascal[0]);
            for (var i = 1; i < pascal.Length; ++i)
            {
                if(char.IsUpper(pascal[i]))
                {
                    tokens.Add(builder.ToString());
                    builder.Clear();
                }
                builder.Append(pascal[i]);
            }
            tokens.Add(builder.ToString());
            return tokens;
        }

        public static string DashToPascal(this string dash)
        {
            return string.Concat(dash.Split('-')
                                     .Select(ToTitle));
        }

        private static string ToTitle(this string s)
        {
            return $"{char.ToUpper(s[0])}{s[1..].ToLower()}";
        }
    }
}
