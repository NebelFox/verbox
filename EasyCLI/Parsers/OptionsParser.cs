using System.Collections.Generic;
using EasyCLI.Models.Styles;

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

        public OptionsParser(OptionsStyle style)
            : this(new KwargParser(style.KwargPrefix, style.KwargSuffix),
                   new SwitchParser(style.SwitchShortPrefix, style.SwitchLongPrefix))
        { }

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
