using System.Collections.Generic;

namespace EasyCLI.Parsers
{
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

        internal Options Parse(IEnumerable<string> tokens)
        {
            var options = new Options(new Dictionary<string, string>(),
                                      new HashSet<string>(),
                                      new LinkedList<string>());
            foreach (string token in tokens)
                Parse(token, options);

            return options;
        }

        private void Parse(string token,
                           Options options)
        {
            if (!(_kwargParser.TryParseToOptions(token, options)
                  || _switchParser.TryParseToOptions(token, options)))
                options.AddArg(token);
        }
    }
}
