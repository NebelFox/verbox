using System.Collections.Generic;

namespace EasyCLI.Models
{
    public class Context
    {
        private readonly IReadOnlyDictionary<string, object> _arguments;

        internal Context(Menu source, 
                        IReadOnlyDictionary<string, object> arguments)
        {
            Source = source;
            _arguments = arguments;
        }
        
        public Menu Source { get; }

        public object this[string name] => _arguments[name];

        public object this[char name] => this[name.ToString()];
    }
}
