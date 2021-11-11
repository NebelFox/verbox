namespace Verbox.Definitions.Parameters
{
    public record OptionDefinition(string Name,
                                   PositionalDefinition Parameter,
                                   string Default)
    {
        public string Represent() => $"--{Name} {Parameter.Represent()}{RepresentDefault()}";

        private string RepresentDefault() => Default != null ? $"={Default}" : string.Empty;
    }
}
