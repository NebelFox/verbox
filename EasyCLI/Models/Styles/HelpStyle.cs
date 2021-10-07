using System.Collections.Generic;
using EasyCLI.Properties;

namespace EasyCLI.Models.Styles
{
    public record HelpStyle(string NamespaceMemberPrefix,
                            string NameBriefSeparator,
                            string SwitchNamesSuffix,
                            IReadOnlyDictionary<ArgTags, string> ArgTagsFormat);
}
