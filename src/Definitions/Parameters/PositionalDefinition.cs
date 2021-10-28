using System;
using Verbox.Properties;

namespace Verbox.Definitions.Parameters
{
    public record PositionalDefinition(string Name, Type Type, ArgTags Tags);
}
