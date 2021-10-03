using System;
using System.Collections.Generic;
using System.Linq;
using EasyCLI.Parsers;

namespace EasyCLI.Models
{
    public class Context
    {
        public Menu Source { get; }
        private readonly IReadOnlyDictionary<string, string> _kwargs;
        private readonly IReadOnlySet<string> _switches;
        private readonly string[] _args;

        internal Context(Menu source, Options options)
            : this(source,
                   new Dictionary<string, string>(options.Kwargs),
                   new HashSet<string>(options.Switches),
                   options.Args.ToArray())
        { }

        private Context(Menu source,
                        IReadOnlyDictionary<string, string> kwargs,
                        IReadOnlySet<string> switches,
                        string[] args)
        {
            Source = source;
            _kwargs = kwargs;
            _switches = switches;
            _args = args;
        }

        public int ArgsCount => _args.Length;

        public int OptionsCount => _kwargs.Count + _switches.Count + ArgsCount;

        public bool TryGetKwarg(string key, out string value)
        {
            return _kwargs.TryGetValue(key, out value);
        }

        public bool this[string key] => _switches.Contains(key);

        public bool this[char key] => this[key.ToString()];

        public string this[int index] => _args[index];

        public string this[Index index] => _args[index];

        public string[] this[Range range] => _args[range];

        public IEnumerable<string> Args => _args;

        public IEnumerable<string> Switches => _switches;

        public IReadOnlyDictionary<string, string> Kwargs => _kwargs;

        public (string, Context) ExtractFirstArg()
        {
            return (this[0], new Context(Source, _kwargs, _switches, _args[1..]));
        }
    }
}
