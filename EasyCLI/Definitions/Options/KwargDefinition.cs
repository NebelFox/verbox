using EasyCLI.Models.Styles;
using EasyCLI.Properties;

namespace EasyCLI.Definitions.Options
{
    public sealed record KwargDefinition(string Name,
                                         string Description,
                                         string DefaultValue)
        : OptionDefinition(Name, Description)
    {
        public override string FormatName(Style style)
        {
            return string.Format(style.Help.ArgTagsFormat[ArgTags.Optional],
                                 $"{style.Options.KwargPrefix}{Name}{style.Options.KwargSuffix}{DefaultValue}");
        }

        public override string BuildDescription(Style style)
        {
            return $"{Name}{style.Help.NameBriefSeparator}{Description}";
        }
    }
}
