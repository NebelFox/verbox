using EasyCLI.Models.Styles;
using EasyCLI.Properties;

namespace EasyCLI.Definitions.Options
{
    public sealed record SwitchDefinition(string Name,
                                          string Description,
                                          char ShortName)
        : OptionDefinition(Name, Description)
    {
        public override string FormatName(Style style)
        {
            return string.Format(style.Help.ArgTagsFormat[ArgTags.Optional],
                                 $"{style.Options.SwitchShortPrefix}{ShortName}{style.Help.SwitchNamesSuffix}{style.Options.SwitchLongPrefix}{Name}");
        }

        public override string BuildDescription(Style style)
        {
            return $"{FormatName(style)}{style.Help.NameBriefSeparator}{Description}";
        }
    }
}
