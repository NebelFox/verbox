using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
            var builder = new StringBuilder();
            builder.Append(Name);
            if (Type != "string")
                builder.Append(':').Append(Type);
            if (Tags.HasFlag(ArgTags.Collective))
                builder.Append("...");
            if (Tags.HasFlag(ArgTags.Optional))
                builder.Insert(0, '[').Append(']');
            builder.Append(RepresentBrief());

            return builder.ToString();
        }

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
