using System.Text.RegularExpressions;

namespace Verbox.Extensions
{
    internal static class RegexExtensions
    {
        public static string ValueOrDefault(this Group group, string @default = null)
        {
            return group.Success ? group.Value : @default;
        }
    }
}
