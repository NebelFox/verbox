namespace Verbox.Definitions.Parameters
{
    internal record OptionDefinition(string Name,
                                   PositionalDefinition Parameter,
                                   string Default)
    {
        public string Represent() => $"--{Name} {Parameter.Represent()}{RepresentDefault()}";

        private string RepresentDefault() => Default != null ? $"={Default}" : string.Empty;
    }
}
