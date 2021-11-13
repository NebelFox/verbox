using System.Collections.Generic;
using Verbox.Models.Executables;
using Verbox.Text;

namespace Verbox.Definitions.Executables
{
    using Typeset = IReadOnlyDictionary<string, Type>;
    
    /// <summary>
    /// Base class for <see cref="Command"/> and <see cref="Namespace"/>
    /// </summary>
    public abstract class ExecutableDefinition
    {
        internal string Name { get; }
        internal string Brief { get; }
        internal string Description { get; set; }

        private protected ExecutableDefinition(string name, string brief)
        {
            Name = name;
            Brief = brief;
        }

        internal abstract string BuildHelp(Style style);

        internal abstract Executable Build(Style style, Typeset typeset);
    }
}
