using System;
using System.Collections.Generic;
using System.Linq;
using Verbox.Definitions.Executables;
using Verbox.Extensions;
using Verbox.Models.Executables;
using Type = Verbox.Text.Type;

// ReSharper disable once CheckNamespace
namespace Verbox
{
    using Typeset = IReadOnlyDictionary<string, Type>;

    /// <summary>
    /// Namespace definition - a collection of commands or other namespaces,
    /// where each item may be referred via its name.
    /// </summary>
    public class Namespace : ExecutableDefinition
    {
        private readonly List<ExecutableDefinition> _executables;

        /// <summary>
        /// Creates a new namespace definition
        /// </summary>
        /// <param name="name">namespace name</param>
        /// <param name="brief">namespace short info</param>
        public Namespace(string name, string brief = null) : base(name, brief)
        {
            _executables = new List<ExecutableDefinition>();
        }

        /// <summary>
        /// Adds description to the namespace.
        /// </summary>
        public Namespace WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        /// Adds <see cref="Verbox.Command"/> or another <see cref="Namespace"/> to the namespace
        /// either at the beginning or at the end.
        /// </summary>
        /// <param name="definition">an executable to add</param>
        /// <param name="prepend">adds the executable at the beginning, if true. Otherwise, at the end</param>
        public Namespace Command(ExecutableDefinition definition,
                                 bool prepend = false)
        {
            _executables.Insert(prepend ? 0 : _executables.Count, definition);
            return this;
        }

        /// <summary>
        /// Adds an executable to namespace either before ar after
        /// an already added to this namespace command. 
        /// </summary>
        /// <param name="definition">A command or a namespace to add</param>
        /// <param name="targetExecutableName">Name of the contained executable to add near</param>
        /// <param name="before">
        /// Adds the executable before the specified one, if true;
        /// otherwise, adds after it.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the namespace doesn't contain an executable named targetExecutableName/>
        /// </exception>
        public void InsertCommandNear(ExecutableDefinition definition,
                                      string targetExecutableName,
                                      bool before = false)
        {
            int index = GetCommandIndex(targetExecutableName);
            if (index == -1)
                throw new ArgumentException(
                    $"The {Name} namespace doesn't contain a command with name \"{targetExecutableName}\"");
            _executables.Insert(index + (before ? 0 : 1), definition);
        }

        private int GetCommandIndex(string name)
        {
            return _executables.FindIndex(cmd => cmd.Name == name);
        }

        internal override ExecutableDefinition Get(string[] path)
        {
            if (path.Length == 0)
                return this;
            int index = GetCommandIndex(path[0]);
            return index != -1 ? _executables[index].Get(path[1..]) : null;
        }

        internal override void SetMissingActions(Action<Context> action)
        {
            foreach (ExecutableDefinition definition in _executables)
                definition.SetMissingActions(action);
        }

        internal override Executable Build(Style style, Typeset typeset)
        {
            return Build(style, BuildHelp(style), typeset);
        }

        internal Models.Executables.Namespace Build(Style style, string help, Typeset typeset)
        {
            return new Models.Executables.Namespace(help, BuildExecutables(style, typeset));
        }

        private IEnumerable<(string, Executable)> BuildExecutables(Style style, Typeset typeset)
        {
            return _executables.Select(e => (e.Name, e.Build(style, typeset)));
        }

        internal override string BuildHelp(Style style)
        {
            return style["dialogue.semantic-separator"]
               .JoinMeaningful(Description,
                               string.Join('\n',
                                           _executables.Select(m => BuildExecutableHelpRow(m, style))));
        }

        private static string BuildExecutableHelpRow(ExecutableDefinition member, Style style)
        {
            return string.Format(style["help.namespace-member-format"],
                                 member.Name,
                                 member.Brief);
        }

        internal override Namespace Copy()
        {
            Namespace copy = new Namespace(Name, Brief).WithDescription(Description);
            copy._executables.AddRange(_executables.Select(m => m.Copy()));
            return copy;
        }
    }
}
