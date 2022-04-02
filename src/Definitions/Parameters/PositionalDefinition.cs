using System.Collections.Generic;
using System.ComponentModel;
using Verbox.Models.Parameters;
using Verbox.Text;

namespace Verbox.Definitions.Parameters
{
    internal record PositionalDefinition(string Name,
                                         string Type,
                                         ArgTags Tags,
                                         string Brief) : ParameterDefinition(Name, Brief)
    {
        public override string ToString()
        {
            return $"{Prefix}{Name}{RepresentType()}{RepresentCollective()}{Postfix}{RepresentBrief()}";
        }

        private string Prefix => Tags.HasFlag(ArgTags.Optional) ? "[" : string.Empty;
        private string RepresentType() => Type != "string" ? $":{Type}" : string.Empty;
        private string RepresentCollective() => Tags.HasFlag(ArgTags.Collective) ? "..." : string.Empty;
        private string Postfix => Tags.HasFlag(ArgTags.Optional) ? "]" : string.Empty;

        public Positional Build(IReadOnlyDictionary<string, Type> typeset)
        {
            Type type = typeset[Type];
            return Tags switch
            {
                ArgTags.None       => new Positional(Name, type),
                ArgTags.Optional   => new Optional(Name, type),
                ArgTags.Collective => new Collective(Name, type),
                (ArgTags)0b11      => new CollectiveOptional(Name, type),
                _ => throw new InvalidEnumArgumentException(nameof(Tags), 
                                                            (int)Tags, 
                                                            typeof(ArgTags))
            };
        }
    }
}
