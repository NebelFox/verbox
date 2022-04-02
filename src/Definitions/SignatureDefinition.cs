using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Verbox.Definitions.Parameters;
using Verbox.Extensions;
using Verbox.Models;
using Verbox.Models.Parameters;
using Type = Verbox.Text.Type;

namespace Verbox.Definitions
{
    using Typeset = IReadOnlyDictionary<string, Type>;

    internal class SignatureDefinition
    {
        private static readonly Regex ParameterDefinitionRegex = new(
            @"^(?<option>--(?<names> [a-zA-Z0-9]+ (?: - [a-zA-Z0-9]+ ){0,2} )\s*)?
                    (?<positional>
                        (?:(?<optional>\[)|(?<!\[) )
                        (?: (?<chevrons><)? (?<name> [a-zA-Z0-9]+ (?: - [a-zA-Z0-9]+ ){0,2} )
                            (?: \: (?<type> [a-zA-Z0-9]+ (?: - [a-zA-Z0-9]+ ){0,2} ) )?
                            (?<collective> \.{3} )? (?(chevrons)>|) )
                        (?(optional) (\])|(?!\]) ))?
                    (?(option)(\s*=\s*(?<default>\S+))?|)
                    (\s+ - \s+ (?<brief>.+) )? $",
            RegexOptions.IgnorePatternWhitespace);

        private readonly List<PositionalDefinition> _positionals;
        private readonly List<ParameterDefinition> _switches;
        private readonly List<OptionDefinition> _options;

        public SignatureDefinition()
        {
            _positionals = new List<PositionalDefinition>();
            _switches = new List<ParameterDefinition>();
            _options = new List<OptionDefinition>();
        }

        public void Parameter(string definition)
        {
            Match match = ParameterDefinitionRegex.Match(definition);
            if (match.Success == false
             || match.Groups["option"].Success == false && match.Groups["positional"].Success == false)
                throw new ArgumentException("Invalid definition format", nameof(definition));

            string brief = match.Groups["brief"].ValueOrDefault();

            if (match.Groups["positional"].Success == false)
            {
                Option(match.Groups["names"].Value, brief);
                return;
            }

            string name = match.Groups["name"].Value;
            string type = match.Groups["type"].ValueOrDefault("string");

            ArgTags tags = ArgTags.None;
            if (match.Groups["optional"].Success)
                tags |= ArgTags.Optional;
            if (match.Groups["collective"].Success)
                tags |= ArgTags.Collective;

            if (match.Groups["option"].Success == false)
            {
                Positional(name, type, tags, brief);
                return;
            }

            string @default = match.Groups["default"].ValueOrDefault();
            Option(match.Groups["names"].Value,
                   new PositionalDefinition(name, type, tags, null),
                   @default,
                   brief);
        }

        private void Positional(string name,
                                string type,
                                ArgTags tags,
                                string brief)
        {
            _positionals.Add(new PositionalDefinition(name, type, tags, brief));
        }

        private void Option(string name,
                            PositionalDefinition parameter,
                            string defaultValue,
                            string brief)
        {
            _options.Add(new OptionDefinition(name, parameter, defaultValue, brief));
        }

        private void Option(string name, string brief)
        {
            _switches.Add(new ParameterDefinition(name, brief));
        }

        private static Option BuildOption(OptionDefinition definition, Typeset typeset)
        {
            object defaultValue = definition.Default != null
                ? ParseDefaultValue(definition, typeset)
                : null;
            return new Option(definition.Name,
                              definition.Parameter.Build(typeset),
                              defaultValue);
        }

        private static object ParseDefaultValue(OptionDefinition definition,
                                                Typeset typeset)
        {
            object value = typeset[definition.Parameter.Type].Parse(definition.Default);
            return definition.Parameter.Tags.HasFlag(ArgTags.Collective)
                ? new[] { value }
                : value;
        }

        internal Signature Build(Typeset typeset)
        {
            return new Signature(_positionals.Select(p => p.Build(typeset)),
                                 _switches.Select(s => s.Name),
                                 _options.Select(o => BuildOption(o, typeset)));
        }

        internal string BuildHelp()
        {
            return "\n\n".JoinMeaningful(BuildParametersHelp("POSITIONAL PARAMETERS", _positionals),
                                         BuildParametersHelp("SWITCHES", _switches),
                                         BuildParametersHelp("OPTIONS", _options));
        }

        private static string BuildParametersHelp(string title, IEnumerable<ParameterDefinition> parameters)
        {
            return $"{title}:\n\t{string.Join("\t\n", parameters.Select(p => p.ToString()))}";
        }

        public SignatureDefinition Copy()
        {
            var copy = new SignatureDefinition();
            copy._positionals.AddRange(_positionals);
            copy._options.AddRange(_options);
            copy._switches.AddRange(_switches);
            return copy;
        }
    }
}
