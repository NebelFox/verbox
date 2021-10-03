using EasyCLI.Models.Executables;
using EasyCLI.Models.Styles;

namespace EasyCLI.Definitions.Executables
{
    public abstract class ExecutableDefinition
    {
        internal string Name { get; }
        internal string Brief { get; set; }
        internal string Description { get; set; }

        private protected ExecutableDefinition(string name)
        {
            Name = name;
        }

        private protected ExecutableDefinition(string name, string brief)
        {
            Name = name;
            Brief = brief;
        }

        internal abstract string BuildHelp(Style style);

        internal abstract Executable Build(Style style);
    }
}
