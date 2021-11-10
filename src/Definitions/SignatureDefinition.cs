using System.Collections.Generic;
using System.Linq;
using Verbox.Definitions.Parameters;
using Verbox.Models;
using Verbox.Models.Parameters;
using Verbox.Models.Styles;
using Verbox.Text;
using Type = Verbox.Text.Type;

namespace Verbox.Definitions
{
    using Typeset = IReadOnlyDictionary<string, Type>;

    public class SignatureDefinition
    {
        private readonly LinkedList<PositionalDefinition> _positionals;
        private readonly LinkedList<string> _switches;
        private readonly LinkedList<OptionDefinition> _options;

        public SignatureDefinition()
        {
            _positionals = new LinkedList<PositionalDefinition>();
            _switches = new LinkedList<string>();
            _options = new LinkedList<OptionDefinition>();
        }

        public SignatureDefinition Positional(string name,
                                               string type,
                                               ArgTags tags = ArgTags.None)
        {
            _positionals.AddLast(new PositionalDefinition(name, type, tags));
            return this;
        }

        public SignatureDefinition Positional(string name,
                                              ArgTags tags = ArgTags.None)
        {
            return Positional(name, "string", tags);
        }

        public SignatureDefinition Option(string name,
                                          string paramName,
                                          string paramType,
                                          ArgTags paramTags = ArgTags.None,
                                          string defaultValue = null)
        {
            var parameter = new PositionalDefinition(paramName,
                                                     paramType,
                                                     paramTags);
            _options.AddLast(new OptionDefinition(name, parameter, defaultValue));
            return this;
        }

        public SignatureDefinition Option(string name)
        {
            _switches.AddLast(name);
            return this;
        }

        private static Positional BuildPositional(PositionalDefinition definition, Typeset typeset)
        {
            return new Positional(definition.Name,
                                  typeset[definition.Type],
                                  MinValuesCount(definition.Tags),
                                  MaxValuesCount(definition.Tags));
        }

        private static int MinValuesCount(ArgTags tags) => tags.HasFlag(ArgTags.Optional) ? 0 : 1;

        private static int MaxValuesCount(ArgTags tags) => tags.HasFlag(ArgTags.Collective) ? int.MaxValue : 1;

        private static Option BuildOption(OptionDefinition definition, Typeset typeset)
        {
            object defaultValue = definition.Default != null
                ? typeset[definition.Parameter.Type].Parse(definition.Default)
                : null;
            return new Option(definition.Name,
                              BuildPositional(definition.Parameter, typeset),
                              defaultValue);
        }

        internal Signature Build(OptionStyle style, Typeset typeset, Tokenizer tokenizer)
        {
            return new Signature(_positionals.Select(p => BuildPositional(p, typeset)),
                                 _switches,
                                 _options.Select(o => BuildOption(o, typeset)),
                                 tokenizer);
        }

        internal string BuildHelp()
        {
            return string.Empty;
        }
    }
}
