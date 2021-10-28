using System;
using System.Collections.Generic;
using Verbox.Definitions;
using Verbox.Definitions.Executables;
using Verbox.Models;
using Verbox.Models.Executables;
using Verbox.Models.Styles;

// ReSharper disable once CheckNamespace
namespace Verbox
{
    public class Command : ExecutableDefinition
    {
        private Action<Context> _action;
        private SignatureDefinition _signature;
        private readonly LinkedList<(string, string)> _examples;

        public Command(string name, string brief) : base(name, brief)
        {
            _examples = new LinkedList<(string, string)>();
        }

        public Command WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public Command Parametrize(SignatureDefinition signature)
        {
            _signature = signature;
            return this;
        }

        public Command Example(string input, string explanation)
        {
            _examples.AddLast((input, explanation));
            return this;
        }

        public Command Action(Action<Context> action)
        {
            _action = action;
            return this;
        }

        internal override Executable Build(Style style)
        {
            _signature ??= new SignatureDefinition();
            return new Models.Executables.Command(BuildHelp(style), 
                                                  _action, 
                                                  _signature.Build(style.Option));
        }

        internal override string BuildHelp(Style style)
        {
            return $"{{Help placeholder of {Name} command}}";
        }
    }
}
