namespace Verbox.Definitions.Parameters
{
    internal record OptionDefinition(string Name,
                                     PositionalDefinition Parameter,
                                     string Default)
    {
        public override string ToString()
        {
            return $"[--{Name} {Parameter}{RepresentDefault()}]";
        }

        private string RepresentDefault() => Default != null ? $"={Default}" : string.Empty;
    }
}
