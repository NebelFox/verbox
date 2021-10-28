using System.Collections.Generic;
using System.Linq;
using Verbox.Definitions.Executables;
using Verbox.Models.Executables;
using Verbox.Models.Styles;

// ReSharper disable once CheckNamespace
namespace Verbox
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
        
        internal override string BuildHelp(Style style)
        {
            return string.Join('\n', _members.Select(m => BuildMemberHelpRow(m, style)));
        }

        private static string BuildMemberHelpRow(ExecutableDefinition member, Style style)
        {
            return @$"{style.Help.NamespaceMemberPrefix}{member.Name}{
                style.Help.NameBriefSeparator}{member.Brief}";
        }
    }
}
