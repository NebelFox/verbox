using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Verbox.Definitions.Parameters;
using Verbox.Models;
using Verbox.Models.Parameters;
using Verbox.Text;
using Type = Verbox.Text.Type;

namespace Verbox.Definitions
{
    using Typeset = IReadOnlyDictionary<string, Type>;

    public class SignatureDefinition
    {
        private static readonly Regex ParameterDefinitionRegex = new(
            @"^(?<option>--(?<names> [a-zA-Z0-9]+ (?: - [a-zA-Z0-9]+ ){0,2} )\s*)?
                    (?<positional>
                        (?:(?<optional>\[)|(?<!\[) )
                        (?: < (?<name> [a-zA-Z0-9]+ (?: - [a-zA-Z0-9]+ ){0,2} )
                            (?: \: (?<type> [a-zA-Z0-9]+ (?: - [a-zA-Z0-9]+ ){0,2} ) )?
                            (?<collective> \.{3} )? > )
                        (?(optional) (\])|(?!\]) ))?
                    (?(option)(\s*=\s*(?<default>\S+))?|)$",
            RegexOptions.IgnorePatternWhitespace);

        private readonly LinkedList<PositionalDefinition> _positionals;
        private readonly LinkedList<string> _switches;
        private readonly LinkedList<OptionDefinition> _options;

        public SignatureDefinition()
        {
            _positionals = new LinkedList<PositionalDefinition>();
            _switches = new LinkedList<string>();
            _options = new LinkedList<OptionDefinition>();
        }

        public SignatureDefinition Parameter(string definition)
        {
            Match match = ParameterDefinitionRegex.Match(definition);
            if (match.Success == false
             || match.Groups["option"].Success == false && match.Groups["positional"].Success == false)
                throw new ArgumentException("Invalid definition format", nameof(definition));

            if (match.Groups["positional"].Success == false)
            {
                Console.WriteLine($"// switch | name: {match.Groups["names"].Value}");
                return Option(match.Groups["names"].Value);
            }

            string name = match.Groups["name"].Value;
            string type = match.Groups["type"].Success ? match.Groups["type"].Value : "string";

            ArgTags tags = ArgTags.None;
            if (match.Groups["optional"].Success)
                tags |= ArgTags.Optional;
            if (match.Groups["collective"].Success)
                tags |= ArgTags.Collective;

            if (match.Groups["option"].Success == false)
            {
                Console.WriteLine($"// positional | name: {name} | type: {type} | tags: {tags}");
                return Positional(name,
                                  type,
                                  tags);
            }
            
            string @default = match.Groups["default"].Success ? match.Groups["default"].Value : null;
            Console.WriteLine(@$"// option | name: {match.Groups["names"].Value} | parameter: {{name: {
                name} | type: {type} | tags: {tags}}} | default: {@default}");
            return Option(match.Groups["names"].Value,
                          name,
                          type,
                          tags,
                          @default);

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

        internal Signature Build(Typeset typeset, Tokenizer tokenizer)
        {
            return new Signature(_positionals.Select(p => BuildPositional(p, typeset)),
                                 _switches,
                                 _options.Select(o => BuildOption(o, typeset)),
                                 tokenizer);
        }

        internal string BuildHelp()
        {
            return string.Join('\n',
                               string.Join(' ', _positionals.Select(p => p.Represent())),
                               string.Join(' ', _switches.Select(name => $"--{name}")),
                               string.Join(' ', _options.Select(o => o.Represent())));
        }
    }
}
