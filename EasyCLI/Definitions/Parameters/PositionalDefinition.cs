using System;
using EasyCLI.Properties;

namespace EasyCLI.Definitions.Parameters
{
    public record PositionalDefinition(string Name, Type Type, ArgTags Tags);
}
