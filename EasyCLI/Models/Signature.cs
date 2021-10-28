using System;
using System.Collections.Generic;
using System.Linq;
using EasyCLI.Models.Parameters;
using EasyCLI.Models.Styles;

namespace EasyCLI.Models
{
    using Arguments = IReadOnlyDictionary<string, object>;

    internal sealed class Signature
    {
        private readonly OptionStyle _optionStyle;

        public Signature(IEnumerable<Positional> positionals,
                         IEnumerable<Option> options,
                         OptionStyle optionStyle)
        {
            Positionals = positionals.ToArray();
            Options = new Dictionary<string, Option>(
                options.Select(o => new KeyValuePair<string, Option>(o.Name, o)));
            _optionStyle = optionStyle;
        }

        public IReadOnlyList<Positional> Positionals { get; }
        public IReadOnlyDictionary<string, Option> Options { get; }

        public Arguments ParseArguments(string[] input)
        {
            (string, bool)[] tokens = Tokenize(input);
            var arguments = new Dictionary<string, object>();
            var positionalIndex = 0;
            var tokenIndex = 0;
            while (tokenIndex < tokens.Length)
            {
                // it's an option
                if (tokens[tokenIndex].Item2)
                {
                    string name = tokens[tokenIndex].Item1;
                    if (!Options.TryGetValue(name, out Option option))
                        throw new ArgumentException($"Unknown option: \"{name}\"");
                    ++tokenIndex;
                    object value = ParsePositional(tokens, ref tokenIndex, option.Parameter);
                    arguments[name] = option.Parameter.Type == typeof(bool) 
                        ? true 
                        : value ?? option.Default;
                }
                else
                {
                    Positional positional = Positionals[positionalIndex++];
                    arguments[positional.Name] = ParsePositional(tokens,
                                                                 ref tokenIndex,
                                                                 positional);
                }
            }

            if (positionalIndex < Positionals.Count
             && AnyMandatoryPositionalsInTail(positionalIndex))
                throw new ArgumentException(
                    $"{Positionals.Count - positionalIndex} mandatory positional parameter(s) missed values");

            return arguments;
        }

        private (string, bool)[] Tokenize(IEnumerable<string> input)
        {
            return input.Select(Tokenize).ToArray();
        }

        private (string, bool) Tokenize(string input)
        {
            return input.StartsWith(_optionStyle.Prefix)
                ? (input[_optionStyle.Prefix.Length..], true)
                : (input, false);
        }

        private bool AnyMandatoryPositionalsInTail(int start)
        {
            return Enumerable.Range(start, Positionals.Count - start)
                             .Any(i => IsMandatory(Positionals[i]));
        }

        private static bool IsMandatory(Positional parameter)
        {
            return parameter.MinValuesCount > 0;
        }

        private static object ParsePositional(IReadOnlyList<(string, bool)> tokens,
                                              ref int current,
                                              Positional parameter)
        {
            var values = new List<string>();

            while (current < tokens.Count && tokens[current].Item2 == false && values.Count < parameter.MaxValuesCount)
                values.Add(tokens[current++].Item1);

            if (values.Count < parameter.MinValuesCount)
                throw new ArgumentException($"Mandatory parameter \"{parameter.Name}\" missed a value");
            
            if (parameter.MaxValuesCount > 1 && values.Count > 0)
                return values.ToArray();
            return values.Count switch
            {
                0 => null,
                1 => values[0],
                _ => values.ToArray()
            };
        }
    }
}
