using System.Collections.Generic;

namespace Verbox.Models
{
    public class Context
    {
        private readonly IReadOnlyDictionary<string, object> _arguments;

        internal Context(Box box, 
                        IReadOnlyDictionary<string, object> arguments)
        {
            Box = box;
            _arguments = arguments;
        }
        
        public Box Box { get; }

        public object this[string name] => _arguments[name];

        public object this[char name] => this[name.ToString()];
    }
}
