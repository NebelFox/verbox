namespace EasyCLI.Definitions.Parameters
{
    public record OptionDefinition(string Name, 
                                   PositionalDefinition Parameter, 
                                   object Default);
}
