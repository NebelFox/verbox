using System;
using System.Collections.Generic;
using Verbox.Definitions;
using Verbox.Definitions.Executables;
using Verbox.Extensions;
using Verbox.Models.Executables;
using Verbox.Text.Tokens;
using Type = Verbox.Text.Type;

namespace Verbox
{
    using Typeset = IReadOnlyDictionary<string, Type>;

    /// <summary>
    /// Command definition
    /// </summary>
    public class Command : ExecutableDefinition
    {
        private Action<Context> _action;
        private SignatureDefinition _signature;
        private readonly List<string> _examples;

        /// <summary>
        /// Creates a new Command definition
        /// </summary>
        /// <param name="name">command name</param>
        /// <param name="brief">short info about the command</param>
        public Command(string name, string brief=null) : base(name, brief)
        {
            _signature = new SignatureDefinition();
            _examples = new List<string>();
        }

        /// <summary>
        /// Adds description to the command
        /// </summary>
        public Command WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        /// Parses each definition to parameter and adds to the command
        /// </summary>
        public Command Parameters(params string[] definitions)
        {
            foreach (string definition in definitions)
                _signature.Parameter(definition);
            return this;
        }

        /// <summary>
        /// Adds each string as a usage example for this command
        /// </summary>
        /// <param name="examples">intended to be a string command with a short explanation</param>
        public Command Examples(params string[] examples)
        {
            _examples.AddRange(examples);
            return this;
        }

        /// <summary>
        /// Sets the function to be invoked on the command call
        /// </summary>
        /// <param name="action">An action to be performed on the command call</param>
        public Command Action(Action<Context> action)
        {
            _action = action;
            return this;
        }

        internal override ExecutableDefinition Get(string[] path)
        {
            return path.Length == 0 ? this : null;
        }

        internal override void SetMissingActions(Action<Context> action)
        {
            _action ??= action;
        }

        internal override Executable Build(Style style, Typeset typeset)
        {
            if (_action == null)
                throw new InvalidOperationException($"Action for {Name} command not set");
            var tokenizer = new Tokenizer(style["input.quotes"],
                                          style["option.prefix"],
                                          style["input.delimiter.short"],
                                          style["input.delimiter.long"]);
            return new Models.Executables.Command(BuildHelp(style),
                                                  _action,
                                                  _signature.Build(typeset, tokenizer));
        }

        internal override string BuildHelp(Style style)
        {
            return style["dialogue.semantic-separator"]
               .JoinMeaningful(Description,
                             _signature.BuildHelp(),
                             string.Join('\n', _examples));
        }

        internal override ExecutableDefinition Copy()
        {
            Command copy = new Command(Name, Brief)
                          .Action(_action)
                          .WithDescription(Description)
                          .Examples(_examples.ToArray());
            copy._signature = _signature.Copy();
            return copy;
        }
    }
}
