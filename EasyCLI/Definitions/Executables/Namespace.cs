using System.Collections.Generic;
using System.Linq;
using EasyCLI.Models.Executables;
using EasyCLI.Models.Styles;
using EasyCLI.Definitions.Executables;

// ReSharper disable once CheckNamespace
namespace EasyCLI
{
    public class Namespace : ExecutableDefinition
    {
        private readonly LinkedList<ExecutableDefinition> _members;
        
        internal Namespace() : this(string.Empty, string.Empty)
        {}

        public Namespace(string name, string brief) : base(name, brief)
        {
            _members = new LinkedList<ExecutableDefinition>();
        }

        public Namespace WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public Namespace Member(ExecutableDefinition definition)
        {
            _members.AddLast(definition);
            return this;
        }
        
        // ReSharper disable once UseDeconstructionOnParameter
        internal override string BuildHelp(Style style)
        {
            return string.Join(style.Separator,
                               _members.Select(member =>
                                                   $"{style.PromptIndicator}{member.Name}{style.CommandHelpSuffix}{member.Brief}"));
        }

        internal override Executable Build(Style style)
        {
            return new Models.Executables.Namespace(BuildHelp(style),
                                                    _members.Select(member => (member.Name, member.Build(style))));
        }

        internal Models.Executables.Namespace Build(Style style, string help)
        {
            return new Models.Executables.Namespace(help,
                                                    _members.Select(member => (member.Name, member.Build(style))));
        }
    }
}
