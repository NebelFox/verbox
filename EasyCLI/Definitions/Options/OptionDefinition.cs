using EasyCLI.Models.Styles;

namespace EasyCLI.Definitions.Options
{
    public abstract record OptionDefinition(string Name,
                                            string Description)
    {
        public abstract string FormatName(Style style);

        public abstract string BuildDescription(Style style);
    }
}
