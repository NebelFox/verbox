using System.Collections.Generic;

namespace Verbox.Models
{
    /// <summary>
    /// Is used to parametrize the command action calls
    /// </summary>
    public class Context
    {
        private readonly IReadOnlyDictionary<string, object> _arguments;

        internal Context(Box box, 
                        IReadOnlyDictionary<string, object> arguments)
        {
            Box = box;
            _arguments = arguments;
        }
        
        /// <summary>
        /// The <see cref="Box"/> the action is called from
        /// </summary>
        public Box Box { get; }

        /// <summary>
        /// Value of a specific argument, got from the call input
        /// </summary>
        /// <param name="name">name of the parameter</param>
        public object this[string name] => _arguments[name];
    }
}
