using System.Collections.Generic;
using Verbox.Properties;

namespace Verbox.Models.Styles
{
    public record HelpStyle(string NamespaceMemberPrefix,
                            string NameBriefSeparator,
                            string SwitchNamesSuffix,
                            IReadOnlyDictionary<ArgTags, string> ArgTagsFormat);
}
