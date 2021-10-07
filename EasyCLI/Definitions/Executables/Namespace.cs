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
        { }

        // ReSharper disable once MemberCanBePrivate.Global
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

        internal override Executable Build(Style style)
        {
            return Build(style, BuildHelp(style));
        }

        internal Models.Executables.Namespace Build(Style style, string help)
        {
            return new Models.Executables.Namespace(help, BuildMembers(style));
        }

        private IEnumerable<(string, Executable)> BuildMembers(Style style)
        {
            return _members.Select(m => (m.Name, m.Build(style)));
        }

        // ReSharper disable once UseDeconstructionOnParameter
        internal override string BuildHelp(Style style)
        {
            string prefix = style.Help.NamespaceMemberPrefix;
            string suffix = style.Help.NameBriefSeparator;
            return string.Join('\n', _members.Select(m => $"{prefix}{m.Name}{suffix}{m.Brief}"));
        }
    }
}
