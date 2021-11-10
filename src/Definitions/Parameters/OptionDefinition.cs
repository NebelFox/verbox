namespace Verbox.Definitions.Parameters
{
    public record OptionDefinition(string Name, 
                                   PositionalDefinition Parameter, 
                                   string Default);
}
