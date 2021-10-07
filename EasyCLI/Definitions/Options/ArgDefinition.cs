using EasyCLI.Models.Styles;
using EasyCLI.Properties;

namespace EasyCLI.Definitions.Options
{
    public sealed record ArgDefinition(string Name,
                                       string Description,
                                       ArgTags Tags)
        : OptionDefinition(Name, Description)
    {
        public override string FormatName(Style style)
        {
            return style.Help.ArgTagsFormat.TryGetValue(Tags, out string format) 
                       ? string.Format(format, $"<{Name}>") 
                       : $"<{Name}>";
        }

        public override string BuildDescription(Style style)
        {
            return $"{Name}{style.Help.NameBriefSeparator}{Description}";
        }
    }
}
