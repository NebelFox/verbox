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
        private int _positionalIndex;
        private int _current;
        private bool _optionsEnabled = true;

        public Signature(IEnumerable<Positional> positionals,
                         IEnumerable<string> switches,
                         IEnumerable<Option> options)
        {
            _positionals = positionals.ToArray();
            _switches = new SortedSet<string>(switches);
            _options = new Dictionary<string, Option>(
                options.Select(o => new KeyValuePair<string, Option>(o.Name, o)));
        }

        public Arguments ParseArguments(IReadOnlyList<Token> tokens)
        {
            var arguments = new Dictionary<string, object>();
            _positionalIndex = 0;
            _current = 0;
            _optionsEnabled = true;
            while (_current < tokens.Count)
            {
                if (tokens[_current].Type == TokenType.LongDelimiter)
                {
                    _optionsEnabled = _optionsEnabled == false;
                    ++_current;
                }
                else if (tokens[_current].IsOption && _optionsEnabled)
                {
                    string name = tokens[_current++].Value;
                    arguments[name] = ParseOption(name, tokens);
                }
                else
                {
                    ParsePositional(tokens, arguments);
                }
            }

            SetOmittedPositionalsToDefault(arguments);
            SetOmittedOptionsToDefaults(arguments);
            return arguments;
        }

        private void ParsePositional(IReadOnlyList<Token> tokens,
                                     IDictionary<string, object> arguments)
        {
            if (_positionalIndex >= _positionals.Count)
                throw new ArgumentException($"Obscure positional argument: '{tokens[_current].Value}'");
            Positional positional = _positionals[_positionalIndex++];
            arguments[positional.Name] = positional.Parse(tokens,
                                                          ref _current,
                                                          _optionsEnabled);
        }

        private object ParseOption(string name,
                                   IReadOnlyList<Token> tokens)
        {
            if (_switches.Contains(name))
                return true;

            if (_options.TryGetValue(name, out Option option))
                return option.Parse(tokens, ref _current);

            throw new ArgumentException($"Unknown option: \"{name}\"");
        }

        private void SetOmittedPositionalsToDefault(IDictionary<string, object> arguments)
        {
            foreach (Positional positional in _positionals.Skip(_positionalIndex))
                arguments.Add(positional.Name, positional.Default);
        }

        private void SetOmittedOptionsToDefaults(Dictionary<string, object> arguments)
        {
            foreach (Option option in _options.Values)
                arguments.TryAdd(option.Name, option.Default);
            foreach (string name in _switches)
                arguments.TryAdd(name, false);
        }
    }
}
