namespace Verbox.Definitions.Parameters
{
    internal record OptionDefinition(string Name,
                                     PositionalDefinition Parameter,
                                     string Default,
                                     string Brief) : ParameterDefinition(Name, Brief)
    {
        public override string ToString()
        {
            return $"[--{Name} {Parameter}{RepresentDefault()}]{RepresentBrief()}";
        }

        private string RepresentDefault() => Default != null ? $"={Default}" : string.Empty;
    }
}
