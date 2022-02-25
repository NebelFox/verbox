using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Verbox
{
    /// <summary>
    /// Parametrizes command action calls
    /// </summary>
    public class Context : IEnumerable<KeyValuePair<string, object>>
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
        /// <param name="name">name of the command parameter</param>
        public object this[string name] => _arguments[name];

        /// <summary>
        /// Supports iteration over the arguments
        /// </summary>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _arguments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// String value of a specific argument
        /// </summary>
        /// <param name="name">name of the command parameter</param>
        public string Get(string name) => _arguments[name].ToString();

        /// <summary>
        /// Arguments of a collective parameter,
        /// each casted to the specified type 
        /// </summary>
        /// <param name="name">name of a collective parameter</param>
        /// <typeparam name="T">Type to cats each value to</typeparam>
        public IEnumerable<T> Get<T>(string name)
        {
            return ((object[])_arguments[name]).Select(o => (T)o);
        }
    }
}
