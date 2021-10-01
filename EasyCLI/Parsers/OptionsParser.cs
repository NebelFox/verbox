using System;
using System.Collections.Generic;
using EasyCLI.Parsers;

namespace EasyCLI
{
    using Options = Tuple<Dictionary<string, string>, HashSet<string>, List<string>>;
    public class OptionsParser
    {
        private readonly KwargParser _kwargParser;
        private readonly SwitchParser _switchParser;

        public OptionsParser(KwargParser kwargParser,
                             SwitchParser switchParser)
        {
            _switchParser = switchParser;
            _kwargParser = kwargParser;
        }

        public Options Parse(IEnumerable<string> tokens)
        {
            var options = new Options(new Dictionary<string, string>(),
                                      new HashSet<string>(),
                                      new List<string>());
            foreach (string token in tokens)
                Parse(token, ref options);

            return options;
        }

        private void Parse(string token,
                           ref Options options)
        {
            if (_kwargParser.TryParse(token, out string key, out string value))
                options.Item1.Add(key, value);
            else if (_switchParser.TryParse(token, out string[] keys))
                options.Item2.UnionWith(keys);
            else
                options.Item3.Add(token);
        }
    }
}
