using System;
using System.Collections.Generic;
using System.Linq;
using Verbox.Definitions.Parameters;
using Verbox.Models;
using Verbox.Models.Parameters;
using Verbox.Models.Styles;
using Verbox.Properties;

namespace Verbox.Definitions
{
    public class SignatureDefinition
    {
        private readonly LinkedList<PositionalDefinition> _positionals;
        private readonly LinkedList<OptionDefinition> _options;

        public SignatureDefinition()
        {
            _positionals = new LinkedList<PositionalDefinition>();
            _options = new LinkedList<OptionDefinition>();
        }

        public SignatureDefinition Positional(string name,
                                              Type type,
                                              ArgTags tags = ArgTags.None)
        {
            _positionals.AddLast(new PositionalDefinition(name, type, tags));
            return this;
        }

        public SignatureDefinition Positional(string name,
                                              ArgTags tags = ArgTags.None)
        {
            return Positional(name, typeof(string), tags);
        }

        public SignatureDefinition Option(string name,
                                          string paramName,
                                          Type paramType,
                                          ArgTags paramTags = ArgTags.None,
                                          object defaultValue = null)
        {
            _options.AddLast(new OptionDefinition(
                                 name,
                                 new PositionalDefinition(paramName,
                                                          paramType,
                                                          paramTags),
                                 defaultValue));
            return this;
        }

        public SignatureDefinition Option(string name)
        {
            return Option(name,
                          string.Empty,
                          typeof(bool),
                          ArgTags.Optional,
                          true);
        }

        internal Signature Build(OptionStyle style)
        {
            return new Signature(_positionals.Select(BuildPositional),
                                 _options.Select(BuildOption),
                                 style);
        }

        private static Positional BuildPositional(PositionalDefinition definition)
        {
            return new Positional(definition.Name,
                                  definition.Type,
                                  definition.Tags.HasFlag(ArgTags.Optional) ? 0 : 1,
                                  definition.Tags.HasFlag(ArgTags.Collective) ? int.MaxValue : 1);
        }

        private static Option BuildOption(OptionDefinition definition)
        {
            return new Option(definition.Name,
                              BuildPositional(definition.Parameter),
                              definition.Default);
        }

        internal string BuildHelp()
        {
            return string.Empty;
        }
    }
}
