namespace Verbox.Definitions.Parameters
{
    internal record ParameterDefinition(string Name, string Brief)
    {
        public override string ToString()
        {
            return $"[--{Name}] - {Brief}";
        }

        protected string RepresentBrief() => Brief != null ? $" - {Brief}" : string.Empty;
    }
}
