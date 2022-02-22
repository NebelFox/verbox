using System;
using System.Collections.Generic;
using System.Linq;
using Verbox.Models.Parameters;
using Verbox.Text.Tokens;

namespace Verbox.Models
{
    using Arguments = IReadOnlyDictionary<string, object>;

    internal sealed class Signature
    {
        private readonly IReadOnlyList<Positional> _positionals;
        private readonly IReadOnlySet<string> _switches;
        private readonly IReadOnlyDictionary<string, Option> _options;
        private readonly Tokenizer _tokenizer;

        public Signature(IEnumerable<Positional> positionals,
                         IEnumerable<string> switches,
                         IEnumerable<Option> options,
                         Tokenizer tokenizer)
        {
            _positionals = positionals.ToArray();
            _switches = new SortedSet<string>(switches);
            _options = new Dictionary<string, Option>(
                options.Select(o => new KeyValuePair<string, Option>(o.Name, o)));
            _tokenizer = tokenizer;
        }

        public Arguments ParseArguments(IEnumerable<string> input)
        {
            IReadOnlyList<Token> tokens = _tokenizer.Tokenize(input);
            var arguments = new Dictionary<string, object>();
            var positionalIndex = 0;
            var current = 0;
            var optionsEnabled = true;
            while (current < tokens.Count)
            {
                if (tokens[current].Type == TokenType.LongDelimiter)
                {
                    optionsEnabled = optionsEnabled == false;
                    ++current;
                }
                else if (tokens[current].IsOption && optionsEnabled)
                {
                    string name = tokens[current++].Value;
                    if (_switches.Contains(name))
                        arguments[name] = true;
                    else if (_options.TryGetValue(name, out Option option))
                        arguments[name] = option.Parse(tokens, ref current);
                    else
                        throw new ArgumentException($"Unknown option: \"{name}\"");
                }
                else
                {
                    if (positionalIndex >= _positionals.Count)
                        throw new ArgumentException($"Obscure positional argument: '{tokens[current].Value}'");
                    Positional positional = _positionals[positionalIndex++];
                    arguments[positional.Name] = positional.Parse(tokens,
                                                                  ref current,
                                                                  optionsEnabled);
                }
            }

            if (positionalIndex < _positionals.Count && AnyMandatoryPositionalsInTail(positionalIndex))
                throw new ArgumentException(
                    $"{_positionals.Count - positionalIndex} mandatory positional parameter(s) missed value(s)");
            SetOmittedOptionsToDefaults(arguments);
            return arguments;
        }

        private bool AnyMandatoryPositionalsInTail(int start)
        {
            return Enumerable.Range(start, _positionals.Count - start)
                             .Any(i => _positionals[i].IsMandatory);
        }

        private void SetOmittedOptionsToDefaults(Dictionary<string, object> arguments)
        {
            foreach (Positional positional in _positionals)
                arguments.TryAdd(positional.Name, positional.Default);
            foreach (Option option in _options.Values)
                arguments.TryAdd(option.Name, option.Default);
            foreach (string name in _switches)
                arguments.TryAdd(name, false);
        }
    }
}
